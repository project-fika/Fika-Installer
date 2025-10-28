using Fika_Installer.Models;
using Fika_Installer.UI;
using Fika_Installer.Utils;

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
                CLI.Parse(args);
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