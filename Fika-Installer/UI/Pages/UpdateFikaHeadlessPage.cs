using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaHeadlessPage(string installDir, string fikaCoreReleaseUrl, string fikaHeadlessReleaseUrl, ILogger logger) : Page(logger)
    {
        public override void OnShow()
        {
            SptInstance sptInstance = new(installDir, CompositeLogger);
            FikaInstaller fikaInstaller = new(installDir, sptInstance, CompositeLogger);

            if (!fikaInstaller.InstallReleaseFromUrl(fikaHeadlessReleaseUrl, "Fika.Headless"))
            {
                return;
            }

            if (!fikaInstaller.InstallReleaseFromUrl(fikaCoreReleaseUrl, "Fika.Release"))
            {
                return;
            }

            CompositeLogger.Success("Fika Headless updated successfully!", true);
        }
    }
}
