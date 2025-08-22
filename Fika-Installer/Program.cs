using Fika_Installer.UI;

namespace Fika_Installer
{
    public class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            Console.Title = Constants.FikaInstallerVersionString;

            string fikaCoreReleaseUrl = Constants.FikaReleaseUrls["Fika.Core"];
            string fikaServerReleaseUrl = Constants.FikaReleaseUrls["Fika.Server"];
            string fikaHeadlessReleaseUrl = Constants.FikaReleaseUrls["Fika.Headless"];
            string installerDirectory = Constants.InstallerDirectory;

            MenuFactory menuFactory = new MenuFactory(installerDirectory, fikaCoreReleaseUrl, fikaServerReleaseUrl, fikaHeadlessReleaseUrl);

            while (true)
            {
                Menu mainMenu = menuFactory.CreateMainMenu();
                mainMenu.Show();
            }
        }
    }
}