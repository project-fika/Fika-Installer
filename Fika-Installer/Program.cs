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

            InitUI();
        }

        static void InitUI()
        {
            Console.Title = Installer.VersionString;
            Console.CursorVisible = false;

            PageLogger pageLogger = new();
            Logger.AddLogger(pageLogger);

            if (args.Length == 0)
            {
                // No arguments provided, launch menu-based installer

                Header.Show();

                MenuFactory menuFactory = new(Installer.CurrentDir);

                while (true)
                {
                    Menu mainMenu = menuFactory.CreateMainMenu();
                    mainMenu.Show();
                }
            }
            else
            {
                // Arguments provided, run requested mode directly without UI
                switch (args[0].ToLower())
                {
                    case "install":
                        Logger.Log("Starting installation");
                        UI.Pages.Methods.Install(Installer.CurrentDir, UI.Pages.Methods.InteractiveMode.NonInteractive);
                        break;
                    case "uninstall":
                        Logger.Log("Starting uninstallation");
                        UI.Pages.Methods.Uninstall(Installer.CurrentDir, UI.Pages.Methods.InteractiveMode.NonInteractive);
                        break;
                    case "update":
                        Logger.Log("Starting update");
                        UI.Pages.Methods.Update(Installer.CurrentDir, UI.Pages.Methods.InteractiveMode.NonInteractive);
                        break;
                    default:
                        Logger.Error($"Unknown command-line argument: {args[0]} - Supported arguments are: install, uninstall, update");
                        break;
                }
            }
        }
    }
}