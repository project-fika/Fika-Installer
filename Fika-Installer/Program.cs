using Fika_Installer.UI;

namespace Fika_Installer
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            string installerDir = InstallerConstants.InstallerDir;

            FileLogger logger = new(installerDir);
            Logger.AddLogger(logger);

            PageLogger pageLogger = new();
            Logger.AddLogger(pageLogger);

            logger.Log("Fika-Installer Start");

            Console.Title = InstallerConstants.VersionString;
            Console.CursorVisible = false;

            Header.Show();

            MenuFactory menuFactory = new(installerDir);

            while (true)
            {
                Menu mainMenu = menuFactory.CreateMainMenu();
                mainMenu.Show();
            }
        }
    }
}