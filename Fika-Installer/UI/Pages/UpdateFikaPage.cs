using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaPage(string installDir, string fikaCoreReleaseUrl, string fikaServerReleaseUrl, ILogger logger) : Page(logger)
    {
        public override void OnShow()
        {
            SptInstance sptInstance = new(installDir, CompositeLogger);
            FikaInstaller fikaInstaller = new(installDir, sptInstance, CompositeLogger);

            if (!fikaInstaller.InstallReleaseFromUrl(fikaCoreReleaseUrl))
            {
                return;
            }

            if (!fikaInstaller.InstallReleaseFromUrl(fikaServerReleaseUrl))
            {
                return;
            }

            CompositeLogger.Success("Fika updated successfully!", true);
        }
    }
}
