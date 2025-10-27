using Fika_Installer.UI;

namespace Fika_Installer
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string logFilePath = Path.Combine(Installer.CurrentDir, "fika-installer.log");

            FileLogger fileLogger = new(logFilePath);
            Logger.AddLogger(fileLogger);

            if (args.Length > 0)
            {
                Logger.SetInteractive(false);
                Logger.Log($"Command-line arguments detected: {string.Join(' ', args)}");

                switch (args[0].ToLower())
                {
                    case "install":
                        Logger.Log("Starting installation");
                        UI.Pages.Methods.Install(Installer.CurrentDir);
                        break;
                    case "uninstall":
                        Logger.Log("Starting uninstallation");
                        UI.Pages.Methods.Uninstall(Installer.CurrentDir);
                        break;
                    case "update":
                        Logger.Log("Starting update");
                        UI.Pages.Methods.Update(Installer.CurrentDir);
                        break;
                    default:
                        Logger.Error($"Unknown command-line argument: {args[0]} - Supported arguments are: install, uninstall, update");
                        break;
                }
            }
            else
            {
                Logger.SetInteractive(true);
                InitUI();
            }
        }

        static void InitUI()
        {
            Console.Title = Installer.VersionString;
            Console.CursorVisible = false;

            PageLogger pageLogger = new();
            Logger.AddLogger(pageLogger);

            Header.Show();

            MenuFactory menuFactory = new(Installer.CurrentDir);

            while (true)
            {
                Menu mainMenu = menuFactory.CreateMainMenu();
                mainMenu.Show();
            }
        }
    }
}