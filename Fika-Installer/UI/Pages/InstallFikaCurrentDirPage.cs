using Fika_Installer.Models;
using Fika_Installer.Spt;
using Fika_Installer.Utils;
using System.Text.Json.Nodes;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaCurrentDirPage(MenuFactory menuFactory, string installDir, string fikaCoreReleaseUrl, string fikaServerReleaseUrl, ILogger logger) : Page(logger)
    {
        private MenuFactory _menuFactory = menuFactory;
        private string _installDir = installDir;
        private string _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
        private string _fikaServerReleaseUrl = fikaServerReleaseUrl;

        public override void OnShow()
        {
            SptInstance? sptInstance;
            SptInstaller? sptInstaller;

            bool isSptInstalled = SptUtils.IsSptInstalled(_installDir);

            if (isSptInstalled)
            {
                sptInstance = new(_installDir, PageLogger);
                sptInstaller = new(_installDir, _installDir, PageLogger);
            }
            else
            {
                string? sptDir = SptUtils.BrowseAndValidateSptDir();

                if (sptDir == null)
                {
                    return;
                }

                Menu installMethodMenu = _menuFactory.CreateInstallMethodMenu();
                MenuChoice installTypeChoice = installMethodMenu.Show();

                string selectedInstallType = installTypeChoice.Text;

                if (Enum.TryParse(selectedInstallType, out InstallMethod installType))
                {
                    sptInstaller = new(_installDir, sptDir, PageLogger);

                    if (!sptInstaller.InstallSpt(installType))
                    {
                        return;
                    }

                    sptInstance = new(_installDir, PageLogger);

                    JsonObject? launcherConfig = sptInstance.GetLauncherConfig();

                    if (launcherConfig != null)
                    {
                        launcherConfig["IsDevMode"] = true;
                        launcherConfig["GamePath"] = _installDir;
                        launcherConfig["Server"]["Url"] = "https://127.0.0.1:6969";

                        sptInstance.SetLauncherConfig(launcherConfig);
                    }
                }
                else
                {
                    return;
                }
            }

            if (!sptInstaller.InstallSptRequirements())
            {
                return;
            }

            FikaInstaller fikaInstaller = new(_installDir, sptInstance, PageLogger);

            if (!fikaInstaller.InstallReleaseFromUrl(_fikaCoreReleaseUrl))
            {
                return;
            }

            if (!fikaInstaller.InstallReleaseFromUrl(_fikaServerReleaseUrl))
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            PageLogger.Log("");
            PageLogger?.Success("Fika installed successfully!", true);
        }
    }
}
