using Fika_Installer.Models;

namespace Fika_Installer
{
    public static class Menus
    {
        public static void MainMenu()
        {
            string fikaRelease = Constants.FikaReleases["Fika"];
            string fikaHeadlessRelease = Constants.FikaReleases["Fika Headless"];

            List<MenuChoice> menuChoices =
            [
                new MenuChoice("Install Fika", ConsoleKey.D1, () => Installer.InstallFika(fikaRelease)),
                new MenuChoice("Install Fika Headless", ConsoleKey.D2, () => Installer.InstallFika(fikaHeadlessRelease))
            ];

            Menu mainMenu = new(menuChoices);
            mainMenu.Show();
        }
    }
}
