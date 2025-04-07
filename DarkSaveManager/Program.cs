using System.Text;

namespace DarkSaveManager;

file static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

        Logger.SetLogFile(Paths.LogFile);

        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();

#if HIGH_DPI
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
#else
        // Windows 11 needs to be explicitly told to use DPI-unaware auto-scaling mode for whatever reason.
        // Windows 10 defaults to DPI-unaware as desired. If you're thinking, "wait, wouldn't this break every
        // single app in the universe moving from Windows 10 to 11?" Yeah, that's what I'm thinking too. But hey,
        // Microsoft knows what's best, right? Sheesh...
        Application.SetHighDpiMode(HighDpiMode.DpiUnawareGdiScaled);
#endif

        Application.Run(new App_Context());
    }

    private sealed class App_Context : ApplicationContext
    {
        internal App_Context() => Core.Init();
    }
}
