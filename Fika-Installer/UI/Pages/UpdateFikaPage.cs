using Fika_Installer.Models.Fika;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaPage(string installDir, List<FikaRelease> releaseList) : Page
    {
        public override void OnShow()
        {
            FikaInstaller fikaInstaller = new(installDir);

            foreach (FikaRelease release in releaseList)
            {
                if (!fikaInstaller.InstallRelease(release))
                {
                    return;
                }
            }

            Logger.Success("Fika updated successfully!", true);
        }
    }
}
