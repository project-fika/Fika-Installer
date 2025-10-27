using Fika_Installer.Spt;
using Fika_Installer.Utils;
using System.Text.Json.Nodes;

namespace Fika_Installer.UI.Pages
{
    public partial class Methods
    {
        public static void Uninstall(string installDir)
        {
            bool fikaDetected = File.Exists(Installer.FikaCorePath(installDir));

            if (!fikaDetected)
            {
                Logger.Error("Fika not found. Please install Fika first.", true);
                return;
            }

            FikaInstaller fikaInstaller = new(installDir);

            if (!fikaInstaller.UninstallFika())
            {
                Logger.Error("Installer failed.");
                return;
            }

            SptInstance sptInstance = new(installDir);

            JsonObject? launcherConfig = sptInstance.GetLauncherConfig();

            if (launcherConfig != null)
            {
                launcherConfig["IsDevMode"] = false;
                launcherConfig["GamePath"] = installDir;
                launcherConfig["Server"]["Url"] = "https://127.0.0.1:6969";

                sptInstance.SetLauncherConfig(launcherConfig);
            }

            Logger.Success("Fika uninstalled successfully!", true);
        }
    }

    public class UninstallFikaPage(MenuFactory menuFactory, string installDir) : Page
    {
        public override void OnShow()
        {
            Menu uninstallFikaMenu = menuFactory.CreateConfirmUninstallFikaMenu();
            MenuChoice selectedChoice = uninstallFikaMenu.Show();

            if (selectedChoice.Text == "No")
            {
                return;
            }

            Methods.Uninstall(installDir);
        }
    }
}