namespace Fika_Installer.UI.Pages
{
    public partial class PageFunctions
    {
        public static void UpdateHeadless(string installDir)
        {
            Logger.Log("Updating Fika Headless...");

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

    public class UpdateFikaHeadlessPage() : Page
    {
        public override void OnShow()
        {
            PageFunctions.UpdateHeadless(Installer.CurrentDir);
        }
    }
}
