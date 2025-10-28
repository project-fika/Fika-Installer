using System.Diagnostics;

namespace Fika_Installer.Utils
{
    public static class FwUtils
    {
        private record FirewallRuleset(
            int version, 
            (
                string displayName,
                string direction,
                string protocol,
                string port,
                string program
            )[] rules
        );

        private static FirewallRuleset FirewallRules(string installDir)
        {
            // Define the firewall rules needed for Fika
            // Version number is checked against current saved state; increment version number to
            //  force (re-)creation of (new) rules
            return new FirewallRuleset(
                version: 1,
                [
                    ("Fika (SPT) - TCP 6969", "Inbound", "TCP", "6969", Path.Combine(installDir, "SPT", SptConstants.ServerExeName)),
                    ("Fika (Core) - UDP 25565", "Inbound", "UDP", "25565", Path.Combine(installDir, EftConstants.GameExeName))
                ]
            );
        }

        /// <summary>
        /// Creates the necessary firewall rules for Fika, if they do not already exist or if forced.
        /// Spawns new instance of current exe with elevated permissions to create the rules.
        /// </summary>
        public static void CreateFirewallRules(string installDir, bool force = false)
        {
            FirewallRuleset ruleset = FirewallRules(installDir);
            if (StateUtils.GetValue<int>("fw_rules_version") < ruleset.version || force)
            {
                Logger.Log("Firewall rules confirmed to need creation.");
                try
                {
                    Logger.Log("Spawning elevated process...");
                    Process? process = Process.Start(new ProcessStartInfo
                    {
                        FileName = Application.ExecutablePath,
                        Arguments = $"create-firewall-rules \"{installDir}\"",
                        Verb = "runas",
                        UseShellExecute = true
                    });
                    
                    if (process == null)
                    {
                        throw new Exception("Failed to start elevated process for firewall rule creation.");
                    }

                    process.WaitForExit();

                    if (process.ExitCode == 0)
                    {
                        Logger.Success("Firewall rules created successfully.");
                    }
                    else
                    {
                        throw new Exception($"Elevated process returned {process.ExitCode}.");
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error($"Failed to create firewall rules: {ex.Message}", true);
                }
                StateUtils.SetValue<int>("fw_rules_version", ruleset.version);
            }
            else
            {
                Logger.Log("Firewall rules already set.");
            }
        }

        /// <summary>
        /// INTERNAL: Writes the needed firewall rules.
        /// Called for create-firewall-rules CLI arg. Needs elevated permissions.
        /// </summary>
        public static void ElevatedSetRules(string installDir)
        {
            FirewallRuleset ruleset = FirewallRules(installDir);
            foreach (var rule in ruleset.rules)
            {
                Console.WriteLine($"Creating firewall rule: {rule.displayName}");
                CreateFirewallRule(
                    rule.displayName,
                    rule.direction,
                    rule.protocol,
                    rule.port,
                    rule.program
                );
            }
        }

        private static void CreateFirewallRule(string displayName, string direction, string protocol, string port, string program = "")
        {
            string powershellCmd = $"-NoProfile -ExecutionPolicy Bypass";
            string firewallCmd = $@"
                $existingRule = Get-NetFirewallRule -DisplayName '{displayName}' -ErrorAction SilentlyContinue |
                Get-NetFirewallApplicationFilter |
                Where-Object {{ $_.Program -eq '{program}' }}

                if (-not $existingRule) {{
                    New-NetFirewallRule -DisplayName '{displayName}' -Direction {direction} -Protocol {protocol} -LocalPort {port} -Program '{program}' -Action Allow -Enabled True -Profile Any
                }}
            ";

            ProcUtils.ExecuteSilent("powershell.exe", $"{powershellCmd} -Command \"{firewallCmd}\"");
        }
    }
}