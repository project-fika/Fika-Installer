using Fika_Installer.Spt;
using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaPage(string installDir, string fikaCoreReleaseUrl, string fikaServerReleaseUrl) : Page
    {
        private string _installDir = installDir;
        private string _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
        private string _fikaServerReleaseUrl = fikaServerReleaseUrl;

        public override void OnShow()
        {
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

            Console.WriteLine();
            ConUtils.WriteSuccess("Fika updated successfully!", true);
        }
    }
}
