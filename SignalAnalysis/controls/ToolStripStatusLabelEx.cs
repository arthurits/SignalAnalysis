using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace System.Windows.Forms
{
    /// <summary>
    /// <cref>ToolStripStatusLabel</cref> extension implmenting cheched status
    /// </summary>
    class ToolStripStatusLabelEx : System.Windows.Forms.ToolStripStatusLabel
    {
        // Another example could be
        // https://www.codeproject.com/Articles/21419/Label-with-ProgressBar-in-a-StatusStrip

        private bool _checked;
        private System.Drawing.Brush _border;
        private System.Drawing.Brush _checkedBackground;

        public event EventHandler CheckedChanged;

        /// <summary>
        /// Class constructor. Sets SteelBlue and LightSkyBlue as defaults colors
        /// </summary>
        public ToolStripStatusLabelEx()
        {
            _border = System.Drawing.Brushes.SteelBlue;
            _checkedBackground = System.Drawing.Brushes.LightSkyBlue;
            BackColor = System.Drawing.Color.Transparent;
        }
        
        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="border"><cref>Brush</cref> for the checked border</param>
        /// <param name="checkedBackground">Brush for the checked background</param>
        public ToolStripStatusLabelEx(System.Drawing.Brush border, System.Drawing.Brush checkedBackground)
        {
            _border = border;
            _checkedBackground = checkedBackground;
        }

        /// <summary>
        /// Sets and gets the border color of the checked button
        /// </summary>
        public System.Drawing.Brush BorderColor
        {
            get { return _border; }
            set { _border = value; }
        }

        /// <summary>
        /// Sets and gets the background color of the checked button
        /// </summary>
        public System.Drawing.Brush CheckedColor
        {
            get { return _checkedBackground; }
            set { _checkedBackground = value; }
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
            EventHandler handler = CheckedChanged;
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
                e.Graphics.FillRectangle(_border, rectButtonFill);

                // fill the entire button offset by 1,1 and height/width subtracted by 2 used as the fill color
                int backgroundHeight = ContentRectangle.Size.Height - 2;
                int backgroundWidth = ContentRectangle.Size.Width - 2;   // Check the label's borders to set up this substraction
                System.Drawing.Rectangle rectBackground = new System.Drawing.Rectangle(1, 1, backgroundWidth, backgroundHeight);
                e.Graphics.FillRectangle(_checkedBackground, rectBackground);
            }
            
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
}
