using Fika_Installer.Models;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaHeadlessPage(string installDir, FikaRelease fikaCoreRelease, FikaRelease fikaHeadlessRelease, ILogger logger) : Page(logger)
    {
        public override void OnShow()
        {
            FikaInstaller fikaInstaller = new(installDir, CompositeLogger);

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
