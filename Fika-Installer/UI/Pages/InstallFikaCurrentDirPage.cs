using Fika_Installer.Models;
using Fika_Installer.Spt;
using System.Text.Json.Nodes;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaCurrentDirPage(MenuFactory menuFactory, string installDir, string fikaCoreReleaseUrl, string fikaServerReleaseUrl, ILogger logger) : Page(logger)
    {
        public override void OnShow()
        {
            SptInstance? sptInstance;

            bool isSptInstalled = SptUtils.IsSptInstalled(installDir);

            if (isSptInstalled)
            {
                sptInstance = new(installDir, CompositeLogger);
            }
            else
            {
                BrowseSptFolderPage browseSptFolderPage = new(FileLogger);
                browseSptFolderPage.Show();

                if (browseSptFolderPage.Result == null)
                {
                    return;
                }

                sptInstance = new(browseSptFolderPage.Result, CompositeLogger);
            }
                
            SptInstaller sptInstaller = new(installDir, sptInstance, CompositeLogger);

            Menu installMethodMenu = menuFactory.CreateInstallMethodMenu();
            MenuChoice installTypeChoice = installMethodMenu.Show();

            if (Enum.TryParse(installTypeChoice.Text, out InstallMethod installType))
            {
                if (!sptInstaller.InstallSpt(installType))
                {
                    return;
                }

                // Change directory to installed SPT directory
                sptInstance = new(installDir, CompositeLogger);

                JsonObject? launcherConfig = sptInstance.GetLauncherConfig();

                if (launcherConfig != null)
                {
                    launcherConfig["IsDevMode"] = true;
                    launcherConfig["GamePath"] = installDir;
                    launcherConfig["Server"]["Url"] = "https://127.0.0.1:6969";

                    sptInstance.SetLauncherConfig(launcherConfig);
                }
            }
            else
            {
                return;
            }

            if (!sptInstaller.InstallSptRequirements())
            {
                return;
            }

            FikaInstaller fikaInstaller = new(installDir, sptInstance, CompositeLogger);

            if (!fikaInstaller.InstallReleaseFromUrl(fikaCoreReleaseUrl))
            {
                return;
            }

            if (!fikaInstaller.InstallReleaseFromUrl(fikaServerReleaseUrl))
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            CompositeLogger?.Success("Fika installed successfully!", true);
        }
    }
}
