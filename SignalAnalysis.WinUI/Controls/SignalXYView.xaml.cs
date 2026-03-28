using Microsoft.UI.Dispatching;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using ScottPlot;
using ScottPlot.Plottables;
using ScottPlot.WinUI;
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

    private readonly DebounceDispatcher _debouncer;
    private readonly ScottPlot.WinUI.WinUIPlot _winUIPlot;
    private readonly Plot _plot;
    private readonly SignalXY _signal;
    private List<double> _xsList = [];
    private List<double> _ysList = [];

    public SignalXYView()
    {
        InitializeComponent();

        _debouncer = new DebounceDispatcher(DispatcherQueue.GetForCurrentThread(), TimeSpan.FromMilliseconds(80));

        //for (int i = 0; i < 200; i++)
        //{
        //    _xsList.Add(i * 0.1);
        //    _ysList.Add(Math.Sin(_xsList[i]));
        //}

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
        WinUIPlotHost.Refresh();
    }

    static void OnXsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (SignalXYView)d;
        ctrl.OnCollectionAssigned(e.OldValue as ObservableCollection<double>, e.NewValue as ObservableCollection<double>, isX: true);
    }

    static void OnYsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var ctrl = (SignalXYView)d;
        ctrl.OnCollectionAssigned(e.OldValue as ObservableCollection<double>, e.NewValue as ObservableCollection<double>, isX: false);
    }

    void OnCollectionAssigned(ObservableCollection<double>? oldCol, ObservableCollection<double>? newCol, bool isX)
    {
        oldCol?.CollectionChanged -= Collections_CollectionChanged;

        newCol?.CollectionChanged += Collections_CollectionChanged;

        // Copiar inmediatamente para mantener consistencia
        UpdateArraysFromCollections();
        RequestRenderDebounced();
    }

    void Collections_CollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        UpdateArraysFromCollections();
        RequestRenderDebounced();
    }

    void UpdateArraysFromCollections()
    {
        var xs = Xs ?? [];
        var ys = Ys ?? [];

        int count = System.Math.Min(xs.Count, ys.Count);
        if (count == 0)
        {
            _xsList.Clear();
            _ysList.Clear();
            return;
        }

        // Copia eficiente: si las colecciones son listas internas, puedes optimizar.
        _xsList.AddRange(xs.Take(count));
        _ysList.AddRange(ys.Take(count));
        //_xsList = xs.Take(count).ToArray();
        //_ysList = ys.Take(count).ToArray();

        //// Actualiza el SignalXY con los nuevos arrays
        //_signal.Update(_xsArray, _ysArray);
    }

    public void RequestRenderDebounced()
    {
        _debouncer.Debounce(() =>
        {
            // Llamar a Render en el host ScottPlot. Ajusta según tu host.
            // Si usas un host que expone Render(), llámalo aquí.
            // Ejemplo genérico:
            //_plot.Render();
            //_winUIPlot.Refresh();
            DispatcherQueue.GetForCurrentThread().TryEnqueue(() =>
            {
                WinUIPlotHost.Refresh();
            });
        });
    }

    public void ForceRender() => WinUIPlotHost.Refresh();

}
