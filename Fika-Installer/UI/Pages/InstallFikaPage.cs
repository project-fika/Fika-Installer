using Fika_Installer.Models;
using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaPage(string installDir, FikaRelease fikaCoreRelease, FikaRelease fikaServerRelease) : Page
    {
        public override void OnShow()
        {
            bool isSptInstalled = SptUtils.IsSptInstalled(installDir);

            if (!isSptInstalled)
            {
                Logger.Error("SPT not found. Please place Fika-Installer inside your SPT directory.", true);
                return;
            }

            SptInstaller sptInstaller = new(installDir);

            if (!sptInstaller.InstallSptRequirements(installDir))
            {
                return;
            }

            FikaInstaller fikaInstaller = new(installDir);

            if (!fikaInstaller.InstallRelease(fikaCoreRelease))
            {
                return;
            }

            if (!fikaInstaller.InstallRelease(fikaServerRelease))
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            Logger.Success("Fika installed successfully!", true);
        }
    }
}
