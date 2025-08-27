using Fika_Installer.Models;
using Fika_Installer.Models.Spt;
using Fika_Installer.Spt;
using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaHeadlessPage(MenuFactory menuFactory, string installDir, string fikaCoreReleaseUrl, string fikaHeadlessReleaseUrl) : Page
    {
        private MenuFactory _menuFactory = menuFactory;
        private string _installDir = installDir;
        private string _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
        private string _fikaHeadlessReleaseUrl = fikaHeadlessReleaseUrl;

        public override void OnShow()
        {
            SptInstaller sptInstaller = new(_installDir, _installDir);
            SptInstance? sptInstance;

            bool isSptInstalled = sptInstaller.IsSptInstalled();

            if (isSptInstalled)
            {
                sptInstance = new(_installDir);
            }
            else
            {
                string? sptDir = sptInstaller.BrowseAndValidateSptDir();

                if (sptDir == null)
                {
                    return;
                }
                
                sptInstance = new(sptDir);
            }

            FikaHeadless fikaHeadless = new(_installDir, sptInstance);

            bool isFikaServerInstalled = fikaHeadless.IsFikaServerInstalled();

            if (!isFikaServerInstalled)
            {
                ConUtils.WriteError("Fika-Server must be installed in the SPT folder before installing Fika-Headless.", true);
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
                    ConUtils.WriteError("An error occurred while creating the headless profile. Please check the SPT Server logs.", true);
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

            bool copyProfileScriptResult = fikaHeadless.CopyProfileScript(headlessProfileId);

            if (!copyProfileScriptResult)
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
                    bool installSptResult = sptInstaller.InstallSpt(installType, true);

                    if (!installSptResult)
                    {
                        return;
                    }
                }
            }

            FikaInstaller fikaInstaller = new(_installDir, sptInstance);

            bool installHeadlessResult = fikaInstaller.InstallReleaseFromUrl(_fikaHeadlessReleaseUrl);

            if (!installHeadlessResult)
            {
                return;
            }

            bool installFikaResult = fikaInstaller.InstallReleaseFromUrl(_fikaCoreReleaseUrl);

            if (!installFikaResult)
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            Console.WriteLine();
            ConUtils.WriteSuccess("Fika Headless installed successfully!", true);
        }
    }
}
