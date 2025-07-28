using Fika_Installer.Models;

namespace Fika_Installer
{
    public static class Menus
    {
        public static void MainMenu()
        {
            while (true)
            {
                List<MenuChoice> menuChoices = [];

                string fikaCorePath = Constants.FikaCorePath;
                bool fikaDetected = File.Exists(fikaCorePath);

                if (fikaDetected)
                {
                    MenuChoice updateFikaChoice = new("Update Fika", ConsoleKey.D1, Pages.UpdateFikaPage);
                    menuChoices.Add(updateFikaChoice);
                }
                else
                {
                    MenuChoice installFikaChoice = new("Install Fika", ConsoleKey.D1, Pages.InstallFikaPage);
                    menuChoices.Add(installFikaChoice);
                }

                string fikaHeadlessPath = Constants.FikaHeadlessPath;
                bool fikaHeadlessDetected = File.Exists(fikaHeadlessPath);

                if (fikaHeadlessDetected)
                {
                    MenuChoice updateFikaChoice = new("Update Fika Headless", ConsoleKey.D2, Pages.UpdateFikaHeadlessPage);
                    menuChoices.Add(updateFikaChoice);
                }
                else
                {
                    MenuChoice installFikaHeadlessChoice = new("Install Fika Headless", ConsoleKey.D2, Pages.InstallFikaHeadlessPage);
                    menuChoices.Add(installFikaHeadlessChoice);
                }

                Menu mainMenu = new(menuChoices);
                mainMenu.Show();
            }
        }
    }
}
