namespace Fika_Installer.Utils
{   
    public static class FwUtils
    {
        public static void CreateFirewallRule(string displayName, string direction, string protocol, string port, string program = "")
        {
            string powershellCmd = $"-NoProfile -ExecutionPolicy Bypass";
            string firewallCmd = $"New-NetFirewallRule -DisplayName '{displayName}' -Direction {direction} -Protocol {protocol} -LocalPort {port} -Program '{program}' -Action Allow -Enabled True";

            ProcUtils.ExecuteSilent("powershell.exe", $"{powershellCmd} -Command \"{firewallCmd}\"");
        }
    }
}