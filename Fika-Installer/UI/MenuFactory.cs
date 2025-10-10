using Fika_Installer.Models;
using Fika_Installer.Models.Spt;
using Fika_Installer.UI.Pages;

namespace Fika_Installer.UI
{
    public class MenuFactory(string installDir)
    {
        private string _fikaCorePath = Path.Combine(installDir, @"BepInEx\plugins\Fika.Core.dll");
        private string _fikaHeadlessPath = Path.Combine(installDir, @"BepInEx\plugins\Fika.Headless.dll");

        public Menu CreateMainMenu()
        {
            List<MenuChoice> mainMenuChoices = [];

            bool fikaDetected = File.Exists(_fikaCorePath);

            if (fikaDetected)
            {
                UpdateFikaPage updateFikaPage = new(installDir);

                MenuChoice updateFikaChoice = new("Update Fika", updateFikaPage);
                mainMenuChoices.Add(updateFikaChoice);

                UninstallFikaPage uninstallFikaPage = new(this, installDir);

                MenuChoice uninstallFikaChoice = new("Uninstall Fika", uninstallFikaPage);
                mainMenuChoices.Add(uninstallFikaChoice);
            }
            else
            {
                InstallFikaPage installFikaPage = new(installDir);

                MenuChoice installFikaChoice = new("Install Fika", installFikaPage);
                mainMenuChoices.Add(installFikaChoice);
            }

            MenuChoice advancedChoice = new("Advanced Options", CreateAdvancedOptionsMenu());
            mainMenuChoices.Add(advancedChoice);

            Menu mainMenu = new(mainMenuChoices);

            return mainMenu;
        }

        public Menu CreateAdvancedOptionsMenu()
        {
            List<MenuChoice> advancedMenuChoices = [];

            bool fikaHeadlessDetected = File.Exists(_fikaHeadlessPath);

            if (fikaHeadlessDetected)
            {
                UpdateFikaHeadlessPage updateFikaHeadlessPage = new(installDir);

                MenuChoice updateFikaHeadlessChoice = new("Update Fika Headless", updateFikaHeadlessPage);
                advancedMenuChoices.Add(updateFikaHeadlessChoice);
            }
            else
            {
                InstallFikaHeadlessPage installFikaHeadlessPage = new(this, installDir);

                MenuChoice installFikaHeadlessChoice = new("Install Fika Headless", installFikaHeadlessPage);
                advancedMenuChoices.Add(installFikaHeadlessChoice);
            }

            bool fikaCoreDetected = File.Exists(_fikaCorePath);

            if (!fikaCoreDetected)
            {
                InstallFikaCurrentDirPage installFikaCurrentDirPage = new(this, installDir);

                MenuChoice installFikaInCurrentFolder = new("Install Fika in current folder", installFikaCurrentDirPage);
                advancedMenuChoices.Add(installFikaInCurrentFolder);
            }

            MenuChoice backChoice = new("Back", () => { });
            advancedMenuChoices.Add(backChoice);

            Menu advancedOptionsMenu = new(advancedMenuChoices);

            return advancedOptionsMenu;
        }

        public Menu CreateProfileSelectionMenu(List<SptProfile> sptProfiles)
        {
            List<MenuChoice> profileSelectionMenuChoices = [];

            foreach (SptProfile profile in sptProfiles)
            {
                MenuChoice menuChoice = new(profile.ProfileId);
                profileSelectionMenuChoices.Add(menuChoice);
            }

            MenuChoice createNewHeadlessProfile = new("Create a new headless profile", "createNewHeadlessProfile");
            profileSelectionMenuChoices.Add(createNewHeadlessProfile);

            Menu profileSelectionMenu = new("Please choose the headless profile to use for your headless client:", profileSelectionMenuChoices);

            return profileSelectionMenu;
        }

        public Menu CreateInstallMethodMenu()
        {
            List<MenuChoice> choices = [];

            string[] installMethods = Enum.GetNames<InstallMethod>();

            foreach (string installMethod in installMethods)
            {
                MenuChoice choice = new(installMethod);
                choices.Add(choice);
            }

            Menu installMethodMenu = new("Please choose your installation method. This will determine how the SPT folder will be copied.", choices);

            return installMethodMenu;
        }

        public Menu CreateConfirmUninstallFikaMenu()
        {
            List<MenuChoice> choices = [];

            MenuChoice choiceYes = new("Yes");
            choices.Add(choiceYes);

            MenuChoice choiceNo = new("No");
            choices.Add(choiceNo);

            Menu uninstallFikaMenu = new("Are you sure you want to uninstall Fika? Your Fika settings will be lost.", choices);

            return uninstallFikaMenu;
        }
    }
}
