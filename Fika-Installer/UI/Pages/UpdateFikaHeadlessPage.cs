namespace Fika_Installer.UI.Pages
{
    public partial class Methods
    {
        public static void UpdateHeadless(string installDir)
        {
            bool fikaDetected = File.Exists(Installer.FikaCorePath(installDir));

            if (!fikaDetected)
            {
                Logger.Error("Fika not found. Please install Fika first.", true);
                return;
            }

            FikaInstaller fikaInstaller = new(installDir);

            if (!fikaInstaller.InstallReleaseList(FikaReleaseLists.HeadlessFika))
            {
                Logger.Error("Installer failed.");
                return;
            }

            Logger.Success("Fika Headless updated successfully!", true);
        }
    }

    public class UpdateFikaHeadlessPage(string installDir) : Page
    {
        public override void OnShow()
        {
            FikaInstaller fikaInstaller = new(installDir);

            Methods.UpdateHeadless(installDir);
        }
    }
}
