using Fika_Installer.Utils;
namespace Fika_Installer.UI.Pages
{
    public partial class PageFunctions
    {
        public static void CreateFirewallRules()
        {
            if (!FwUtils.CreateFirewallRules())
            {
                Logger.Error("An error occurred when creating firewall rules.", true);
                return;
            }

            Logger.Success("Firewall rules created successfully.", true);
        }
    }

    public class AddFirewallRulesPage() : Page
    {
        public override void OnShow()
        {
            PageFunctions.CreateFirewallRules();
        }
    }
}
