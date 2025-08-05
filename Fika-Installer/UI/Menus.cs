using Fika_Installer.Controllers;
using Fika_Installer.Models;
using Fika_Installer.Models.UI;
using Fika_Installer.Utils;

namespace Fika_Installer.UI
{
    public class Menus
    {
        private AppController _appController;

        public Menus(AppController uiController)
        {
            _appController = uiController;
        }

        public void MainMenu()
        {            
            List<MenuChoice> menuChoices = [];

            string fikaCorePath = Constants.FikaCorePath;
            bool fikaDetected = File.Exists(fikaCorePath);

            if (fikaDetected)
            {
                MenuChoice updateFikaChoice = new("UpdateFika","Update Fika", ConsoleKey.D1);
                menuChoices.Add(updateFikaChoice);
            }
            else
            {
                MenuChoice installFikaChoice = new("InstallFika","Install Fika", ConsoleKey.D1);
                menuChoices.Add(installFikaChoice);
            }

            string fikaHeadlessPath = Constants.FikaHeadlessPath;
            bool fikaHeadlessDetected = File.Exists(fikaHeadlessPath);

            if (fikaHeadlessDetected)
            {
                MenuChoice updateFikaChoice = new("UpdateFikaHeadless", "Update Fika Headless", ConsoleKey.D2);
                menuChoices.Add(updateFikaChoice);
            }
            else
            {
                MenuChoice installFikaHeadlessChoice = new("InstallFikaHeadless","Install Fika Headless", ConsoleKey.D2);
                menuChoices.Add(installFikaHeadlessChoice);
            }

            Menu mainMenu = new(menuChoices);
            MenuResult menuResult = mainMenu.Show();

            switch (menuResult.Id)
            {
                case "InstallFika":
                    _appController.Pages.InstallFikaPage();
                    break;
                case "UpdateFika":
                    _appController.Pages.UpdateFikaPage(); 
                    break;
                case "InstallFikaHeadless":
                    _appController.Pages.InstallFikaHeadlessPage();
                    break;
                case "UpdateFikaHeadless":
                    _appController.Pages.UpdateFikaHeadlessPage();
                    break;
            }
        }

        public void ProfileSelectionMenu(string sptFolder)
        {
            List<MenuChoice> menuChoices = [];
            
            string sptProfilesPath = Path.Combine(sptFolder, @"user\profiles\");

            SptProfile[] sptHeadlessProfiles = SptUtils.GetSptProfiles(sptProfilesPath, true);

            if (sptHeadlessProfiles.Length > 0)
            {
                string menuMessage = "Please choose the headless profile to use for your Fika Headless:";

                int keyNumber = 1;
                int keyNumberAsciiBegin = 48; // Number 1 ascii code

                foreach (SptProfile sptHeadlessProfile in sptHeadlessProfiles)
                {
                    string profileId = sptHeadlessProfile.ProfileId;
                    ConsoleKey menuKey = (ConsoleKey)(keyNumber + keyNumberAsciiBegin);

                    MenuChoice menuChoice = new(profileId, profileId, menuKey);
                    menuChoices.Add(menuChoice);

                    keyNumber++;
                }

                // TODO: handle when there is more than 9 headless profiles...

                ConsoleKey createNewProfileMenuKey = (ConsoleKey)(keyNumber + keyNumberAsciiBegin);
                MenuChoice createNewProfileMenuChoice = new("CreateNewHeadlessProfile", "Create new headless profile", createNewProfileMenuKey);
                menuChoices.Add(createNewProfileMenuChoice);

                Menu profileSelectionMenu = new(menuMessage, menuChoices);
                MenuResult menuResult = profileSelectionMenu.Show();

                string selectionId = menuResult.Id;

                if (selectionId == "CreateNewHeadlessProfile")
                {
                    _appController.Pages.SetupNewHeadlessProfilePage(sptFolder);
                }
                else
                {
                    foreach (SptProfile sptHeadlessProfile in sptHeadlessProfiles)
                    {
                        if (selectionId == sptHeadlessProfile.ProfileId)
                        {
                            _appController.Pages.SetupHeadlessProfilePage(sptHeadlessProfile, sptFolder);
                        }
                    }
                }
            }
            else
            {
                _appController.Pages.SetupNewHeadlessProfilePage(sptFolder);
            }
        }

        public string InstallationTypeMenu()
        {
            List<MenuChoice> menuChoices = [];

            MenuChoice hardCopyChoice = new("HardCopy", "Hard Copy of SPT folder", ConsoleKey.D1);
            MenuChoice symlinkChoice = new("Symlink", "Symlink of SPT folder", ConsoleKey.D2);

            menuChoices.Add(hardCopyChoice);
            menuChoices.Add(symlinkChoice);

            Menu menu = new("Please choose the headless installation type:", menuChoices);
            MenuResult menuResult = menu.Show();

            return menuResult.Id;
        }
    }
}
