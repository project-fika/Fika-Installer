namespace Fika_Installer.UI.Pages
{
    public partial class PageFunctions
    {
        public static void UpdateFika(string installDir)
        {
            Logger.Log("Updating Fika...");

            bool fikaDetected = File.Exists(Installer.FikaCorePath(installDir));

            if (!fikaDetected)
            {
                Logger.Error("Fika not found. Please install Fika first.", true);
                return;
            }

            FikaInstaller fikaInstaller = new(installDir);

            if (!fikaInstaller.InstallReleaseList(FikaReleaseLists.StandardFika))
            {
                Logger.Error("Installer failed.");
                return;
            }

            Logger.Success("Fika updated successfully!", true);
        }
    }

    public class UpdateFikaPage(string installDir) : Page
    {
        public override void OnShow()
        {
            PageFunctions.UpdateFika(installDir);
        }
    }
}
