using Fika_Installer.Models;
using Fika_Installer.Models.Spt;
using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaHeadlessPage(MenuFactory menuFactory, string installDir, string fikaCoreReleaseUrl, string fikaHeadlessReleaseUrl, ILogger logger) : Page(logger)
    {
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

            FikaHeadless fikaHeadless = new(installDir, sptInstance, CompositeLogger);

            if (!fikaHeadless.IsFikaServerInstalled())
            {
                CompositeLogger.Error("Fika-Server must be installed in the SPT folder before installing Fika-Headless.", true);
                return;
            }

            List<SptProfile> sptProfiles = sptInstance.GetHeadlessProfiles();

            Menu profileSelectionMenu = menuFactory.CreateProfileSelectionMenu(sptProfiles);
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

            SptInstaller sptInstaller = new(installDir, sptInstance, CompositeLogger);

            if (!isSptInstalled)
            {
                Menu installMethodMenu = menuFactory.CreateInstallMethodMenu();
                MenuChoice installMethodMenuChoice = installMethodMenu.Show();

                string selectedInstallMethod = installMethodMenuChoice.Text;

                if (Enum.TryParse(selectedInstallMethod, out InstallMethod installType))
                {
                    if (!sptInstaller.InstallSpt(installType, true))
                    {
                        return;
                    }

                    // Change directory to installed SPT directory
                    sptInstance = new(installDir, CompositeLogger);
                }
            }

            if (!sptInstaller.InstallSptRequirements())
            {
                return;
            }

            FikaInstaller fikaInstaller = new(installDir, sptInstance, CompositeLogger);

            if (!fikaInstaller.InstallReleaseFromUrl(fikaHeadlessReleaseUrl))
            {
                return;
            }

            if (!fikaInstaller.InstallReleaseFromUrl(fikaCoreReleaseUrl))
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            CompositeLogger.Success("Fika Headless installed successfully!", true);
        }
    }
}
