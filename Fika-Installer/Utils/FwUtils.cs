using System.Diagnostics;

namespace Fika_Installer.Utils
{
    public static class FwUtils
    {
        private const string _powershellCmd = "-NoProfile -ExecutionPolicy Bypass -Command";

        public record FirewallRule(
            string DisplayName,
            string Direction,
            string Protocol,
            string Port,
            string Program
        );

        private static FirewallRule[] FirewallRules(string installDir)
        {
            return [
                new("Fika (SPT) - TCP 6969", "Inbound", "TCP", "6969", Path.Combine(installDir, "SPT", SptConstants.ServerExeName)),
                new("Fika (Core) - UDP 25565", "Inbound", "UDP", "25565", Path.Combine(installDir, EftConstants.GameExeName))
            ];
        }

        /// <summary>
        /// Creates the necessary firewall rules for Fika, if they do not already exist or if forced.
        /// Spawns new instance of current exe with elevated permissions to create the rules.
        /// </summary>
        public static void CreateFirewallRules(string installDir, bool force = false)
        {
            FirewallRule[] firewallRuleSet = FirewallRules(installDir);

            Logger.Log("Checking existing firewall rules...");

            foreach (var rule in firewallRuleSet)
            {
                string firewallCmd = $@"
                    $existingRule = Get-NetFirewallRule -DisplayName '{rule.DisplayName}' |
                    Get-NetFirewallApplicationFilter |
                    Where-Object {{ $_.Program -eq '{rule.Program}' }}
                    
                    if (-not $existingRule) {{ throw 'Not Found' }}
                ";

                Process? powershellProcess = ProcUtils.Execute("powershell.exe", $"{_powershellCmd} \"{firewallCmd}\"", ProcessWindowStyle.Hidden);

                if (powershellProcess == null)
                {
                    Logger.Error("Failed to verify firewall rules.");
                    return;
                }

                bool ruleAlreadySet = powershellProcess.ExitCode == 0;

                if (!ruleAlreadySet || force)
                {
                    try
                    {
                        string firewallRuleArgs = $"\"{rule.DisplayName}\" \"{rule.Direction}\" \"{rule.Protocol}\" \"{rule.Port}\" \"{rule.Program}\"";
                        
                        Logger.Log($"Creating firewall rule: {firewallRuleArgs}...");

                        Process? elevatedProcess = ProcUtils.Execute(Application.ExecutablePath, $"create-firewall-rule {firewallRuleArgs}", ProcessWindowStyle.Minimized, true);

                        if (elevatedProcess == null)
                        {
                            Logger.Error("Failed to start elevated process for firewall rule creation.", true);
                            return;
                        }

                        if (elevatedProcess.ExitCode == 0)
                        {
                            Logger.Log("Firewall rule created successfully.");
                        }
                        else
                        {
                            Logger.Error($"Failed to create firewall rule. Elevated process returned exit code {elevatedProcess.ExitCode}.", true);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        Logger.Error($"Failed to create firewall rules: {ex.Message}", true);
                    }
                }
                else
                {
                    Logger.Log("Firewall rule already set.");
                }
            }
        }

        /// <summary>
        /// Writes the needed firewall rules.
        /// Called for create-firewall-rules CLI arg. Needs elevated permissions.
        /// </summary>
        public static void CreateFirewallRule(string displayName, string direction, string protocol, string port, string program = "")
        {
            string firewallCmd = $@"
                    New-NetFirewallRule -DisplayName '{displayName}' -Direction {direction} -Protocol {protocol} -LocalPort {port} -Program '{program}' -Action Allow -Enabled True -Profile Any
            ";

            ProcUtils.Execute("powershell.exe", $"{_powershellCmd} \"{firewallCmd}\"", ProcessWindowStyle.Hidden, true);
        }
    }
}