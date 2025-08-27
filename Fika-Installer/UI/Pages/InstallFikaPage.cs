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
            SptInstaller sptInstaller = new(_installDir, _installDir);
            SptInstance? sptInstance;

            bool isSptInstalled = sptInstaller.IsSptInstalled();

            if (isSptInstalled)
            {
                sptInstance = new(_installDir);
            }
            else
            {
                ConUtils.WriteError("SPT not found. Please place Fika-Installer inside your SPT directory.", true);
                return;
            }

            FikaInstaller fikaInstaller = new(_installDir, sptInstance);

            bool installResult = fikaInstaller.InstallReleaseFromUrl(_fikaCoreReleaseUrl);

            if (!installResult)
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
