using Fika_Installer.Spt;
using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public partial class Methods
    {
        public static void Install(string installDir, InteractiveMode interactive)
        {
            bool fikaDetected = File.Exists(Installer.FikaCorePath(installDir));
            
            if (fikaDetected)
            {
                Logger.Error("Fika is already installed.", interactive == InteractiveMode.Interactive);
                if (interactive == InteractiveMode.NonInteractive) { Environment.Exit(1); }
                return;
            }

            bool isSptInstalled = SptUtils.IsSptInstalled(installDir);

            if (!isSptInstalled)
            {
                Logger.Error("SPT not found. Please place Fika-Installer inside your SPT directory.", interactive == InteractiveMode.Interactive);
                if (interactive == InteractiveMode.NonInteractive) { Environment.Exit(1); }
                return;
            }

            SptInstaller sptInstaller = new(installDir);

            if (!sptInstaller.InstallSptRequirements(installDir))
            {
                Logger.Error("SPT Installer failed.");
                if (interactive == InteractiveMode.NonInteractive) { Environment.Exit(1); }
                return;
            }

            FikaInstaller fikaInstaller = new(installDir);

            if (!fikaInstaller.InstallReleaseList(FikaReleaseLists.StandardFika))
            {
                Logger.Error("Installer failed.");
                if (interactive == InteractiveMode.NonInteractive) { Environment.Exit(1); }
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            Logger.Success("Fika installed successfully!", interactive == InteractiveMode.Interactive);
        }
    }

    public class InstallFikaPage(string installDir) : Page
    {
        public override void OnShow()
        {
            Methods.Install(installDir, Methods.InteractiveMode.Interactive);
        }
    }
}
