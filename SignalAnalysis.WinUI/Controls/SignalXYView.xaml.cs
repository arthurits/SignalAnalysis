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

public sealed partial class SignalXYView : UserControl
{
    public static readonly DependencyProperty XsProperty =
            DependencyProperty.Register(nameof(Xs), typeof(ObservableCollection<double>), typeof(SignalXYView),
                new PropertyMetadata(null, OnXsChanged));

    public static readonly DependencyProperty YsProperty =
        DependencyProperty.Register(nameof(Ys), typeof(ObservableCollection<double>), typeof(SignalXYView),
            new PropertyMetadata(null, OnYsChanged));

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

    // Método público para que la vista que contiene este control pueda añadir el host visual
    // y enlazar el Plot creado aquí al host concreto de ScottPlot WinUI.
    public Plot GetPlot() => _plot;

    // Internal property to hold the reference to the ScottPlot host control, which is used to call Refresh() when needed.
    // This should be set by the parent view after adding the host control to the visual tree.
    private readonly Lock _sync = new();
    private readonly DebounceDispatcher _debouncer;
    private readonly ScottPlot.WinUI.WinUIPlot _winUIPlot;
    private readonly Plot _plot;
    private readonly SignalXY _signal;

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
    

    public SignalXYView()
    {
        InitializeComponent();

        _debouncer = new DebounceDispatcher(DispatcherQueue.GetForCurrentThread(), TimeSpan.FromMilliseconds(80));

        // Crear Plot y SignalXY
        _winUIPlot = new();
        _plot = _winUIPlot.Plot;
        _signal = _plot.Add.SignalXY(_xsList, _ysList);

        // Insertar el host visual de ScottPlot en el Grid RootGrid
        // Ajusta la siguiente línea al control WinUI que uses para renderizar ScottPlot.
        // Ejemplo genérico: si usas ScottPlot.WinUI.WinUIPlot, crea una instancia y asígnale _plot.
        // Aquí dejamos un placeholder: el usuario debe reemplazar por su host concreto.
        RootGrid.Children.Add(_winUIPlot); // <-- reemplazar por tu host
        WinUIPlotHost = _winUIPlot; // <-- si tu host tiene una propiedad Plot, asígnale el plot creado aquí.
        //WinUIPlotHost.Refresh();
    }

    private static void OnXsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (SignalXYView)d;
        ctrl.OnCollectionAssigned(e.OldValue as ObservableCollection<double>, e.NewValue as ObservableCollection<double>);
    }

    private static void OnYsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (SignalXYView)d;
        ctrl.OnCollectionAssigned(e.OldValue as ObservableCollection<double>, e.NewValue as ObservableCollection<double>);
    }

    private void OnCollectionAssigned(ObservableCollection<double>? oldCol, ObservableCollection<double>? newCol)
    {
        // Unsubscribe from old collection events and subscribe to new collection events
        oldCol?.CollectionChanged -= Collections_CollectionChanged;
        newCol?.CollectionChanged += Collections_CollectionChanged;

        // Immediately process the new collection to update the plot with any existing data.
        // This is done the UI thread to ensure thread safety when accessing ObservableCollection and updating the plot.
        // This ensures that if the collection already has items when assigned, they will be reflected in the plot right away.       
        DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
        {
            ProcessInitialCollections();
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

            //// Temporarily store the new X if there isn't a corresponding Y yet. We will add it to the plot once we get the matching Y.
            //if (_ysList.Count < _xsList.Count + 1)
            //{
            //    // There is no corresponding Y for this new X yet, store it as pending until we get the next Y. This handles the case where X is added before Y.
            //    _pendingX = x;
            //}
            //else
            //{
            //    // There is already a corresponding Y for this new X, we can add it directly to the plot. This handles the case where Y is added before X.
            //    _xsList.Add(x);
            //    RequestRenderDebounced();
            //}
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
    /// Handles collection change events for actions other than Add, such as Reset, Remove, or Replace, to update
    /// internal state accordingly.
    /// </summary>
    /// <remarks>This method responds to Reset, Remove, and Replace actions by reprocessing or clearing
    /// internal data structures to maintain consistency with the observed collection. For large collections,
    /// reprocessing may be less efficient.</remarks>
    /// <param name="sender">The source of the collection change event. This is typically the collection being observed.</param>
    /// <param name="e">The event data containing information about the type of collection change and the affected items.</param>
    private void HandleCollectionNonAdd(object? sender, NotifyCollectionChangedEventArgs e)
    {
        // Implement handling for Reset, Remove, Replace actions if needed.
        // For simplicity, we can just clear and reprocess everything on Reset, and for Remove/Replace we can also just reprocess everything,
        // but this is less efficient for large collections.
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            _xsList.Clear();
            _ysList.Clear();
            _pendingX = null;
            _pendingY = null;
            RequestRenderDebounced();
        }
        // For remove/replace actions, you would need to identify which items were removed/replaced and update _xsList/_ysList accordingly,
        // which can be more complex depending on how you want to handle it.
        // For simplicity, we can just call ProcessInitialCollections() to re-sync everything, but this is less efficient for large collections.
        else
        {
            ProcessInitialCollections();
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
