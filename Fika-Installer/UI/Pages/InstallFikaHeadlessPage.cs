using Fika_Installer.Fika;
using Fika_Installer.Models;
using Fika_Installer.Models.Spt;
using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public partial class Methods
    {
        public static void InstallHeadless(string installDir, string? headlessProfileId, InstallMethod? installMethod)
        {
            SptInstance sptInstance = new(installDir);
            bool isSptInstalled = SptUtils.IsSptInstalled(installDir);

            FikaHeadless fikaHeadless = new(sptInstance);

            if (!fikaHeadless.IsFikaServerInstalled())
            {
                Logger.Error("Fika-Server must be installed before installing Fika-Headless.", true);
                return;
            }

            if (headlessProfileId == null)
            {
                headlessProfileId = fikaHeadless.CreateHeadlessProfile();

                if (string.IsNullOrEmpty(headlessProfileId))
                {
                    Logger.Error("An error occurred while creating the headless profile. Please check the SPT Server logs.", true);
                    return;
                }
            }
            if (string.IsNullOrEmpty(headlessProfileId))
            {
                Logger.Error("Headless profile ID unexpectedly null.", true);
                return;
            }

            if (!fikaHeadless.CopyHeadlessConfig(headlessProfileId, installDir))
            {
                Logger.Error("Failed to copy headless profile config.", true);
                return;
            }

            if (!isSptInstalled)
            {
                SptInstaller selectedSptInstaller = new(sptInstance.GamePath);

                if (!selectedSptInstaller.InstallSpt(installDir, installMethod ?? InstallMethod.HardCopy, true))
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

    public class InstallFikaHeadlessPage(MenuFactory menuFactory, string installDir) : Page
    {
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

                installDir = browseSptFolderPage.Result;
                sptInstance = new(installDir);
            }            

            List<SptProfile> sptProfiles = sptInstance.GetHeadlessProfiles();

            Menu profileSelectionMenu = menuFactory.CreateProfileSelectionMenu(sptProfiles);
            MenuChoice profileSelectionChoice = profileSelectionMenu.Show();

            if (profileSelectionChoice.Id == "createNewHeadlessProfile")
            {
                _headlessProfileId = null;
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
        }
    }
}
