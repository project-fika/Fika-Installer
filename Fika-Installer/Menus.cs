namespace Fika_Installer
{
    public static class Menus
    {
        public static void MainMenu()
        {
            List<MenuChoice> menuChoices = [];

            bool fikaDetected = File.Exists(Constants.FikaCorePath);
            string fikaRelease = Constants.FikaReleases["Fika.Core"];

            if (fikaDetected)
            {
                MenuChoice updateFikaChoice = new("Update Fika", ConsoleKey.D1, () => Installer.UpdateFika(fikaRelease));
                menuChoices.Add(updateFikaChoice);
            }
            else
            {
                MenuChoice installFikaChoice = new("Install Fika", ConsoleKey.D1, () => Installer.InstallFika(fikaRelease));
                menuChoices.Add(installFikaChoice);
            }

            bool fikaHeadlessDetected = File.Exists(Constants.FikaHeadlessPath);
            string fikaHeadlessRelease = Constants.FikaReleases["Fika.Headless"];

            if (fikaHeadlessDetected)
            {
                MenuChoice updateFikaChoice = new("Update Fika Headless", ConsoleKey.D1, () => Installer.UpdateFika(fikaHeadlessRelease));
                menuChoices.Add(updateFikaChoice);
            }
            else
            {
                MenuChoice installFikaHeadlessChoice = new("Install Fika Headless", ConsoleKey.D2, () => Installer.InstallFika(fikaHeadlessRelease));
                menuChoices.Add(installFikaHeadlessChoice);
            }

            Menu mainMenu = new(menuChoices);
            mainMenu.Show();
        }
    }
}
