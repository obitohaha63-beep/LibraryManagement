namespace Library.Desktop;

static class Program
{
    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // To customize application configuration such as set high DPI settings or default font,
        // see https://aka.ms/applicationconfiguration.
        ApplicationConfiguration.Initialize();
        using var api = new Services.LibraryApiClient();
        using var login = new LoginForm(api);
        if (login.ShowDialog() == DialogResult.OK) Application.Run(new MainForm(api));
    }    
}
