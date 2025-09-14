using Fika_Installer.Models;
using Fika_Installer.Models.Spt;
using Fika_Installer.UI.Pages;

namespace Fika_Installer.UI
{
    public class MenuFactory
    {
        private string _fikaCoreReleaseUrl;
        private string _fikaServerReleaseUrl;
        private string _fikaHeadlessReleaseUrl;
        private string _installDir;
        private string _fikaCorePath;
        private string _fikaHeadlessPath;
        private ILogger _logger;

        public MenuFactory(string installDir, string fikaCoreReleaseUrl, string fikaServerReleaseUrl, string fikaHeadlessReleaseUrl, ILogger logger)
        {
            _installDir = installDir;
            _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
            _fikaServerReleaseUrl = fikaServerReleaseUrl;
            _fikaHeadlessReleaseUrl = fikaHeadlessReleaseUrl;
            _fikaCorePath = Path.Combine(_installDir, @"BepInEx\plugins\Fika.Core.dll");
            _fikaHeadlessPath = Path.Combine(_installDir, @"BepInEx\plugins\Fika.Headless.dll");
            _logger = logger;
        }

        public Menu CreateMainMenu()
        {
            List<MenuChoice> choices = [];

            bool fikaDetected = File.Exists(_fikaCorePath);

            if (fikaDetected)
            {
                UpdateFikaPage updateFikaPage = new(_installDir, _fikaCoreReleaseUrl, _fikaServerReleaseUrl, _logger);

                MenuChoice updateFikaChoice = new("Update Fika", updateFikaPage);
                choices.Add(updateFikaChoice);
            }
            else
            {
                InstallFikaPage installFikaPage = new(_installDir, _fikaCoreReleaseUrl, _fikaServerReleaseUrl, _logger);

                MenuChoice installFikaChoice = new("Install Fika", installFikaPage);
                choices.Add(installFikaChoice);
            }

            MenuChoice advancedChoice = new("Advanced Options", CreateAdvancedOptionsMenu());
            choices.Add(advancedChoice);

            Menu mainMenu = new(choices);

            return mainMenu;
        }

        public Menu CreateAdvancedOptionsMenu()
        {

            List<MenuChoice> choices = [];

            bool fikaHeadlessDetected = File.Exists(_fikaHeadlessPath);

            if (fikaHeadlessDetected)
            {
                UpdateFikaHeadlessPage updateFikaHeadlessPage = new(_installDir, _fikaCoreReleaseUrl, _fikaHeadlessReleaseUrl, _logger);
                MenuChoice updateFikaHeadlessChoice = new("Update Fika Headless", updateFikaHeadlessPage);
                choices.Add(updateFikaHeadlessChoice);
            }
            else
            {
                InstallFikaHeadlessPage installFikaHeadlessPage = new(this, _installDir, _fikaCoreReleaseUrl, _fikaHeadlessReleaseUrl, _logger);

                MenuChoice installFikaHeadlessChoice = new("Install Fika Headless", installFikaHeadlessPage);
                choices.Add(installFikaHeadlessChoice);
            }

            bool fikaCoreDetected = File.Exists(_fikaCorePath);

            if (!fikaCoreDetected)
            {
                InstallFikaCurrentDirPage installFikaCurrentDirPage = new(this, _installDir, _fikaCoreReleaseUrl, _fikaServerReleaseUrl, _logger);

                MenuChoice installFikaInCurrentFolder = new("Install Fika in current folder", installFikaCurrentDirPage);
                choices.Add(installFikaInCurrentFolder);
            }

            MenuChoice backChoice = new("Back", () => { });
            choices.Add(backChoice);

            Menu advancedOptionsMenu = new(choices);

            return advancedOptionsMenu;
        }

        public Menu CreateProfileSelectionMenu(List<SptProfile> sptProfiles)
        {
            List<MenuChoice> choices = [];

            foreach (SptProfile profile in sptProfiles)
            {
                MenuChoice menuChoice = new(profile.ProfileId);
                choices.Add(menuChoice);
            }

            MenuChoice createNewHeadlessProfile = new("Create a new headless profile", "createNewHeadlessProfile");
            choices.Add(createNewHeadlessProfile);

            Menu profileSelectionMenu = new("Please choose the headless profile to use for your headless client:", choices);

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
