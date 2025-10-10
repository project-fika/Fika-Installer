namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaPage(string installDir) : Page
    {
        public override void OnShow()
        {
            FikaInstaller fikaInstaller = new(installDir);

            if (!fikaInstaller.InstallReleaseList(FikaReleaseLists.StandardFika))
            {
                return;
            }

            Logger.Success("Fika updated successfully!", true);
        }
    }
}
