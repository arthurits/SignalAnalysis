using Microsoft.UI.Input;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media.Imaging;

using SignalAnalysis.Contracts.Services;
using SignalAnalysis.Converters;
using SignalAnalysis.Helpers;
using SignalAnalysis.Models;
using SignalAnalysis.ViewModels;
using System.Data;
using System.Diagnostics;
using System.Globalization;
using System.Text.Json;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;

namespace SignalAnalysis.Views;

/// <summary>
/// StartUpPage to be used as a template.
/// </summary>
public sealed partial class StartUpPage : Page, IDisposable
{
    public StartUpViewModel ViewModel { get; }
    private readonly ILocalizationService _localizationService;

    private List<string> PlotPalettes { get; set; } = [];

    private bool rememberFileDialogPath;

    public StartUpPage()
    {
        InitializeComponent();

        ViewModel = App.GetService<StartUpViewModel>();
        DataContext = ViewModel;
        _localizationService = App.GetService<ILocalizationService>();

        // Initialize the plot palettes
        ScottPlot.Palette.GetPalettes().ToList().ForEach(x => PlotPalettes.Add(x.Name));
    }

    public void Dispose()
    {
    }

    private void Page_Loaded(object sender, RoutedEventArgs e)
    {
        rememberFileDialogPath = true;
        //rememberFileDialogPath = _settings.RememberFileDialogPath;
        //maxTasks = _settings.MaxTasks;
        //ViewModel.MaxTasks = maxTasks;

        plotPaletteCombo.SelectedValue = ViewModel.PlotPalette;

        //int items = DataTable.Items.Count;
        //AddButton.IsEnabled = items >= 0 && items < maxTasks;
        //DuplicateButton.IsEnabled = DuplicateButton.IsEnabled && items > 0 && items < maxTasks;
        //ExampleButton.IsEnabled = items >= 0 && items < maxTasks;
    }

