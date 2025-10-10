namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaHeadlessPage(string installDir) : Page
    {
        public override void OnShow()
        {
            FikaInstaller fikaInstaller = new(installDir);

            if (!fikaInstaller.InstallReleaseList(FikaReleaseLists.HeadlessFika))
            {
                return;
            }

            Logger.Success("Fika Headless updated successfully!", true);
        }
    }
}
