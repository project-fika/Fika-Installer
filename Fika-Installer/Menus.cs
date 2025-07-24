using Fika_Installer.Models;

namespace Fika_Installer
{
    public static class Menus
    {
        public static void MainMenu()
        {
            List<MenuChoice> menuChoices =
            [
                new MenuChoice("Install Fika", ConsoleKey.D1, Installer.InstallFika),
                new MenuChoice("Install Fika Headless", ConsoleKey.D2, Installer.InstallFikaHeadless)
            ];

            Menu mainMenu = new(menuChoices);
            mainMenu.Show();
        }
    }
}
