using Fika_Installer.Models;
using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaHeadlessPage(string installDir, FikaRelease fikaCoreRelease, FikaRelease fikaHeadlessRelease, ILogger logger) : Page(logger)
    {
        public override void OnShow()
        {
            SptInstance sptInstance = new(installDir, CompositeLogger);
            FikaInstaller fikaInstaller = new(installDir, sptInstance, CompositeLogger);

            if (!fikaInstaller.InstallRelease(fikaHeadlessRelease))
            {
                return;
            }

            if (!fikaInstaller.InstallRelease(fikaCoreRelease))
            {
                return;
            }

            CompositeLogger.Success("Fika Headless updated successfully!", true);
        }
    }
}
