using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaPage(string installDir, string fikaCoreReleaseUrl, string fikaServerReleaseUrl, ILogger logger) : Page(logger)
    {
        private string _installDir = installDir;
        private string _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
        private string _fikaServerReleaseUrl = fikaServerReleaseUrl;

        public override void OnShow()
        {
            SptInstance sptInstance = new(_installDir, PageLogger);
            FikaInstaller fikaInstaller = new(_installDir, sptInstance, PageLogger);

            if (!fikaInstaller.InstallReleaseFromUrl(_fikaCoreReleaseUrl))
            {
                return;
            }

            if (!fikaInstaller.InstallReleaseFromUrl(_fikaServerReleaseUrl))
            {
                return;
            }

            PageLogger.Log("");
            PageLogger.Success("Fika updated successfully!", true);
        }
    }
}
