using Fika_Installer.Models;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaPage(string installDir, FikaRelease fikaCoreRelease, FikaRelease fikaServerRelease, ILogger logger) : Page(logger)
    {
        public override void OnShow()
        {
            FikaInstaller fikaInstaller = new(installDir, CompositeLogger);

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
