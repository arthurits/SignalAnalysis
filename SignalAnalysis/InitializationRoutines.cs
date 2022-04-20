namespace SignalAnalysis;

partial class FrmMain
{
    /// <summary>
    /// Initialize the tool strip panel and all the controls inside
    /// </summary>
    /// <seealso>https://docs.microsoft.com/en-us/dotnet/api/system.windows.forms.toolstrippanel?view=windowsdesktop-6.0
    // https://stackoverflow.com/questions/40382105/how-to-add-two-toolstripcombobox-and-separator-horizontally-to-one-toolstripdrop</seealso>
    private void InitializeToolStripPanel()
    {
        Font toolFont = new("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);

        stripComboSeries = new()
        {
            AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend,
            AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None,
            DropDownHeight = 110,
            DropDownWidth = 122,
            FlatStyle = System.Windows.Forms.FlatStyle.Standard,
            Font = toolFont,
            IntegralHeight = true,
            MaxDropDownItems = 9,
            MergeAction = System.Windows.Forms.MergeAction.MatchOnly,
            Name = "cboSeries",
            Size = new System.Drawing.Size(120, 25),
            Sorted = false,
            ToolTipText = "Select data series"
        };
        stripComboWindows = new()
        {
            AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend,
            AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.None,
            DropDownHeight = 110,
            DropDownWidth = 122,
            FlatStyle = System.Windows.Forms.FlatStyle.Standard,
            Font = toolFont,
            IntegralHeight = true,
            MaxDropDownItems = 9,
            MergeAction = System.Windows.Forms.MergeAction.MatchOnly,
            Name = "cboWindows",
            Size = new System.Drawing.Size(120, 25),
            Sorted = false,
            ToolTipText = "Select FFT window"
        };
        stripComboSeries.SelectedIndexChanged += ComboSeries_SelectedIndexChanged;
        stripComboWindows.SelectedIndexChanged += ComboWindow_SelectedIndexChanged;

        tspTop = new();
        tspTop.Dock = DockStyle.Top;
        tspTop.Name = "StripPanelTop";

        ToolStrip toolStripMain = new()
        {
            Font = toolFont,
            ImageScalingSize = new System.Drawing.Size(48, 48),
            Location = new System.Drawing.Point(0, 0),
            Renderer = new customRenderer<ToolStripButton>(System.Drawing.Brushes.SteelBlue, System.Drawing.Brushes.LightSkyBlue),
            RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional,
            TabIndex = 1,
            Text = "Main toolbar"
        };

        ToolStripItem toolStripItem;
        toolStripItem = toolStripMain.Items.Add("Exit", new System.Drawing.Icon(EmbeddedResources.IconExit, 48, 48).ToBitmap(), new EventHandler(Exit_Click));
        toolStripItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
        toolStripItem.Name = "Exit";
        toolStripItem = toolStripMain.Items.Add("Open", new System.Drawing.Icon(EmbeddedResources.IconOpen, 48, 48).ToBitmap(), new EventHandler(Open_Click));
        toolStripItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
        toolStripItem.Name = "Open";
        toolStripItem = toolStripMain.Items.Add("Export", new System.Drawing.Icon(EmbeddedResources.IconExport, 48, 48).ToBitmap(), new EventHandler(Export_Click));
        toolStripItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
        toolStripItem.Name = "Export";
        toolStripMain.Items.Add(new ToolStripSeparator());
        toolStripMain.Items.Add(stripComboSeries);
        toolStripMain.Items.Add(stripComboWindows);
        toolStripMain.Items.Add(new ToolStripSeparator());
        toolStripItem = toolStripMain.Items.Add("Settings", new System.Drawing.Icon(EmbeddedResources.IconSettings, 48, 48).ToBitmap(), new EventHandler(Settings_Click));
        toolStripItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
        toolStripItem.Name = "Settings";
        toolStripMain.Items.Add(new ToolStripSeparator());
        toolStripItem = toolStripMain.Items.Add("About", new System.Drawing.Icon(EmbeddedResources.IconAbout, 48, 48).ToBitmap(), new EventHandler(About_Click));
        toolStripItem.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
        toolStripItem.Name = "About";

        tspTop.Join(toolStripMain);
        this.Controls.Add(tspTop);

        return;
    }

    /// <summary>
    /// Initialize the status strip and all the controls inside
    /// </summary>
    private void InitializeStatusStrip()
    {
        tspBottom = new();
        tspBottom.Dock = DockStyle.Bottom;
        tspBottom.Name = "StripPanelBottom";

        StatusStrip statusStrip = new()
        {
            Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point),
            ImageScalingSize = new System.Drawing.Size(16, 16),
            Name = "StatusStrip",
            ShowItemToolTips = true,
            Renderer = new customRenderer<ToolStripStatusLabelEx>(Brushes.SteelBlue, Brushes.LightSkyBlue),
            RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional,
            Size = new System.Drawing.Size(934, 28),
            TabIndex = 1,
            Text = "Status bar"
        };

        int item;
        item = statusStrip.Items.Add(new ToolStripStatusLabel()
        {
            AutoSize = true,
            BorderSides = ToolStripStatusLabelBorderSides.Right,
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            Name = "LabelEmpty",
            Spring = true,
            TextAlign = System.Drawing.ContentAlignment.MiddleLeft,
            ToolTipText = ""
        });

        stripLblCulture = new ToolStripStatusLabel()
        {
            AutoSize = false,
            BorderSides = ToolStripStatusLabelBorderSides.Right,
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            Name = "LabelCulture",
            Size = new System.Drawing.Size(60, 23),
            TextAlign = System.Drawing.ContentAlignment.MiddleCenter,
            ToolTipText = "User interface language"
        };
        stripLblCulture.Click += Language_Click;
        item = statusStrip.Items.Add(stripLblCulture);

        item = statusStrip.Items.Add(new ToolStripStatusLabelEx()
        {
            AutoSize = false,
            Checked = false,    // Inverse because we are later simulating a click
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            Name = "LabelExPower",
            Size = new System.Drawing.Size(28, 23),
            Text = "P",
            ToolTipText = "Power spectra (dB)"
        });
        statusStrip.Items[item].Click += LabelEx_Click;
        ((ToolStripStatusLabelEx)statusStrip.Items[item]).CheckedChanged += LabelEx_CheckedChanged;
        statusStrip.Items[item].PerformClick();

        item = statusStrip.Items.Add(new ToolStripStatusLabelEx()
        {
            AutoSize = false,
            Checked = true,    // Inverse because we are later simulating a click
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            Name = "LabelExCumulative",
            Size = new System.Drawing.Size(28, 23),
            Text = "F",
            ToolTipText = "Cumulative fractal dimension"
        });
        statusStrip.Items[item].Click += LabelEx_Click;
        ((ToolStripStatusLabelEx)statusStrip.Items[item]).CheckedChanged += LabelEx_CheckedChanged;
        statusStrip.Items[item].PerformClick();

        item = statusStrip.Items.Add(new ToolStripStatusLabelEx()
        {
            AutoSize = false,
            Checked = true,    // Inverse because we are later simulating a click
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            Name = "LabelExEntropy",
            Size = new System.Drawing.Size(28, 23),
            Text = "E",
            ToolTipText = "Approximate and sample entropy"
        });
        statusStrip.Items[item].Click += LabelEx_Click;
        ((ToolStripStatusLabelEx)statusStrip.Items[item]).CheckedChanged += LabelEx_CheckedChanged;
        statusStrip.Items[item].PerformClick();

        item = statusStrip.Items.Add(new ToolStripStatusLabelEx()
        {
            AutoSize = false,
            Checked = true,    // Inverse because we are later simulating a click
            DisplayStyle = ToolStripItemDisplayStyle.Text,
            Name = "LabelExCrossHair",
            Size = new System.Drawing.Size(28, 23),
            Text = "C",
            ToolTipText = "Plot's crosshair mode"
        });
        statusStrip.Items[item].Click += LabelEx_Click;
        ((ToolStripStatusLabelEx)statusStrip.Items[item]).CheckedChanged += LabelEx_CheckedChanged;
        statusStrip.Items[item].PerformClick();

        tspBottom.Join(statusStrip);
        this.Controls.Add(tspBottom);
    }

    /// <summary>
    /// Initialize the main menu
    /// </summary>
    private void InitializeMenu()
    {

    }
}