    private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        ScrollViewPage.Height = ActualHeight;
        ScrollViewPage.Width = ActualWidth;
    }

    private void TaskItemBorder_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var border = (Border)sender;
        var r = border.ActualHeight / 2;
        border.CornerRadius = new CornerRadius(r);
        //border.Background   = new SolidColorBrush(Microsoft.UI.Colors.Transparent);
    }

    private async void SavePlot_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
    }

    private void ShowLegend_Click(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        if (ViewModel.PlotLegendChecked)
        {
            //PlotSimpleTasks.Plot.ShowLegend();
            //PlotCompositeTasks.Plot.ShowLegend();
            PlotTaskFactors.Plot.ShowLegend();
        }
        else
        {
            //PlotSimpleTasks.Plot.HideLegend();
            //PlotCompositeTasks.Plot.HideLegend();
            PlotTaskFactors.Plot.HideLegend();
        }

        //PlotSimpleTasks.Refresh();
        //PlotCompositeTasks.Refresh();
        PlotTaskFactors.Refresh();
    }

    private void UpdatePlotSimpleTasks_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is AppBarToggleButton toggleButton)
        {
            if (toggleButton.IsChecked == true)
            {
                //CreatePlotSimple();
                //CreatePlotFactors(series: Math.Min(plotSeries.SelectedIndex - 1, ViewModel.AllTaskData.Count - 2));
            }
        }
    }

    private void UpdatePlotCompositeTasks_Checked(object sender, RoutedEventArgs e)
    {
        if (sender is AppBarToggleButton toggleButton)
        {
            if (toggleButton.IsChecked == true)
            {
                //CreatePlotComposite();
            }
        }
    }

    private void Series_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox combo)
        {
            if (combo.SelectedIndex == -1)
            {
                combo.SelectedIndex = ViewModel.PlotSeries.Count - 1;
            }

            var selectedItem = (PlotSeries)combo.SelectedItem;

            //ClearPlots();
            //DrawPlots(series: selectedItem.Item2);

            // For the time being, changing the series selection only modifies the factors plot
            PlotTaskFactors.Plot.Clear();
            //CreatePlotFactors(series: selectedItem.Value);
        }
    }

    private void Palette_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox combo)
        {
            if (combo.SelectedValue.ToString() != ViewModel.PlotPalette)
            {
                return;
            }

            // This is not needed, since plots are custom-drawn
            var selectedPalette = ScottPlot.Palette.GetPalettes().Cast<ScottPlot.IPalette>().Where(x => x.Name == ViewModel.PlotPalette).First();
            signalXYView.GetPlot().Add.Palette = selectedPalette;
            signalXYView.ForceRender();
            //PlotSimpleTasks.Plot.Add.Palette = selectedPalette;
            //PlotCompositeTasks.Plot.Add.Palette = selectedPalette;
            //PlotTaskFactors.Plot.Add.Palette = selectedPalette;

            //ClearPlots();
            //CreatePlotSimple();
            //CreatePlotComposite();
            //CreatePlotFactors(series: Math.Min(plotSeries.SelectedIndex - 1, ViewModel.AllTaskData.Count - 2));
        }
    }

    #region OpenFile

    private async void OpenFile_Drop(object sender, DragEventArgs e)
    {
        if (e.DataView.Contains(StandardDataFormats.StorageItems))
        {
            var items = await e.DataView.GetStorageItemsAsync();
            if (items.Count == 1)
            {
                var storageFile = items[0] as StorageFile;

                if (storageFile?.FileType == ".sig")
                {
                    _ = await OpenFile(storageFile);
                }
                else
                {
                    _ = await MessageBox.Show(
                        messageBoxText: "MsgBoxFileTypeContent".GetLocalized("MessageBox"),
                        caption: "MsgBoxFileTypeTitle".GetLocalized("MessageBox"),
                        primaryButtonText: "MsgBoxFileTypePrimary".GetLocalized("MessageBox"),
                        icon: MessageBox.MessageBoxImage.Error);
                }
            }
            else
            {
                _ = await MessageBox.Show(
                    messageBoxText: "MsgBoxDropOneFileContent".GetLocalized("MessageBox"),
                    caption: "MsgBoxDropOneFileTitle".GetLocalized("MessageBox"),
                    primaryButtonText: "MsgBoxDropOneFilePrimary".GetLocalized("MessageBox"),
                    icon: MessageBox.MessageBoxImage.Error);
            }
        }
    }

    private void OpenFile_DragOver(object sender, DragEventArgs e)
    {
        e.AcceptedOperation = DataPackageOperation.Copy;

        // Sets custom UI text
        e.DragUIOverride.Caption = "Open Signal file";

        // Sets a custom glyph
        e.DragUIOverride.SetContentFromBitmapImage(
            new BitmapImage(new Uri("ms-appx:///Assets/DragLiberty.png", UriKind.RelativeOrAbsolute)));

        e.DragUIOverride.IsCaptionVisible = true; // Sets if the caption is visible
        e.DragUIOverride.IsContentVisible = true; // Sets if the dragged content is visible
        e.DragUIOverride.IsGlyphVisible = true; // Sets if the glyph is visibile
    }
    private async void OpenFile_Click(object sender, RoutedEventArgs e)
    {
        // If there is some data already entered, inform that this action will overwrite it
        //if (ViewModel.AllTaskData.Count > 0)
        //{
        //    var result = await MessageBox.Show(
        //        App.MainWindow.Content.XamlRoot,
        //        $"Opening a file will overwrite current data.{Environment.NewLine}Are you sure you want to continue?",
        //        "Data overwrite",
        //        "Yes",
        //        "No",
        //        defaultButton: MessageBox.MessageBoxButtonDefault.SecondaryButton,
        //        icon: MessageBox.MessageBoxImage.Question);
        //    if (result == ContentDialogResult.Secondary)
        //    {
        //        return;
        //    }
        //}

        // Modify cursor shape
        var currentCursor = this.ProtectedCursor;
        this.ProtectedCursor = InputSystemCursor.Create(InputSystemCursorShape.Wait);

        // Create a file picker
        var openPicker = new Windows.Storage.Pickers.FileOpenPicker();

        // Initialize the file picker with the window handle (HWND)
        var window = App.MainWindow;
        var hWnd = WinRT.Interop.WindowNative.GetWindowHandle(window);
        WinRT.Interop.InitializeWithWindow.Initialize(openPicker, hWnd);

        // Set options for your file picker
        openPicker.ViewMode = PickerViewMode.Thumbnail;
        openPicker.FileTypeFilter.Add(".sig");
        openPicker.FileTypeFilter.Add(".elux");

        if (!rememberFileDialogPath)
        {
            var random = new Random(Convert.ToInt32(DateTime.Now.Ticks & 0x0000FFFF));
            openPicker.SettingsIdentifier = random.ToString();
            openPicker.SuggestedStartLocation = PickerLocationId.Desktop;
        }

        // Open the picker for the user to pick a file
        var file = await openPicker.PickSingleFileAsync();
        if (file is not null)
        {
            var result = await OpenFile(file);
        }

        // Set the cursor to the previous one
        this.ProtectedCursor = currentCursor;
    }

    private async Task<bool> OpenFile(StorageFile? file)
    {
        if (file is null)
        {
            _ = await MessageBox.Show(
                messageBoxText: "MsgBoxOpenFileErrorContent".GetLocalized("MessageBox"),
                caption: "MsgBoxOpenFileErrorTitle".GetLocalized("MessageBox"),
                primaryButtonText: "MsgBoxOpenFileErrorPrimary".GetLocalized("MessageBox"),
                icon: MessageBox.MessageBoxImage.Information);
            return false;
        }

        try
        {
            string jsonString = await FileIO.ReadTextAsync(file, Windows.Storage.Streams.UnicodeEncoding.Utf8);
            return await OpenJsonDocument(jsonString, file.FileType);
        }
        catch (Exception ex)
        {
            // Log o mostrar mensaje según tu infraestructura
            Debug.WriteLine(ex);
            return false;
        }
    }

    /// <summary>
    /// Parses data from a json string
    /// </summary>
    /// <param name="jsonString">String containing the data</param>
    /// <returns><see langword="True"/> if successful, <see langword="false"/> otherwise</returns>
    private async Task<bool> OpenJsonDocument(string jsonString, string fileType)
    {

        try
        {
            // 1) Intentar interpretar como JSON
            bool handled = false;

            try
            {
                // Intento de deserializar a DocumentBase (o a tipos concretos)
                var optionsForRead = new JsonSerializerOptions { PropertyNamingPolicy = new EluxlNamingPolicy() };
                // Añade converters si los necesitas
                // optionsForRead.Converters.Add(new LocalizedDateTimeConverter(...));

                // Si conoces el tipo raíz, intenta deserializarlo
                DocumentBase docFromJson = DocumentFactory.DeserializeFromJson(jsonString);
                if (docFromJson is not null)
                {
                    // Trabaja con docFromJson
                    handled = true;
                }
            }
            catch (JsonException)
            {
                // No es JSON válido, seguir al parseo por texto
            }

            if (!handled)
            {
                // 2) Tratar como texto plano (.sig o .elux)
                var lines = jsonString.Split(["\r\n", "\n"], StringSplitOptions.None);

                // Ejemplo: parseo específico para SignalDto
                try
                {
                    var dto = DocumentFactory.ParseFromText(lines);

                    if (dto is not null)
                    {
                        // Opcional: serializar/deserializar para validar
                        var options = dto.CreateJsonOptions(serializeDoublesAsStrings: false);
                        var json = JsonSerializer.Serialize(dto, options);

                        var dto2 = JsonSerializer.Deserialize<SignalDto>(json, options);

                        // Si necesitas convertir a EluxlDto con converters:
                        var optionsForRead = new JsonSerializerOptions { PropertyNamingPolicy = new EluxlNamingPolicy() };
                        optionsForRead.Converters.Add(new LocalizedDateTimeConverter(new CultureInfo(dto.CultureName)));
                        optionsForRead.Converters.Add(new LocalizedTimeSpanConverter(new CultureInfo(dto.CultureName)));
                        var dto3 = JsonSerializer.Deserialize<EluxlDto>(json, optionsForRead);

                        // Trabaja con dto / dto2 / dto3 según corresponda
                        return true;
                    }
                }
                catch
                {
                    // Ignorar y probar parseo genérico
                }

                // 3) Parseo genérico con DocumentFactory
                try
                {
                    DocumentBase doc = DocumentFactory.ParseFromText(lines);
                    ViewModel.DocumentDto = doc;
                    if (doc is EluxlDto docElux)
                    {
                        //ViewModel.PlotSeries.Clear();
                        //// Añadir cada nombre como PlotSeries, conservando el índice
                        //int index = 0;
                        //foreach (var name in docElux.SeriesNames)
                        //{
                        //    ViewModel.PlotSeries.Add(new PlotSeries(name, index));
                        //    index++;
                        //}
                        ////// LINQ alternative
                        ////foreach (var item in docElux.SeriesNames.Select((name, i) => new PlotSeries(name, i)))
                        ////{
                        ////    ViewModel.PlotSeries.Add(item);
                        ////}
                    }
                    else if (doc is SignalDto sig)
                    {
                        // trabajar con sig
                    }
                    else
                    {
                        // tipo desconocido
                    }

                    return true;
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex);
                    return false;
                }
            }

            return true;
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex);
            return false;
        }

        //// Leer fichero
        //var lines = File.ReadAllLines("miarchivo.sig");
        //var dto = SignalDto.ParseFromSigText(lines);

        //// Serializar a JSON (opcional: guardar doubles como strings con coma decimal)
        //var options = dto.CreateJsonOptions(serializeDoublesAsStrings: false);
        //var json = JsonSerializer.Serialize(dto, options);

        //// Deserializar (asegúrate de crear las mismas opciones si usaste converter de doubles)
        //var dto2 = JsonSerializer.Deserialize<SignalDto>(json, options);

        //var optionsForRead = new JsonSerializerOptions { PropertyNamingPolicy = new EluxlNamingPolicy() };
        //optionsForRead.Converters.Add(new LocalizedDateTimeConverter(new CultureInfo(dto.CultureName)));
        //optionsForRead.Converters.Add(new LocalizedTimeSpanConverter(new CultureInfo(dto.CultureName)));
        //var dto3 = JsonSerializer.Deserialize<EluxlDto>(json, optionsForRead);

        //// 1) Parsear desde texto (archivo .eluxl o .sig)
        //var lines2 = File.ReadAllLines("miarchivo.sig");
        //DocumentBase doc = DocumentFactory.ParseFromText(lines2);
        //if (doc is EluxlDto elux) { /* trabajar con elux */ }
        //if (doc is SignalDto sig) { /* trabajar con sig */ }

        //// 2) Deserializar desde JSON (detecta tipo y cultura)
        //DocumentBase docFromJson = DocumentFactory.DeserializeFromJson(json);

        //// 3) Serializar a JSON (valida cabecera)
        //string jsonOut = DocumentFactory.SerializeToJson(doc);

        //// Deserialize the json string to get the header information
        //JobJsonDto? data;
        //try
        //{
        //    data = JsonSerializer.Deserialize<JobJsonDto>(jsonString, _jsonSerializerOptions);
        //}
        //catch (JsonException ex)
        //{
        //    System.Diagnostics.Debug.WriteLine($"JSON format error: {ex.Message}");
        //    await MessageBox.Show(
        //        App.MainWindow.Content.XamlRoot,
        //        $"The selected file does not contain valid data{System.Environment.NewLine}or it is corrupted.",
        //        "Reading error",
        //        "OK",
        //        icon: MessageBox.MessageBoxImage.Error);
        //    return false;
        //}
        //catch (Exception ex)
        //{
        //    System.Diagnostics.Debug.WriteLine($"Unexpected error while reading Json file: {ex.Message}");
        //    await MessageBox.Show(
        //        App.MainWindow.Content.XamlRoot,
        //        "Unexpected error while reading the file.",
        //        "Error",
        //        "OK",
        //        icon: MessageBox.MessageBoxImage.Error);
        //    return false;
        //}

        //if (data == null)
        //{
        //    await MessageBox.Show(
        //        App.MainWindow.Content.XamlRoot,
        //        "File format mismatch",
        //        "Error",
        //        "OK",
        //        icon: MessageBox.MessageBoxImage.Error);
        //    return false;
        //}

        //// Get the header information
        //var modelType = data.DocumentType ?? string.Empty;
        //var fileVersion = data.FileVersion;

        //// Process the data depending on the model type and version
        //bool result;
        //if (modelType == "Lifting model")
        //{
        //    result = fileVersion switch
        //    {
        //        -1 => false,
        //        0 => false,
        //        1.0 => ModelDataFromJson_v_1(data),
        //        _ => false,
        //    };

        //    if (result == false)
        //    {
        //        _ = await MessageBox.Show(
        //            App.MainWindow.Content.XamlRoot,
        //            "Not a Lifting model file type",
        //            "Error",
        //            "OK",
        //            icon: MessageBox.MessageBoxImage.Error);
        //    }
        //}
        //else if (modelType == "LM-MMH model")
        //{
        //    var msgResult = await MessageBox.Show(
        //        App.MainWindow.Content.XamlRoot,
        //        "This file contains data for the 'LM-MMH' model.\nAre you sure want to open it?",
        //        "Data type",
        //        "Yes",
        //        "No",
        //        icon: MessageBox.MessageBoxImage.Question);

        //    if (msgResult == ContentDialogResult.Primary)
        //    {
        //        Frame.Navigate(typeof(LibertyPage), jsonString);
        //    }

        //    result = true;
        //}
        //else
        //{
        //    _ = await MessageBox.Show(
        //        App.MainWindow.Content.XamlRoot,
        //        "File format mismatch",
        //        "Error",
        //        "OK",
        //        icon: MessageBox.MessageBoxImage.Error);
        //    result = false;
        //}

        //return result;
        return true;
    }


    /// <summary>
    /// Retrieves the model data from the json string
    /// </summary>
    /// <returns></returns>
    //private bool ModelDataFromJson_v_1(JobJsonDto data)
    //{
    //    bool result = true;

    //    try
    //    {
    //        // Build SimpleTasks from AllTasks and SimpleTaskIndices
    //        var allTasks = data.AllTasks ?? [];
    //        var simpleIndexSet = data.SimpleTaskIndices?.ToHashSet() ?? [];
    //        var simpleTasks = allTasks
    //            .Where(t => simpleIndexSet.Contains(t.TaskIndex))
    //            .ToList();

    //        //var simpleFromLocation = allTasks
    //        //    .Where(t => t.TaskLocation is null || t.TaskLocation.IsTaskIn_Simple)
    //        //    .ToList();

    //        //// Join both filters and delete any duplicate element based on TaskIndex
    //        //var simpleTasks = simpleFromIndices
    //        //    .Concat(simpleFromLocation)
    //        //    .GroupBy(t => t.TaskIndex)
    //        //    .Select(g => g.First())
    //        //    .ToList();

    //        // Expand each task by its index and group by (ContainerType, TaskIndex)
    //        // --- Construir lookup general por (TaskContainerType, index) --- 
    //        // Asumimos que TaskContainerType tiene valores como: Simple, Composite, Variable, Sequential 
    //        // Mapeamos: CLI -> Composite, VLI -> Variable, SLI -> Sequential
    //        var tasksLookup = allTasks
    //            .SelectMany(t =>
    //            {
    //                var loc = t.TaskLocation;
    //                if (loc == null)
    //                {
    //                    // Si no hay TaskLocation, considerarlo simple (ya lo hemos añadido a simpleTasks)
    //                    return Enumerable.Empty<((TaskContainerType, int) Key, SimpleTask Task)>();
    //                }
    //                // Recolectar pares (ContainerType, index) según las listas no vacías
    //                var pairs = new List<((TaskContainerType, int), SimpleTask)>();
    //                if (loc.ContainerIndex_CLI is { Count: > 0 })
    //                {
    //                    pairs.AddRange(loc.ContainerIndex_CLI
    //                        .Distinct()
    //                        .Select(idx => ((TaskContainerType.Composite, idx), t)));
    //                }
    //                if (loc.ContainerIndex_VLI is { Count: > 0 })
    //                {
    //                    pairs.AddRange(loc.ContainerIndex_VLI
    //                        .Distinct()
    //                        .Select(idx => ((TaskContainerType.Variable, idx), t)));
    //                }
    //                if (loc.ContainerIndex_SLI is { Count: > 0 })
    //                {
    //                    pairs.AddRange(loc.ContainerIndex_SLI
    //                        .Distinct()
    //                        .Select(idx => ((TaskContainerType.Sequential, idx), t)));
    //                }
    //                // Si ninguna lista tiene elementos, la tarea es "simple" y no necesita entrar en el lookup
    //                return pairs;
    //            })
    //            .ToLookup(x => x.Key, x => x.Task);

    //        //// Example of accessing tasks by (ContainerType, TaskIndex)
    //        //var test = tasksLookup[(TaskContainerType.Simple, 0)];

    //        // Build CompositeTasks and its SubTasks
    //        var compositeTasks = data.CompositeTasks ?? [];
    //        foreach (var comp in compositeTasks)
    //        {
    //            // Look for (Composite, comp.TaskIndex)
    //            var key = (TaskContainerType.Composite, comp.TaskIndex);
    //            var subtasks = tasksLookup[key] ?? [];

    //            // Add subtasks to the CompositeTask
    //            comp.SubTasks.Clear();
    //            foreach (var st in subtasks)
    //            {
    //                comp.SubTasks.Add(st);
    //            }
    //            //comp.SubTasks.add = new ObservableCollection<SimpleTask>(subtasks);
    //        }

    //        // Build VariableTasks and its SubTasks
    //        var variableTasks = data.VariableTasks ?? [];
    //        foreach (var comp in variableTasks)
    //        {
    //            // Look for (Composite, comp.TaskIndex)
    //            var key = (TaskContainerType.Variable, comp.TaskIndex);
    //            var subtasks = tasksLookup[key] ?? [];

    //            // Add subtasks to the CompositeTask
    //            comp.SubTasks.Clear();
    //            foreach (var st in subtasks)
    //            {
    //                comp.SubTasks.Add(st);
    //            }
    //            //comp.SubTasks.add = new ObservableCollection<SimpleTask>(subtasks);
    //        }

    //        // Build SequentialTasks and its SubTasks
    //        var sequentialTasks = data.SequentialTasks ?? [];
    //        foreach (var comp in sequentialTasks)
    //        {
    //            // Look for (Composite, comp.TaskIndex)
    //            var key = (TaskContainerType.Sequential, comp.TaskIndex);
    //            var subtasks = tasksLookup[key] ?? [];

    //            // Add subtasks to the CompositeTask
    //            comp.SubTasks.Clear();
    //            foreach (var st in subtasks)
    //            {
    //                comp.SubTasks.Add(st);
    //            }
    //            //comp.SubTasks.add = new ObservableCollection<SimpleTask>(subtasks);
    //        }

    //        // Create the final Job object
    //        Job _job = new()
    //        {
    //            AllTasks = allTasks,
    //            SimpleTasks = simpleTasks,
    //            CompositeTasks = compositeTasks,
    //            VariableTasks = variableTasks,
    //            SequentialTasks = sequentialTasks,
    //        };

    //        //// For each CompositeTask, build its SubTasks from AllTasks and JsonSubTaskIndices
    //        //var i = 0;
    //        //foreach (var ct in _job.CompositeTasks)
    //        //{
    //        //    //var indices = new HashSet<int>(ct.JsonSubTaskIndices);
    //        //    //ct.SubTasks = new ObservableCollection<SimpleTask>(
    //        //    //    (data.AllTasks ?? [])
    //        //    //        .Where(st => indices.Contains(st.TaskIndex))
    //        //    //        .ToList()
    //        //    //);
    //        //    var subtasks = (data.AllTasks ?? []).Where(st => st.NewTaskLocation.ContainerType == TaskContainerType.Composite && st.NewTaskLocation.TaskIndex == i).ToList();
    //        //    foreach (var st in subtasks)
    //        //    {
    //        //        ct.SubTasks.Add(st);
    //        //    }
    //        //}

    //        ViewModel.Job = _job;
    //    }
    //    catch
    //    {
    //        result = false;
    //    }

    //    return result;
    //}

    #endregion OpenFile

}
