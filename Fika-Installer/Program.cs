using Fika_Installer.Models;
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

            FikaRelease fikaCoreRelease = FikaConstants.FikaReleases["Fika.Core"];
            FikaRelease fikaServerRelease = FikaConstants.FikaReleases["Fika.Server"];
            FikaRelease fikaHeadlessRelease = FikaConstants.FikaReleases["Fika.Headless"];

            MenuFactory menuFactory = new(installerDir, fikaCoreRelease, fikaServerRelease, fikaHeadlessRelease);

            while (true)
            {
                Menu mainMenu = menuFactory.CreateMainMenu();
                mainMenu.Show();
            }
        }
    }
}