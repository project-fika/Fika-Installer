namespace Fika_Installer.Utils
{
    public static class FwUtils
    {
        public static void ApplyFirewallRules(string installDir)
        {
            string powershellCmd = $"-NoProfile -ExecutionPolicy Bypass";

            string sptServerPath = Path.Combine(installDir, "SPT.Server.exe");
            string sptFirewallRuleCmd = $"New-NetFirewallRule -DisplayName 'Fika (SPT) - TCP 6969' -Direction Inbound -Protocol TCP -LocalPort 6969 -Program \"{sptServerPath}\" -Action Allow -Enabled True";
            
            ProcUtils.ExecuteSilent("powershell.exe", $"{powershellCmd} -Command \"{sptFirewallRuleCmd}\"");

            string escapeFromTarkovPath = Path.Combine(installDir, "EscapeFromTarkov.exe");
            string escapeFromTarkovFirewallRuleCmd = $"New-NetFirewallRule -DisplayName 'Fika (Core) - UDP 25565' -Direction Inbound -Protocol UDP -LocalPort 25565 -Program \"{escapeFromTarkovPath}\" -Action Allow -Enabled True";

            ProcUtils.ExecuteSilent("powershell.exe", $"{powershellCmd} -Command \"{escapeFromTarkovFirewallRuleCmd}\"");
        }
    }
}