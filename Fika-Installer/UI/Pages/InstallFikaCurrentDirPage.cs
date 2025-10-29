using Fika_Installer.Models.Enums;
using Fika_Installer.Spt;
using Fika_Installer.Utils;
using System.Text.Json.Nodes;
namespace Fika_Installer.UI.Pages
{
    public partial class PageFunctions
    {
        public static void InstallFikaCurrentDir(string installDir, string sptFolder, InstallMethod installMethod)
        {
            Logger.Log("Installing Fika in current folder...");

            bool isSptInstalled = SptUtils.IsSptInstalled(installDir);
            bool isSptFolderDetected = SptUtils.IsSptFolderDetected(installDir);

            if (!isSptInstalled && !isSptFolderDetected)
            {
                SptInstaller selectedSptInstaller = new(sptFolder);

                if (!selectedSptInstaller.InstallSpt(installDir, installMethod))
                {
                    return;
                }
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

            FwUtils.CreateFirewallRules(installDir);

            Logger.Success("Fika installed successfully!", true);
        }
    }
    
    public class InstallFikaCurrentDirPage(MenuFactory menuFactory, string installDir) : Page
    {
        public override void OnShow()
        {
            string sptFolder;
            InstallMethod installMethod = InstallMethod.HardCopy;

            bool isSptInstalled = SptUtils.IsSptInstalled(installDir);

            if (isSptInstalled)
            {
                sptFolder = installDir;
            }
            else
            {
                BrowseSptFolderPage browseSptFolderPage = new();
                browseSptFolderPage.Show();

                if (browseSptFolderPage.Result == null)
                {
                    return;
                }

                sptFolder = browseSptFolderPage.Result;

                Menu installMethodMenu = menuFactory.CreateInstallMethodMenu();
                MenuChoice installTypeChoice = installMethodMenu.Show();

                if (!Enum.TryParse(installTypeChoice.Text, out installMethod))
                {
                    // This should never happen
                    return;
                }
            }

            PageFunctions.InstallFikaCurrentDir(installDir, sptFolder, installMethod);
        }
    }
}
