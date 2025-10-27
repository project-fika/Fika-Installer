using Fika_Installer.Spt;
using Fika_Installer.Utils;
using System.Text.Json.Nodes;

namespace Fika_Installer.UI.Pages
{
    public partial class Methods
    {
        public static void Update(string installDir, InteractiveMode interactive)
        {
            bool fikaDetected = File.Exists(Installer.FikaCorePath(installDir));

            if (!fikaDetected)
            {
                Logger.Error("Fika not found. Please install Fika first.", interactive == InteractiveMode.Interactive);
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

            Logger.Success("Fika updated successfully!", interactive == InteractiveMode.Interactive);
        }
    }

    public class UpdateFikaPage(string installDir) : Page
    {
        public override void OnShow()
        {
            Methods.Update(installDir, Methods.InteractiveMode.Interactive);
        }
    }
}
