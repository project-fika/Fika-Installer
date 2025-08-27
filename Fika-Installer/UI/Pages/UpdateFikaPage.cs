using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaPage : Page
    {
        private string _installDir;
        private string _fikaCoreReleaseUrl;
        private string _fikaServerReleaseUrl;

        public UpdateFikaPage(string installDir, string fikaCoreReleaseUrl, string fikaServerReleaseUrl)
        {
            _installDir = installDir;
            _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
            _fikaServerReleaseUrl = fikaServerReleaseUrl;
        }

        public override void OnShow()
        {
            FikaInstaller fikaInstaller = new(_installDir, _installDir);

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

            ConUtils.WriteSuccess("Fika updated successfully!", true);
        }
    }
}
