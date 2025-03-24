namespace DarkSaveManager;

internal static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        Application.Run(new App_Context());
    }

    private sealed class App_Context : ApplicationContext
    {
        internal App_Context() => Core.Init();
    }
}
