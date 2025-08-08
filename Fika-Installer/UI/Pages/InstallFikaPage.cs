using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaPage : Page
    {
        private string _installDir;
        private string _fikaCoreReleaseUrl;
        private string _fikaServerReleaseUrl;
        
        public InstallFikaPage(string installDir, string fikaCoreReleaseUrl, string fikaServerReleaseUrl)
        {
            _installDir = installDir;
            _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
            _fikaServerReleaseUrl = fikaServerReleaseUrl;
        }
        
        public override void OnShow()
        {
            FikaInstaller fikaInstaller = new(_installDir);

            bool isSptInstalled = fikaInstaller.IsSptInstalled();

            if (!isSptInstalled)
            {
                string sptFolder = fikaInstaller.BrowseSptFolderAndValidate();

                if (string.IsNullOrEmpty(sptFolder))
                {
                    return;
                }

                fikaInstaller.InstallDir = sptFolder;
            }

            bool installResult = fikaInstaller.InstallRelease(_fikaCoreReleaseUrl);

            if (!installResult)
            {
                return;
            }

            bool installFikaServerResult = fikaInstaller.InstallRelease(_fikaServerReleaseUrl);

            if (!installFikaServerResult)
            {
                return;
            }

            ConUtils.WriteSuccess("Fika installed successfully!", true);
        }
    }
}
