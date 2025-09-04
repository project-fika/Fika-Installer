using Fika_Installer.Spt;
using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaPage(string installDir, string fikaCoreReleaseUrl, string fikaServerReleaseUrl) : Page
    {
        private string _installDir = installDir;
        private string _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
        private string _fikaServerReleaseUrl = fikaServerReleaseUrl;

        public override void OnShow()
        {
            bool isSptInstalled = SptUtils.IsSptInstalled(_installDir);

            if (!isSptInstalled)
            {
                ConUtils.WriteError("SPT not found. Please place Fika-Installer inside your SPT directory.", true);
                return;
            }
            
            SptInstaller sptInstaller = new(_installDir, _installDir);

            if (!sptInstaller.InstallSptRequirements())
            {
                return;
            }

            SptInstance sptInstance = new(_installDir);

            FikaInstaller fikaInstaller = new(_installDir, sptInstance);

            if (!fikaInstaller.InstallReleaseFromUrl(_fikaCoreReleaseUrl))
            {
                return;
            }

            if (!fikaInstaller.InstallReleaseFromUrl(_fikaServerReleaseUrl))
            {
                return;
            }

            fikaInstaller.ApplyFirewallRules();

            Console.WriteLine();
            ConUtils.WriteSuccess("Fika installed successfully!", true);
        }
    }
}
