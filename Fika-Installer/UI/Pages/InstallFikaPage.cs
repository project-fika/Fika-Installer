using Fika_Installer.Spt;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaPage(string installDir, string fikaCoreReleaseUrl, string fikaServerReleaseUrl, ILogger logger) : Page(logger)
    {
        private string _installDir = installDir;
        private string _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
        private string _fikaServerReleaseUrl = fikaServerReleaseUrl;

        public override void OnShow()
        {
            bool isSptInstalled = SptUtils.IsSptInstalled(_installDir);

            if (!isSptInstalled)
            {
                CompositeLogger.Error("SPT not found. Please place Fika-Installer inside your SPT directory.", true);
                return;
            }

            SptInstance sptInstance = new(_installDir, CompositeLogger);
            SptInstaller sptInstaller = new(_installDir, sptInstance, CompositeLogger);

            if (!sptInstaller.InstallSptRequirements())
            {
                return;
            }

            FikaInstaller fikaInstaller = new(_installDir, sptInstance, CompositeLogger);

            if (!fikaInstaller.InstallReleaseFromUrl(_fikaCoreReleaseUrl))
            {
                return;
            }

            if (!fikaInstaller.InstallReleaseFromUrl(_fikaServerReleaseUrl))
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            CompositeLogger?.Success("Fika installed successfully!", true);
        }
    }
}
