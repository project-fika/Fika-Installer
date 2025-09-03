using Fika_Installer.Spt;
using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public class UpdateFikaHeadlessPage(string installDir, string fikaCoreReleaseUrl, string fikaHeadlessReleaseUrl) : Page
    {
        private string _installDir = installDir;
        private string _fikaCoreReleaseUrl = fikaCoreReleaseUrl;
        private string _fikaHeadlessReleaseUrl = fikaHeadlessReleaseUrl;

        public override void OnShow()
        {
            SptInstance sptInstance = new(_installDir);
            FikaInstaller fikaInstaller = new(_installDir, sptInstance);

            if (!fikaInstaller.InstallReleaseFromUrl(_fikaHeadlessReleaseUrl))
            {
                return;
            }

            if (!fikaInstaller.InstallReleaseFromUrl(_fikaCoreReleaseUrl))
            {
                return;
            }

            Console.WriteLine();
            ConUtils.WriteSuccess("Fika Headless updated successfully!", true);
        }
    }
}
