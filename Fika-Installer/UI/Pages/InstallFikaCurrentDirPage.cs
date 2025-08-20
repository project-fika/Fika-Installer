using Fika_Installer.Models;
using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaCurrentDirPage : Page
    {
        private MenuFactory _menuFactory;
        private string _installDir;
        private string _sptFolder;
        private string _fikaCoreReleaseUrl;
        private string _fikaServerReleaseUrl;

        public InstallFikaCurrentDirPage(MenuFactory menuFactory, string installDir, string sptFolder, string fikaCoreReleaseUrl, string fikaServerReleaseUrl)
        {
            _menuFactory = menuFactory;
            _installDir = installDir;
            _sptFolder = sptFolder;
            _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
            _fikaServerReleaseUrl = fikaServerReleaseUrl;
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

                Menu installMethodMenu = _menuFactory.CreateInstallMethodMenu();
                MenuChoice installTypeChoice = installMethodMenu.Show();

                string selectedInstallType = installTypeChoice.Text;

                if (Enum.TryParse(selectedInstallType, out InstallMethod installType))
                {
                    bool installSptResult = fikaInstaller.InstallSpt(installType);

                    if (!installSptResult)
                    {
                        return;
                    }

                    fikaInstaller.ConfigureSptLauncherConfig();
                }
            }

            bool installFikaCoreResult = fikaInstaller.InstallRelease(_fikaCoreReleaseUrl);

            if (!installFikaCoreResult)
            {
                return;
            }

            bool installFikaServerResult = fikaInstaller.InstallRelease(_fikaServerReleaseUrl);

            if (!installFikaServerResult)
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules(_installDir, _installDir);

            ConUtils.WriteSuccess("Fika installed successfully!", true);
        }
    }
}
