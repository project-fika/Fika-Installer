using Fika_Installer.Models.Fika;
using Fika_Installer.Spt;
using Fika_Installer.Utils;

namespace Fika_Installer.UI.Pages
{
    public class InstallFikaPage(string installDir, List<FikaRelease> releaseList) : Page
    {
        public override void OnShow()
        {
            bool isSptInstalled = SptUtils.IsSptInstalled(installDir);

            if (!isSptInstalled)
            {
                ConUtils.WriteError("SPT not found. Please place Fika-Installer inside your SPT directory.", true);
                return;
            }

            SptInstaller sptInstaller = new(installDir);

            if (!sptInstaller.InstallSptRequirements(installDir))
            {
                return;
            }

            FikaInstaller fikaInstaller = new(installDir);

            foreach (FikaRelease release in releaseList)
            {
                if (!fikaInstaller.InstallRelease(release))
                {
                    return;
                }
            }

            fikaInstaller.ApplyFirewallRules();

            Logger.Success("Fika installed successfully!", true);
        }
    }
}
