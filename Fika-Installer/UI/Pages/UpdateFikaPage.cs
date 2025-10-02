using Fika_Installer.Models;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaPage(string installDir, FikaRelease fikaCoreRelease, FikaRelease fikaServerRelease) : Page
    {
        public override void OnShow()
        {
            FikaInstaller fikaInstaller = new(installDir);

            if (!fikaInstaller.InstallRelease(fikaCoreRelease))
            {
                return;
            }

            if (!fikaInstaller.InstallRelease(fikaServerRelease))
            {
                return;
            }

            Logger.Success("Fika updated successfully!", true);
        }
    }
}
