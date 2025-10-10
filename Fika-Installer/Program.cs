using Fika_Installer.UI;

namespace Fika_Installer
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            SetupLoggers();

            SetupConsole();

            Header.Show();

            MenuFactory menuFactory = new(Installer.CurrentDir);

            while (true)
            {
                Menu mainMenu = menuFactory.CreateMainMenu();
                mainMenu.Show();
            }
        }

        static void SetupLoggers()
        {
            string logFilePath = Path.Combine(Installer.CurrentDir, "fika-installer.log");

            FileLogger fileLogger = new(logFilePath);
            Logger.AddLogger(fileLogger);

            PageLogger pageLogger = new();
            Logger.AddLogger(pageLogger);
        }

        static void SetupConsole()
        {
            Console.Title = Installer.VersionString;
            Console.CursorVisible = false;
        }
    }
}