using Fika_Installer.Models;
using Fika_Installer.Spt;
using Fika_Installer.Utils;
using System.Text.Json.Nodes;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaCurrentDirPage : Page
    {
        private MenuFactory _menuFactory;
        private string _installDir;
        private string _fikaCoreReleaseUrl;
        private string _fikaServerReleaseUrl;

        public InstallFikaCurrentDirPage(MenuFactory menuFactory, string installDir, string fikaCoreReleaseUrl, string fikaServerReleaseUrl)
        {
            _menuFactory = menuFactory;
            _installDir = installDir;
            _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
            _fikaServerReleaseUrl = fikaServerReleaseUrl;
        }

        public override void OnShow()
        {
            SptInstaller sptInstaller = new(_installDir, _installDir);
            SptInstance? sptInstance;

            bool isSptInstalled = sptInstaller.IsSptInstalled();

            if (isSptInstalled)
            {
                sptInstance = new(_installDir);
            }
            else
            {
                string? sptDir = sptInstaller.BrowseAndValidateSptDir();

                if (sptDir == null)
                {
                    return;
                }

                Menu installMethodMenu = _menuFactory.CreateInstallMethodMenu();
                MenuChoice installTypeChoice = installMethodMenu.Show();

                string selectedInstallType = installTypeChoice.Text;

                if (Enum.TryParse(selectedInstallType, out InstallMethod installType))
                {
                    bool installSptResult = sptInstaller.InstallSpt(installType);

                    if (!installSptResult)
                    {
                        return;
                    }

                    sptInstance = new(_installDir);

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

            FikaInstaller fikaInstaller = new(_installDir, sptInstance);

            bool installFikaCoreResult = fikaInstaller.InstallReleaseFromUrl(_fikaCoreReleaseUrl);

            if (!installFikaCoreResult)
            {
                return;
            }

            bool installFikaServerResult = fikaInstaller.InstallReleaseFromUrl(_fikaServerReleaseUrl);

            if (!installFikaServerResult)
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            Console.WriteLine();
            ConUtils.WriteSuccess("Fika installed successfully!", true);
        }
    }
}
