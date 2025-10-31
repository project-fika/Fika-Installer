using System.Diagnostics;

namespace Fika_Installer.Utils
{
    public static class FwUtils
    {
        private const string _psExeName = "Powershell.exe";
        private const string _psCmdArgs = "-NoProfile -ExecutionPolicy Bypass -Command";
        private static readonly FirewallRule[] _firewallRules =
        [
            new("Fika (SPT) - TCP 6969", "Inbound", "TCP", "6969", Path.Combine(Installer.CurrentDir, "SPT", SptConstants.ServerExeName)),
            new("Fika (Core) - UDP 25565", "Inbound", "UDP", "25565", Path.Combine(Installer.CurrentDir, EftConstants.GameExeName))
        ];

        public record FirewallRule(
            string DisplayName,
            string Direction,
            string Protocol,
            string Port,
            string Program
        );

        /// <summary>
        /// Creates the necessary firewall rules for Fika, if they do not already exist or if forced.
        /// Spawns new instance of current exe with elevated permissions to create the rules.
        /// </summary>
        public static bool CreateFirewallRules(string installDir, bool force = false)
        {
            bool success = FirewallRulesExists(out bool rulesExists);

            if (!success)
            {
                return false;
            }

            if (!rulesExists || force)
            {
                Logger.Log("Creating firewall rules...");
                
                try
                {
                    Process? elevatedProcess = ProcUtils.Execute(Application.ExecutablePath, $"create-firewall-rules", ProcessWindowStyle.Minimized, true);

                    if (elevatedProcess == null)
                    {
                        Logger.Error("Failed to start elevated process for firewall rule creation.", true);
                        return false;
                    }

                    // TODO: Powershell does not return an exit code when error is thrown on a cmdlet.
                    if (elevatedProcess.ExitCode == 0)
                    {
                        return true;
                    }
                    else
                    {
                        Logger.Error($"Elevated process returned exit code {elevatedProcess.ExitCode}.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    return false;
                }
            }
            else
            {
                Logger.Log("Firewall rules already set.");
                return true;
            }
        }

        /// <summary>
        /// Remove firewall rules for Fika, if they exist or if forced.
        /// Spawns new instance of current exe with elevated permissions to remove the rules.
        /// </summary>
        public static bool RemoveFirewallRules(bool force = false)
        {
            bool success = FirewallRulesExists(out bool rulesExists);

            if (!success)
            {
                return false;
            }

            if (rulesExists || force)
            {
                Logger.Log("Removing firewall rules...");

                try
                {
                    Process? elevatedProcess = ProcUtils.Execute(Application.ExecutablePath, $"remove-firewall-rules", ProcessWindowStyle.Minimized, true);

                    if (elevatedProcess == null)
                    {
                        Logger.Error("Failed to start elevated process for firewall rule removal.", true);
                        return false;
                    }

                    if (elevatedProcess.ExitCode == 0)
                    {
                        return true;
                    }
                    else
                    {
                        Logger.Error($"Elevated process returned exit code {elevatedProcess.ExitCode}.");
                        return false;
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message);
                    return false;
                }
            }
            else
            {
                Logger.Log("Firewall rules not found.");
                return true;
            }
        }

        private static void CreateFirewallRule(string displayName, string direction, string protocol, string port, string program = "")
        {           
            string createFwRuleCmd = $@"
                $existingRule = Get-NetFirewallRule -DisplayName '{displayName}' -ErrorAction SilentlyContinue |
                Get-NetFirewallApplicationFilter |
                Where-Object {{ $_.Program -eq '{program}' }}

                if (-not $existingRule) {{
                    New-NetFirewallRule -DisplayName '{displayName}' -Direction {direction} -Protocol {protocol} -LocalPort {port} -Program '{program}' -Action Allow -Enabled True -Profile Any
                }}
            ";

            ProcUtils.Execute(_psExeName, $"{_psCmdArgs} \"{createFwRuleCmd}\"", ProcessWindowStyle.Hidden, true);
        }

        /// <summary>
        /// INTERNAL: Writes the needed firewall rules.
        /// Called for create-firewall-rules CLI arg. Needs elevated permissions.
        /// </summary>
        public static void CreateFirewallRulesElevated(string installDir)
        {
            if (!SecUtils.IsRunAsAdmin())
            {
                Logger.Error("Elevated permissions required to create firewall rules.");
                return;
            }
            
            foreach (var rule in _firewallRules)
            {
                if (File.Exists(rule.Program))
                {
                    Logger.Log($"Creating firewall rule: {rule.DisplayName} {rule.Direction} {rule.Protocol} {rule.Port} {rule.Program}");

                    CreateFirewallRule(
                        rule.DisplayName,
                        rule.Direction,
                        rule.Protocol,
                        rule.Port,
                        rule.Program
                    );
                }
            }
        }

        private static void RemoveFirewallRule(string displayName, string program = "")
        {
            string removeFwRuleCmd = $@"
                Get-NetFirewallRule -DisplayName '{displayName}' -ErrorAction SilentlyContinue |
                Get-NetFirewallApplicationFilter |
                Where-Object {{ $_.Program -eq '{program}' }} |
                Remove-NetFirewallRule -ErrorAction SilentlyContinue
            ";

            ProcUtils.Execute(_psExeName, $"{_psCmdArgs} \"{removeFwRuleCmd}\"", ProcessWindowStyle.Hidden, true);
        }

        public static void RemoveFirewallRulesElevated(string installDir)
        {
            if (!SecUtils.IsRunAsAdmin())
            {
                Logger.Error("Elevated permissions required to remove firewall rules.");
                return;
            }

            foreach (var rule in _firewallRules)
            {
                if (File.Exists(rule.Program))
                {
                    Logger.Log($"Removing firewall rule: {rule.DisplayName} {rule.Direction} {rule.Protocol} {rule.Port} {rule.Program}");

                    RemoveFirewallRule(
                        rule.DisplayName,
                        rule.Program
                    );
                }
            }
        }

        private static bool FirewallRulesExists(out bool rulesExists)
        {
            Logger.Log("Checking existing firewall rules...");

            rulesExists = false;

            foreach (var rule in _firewallRules)
            {
                string firewallCmd = $@"
                $existingRule = Get-NetFirewallRule -DisplayName '{rule.DisplayName}' -ErrorAction SilentlyContinue |
                Get-NetFirewallApplicationFilter |
                Where-Object {{ $_.Program -eq '{rule.Program}' }}
                    
                if (-not $existingRule) {{ exit 1 }}
            ";

                Process? psProcess = ProcUtils.Execute(_psExeName, $"{_psCmdArgs} \"{firewallCmd}\"", ProcessWindowStyle.Hidden);

                if (psProcess == null)
                {
                    Logger.Error("Error when checking firewall rules.");
                    return false;
                }

                // Returns exit code 1 if rule does not exist
                if (psProcess.ExitCode == 0)
                {
                    rulesExists = true;
                }
                else
                {
                    rulesExists = false;
                    break;
                }
            }

            return true;
        }
    }
}