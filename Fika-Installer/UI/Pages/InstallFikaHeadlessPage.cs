using Fika_Installer.Models;
using Fika_Installer.Models.UI;
using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaHeadlessPage : Page
    {
        private MenuFactory _menuFactory;
        private string _installDir;
        private string _sptFolder;
        private string _fikaCoreReleaseUrl;
        private string _fikaHeadlessReleaseUrl;

        public InstallFikaHeadlessPage(MenuFactory menuFactory, string installDir, string sptFolder, string fikaCoreReleaseUrl, string fikaHeadlessReleaseUrl)
        {
            _menuFactory = menuFactory;
            _installDir = installDir;
            _sptFolder = sptFolder;
            _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
            _fikaHeadlessReleaseUrl = fikaHeadlessReleaseUrl;
        }

        public override void Draw()
        {
            FikaInstaller fikaInstaller = new(_installDir, _sptFolder);

            bool isSptInstalled = fikaInstaller.IsSptInstalled();

            if (!isSptInstalled)
            {
                _sptFolder = fikaInstaller.BrowseSptFolderAndValidate();

                if (string.IsNullOrEmpty(_sptFolder))
                {
                    return;
                }
            }

            FikaHeadless fikaHeadless = new FikaHeadless(_installDir, _sptFolder);

            bool isFikaServerInstalled = fikaHeadless.IsFikaServerInstalled();

            if (!isFikaServerInstalled)
            {
                ConUtils.WriteError("Fika-Server must be installed in the SPT folder before installing Fika-Headless.", true);
                return;
            }

            bool isFikaServerConfigFound = fikaHeadless.IsFikaConfigFound();

            if (!isFikaServerConfigFound)
            {
                ConUtils.WriteError("Please run SPT.Server.exe at least once before installing Fika-Headless.", true);
                return;
            }

            List<SptProfile> sptProfileIds = fikaHeadless.SptProfiles;

            Menu profileSelectionMenu = _menuFactory.CreateProfileSelectionMenu(sptProfileIds);
            MenuChoice profileSelectionChoice = profileSelectionMenu.Show();

            string headlessProfileId = "";

            if (profileSelectionChoice.Id == "createNewHeadlessProfile")
            {
                SptProfile? headlessProfile = fikaHeadless.CreateHeadlessProfile();

                if (headlessProfile == null)
                {
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
                    bool installSptResult = fikaInstaller.InstallSpt(installType, true);

                    if (!installSptResult)
                    {
                        return;
                    }
                }
            }

            bool installHeadlessResult = fikaInstaller.InstallRelease(_fikaHeadlessReleaseUrl);

            if (!installHeadlessResult)
            {
                return;
            }

            bool installFikaResult = fikaInstaller.InstallRelease(_fikaCoreReleaseUrl);

            if (!installFikaResult)
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules(_installDir, _sptFolder);

            ConUtils.WriteSuccess("Fika Headless installed successfully!", true);
        }
    }
}
