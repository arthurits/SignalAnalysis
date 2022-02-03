using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SignalAnalysis;

partial class FrmAbout : Form
{

    #region Descriptores de acceso de atributos de ensamblado

    public string AssemblyTitle
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attributes.Length > 0)
            {
                AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                if (titleAttribute.Title != "")
                {
                    return titleAttribute.Title;
                }
            }
            return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().Location);
        }
    }

    public string AssemblyVersion
    {
        get
        {
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }
    }

    public string AssemblyDescription
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            return String.Concat(((AssemblyDescriptionAttribute)attributes[0]).Description,
                ".",
                System.Environment.NewLine,
                System.Environment.NewLine,
                "No commercial use allowed whatsoever. Contact the author for any inquires.",
                System.Environment.NewLine,
                System.Environment.NewLine,
                "If you find this software useful, please consider supporting it!");
        }
    }

    public string AssemblyProduct
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            return ((AssemblyProductAttribute)attributes[0]).Product;
        }
    }

    public string AssemblyCopyright
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
        }
    }

    public string AssemblyCompany
    {
        get
        {
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
            if (attributes.Length == 0)
            {
                return "";
            }
            return ((AssemblyCompanyAttribute)attributes[0]).Company;
        }
    }

    #endregion

    public FrmAbout()
    {
        InitializeComponent();
        // this.Text = String.Format("About {0}", AssemblyTitle);
        this.labelProductName.Text = AssemblyProduct;
        this.labelVersion.Text = String.Format("Version {0}", AssemblyVersion);
        this.labelCopyright.Text = AssemblyCopyright;
        this.labelCompanyName.Text = AssemblyCompany;
        this.textBoxDescription.Text = AssemblyDescription;

        // Set form icons and images
        var path = Path.GetDirectoryName(Environment.ProcessPath);
        //if (System.IO.File.Exists(path + @"\images\about.ico")) this.Icon = new Icon(path + @"\images\about.ico");

        //Bitmap image = new Icon(path + @"\images\logo.ico", 256, 256).ToBitmap();
        if (System.IO.File.Exists(path + @"\images\logo@256.png")) this.logoPictureBox.Image = new Bitmap(path + @"\images\logo@256.png");
    }

    //[System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Demand, Name = "FullTrust")]
    protected override void WndProc(ref Message m)
    {
        const int WM_PARENTNOTIFY = 0x210;
        const int WM_LBUTTONDOWN = 0x201;
        //const int WM_LBUTTONUP = 0x202;
        //const int WM_KEYDOWN = 0x100;
        const int WM_KEYUP = 0x101;
        const int VK_ESCAPE = 0x1B;
        const int VK_RETURN = 0x0D;

        // https://stackoverflow.com/questions/27646476/how-to-fire-form-click-event-even-when-clicking-on-user-controls-in-c-sharp
        if (m.Msg == WM_PARENTNOTIFY && m.WParam.ToInt32() == WM_LBUTTONDOWN)
        {
            this.Close();

            //// get the clicked position
            //var x = (int)(m.LParam.ToInt32() & 0xFFFF);
            //var y = (int)(m.LParam.ToInt32() >> 16);

            //// get the clicked control
            //var childControl = this.GetChildAtPoint(new Point(x, y));

            //// call onClick (which fires Click event)
            //OnClick(EventArgs.Empty)

            // do something else...
        }
        else if (m.Msg == WM_KEYUP && ((m.WParam.ToInt32() == VK_ESCAPE) | (m.WParam.ToInt32() == VK_RETURN)))
            this.Close();

        base.WndProc(ref m);
    }
}
