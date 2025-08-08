using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaPage : Page
    {
        private string _installDir;
        private string _fikaCoreReleaseUrl;
        private string _fikaServerReleaseUrl;
        
        public InstallFikaPage(string installDir, string fikaCoreReleaseUrl, string fikaServerReleaseUrl)
        {
            _installDir = installDir;
            _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
            _fikaServerReleaseUrl = fikaServerReleaseUrl;
        }
        
        public override void Draw()
        {
            FikaInstaller fikaInstaller = new(_installDir);

            bool isSptInstalled = fikaInstaller.IsSptInstalled();

            if (!isSptInstalled)
            {
                ConUtils.WriteError("SPT not found. Please place Fika-Installer inside your SPT directory.", true);
                return;
            }

            bool installResult = fikaInstaller.InstallRelease(_fikaCoreReleaseUrl);

            if (!installResult)
            {
                return;
            }

            bool installFikaServerResult = fikaInstaller.InstallRelease(_fikaServerReleaseUrl);

            if (!installFikaServerResult)
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            ConUtils.WriteSuccess("Fika installed successfully!", true);
        }
    }
}
