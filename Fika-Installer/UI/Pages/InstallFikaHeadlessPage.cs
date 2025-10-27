using Fika_Installer.Fika;
using Fika_Installer.Models;
using Fika_Installer.Models.Spt;
using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaHeadlessPage(MenuFactory menuFactory, string installDir) : Page
    {
        private bool _createHeadlessProfile = false;
        private string? _headlessProfileId;
        private InstallMethod _installMethod;

        public override void OnShow()
        {
            SptInstance? sptInstance;

            bool isSptInstalled = SptUtils.IsSptInstalled(installDir);

            if (isSptInstalled)
            {
                sptInstance = new(installDir);
            }
            else
            {
                BrowseSptFolderPage browseSptFolderPage = new();
                browseSptFolderPage.Show();

                if (browseSptFolderPage.Result == null)
                {
                    return;
                }

                sptInstance = new(browseSptFolderPage.Result);
            }

            FikaHeadless fikaHeadless = new(sptInstance);

            if (!fikaHeadless.IsFikaServerInstalled())
            {
                Logger.Error("Fika-Server must be installed before installing Fika-Headless.", true);
                return;
            }

            List<SptProfile> sptProfiles = sptInstance.GetHeadlessProfiles();

            Menu profileSelectionMenu = menuFactory.CreateProfileSelectionMenu(sptProfiles);
            MenuChoice profileSelectionChoice = profileSelectionMenu.Show();

            if (profileSelectionChoice.Id == "createNewHeadlessProfile")
            {
                _createHeadlessProfile = true;
            }
            else
            {
                _headlessProfileId = profileSelectionChoice.Text;
            }

            if (!isSptInstalled)
            {
                Menu installMethodMenu = menuFactory.CreateInstallMethodMenu();
                MenuChoice installMethodMenuChoice = installMethodMenu.Show();

                if (!Enum.TryParse(installMethodMenuChoice.Text, out _installMethod))
                {
                    return;
                }
            }

            if (_createHeadlessProfile)
            {
                _headlessProfileId = fikaHeadless.CreateHeadlessProfile();

                if (string.IsNullOrEmpty(_headlessProfileId))
                {
                    Logger.Error("An error occurred while creating the headless profile. Please check the SPT Server logs.", true);
                    return;
                }
            }

            if (string.IsNullOrEmpty(_headlessProfileId))
            {
                return;
            }

            if (!fikaHeadless.CopyHeadlessConfig(_headlessProfileId, installDir))
            {
                return;
            }

            if (!isSptInstalled)
            {
                SptInstaller selectedSptInstaller = new(sptInstance.GamePath);

                if (!selectedSptInstaller.InstallSpt(installDir, _installMethod, true))
                {
                    return;
                }
            }

            SptInstaller sptInstaller = new(installDir);

            if (!sptInstaller.InstallSptRequirements(installDir))
            {
                return;
            }

            FikaInstaller fikaInstaller = new(installDir);

            if (!fikaInstaller.InstallReleaseList(FikaReleaseLists.HeadlessFika))
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            Logger.Success("Fika Headless installed successfully!", true);
        }
    }
}
