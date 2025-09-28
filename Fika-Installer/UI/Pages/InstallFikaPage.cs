using Fika_Installer.Models;
using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaPage(string installDir, FikaRelease fikaCoreRelease, FikaRelease fikaServerRelease, ILogger logger) : Page(logger)
    {
        public override void OnShow()
        {
            bool isSptInstalled = SptUtils.IsSptInstalled(installDir);

            if (!isSptInstalled)
            {
                CompositeLogger.Error("SPT not found. Please place Fika-Installer inside your SPT directory.", true);
                return;
            }

            SptInstance sptInstance = new(installDir, CompositeLogger);
            SptInstaller sptInstaller = new(installDir, sptInstance, CompositeLogger);

            if (!sptInstaller.InstallSptRequirements())
            {
                return;
            }

            FikaInstaller fikaInstaller = new(installDir, sptInstance, CompositeLogger);

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
