using Fika_Installer.Spt;
using System.Text.Json.Nodes;

namespace Fika_Installer.UI.Pages
{
    public class UninstallFikaPage(MenuFactory menuFactory, string installDir, ILogger logger) : Page(logger)
    {
        public override void OnShow()
        {
            Menu uninstallFikaMenu = menuFactory.CreateConfirmUninstallFikaMenu();
            MenuChoice selectedChoice = uninstallFikaMenu.Show();

            if (selectedChoice.Text == "No")
            {
                return;
            }

            FikaInstaller fikaInstaller = new(installDir, CompositeLogger);

            if (!fikaInstaller.UninstallFika())
            {
                return;
            }

            SptInstance sptInstance = new(installDir, CompositeLogger);

            JsonObject? launcherConfig = sptInstance.GetLauncherConfig();

            if (launcherConfig != null)
            {
                launcherConfig["IsDevMode"] = false;
                launcherConfig["GamePath"] = installDir;
                launcherConfig["Server"]["Url"] = "https://127.0.0.1:6969";

                sptInstance.SetLauncherConfig(launcherConfig);
            }

            CompositeLogger.Success("Fika uninstalled successfully!", true);
        }
    }
}