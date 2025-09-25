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
            string fikaCoreReleaseUrl = FikaConstants.ReleaseUrls["Fika.Core"];
            string fikaServerReleaseUrl = FikaConstants.ReleaseUrls["Fika.Server"];
            string fikaHeadlessReleaseUrl = FikaConstants.ReleaseUrls["Fika.Headless"];

            MenuFactory menuFactory = new(installerDirectory, fikaCoreReleaseUrl, fikaServerReleaseUrl, fikaHeadlessReleaseUrl, logger);

            while (true)
            {
                Menu mainMenu = menuFactory.CreateMainMenu();
                mainMenu.Show();
            }
        }
    }
}