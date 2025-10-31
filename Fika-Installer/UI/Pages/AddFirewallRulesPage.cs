using Fika_Installer.Utils;
namespace Fika_Installer.UI.Pages
{
    public partial class PageFunctions
    {
        public static void CreateFirewallRules(string installDir)
        {
            if (!FwUtils.CreateFirewallRules(installDir))
            {
                Logger.Error("An error occurred when creating firewall rules.", true);
                return;
            }

            Logger.Success("Firewall rules created successfully.", true);
        }
    }

    public class AddFirewallRulesPage(string installDir) : Page
    {
        public override void OnShow()
        {
            PageFunctions.CreateFirewallRules(installDir);
        }
    }
}
