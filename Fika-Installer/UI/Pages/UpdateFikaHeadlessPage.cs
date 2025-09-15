using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaHeadlessPage(string installDir, string fikaCoreReleaseUrl, string fikaHeadlessReleaseUrl, ILogger logger) : Page(logger)
    {
        private string _installDir = installDir;
        private string _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
        private string _fikaHeadlessReleaseUrl = fikaHeadlessReleaseUrl;

        public override void OnShow()
        {
            SptInstance sptInstance = new(_installDir, CompositeLogger);
            FikaInstaller fikaInstaller = new(_installDir, sptInstance, CompositeLogger);

            if (!fikaInstaller.InstallReleaseFromUrl(_fikaHeadlessReleaseUrl))
            {
                return;
            }

            if (!fikaInstaller.InstallReleaseFromUrl(_fikaCoreReleaseUrl))
            {
                return;
            }

            CompositeLogger.Log("");
            CompositeLogger.Success("Fika Headless updated successfully!", true);
        }
    }
}
