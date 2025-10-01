using Fika_Installer.Models;
using Fika_Installer.Models.Spt;
using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaHeadlessPage(MenuFactory menuFactory, string installDir, FikaRelease fikaCoreRelease, FikaRelease fikaHeadlessRelease, ILogger logger) : Page(logger)
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
                sptInstance = new(installDir, CompositeLogger);
            }
            else
            {
                BrowseSptFolderPage browseSptFolderPage = new(FileLogger);
                browseSptFolderPage.Show();

                if (browseSptFolderPage.Result == null)
                {
                    return;
                }

                sptInstance = new(browseSptFolderPage.Result, CompositeLogger);
            }

            FikaHeadless fikaHeadless = new(sptInstance, CompositeLogger);

            if (!fikaHeadless.IsFikaServerInstalled())
            {
                CompositeLogger.Error("Fika-Server must be installed in the SPT folder before installing Fika-Headless.", true);
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
                SptProfile? headlessProfile = fikaHeadless.CreateHeadlessProfile();

                if (headlessProfile == null)
                {
                    CompositeLogger.Error("An error occurred while creating the headless profile. Please check the SPT Server logs.", true);
                    return;
                }

                _headlessProfileId = headlessProfile.ProfileId;
            }

            if (string.IsNullOrEmpty(_headlessProfileId))
            {
                return;
            }

            if (!fikaHeadless.CopyProfileScript(_headlessProfileId, installDir))
            {
                return;
            }

            if (!isSptInstalled)
            {
                SptInstaller selectedSptInstaller = new(sptInstance.SptPath, CompositeLogger);

                if (!selectedSptInstaller.InstallSpt(installDir, _installMethod, true))
                {
                    return;
                }
            }

            SptInstaller sptInstaller = new(installDir, CompositeLogger);

            if (!sptInstaller.InstallSptRequirements(installDir))
            {
                return;
            }

            FikaInstaller fikaInstaller = new(installDir, CompositeLogger);

            if (!fikaInstaller.InstallRelease(fikaHeadlessRelease))
            {
                return;
            }

            if (!fikaInstaller.InstallRelease(fikaCoreRelease))
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            CompositeLogger.Success("Fika Headless installed successfully!", true);
        }
    }
}
