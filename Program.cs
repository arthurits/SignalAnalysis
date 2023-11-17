namespace SignalAnalysis
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.SetDefaultFont(new Font(new FontFamily("Microsoft Sans Serif"), 10f));
            Application.Run(new FrmMain());
        }
    }
}