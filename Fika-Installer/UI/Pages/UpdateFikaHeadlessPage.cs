using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaHeadlessPage : Page
    {
        private string _installDir;
        private string _fikaCoreReleaseUrl;
        private string _fikaHeadlessReleaseUrl;

        public UpdateFikaHeadlessPage(string installDir, string fikaCoreReleaseUrl, string fikaHeadlessReleaseUrl)
        {
            _installDir = installDir;
            _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
            _fikaHeadlessReleaseUrl = fikaHeadlessReleaseUrl;
        }


        public override void Draw()
        {
            FikaInstaller fikaInstaller = new(_installDir);

            bool installHeadlessResult = fikaInstaller.InstallRelease(_fikaHeadlessReleaseUrl);

            if (!installHeadlessResult)
            {
                return;
            }

            bool installFikaCoreResult = fikaInstaller.InstallRelease(_fikaCoreReleaseUrl);

            if (!installFikaCoreResult)
            {
                return;
            }

            ConUtils.WriteSuccess("Fika Headless updated successfully!", true);
        }
    }
}
