namespace System.Windows.Forms;

/// <summary>
/// <cref>ToolStripStatusLabel</cref> extension implmenting cheched status
/// </summary>
class ToolStripStatusLabelEx : System.Windows.Forms.ToolStripStatusLabel
{
    // Another example could be
    // https://www.codeproject.com/Articles/21419/Label-with-ProgressBar-in-a-StatusStrip

    private bool _checked = false;
    public System.Drawing.Brush CheckedBorder { get; set; } = System.Drawing.Brushes.SteelBlue;
    public System.Drawing.Brush CheckedBackground { get; set; } = System.Drawing.Brushes.LightSkyBlue;
    public System.Drawing.Color ForeColorChecked { get; set; } = Color.Black;
    public System.Drawing.Color ForeColorUnchecked { get; set; } = Color.LightGray;

    public event EventHandler? CheckedChanged;

    /// <summary>
    /// Class constructor. Sets BackColor to transparent.
    /// </summary>
    public ToolStripStatusLabelEx()
    {
        BackColor = System.Drawing.Color.Transparent;
    }

    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="border"><cref>Brush</cref> for the checked border</param>
    /// <param name="checkedBackground">Brush for the checked background</param>
    /// <param name="foreColorChecked">Color font for the checked status</param>
    /// <param name="foreColorUnchecked">Color font for the unchecked status</param>
    public ToolStripStatusLabelEx(System.Drawing.Brush border, System.Drawing.Brush checkedBackground, System.Drawing.Color foreColorChecked, System.Drawing.Color foreColorUnchecked)
        : this()
    {
        CheckedBorder = border;
        CheckedBackground = checkedBackground;
        ForeColorChecked = foreColorChecked;
        ForeColorUnchecked = foreColorUnchecked;
    }

    public bool Checked
    {
        get => _checked;
        set
        {
            _checked = value;
            Invalidate();       // Force repainting of the client area
            OnCheckedChanged(new EventArgs());
        }
    }

    protected virtual void OnCheckedChanged(EventArgs e)
    {
        EventHandler? handler = CheckedChanged;
        handler?.Invoke(this, e);
    }

    /// <summary>
    /// Paint function
    /// </summary>
    /// <param name="e"></param>
    protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
    {
        // Only render if the state is checked
        if (_checked)
        {
            // fill the entire button with a color (will be used as a border)
            System.Drawing.Rectangle rectButtonFill = new System.Drawing.Rectangle(System.Drawing.Point.Empty, new System.Drawing.Size(ContentRectangle.Size.Width, ContentRectangle.Size.Height));
            e.Graphics.FillRectangle(CheckedBorder, rectButtonFill);

            // fill the entire button offset by 1,1 and height/width subtracted by 2 used as the fill color
            int backgroundHeight = ContentRectangle.Size.Height - 2;
            int backgroundWidth = ContentRectangle.Size.Width - 2;   // Check the label's borders to set up this substraction
            System.Drawing.Rectangle rectBackground = new System.Drawing.Rectangle(1, 1, backgroundWidth, backgroundHeight);
            e.Graphics.FillRectangle(CheckedBackground, rectBackground);

            // Set the fore color
            ForeColor = ForeColorChecked;
        }
        else
            ForeColor = ForeColorUnchecked;

        base.OnPaint(e);
    }

    /// <summary>
    /// Implement hover rendering? Or use a CustomRender?
    /// </summary>
    /// <param name="e"></param>
    protected override void OnMouseHover(EventArgs e)
    {
        base.OnMouseHover(e);
    }

    protected override void OnClick(EventArgs e)
    {
        //_checked = !_checked;
        base.OnClick(e);
    }
}