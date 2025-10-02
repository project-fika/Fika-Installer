using Fika_Installer.Models;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaHeadlessPage(string installDir, FikaRelease fikaCoreRelease, FikaRelease fikaHeadlessRelease) : Page
    {
        public override void OnShow()
        {
            FikaInstaller fikaInstaller = new(installDir);

            if (!fikaInstaller.InstallRelease(fikaHeadlessRelease))
            {
                return;
            }

            if (!fikaInstaller.InstallRelease(fikaCoreRelease))
            {
                return;
            }

            Logger.Success("Fika Headless updated successfully!", true);
        }
    }
}
