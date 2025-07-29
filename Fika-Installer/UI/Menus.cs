using Fika_Installer.Models;

namespace Fika_Installer.UI
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

                if (menuResult.ValidEntry)
                {
                    switch (menuResult.Id)
                    {
                        case "InstallFika":
                            Pages.InstallFikaPage();
                            break;
                        case "UpdateFika":
                            Pages.UpdateFikaPage(); 
                            break;
                        case "InstallFikaHeadless":
                            Pages.InstallFikaHeadlessPage();
                            break;
                        case "UpdateFikaHeadless":
                            Pages.UpdateFikaHeadlessPage();
                            break;
                    }
                }
            }
        }

        public static void ProfileSelectionMenu(string sptFolder)
        {
            List<MenuChoice> menuChoices = [];
            
            string sptProfilesPath = Path.Combine(sptFolder, @"user\profiles\");

            SptProfile[] sptHeadlessProfiles = SptUtils.GetSptProfiles(sptProfilesPath, true);

            if (sptHeadlessProfiles.Length > 0)
            {
                string menuMessage = "Please choose the headless profile to use for your Fika Headless:";

                int keyNumber = 1;
                int keyNumberAsciiBegin = 48;

                foreach (SptProfile sptHeadlessProfile in sptHeadlessProfiles)
                {
                    string profileId = sptHeadlessProfile.ProfileId;
                    ConsoleKey menuKey = (ConsoleKey)(keyNumber + keyNumberAsciiBegin);

                    MenuChoice menuChoice = new(profileId, profileId, menuKey);
                    menuChoices.Add(menuChoice);

                    keyNumber++;
                }

                ConsoleKey createNewProfileMenuKey = (ConsoleKey)(keyNumber + keyNumberAsciiBegin);
                MenuChoice CreateNewProfileMenuChoice = new("CreateNewHeadlessProfile", "Create new headless profile", createNewProfileMenuKey);

                Menu profileSelectionMenu = new(menuMessage, menuChoices);
                MenuResult menuResult = profileSelectionMenu.Show();

                string selectionId = menuResult.Id;

                foreach (SptProfile sptHeadlessProfile in sptHeadlessProfiles)
                {
                    if (selectionId == sptHeadlessProfile.ProfileId)
                    {
                        Pages.SetupHeadlessProfilePage(sptHeadlessProfile, sptFolder);
                    }
                }

                if (selectionId == "CreateNewHeadlessProfile")
                {
                    Pages.SetupNewHeadlessProfilePage(sptFolder);
                }
            }
            else
            {
                Pages.SetupNewHeadlessProfilePage(sptFolder);
            }
        }
    }
}
