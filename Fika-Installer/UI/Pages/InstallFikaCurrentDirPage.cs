using Fika_Installer.Models;
using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaCurrentDirPage(MenuFactory menuFactory, string installDir, FikaRelease fikaCoreRelease, FikaRelease fikaServerRelease, ILogger logger) : Page(logger)
    {
        public override void OnShow()
        {
            bool isSptInstalled = SptUtils.IsSptInstalled(installDir);

            if (!isSptInstalled)
            {
                BrowseSptFolderPage browseSptFolderPage = new(FileLogger);
                browseSptFolderPage.Show();

                if (browseSptFolderPage.Result == null)
                {
                    return;
                }

                Menu installMethodMenu = menuFactory.CreateInstallMethodMenu();
                MenuChoice installTypeChoice = installMethodMenu.Show();

                if (Enum.TryParse(installTypeChoice.Text, out InstallMethod installType))
                {
                    SptInstaller selectedSptInstaller = new(browseSptFolderPage.Result, CompositeLogger);

                    if (!selectedSptInstaller.InstallSpt(installDir, installType))
                    {
                        return;
                    }
                }
                else
                {
                    return;
                }
            }

            SptInstaller sptInstaller = new(installDir, CompositeLogger);

            if (!sptInstaller.InstallSptRequirements(installDir))
            {
                return;
            }

            FikaInstaller fikaInstaller = new(installDir, CompositeLogger);

            if (!fikaInstaller.InstallRelease(fikaCoreRelease))
            {
                return;
            }

            if (!fikaInstaller.InstallRelease(fikaServerRelease))
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            CompositeLogger?.Success("Fika installed successfully!", true);
        }
    }
}
