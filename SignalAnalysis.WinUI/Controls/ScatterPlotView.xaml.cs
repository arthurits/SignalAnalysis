using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ScottPlot;
using ScottPlot.Plottables;
using SignalAnalysis.Helpers;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace SignalAnalysis.Controls;

public sealed partial class ScatterPlotView : UserControl
{
    public static readonly DependencyProperty XsProperty =
            DependencyProperty.Register(nameof(Xs), typeof(ObservableCollection<double>), typeof(ScatterPlotView),
                new PropertyMetadata(null, OnXsChanged));

    public static readonly DependencyProperty YsProperty =
        DependencyProperty.Register(nameof(Ys), typeof(ObservableCollection<double>), typeof(ScatterPlotView),
            new PropertyMetadata(null, OnYsChanged));

    public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register(nameof(Points), typeof(ObservableCollection<(double X, double Y)>), typeof(ScatterPlotView),
                new PropertyMetadata(null, OnPointsChanged));

    public ObservableCollection<double> Xs
    {
        get => (ObservableCollection<double>)GetValue(XsProperty);
        set => SetValue(XsProperty, value);
    }

    public ObservableCollection<double> Ys
    {
        get => (ObservableCollection<double>)GetValue(YsProperty);
        set => SetValue(YsProperty, value);
    }

    public ObservableCollection<(double X, double Y)> Points
    {
        get => (ObservableCollection<(double X, double Y)>)GetValue(PointsProperty);
        set => SetValue(PointsProperty, value);
    }

    // Método público para que la vista que contiene este control pueda añadir el host visual
    // y enlazar el Plot creado aquí al host concreto de ScottPlot WinUI.
    public Plot GetPlot() => _plot;

    // Internal property to hold the reference to the ScottPlot host control, which is used to call Refresh() when needed.
    // This should be set by the parent view after adding the host control to the visual tree.
    private readonly Lock _sync = new();
    private readonly DebounceDispatcher _debouncer;
    private readonly ScottPlot.WinUI.WinUIPlot _winUIPlot;
    private readonly Plot _plot;
    private readonly Scatter _scatter;

    // Internal Lists to hold the actual data points for the plot.
    // These are updated based on changes to the Xs and Ys collections, and are what the SignalXY plottable uses as its data source.
    // We maintain these internal lists to ensure that we only add complete pairs of X and Y values to the plot,
    // and to handle cases where X and Y values may arrive in an interleaved manner.
    private readonly List<double> _xsList = [];
    private readonly List<double> _ysList = [];

    // Internal buffers to hold pending X or Y values when they arrive without their corresponding pair,
    // to ensure that we only add complete pairs to the plot. This handles the case where X and Y are added in an interleaved manner.
    private double? _pendingX = null;
    private double? _pendingY = null;

    public ScatterPlotView()
    {
        InitializeComponent();

        _debouncer = new DebounceDispatcher(DispatcherQueue.GetForCurrentThread(), TimeSpan.FromMilliseconds(80));

        // Create the ScottPlot Plot and Scatter plottable, and set up the host control for WinUI.
        _winUIPlot = new();
        _plot = _winUIPlot.Plot;
        _scatter = _plot.Add.Scatter(_xsList, _ysList);

        // Insert the ScottPlot host control into the visual tree of this UserControl.
        // In this case, we assume there is a Grid named "RootGrid" in the XAML where we want to place the plot.
        RootGrid.Children.Add(_winUIPlot);
        WinUIPlotHost = _winUIPlot;
    }

