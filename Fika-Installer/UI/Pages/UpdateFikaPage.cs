using Fika_Installer.Models;
using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaPage(string installDir, FikaRelease fikaCoreRelease, FikaRelease fikaServerRelease, ILogger logger) : Page(logger)
    {
        public override void OnShow()
        {
            SptInstance sptInstance = new(installDir, CompositeLogger);
            FikaInstaller fikaInstaller = new(installDir, sptInstance, CompositeLogger);

            if (!fikaInstaller.InstallRelease(fikaCoreRelease))
            {
                return;
            }

            if (!fikaInstaller.InstallRelease(fikaServerRelease))
            {
                return;
            }

            CompositeLogger.Success("Fika updated successfully!", true);
        }
    }
}
