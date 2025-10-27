using Fika_Installer.Spt;
using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public partial class Methods
    {
        public static void Install(string installDir)
        {
            bool fikaDetected = File.Exists(Installer.FikaCorePath(installDir));
            
            if (fikaDetected)
            {
                Logger.Error("Fika is already installed.", true);
                return;
            }

            bool isSptInstalled = SptUtils.IsSptInstalled(installDir);

            if (!isSptInstalled)
            {
                Logger.Error("SPT not found. Please place Fika-Installer inside your SPT directory.", true);
                return;
            }

            SptInstaller sptInstaller = new(installDir);

            if (!sptInstaller.InstallSptRequirements(installDir))
            {
                Logger.Error("SPT Installer failed.");
                return;
            }

            FikaInstaller fikaInstaller = new(installDir);

            if (!fikaInstaller.InstallReleaseList(FikaReleaseLists.StandardFika))
            {
                Logger.Error("Installer failed.");
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            Logger.Success("Fika installed successfully!", true);
        }
    }

    public class InstallFikaPage(string installDir) : Page
    {
        public override void OnShow()
        {
            Methods.Install(installDir);
        }
    }
}