    private static void OnXsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (ScatterPlotView)d;
        ctrl.OnCollectionAssigned(e.OldValue as ObservableCollection<double>, e.NewValue as ObservableCollection<double>, isXs: true, isYs: false);
    }

    private static void OnYsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (ScatterPlotView)d;
        ctrl.OnCollectionAssigned(e.OldValue as ObservableCollection<double>, e.NewValue as ObservableCollection<double>, isXs: false, isYs: true);
    }

    private static void OnPointsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (ScatterPlotView)d;
        ctrl.OnPointsAssigned(e.OldValue as ObservableCollection<(double X, double Y)>, e.NewValue as ObservableCollection<(double X, double Y)>);
    }

    private void OnCollectionAssigned(ObservableCollection<double>? oldCol, ObservableCollection<double>? newCol, bool isXs, bool isYs)
    {
        // Unsubscribe from old collection events and subscribe to new collection events
        oldCol?.CollectionChanged -= Collections_CollectionChanged;
        newCol?.CollectionChanged += Collections_CollectionChanged;

        // Coalesce assignments so that if both Xs and Ys are set almost simultaneously we process them together.
        _debouncer.Debounce(() =>
        {
            // Immediately process the new collection to update the plot with any existing data.
            // This is done the UI thread to ensure thread safety when accessing ObservableCollection and updating the plot.
            // This ensures that if the collection already has items when assigned, they will be reflected in the plot right away.       
            DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
                {
                    // Si se asignaron Points desde el VM, preferimos reconstruir desde Points
                    if (Points is not null && Points.Count > 0)
                    {
                        RebuildFromPoints();
                        return;
                    }
                    else
                    {
                        // Si Ys ya existen y Xs se asignó, actualizamos solo Xs; si no, reconstruimos según disponibilidad
                        if (isXs && !isYs)
                        {
                            if (Xs is not null)
                            {
                                UpdateXs(Xs);
                                return;
                            }
                        }
                        else if (!isXs && isYs)
                        {
                            if (Ys is not null)
                            {
                                UpdateYs(Ys);
                                return;
                            }
                        }

                        // Si ambas colecciones están presentes y hay desalineación, reconstruimos para coherencia
                        if (Xs is not null && Ys is not null)
                        {
                            // Si la diferencia es grande o hay dudas, reconstruir
                            if (Math.Abs(Xs.Count - Ys.Count) > 0)
                                ProcessInitialCollections(rebuildAll: true);
                        }
                    }
                });
        });
    }

    private void OnPointsAssigned(ObservableCollection<(double X, double Y)>? oldCol, ObservableCollection<(double X, double Y)>? newCol)
    {
        if (oldCol is not null) oldCol.CollectionChanged -= Points_CollectionChanged;
        if (newCol is not null) newCol.CollectionChanged += Points_CollectionChanged;

        _debouncer.Debounce(() =>
        {
            DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
            {
                RebuildFromPoints();
            });
        });
    }

    /// <summary>
    /// Handles changes to the Xs or Ys collections and updates the plot accordingly.
    /// </summary>
    /// <remarks>This method ensures that collection changes are processed on the UI thread to maintain thread
    /// safety when updating UI-bound data. Only Add actions are handled directly for performance reasons; other actions
    /// such as Reset, Remove, or Replace are delegated to a separate handler.</remarks>
    /// <param name="sender">The collection that raised the event. Expected to be either the Xs or Ys collection.</param>
    /// <param name="e">The event data containing information about the type of change and the affected items.</param>
    private void Collections_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Ensure we are on the UI thread to safely access ObservableCollection and update the plot.
        DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
        {
            // Only handle Add actions here for performance. Other actions (Reset, Remove, Replace) can be handled in a more general way if needed.
            if (e.Action == NotifyCollectionChangedAction.Add && e.NewItems is not null)
            {
                if (ReferenceEquals(sender, Xs))
                {
                    foreach (double x in e.NewItems.Cast<double>()) HandleNewX(x);
                }
                else if (ReferenceEquals(sender, Ys))
                {
                    foreach (double y in e.NewItems.Cast<double>()) HandleNewY(y);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                // Reset action can be handled by clearing the internal lists and pending values, and then requesting a render.
                // This is a simple way to handle Reset, but it may not be the most efficient for large collections.
                lock (_sync)
                {
                    _xsList.Clear();
                    _ysList.Clear();
                    _pendingX = null;
                    _pendingY = null;
                }
                RequestRenderDebounced();
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove && e.OldItems is not null)
            {
                // Remove: eliminar por índice si se proporciona NewStartingIndex/OldStartingIndex
                // ObservableCollection normalmente rellena OldStartingIndex con la posición eliminada
                int idx = e.OldStartingIndex;
                if (idx >= 0)
                {
                    lock (_sync)
                    {
                        if (idx < _xsList.Count) _xsList.RemoveAt(idx);
                        if (idx < _ysList.Count) _ysList.RemoveAt(idx);
                    }
                    RequestRenderDebounced();
                }
                else
                {
                    // Si no hay índice fiable, reconstruir desde las colecciones
                    ProcessInitialCollections(rebuildAll: true);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Replace && e.NewItems is not null)
            {
                // Replace: actualizar por índice
                int idx = e.NewStartingIndex;
                if (idx >= 0)
                {
                    lock (_sync)
                    {
                        if (ReferenceEquals(sender, Xs))
                        {
                            if (idx < _xsList.Count) _xsList[idx] = (double)e.NewItems[0];
                        }
                        else if (ReferenceEquals(sender, Ys))
                        {
                            if (idx < _ysList.Count) _ysList[idx] = (double)e.NewItems[0];
                        }
                    }
                    RequestRenderDebounced();
                }
                else
                {
                    ProcessInitialCollections(rebuildAll: true);
                }
            }
            else
            {
                //// Less common actions (Reset, Remove, Replace) can be handled here if needed. For simplicity, we can just call a method to handle them.
                //HandleCollectionNonAdd(sender, e);
                // Otros casos: reconstruir para mantener coherencia
                ProcessInitialCollections(rebuildAll: true);
            }
        });

    }

    private void Points_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
        {
            // For simplicity: on any non-trivial change, rebuild from Points
            RebuildFromPoints();
        });
    }

    /// <summary>
    /// Processes the initial X and Y collections, pairing available elements and handling any pending unmatched values.
    /// </summary>
    /// <param name="rebuildAll">Indicates whether to rebuild the entire internal state from the collections, which may be necessary for actions like Reset.
    /// For Add actions, this can be false to optimize performance by only processing new items.</param>
    /// <remarks>This method attempts to match any previously pending X or Y values with new elements from the
    /// collections before adding new pairs. It ensures that only complete pairs are added and optimizes rendering by
    /// batching updates.</remarks>
    private void ProcessInitialCollections(bool rebuildAll = false)
    {
        var xs = Xs ?? [];
        var ys = Ys ?? [];

        lock (_sync)
        {
            if (rebuildAll)
            {
                _xsList.Clear();
                _ysList.Clear();
                _pendingX = null;
                _pendingY = null;
            }

            // In case there are pending X or Y from previous additions, try to match them first before adding new pairs
            if (_pendingX.HasValue && ys.Count > _ysList.Count)
            {
                // There is a pending X and a corresponding Y available, add the pair
                _xsList.Add(_pendingX.Value);
                _ysList.Add(ys[_ysList.Count]);
                _pendingX = null;
            }
            else if (_pendingY.HasValue && xs.Count > _xsList.Count)
            {
                // There is a pending Y and a corresponding X available, add the pair
                _xsList.Add(xs[_xsList.Count]);
                _ysList.Add(_pendingY.Value);
                _pendingY = null;
            }

            // índice desde el que aún no hemos copiado pares completos
            int existingPairs = Math.Min(_xsList.Count, _ysList.Count);
            int targetPairs = Math.Min(xs.Count, ys.Count);

            // Add pairs from existingPairs to targetPairs. This ensures we only add complete pairs and we won't add an X without its corresponding Y or vice versa.
            if (targetPairs > existingPairs)
            {
                // Add all new pairs from existingPairs to target (exclusive)
                for (int i = existingPairs; i < targetPairs; i++)
                {
                    _xsList.Add(xs[i]);
                    _ysList.Add(ys[i]);
                }

                // Only request render once after processing all new pairs for better performance
                RequestRenderDebounced();
            }

            // If the collections are missaligned (e.g., X has more items than Y or vice versa), we can store the next pending value for the next addition.
            // This handles the case where one collection has more items than the other, and we want to ensure that we only add complete pairs to the plot.
            if (xs.Count > ys.Count && xs.Count > _xsList.Count)
            {
                _pendingX = xs[_xsList.Count];
            }
            else if (ys.Count > xs.Count && ys.Count > _ysList.Count)
            {
                _pendingY = ys[_ysList.Count];
            }
        }
    }

    private void RebuildFromPoints()
    {
        if (Points is null) return;

        lock (_sync)
        {
            _xsList.Clear();
            _ysList.Clear();
            foreach (var p in Points)
            {
                _xsList.Add(p.X);
                _ysList.Add(p.Y);
            }
        }

        RequestRenderDebounced();
    }

    // Actualiza solo las Xs de forma eficiente manteniendo Ys
    public void UpdateXs(IList<double> xs)
    {
        if (xs is null) return;

        lock (_sync)
        {
            // Si no hay Ys, guardamos Xs internamente hasta que Ys estén disponibles
            if (_ysList.Count == 0 && (Ys is null || Ys.Count == 0))
            {
                // Guardar como reconstrucción completa cuando Ys lleguen
                _xsList.Clear();
                _xsList.AddRange(xs.Take(xs.Count));
                // No render hasta que haya Ys
                return;
            }

            int yCount = _ysList.Count;
            int newCount = Math.Min(xs.Count, yCount);

            // Replace existing
            int common = Math.Min(_xsList.Count, newCount);
            for (int i = 0; i < common; i++)
                _xsList[i] = xs[i];

            // Add new pairs if xs longer
            for (int i = _xsList.Count; i < newCount; i++)
            {
                _xsList.Add(xs[i]);
                // _ysList already has the corresponding Y
            }

            // Trim if xs shorter
            while (_xsList.Count > newCount)
            {
                int last = _xsList.Count - 1;
                _xsList.RemoveAt(last);
                if (last < _ysList.Count) _ysList.RemoveAt(last);
            }
        }

        RequestRenderDebounced();
    }

    // Actualiza solo las Ys de forma eficiente manteniendo Xs
    public void UpdateYs(IList<double> ys)
    {
        if (ys is null) return;

        lock (_sync)
        {
            // Si no hay Xs, guardamos Ys internamente hasta que Xs estén disponibles
            if (_xsList.Count == 0 && (Xs is null || Xs.Count == 0))
            {
                _ysList.Clear();
                _ysList.AddRange(ys.Take(ys.Count));
                return;
            }

            int xCount = _xsList.Count;
            int newCount = Math.Min(ys.Count, xCount);

            int common = Math.Min(_ysList.Count, newCount);
            for (int i = 0; i < common; i++)
                _ysList[i] = ys[i];

            for (int i = _ysList.Count; i < newCount; i++)
            {
                _ysList.Add(ys[i]);
            }

            while (_ysList.Count > newCount)
            {
                int last = _ysList.Count - 1;
                _ysList.RemoveAt(last);
                if (last < _xsList.Count) _xsList.RemoveAt(last);
            }
        }

        RequestRenderDebounced();
    }

    private void HandleNewX(double x)
    {
        lock (_sync)
        {
            // If there is a pending Y, pair it with this new X and add to the plot.
            // This handles the case where Y was added before X and we were waiting for X to complete the pair.
            if (_pendingY.HasValue)
            {
                _xsList.Add(x);
                _ysList.Add(_pendingY.Value);
                _pendingY = null;

                // Only when we have a complete pair (pending X + new Y) do we request a render, to avoid rendering for every single addition when they come in pairs.
                RequestRenderDebounced();
            }
            else
            {
                // If there is no pending Y, we have a new X without a corresponding Y yet. We will store this X as pending until we get the next Y. This handles the case where X is added before Y.
                // Note: if the collections are being added in pairs (X and Y added together), this will ensure that we only add complete pairs to the plot, and we won't add an X without its corresponding Y.
                _pendingX = x;
            }
        }
    }

    private void HandleNewY(double y)
    {
        lock (_sync)
        {
            if (_pendingX.HasValue)
            {
                // Pair the pending X with this new Y and add to the plot. This handles the case where X was added before Y and we were waiting for Y to complete the pair.
                _xsList.Add(_pendingX.Value);
                _ysList.Add(y);
                _pendingX = null;

                // Only when we have a complete pair (pending X + new Y) do we request a render, to avoid rendering for every single addition when they come in pairs.
                RequestRenderDebounced();
            }
            else
            {
                // If there is no pending X, we have a new Y without a corresponding X yet. We will store this Y as pending until we get the next X. This handles the case where Y is added before X.
                _pendingY = y;
            }
        }
    }

    /// <summary>
    /// Requests a render operation, ensuring that multiple rapid requests are coalesced into a single update.
    /// </summary>
    /// <remarks>This method schedules a refresh of the plot host using a debouncing mechanism to avoid
    /// redundant rendering. It is thread-safe and can be called multiple times in quick succession without causing
    /// unnecessary renders.</remarks>
    public void RequestRenderDebounced()
    {
        _debouncer.Debounce(() =>
        {
            // Llamar a Render en el host ScottPlot. Ajusta según tu host.
            DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
            {
                ForceRender();
                _plot.Axes.AutoScale();
            });
        });
    }

    /// <summary>
    /// Forces the plot to refresh and redraw its contents immediately.
    /// </summary>
    /// <remarks>Call this method to update the plot when changes have been made that are not automatically
    /// reflected. This is typically used when custom updates or external changes require the plot to be re-rendered
    /// outside of the normal update cycle.</remarks>
    public void ForceRender() => WinUIPlotHost.Refresh();
}
