using Fika_Installer.Models;
using Fika_Installer.UI;

namespace Fika_Installer
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Logger logger = new();
            logger.Log("Fika-Installer Start");

            Console.Title = InstallerConstants.VersionString;
            Console.CursorVisible = false;

            Header.Show();

            string installerDirectory = InstallerConstants.InstallerDir;
            FikaRelease fikaCoreRelease = FikaConstants.FikaReleases["Fika.Core"];
            FikaRelease fikaServerRelease = FikaConstants.FikaReleases["Fika.Server"];
            FikaRelease fikaHeadlessRelease = FikaConstants.FikaReleases["Fika.Headless"];

            MenuFactory menuFactory = new(installerDirectory, fikaCoreRelease, fikaServerRelease, fikaHeadlessRelease, logger);

            while (true)
            {
                Menu mainMenu = menuFactory.CreateMainMenu();
                mainMenu.Show();
            }
        }
    }
}