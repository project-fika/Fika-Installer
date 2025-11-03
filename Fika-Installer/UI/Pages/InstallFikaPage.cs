using Fika_Installer.Models.Enums;
using Fika_Installer.Spt;
using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public partial class PageFunctions
    {
        public static void InstallFika()
        {
            Logger.Log("Installing Fika...");

            bool fikaDetected = File.Exists(Installer.FikaCorePath(Installer.CurrentDir));

            if (fikaDetected)
            {
                Logger.Error("Fika is already installed.", true);
                return;
            }

            bool isSptInstalled = SptUtils.IsSptInstalled(Installer.CurrentDir);
            bool isSptFolderDetected = SptUtils.IsSptFolderDetected(Installer.CurrentDir);

            if (!isSptInstalled && !isSptFolderDetected)
            {
                Logger.Error("SPT not found. Please place Fika-Installer inside your SPT directory.", true);
                return;
            }

            SptInstaller sptInstaller = new(Installer.CurrentDir);

            if (!sptInstaller.InstallSptRequirements(Installer.CurrentDir))
            {
                Logger.Error("Failed to install SPT requirements.", true);
                return;
            }

            FikaInstaller fikaInstaller = new(Installer.CurrentDir);

            if (!fikaInstaller.InstallReleaseList(FikaReleaseLists.StandardFika))
            {
                Logger.Error("Installer failed.");
                return;
            }

            FwUtils.CreateFirewallRules();

            Logger.Success("Fika installed successfully!", true);
        }
    }

    public class InstallFikaPage() : Page
    {
        public override void OnShow()
        {
            PageFunctions.InstallFika();
        }
    }
}
