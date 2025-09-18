using Fika_Installer.Models;
using Fika_Installer.Models.Spt;
using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaHeadlessPage(MenuFactory menuFactory, string installDir, string fikaCoreReleaseUrl, string fikaHeadlessReleaseUrl, ILogger logger) : Page(logger)
    {
        private MenuFactory _menuFactory = menuFactory;
        private string _installDir = installDir;
        private string _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
        private string _fikaHeadlessReleaseUrl = fikaHeadlessReleaseUrl;

        public override void OnShow()
        {
            SptInstance? sptInstance;
            SptInstaller? sptInstaller;

            bool isSptInstalled = SptUtils.IsSptInstalled(_installDir);

            if (isSptInstalled)
            {
                sptInstance = new(_installDir, CompositeLogger);
                sptInstaller = new(_installDir, sptInstance, CompositeLogger);
            }
            else
            {
                sptInstance = SptUtils.BrowseAndValidateSptDir(CompositeLogger);

                if (sptInstance == null)
                {
                    return;
                }

                sptInstaller = new(_installDir, sptInstance, CompositeLogger);
            }

            FikaHeadless fikaHeadless = new(_installDir, sptInstance, CompositeLogger);

            if (!fikaHeadless.IsFikaServerInstalled())
            {
                CompositeLogger.Error("Fika-Server must be installed in the SPT folder before installing Fika-Headless.", true);
                return;
            }

            List<SptProfile> sptProfiles = sptInstance.GetHeadlessProfiles();

            Menu profileSelectionMenu = _menuFactory.CreateProfileSelectionMenu(sptProfiles);
            MenuChoice profileSelectionChoice = profileSelectionMenu.Show();

            string headlessProfileId;

            if (profileSelectionChoice.Id == "createNewHeadlessProfile")
            {
                SptProfile? headlessProfile = fikaHeadless.CreateHeadlessProfile();

                if (headlessProfile == null)
                {
                    CompositeLogger.Error("An error occurred while creating the headless profile. Please check the SPT Server logs.", true);
                    return;
                }

                headlessProfileId = headlessProfile.ProfileId;
            }
            else
            {
                headlessProfileId = profileSelectionChoice.Text;
            }

            if (string.IsNullOrEmpty(headlessProfileId))
            {
                return;
            }

            if (!fikaHeadless.CopyProfileScript(headlessProfileId))
            {
                return;
            }

            if (!isSptInstalled)
            {
                Menu installMethodMenu = _menuFactory.CreateInstallMethodMenu();
                MenuChoice installMethodMenuChoice = installMethodMenu.Show();

                string selectedInstallMethod = installMethodMenuChoice.Text;

                if (Enum.TryParse(selectedInstallMethod, out InstallMethod installType))
                {
                    if (!sptInstaller.InstallSpt(installType, true))
                    {
                        return;
                    }

                    // Change directory to installed SPT directory
                    sptInstance = new(_installDir, CompositeLogger);
                }
            }

            if (!sptInstaller.InstallSptRequirements())
            {
                return;
            }

            FikaInstaller fikaInstaller = new(_installDir, sptInstance, CompositeLogger);

            if (!fikaInstaller.InstallReleaseFromUrl(_fikaHeadlessReleaseUrl))
            {
                return;
            }

            if (!fikaInstaller.InstallReleaseFromUrl(_fikaCoreReleaseUrl))
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            CompositeLogger.Log("");
            CompositeLogger.Success("Fika Headless installed successfully!", true);
        }
    }
}
