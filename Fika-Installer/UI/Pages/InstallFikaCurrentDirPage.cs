using Fika_Installer.Models;
using Fika_Installer.Models.Fika;
using Fika_Installer.Spt;
using System.Text.Json.Nodes;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaCurrentDirPage(MenuFactory menuFactory, string installDir) : Page
    {
        public override void OnShow()
        {
            bool isSptInstalled = SptUtils.IsSptInstalled(installDir);

            if (!isSptInstalled)
            {
                BrowseSptFolderPage browseSptFolderPage = new();
                browseSptFolderPage.Show();

                if (browseSptFolderPage.Result == null)
                {
                    return;
                }

                Menu installMethodMenu = menuFactory.CreateInstallMethodMenu();
                MenuChoice installTypeChoice = installMethodMenu.Show();

                if (Enum.TryParse(installTypeChoice.Text, out InstallMethod installType))
                {
                    SptInstaller selectedSptInstaller = new(browseSptFolderPage.Result);

                    if (!selectedSptInstaller.InstallSpt(installDir, installType))
                    {
                        return;
                    }

                    SptInstance sptInstance = new(installDir);
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
            }

            SptInstaller sptInstaller = new(installDir);

            if (!sptInstaller.InstallSptRequirements(installDir))
            {
                return;
            }

            FikaInstaller fikaInstaller = new(installDir);

            if (!fikaInstaller.InstallReleaseList(FikaReleaseLists.StandardFika))
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            Logger.Success("Fika installed successfully!", true);
        }
    }
}
