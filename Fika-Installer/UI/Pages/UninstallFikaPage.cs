using Fika_Installer.Spt;

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

            SptInstance sptInstance = new(installDir, CompositeLogger);
            FikaInstaller fikaInstaller = new(installDir, sptInstance, CompositeLogger);

            if (!fikaInstaller.UninstallFika())
            {
                return;
            }

            CompositeLogger.Success("Fika uninstalled successfully!", true);
        }
    }
}