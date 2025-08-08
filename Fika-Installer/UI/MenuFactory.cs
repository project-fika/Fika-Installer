using Fika_Installer.Models;
using Fika_Installer.Models.UI;
using Fika_Installer.UI.Pages;

namespace Fika_Installer.UI
{
    public class MenuFactory
    {
        private string _fikaCoreReleaseUrl;
        private string _fikaServerReleaseUrl;
        private string _fikaHeadlessReleaseUrl;
        private string _installDir;
        private string _sptFolder;

        public MenuFactory(string fikaCoreReleaseUrl, string fikaServerReleaseUrl, string fikaHeadlessReleaseUrl, string installerDirectory)
        {
            _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
            _fikaServerReleaseUrl = fikaServerReleaseUrl;
            _fikaHeadlessReleaseUrl = fikaHeadlessReleaseUrl;
            _installDir = installerDirectory;
            _sptFolder = installerDirectory;
        }
        
        public Menu CreateMainMenu()
        {
            List<MenuChoice> choices = [];

            string fikaCorePath = Path.Combine(_installDir, @"BepInEx\plugins\Fika.Core.dll");
            bool fikaDetected = File.Exists(fikaCorePath);

            if (fikaDetected)
            {
                UpdateFikaPage updateFikaPage = new UpdateFikaPage(_installDir, _fikaCoreReleaseUrl, _fikaServerReleaseUrl);
                
                MenuChoice updateFikaChoice = new("Update Fika", updateFikaPage);
                choices.Add(updateFikaChoice);
            }
            else
            {
                InstallFikaPage installFikaPage = new InstallFikaPage(_installDir, _fikaCoreReleaseUrl, _fikaServerReleaseUrl);

                MenuChoice installFikaChoice = new("Install Fika", installFikaPage);
                choices.Add(installFikaChoice);
            }

            MenuChoice advancedChoice = new("Advanced Options", CreateAdvancedOptionsMenu());
            choices.Add(advancedChoice);

            Menu mainMenu = new Menu(choices);
            return mainMenu;
        }

        public Menu CreateAdvancedOptionsMenu()
        {
            List<MenuChoice> choices = [];

            string fikaHeadlessPath = Path.Combine(_installDir, @"BepInEx\plugins\Fika.Headless.dll");
            bool fikaHeadlessDetected = File.Exists(fikaHeadlessPath);

            MenuChoice installFikaInCurrentFolder = new("Install Fika in current folder", () => { });
            choices.Add(installFikaInCurrentFolder);

            if (fikaHeadlessDetected)
            {
                UpdateFikaHeadlessPage updateFikaHeadlessPage = new(_installDir, _fikaCoreReleaseUrl, _fikaHeadlessReleaseUrl);
                MenuChoice updateFikaHeadlessChoice = new("Update Fika Headless", updateFikaHeadlessPage);
                choices.Add(updateFikaHeadlessChoice);
            }
            else
            {
                InstallFikaHeadlessPage installFikaHeadlessPage = new(this, _installDir, _sptFolder, _fikaCoreReleaseUrl, _fikaHeadlessReleaseUrl);
                
                MenuChoice installFikaHeadlessChoice = new("Install Fika Headless", installFikaHeadlessPage);
                choices.Add(installFikaHeadlessChoice);
            }

            MenuChoice backChoice = new("Back", () => { });
            choices.Add(backChoice);

            Menu advancedOptionsMenu = new Menu(choices);
            return advancedOptionsMenu;
        }

        public Menu CreateProfileSelectionMenu(List<SptProfile> sptProfiles)
        {
            List<MenuChoice> choices = [];

            foreach (SptProfile profile in sptProfiles)
            {
                MenuChoice menuChoice = new MenuChoice(profile.ProfileId);
                choices.Add(menuChoice);
            }

            MenuChoice createNewHeadlessProfile = new MenuChoice("Create a new headless profile", "createNewHeadlessProfile");
            choices.Add(createNewHeadlessProfile);

            Menu profileSelectionMenu = new Menu("Please choose the headless profile to use for your headless client:", choices);
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
    }
}
