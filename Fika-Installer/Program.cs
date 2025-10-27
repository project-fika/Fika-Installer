using Fika_Installer.Models;
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
                        switch (args[1].ToLower())
                        {
                            case "fika":
                                Logger.Log("Installing: fika");
                                UI.Pages.Methods.InstallFika(Installer.CurrentDir);
                                break;
                            case "headless":
                                Logger.Log("Installing: headless");
                                string? headlessProfileId = null;
                                if (args.Length >= 3)
                                {
                                    headlessProfileId = args[2];
                                }
                                InstallMethod? installMethod = null;
                                if (args.Length >= 4)
                                {
                                    switch (args[3])
                                    {
                                        case "hardcopy":
                                            installMethod = InstallMethod.HardCopy;
                                            break;
                                        case "symlink":
                                            installMethod = InstallMethod.Symlink;
                                            break;
                                        default:
                                            Logger.Error("Invalid install method argument; supported arguments: hardcopy, symlink");
                                            break;
                                    }
                                }
                                UI.Pages.Methods.InstallHeadless(Installer.CurrentDir, headlessProfileId, installMethod);
                                break;
                            default:
                                Logger.Error("Install command requires argument; supported arguments: fika, headless");
                                break;
                        }
                        break;
                    case "uninstall":
                        Logger.Log("Starting uninstallation");
                        UI.Pages.Methods.Uninstall(Installer.CurrentDir);
                        break;
                    case "update":
                        Logger.Log("Starting update");
                        switch (args[1].ToLower())
                        {
                            case "fika":
                                Logger.Log("Updating: fika");
                                UI.Pages.Methods.UpdateFika(Installer.CurrentDir);
                                break;
                            case "headless":
                                Logger.Log("Updating: headless");
                                UI.Pages.Methods.UpdateHeadless(Installer.CurrentDir);
                                break;
                            default:
                                Logger.Error("Update command requires argument; supported arguments: fika, headless");
                                break;
                        }
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