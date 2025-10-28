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

            PageLogger pageLogger = new();
            Logger.AddLogger(pageLogger);

            if (args.Length > 0)
            {
                Logger.SetInteractive(false);
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