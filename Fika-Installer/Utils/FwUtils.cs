namespace Fika_Installer.Utils
{
    public static class FwUtils
    {
        public static void CreateFirewallRule(string displayName, string direction, string protocol, string port, string program = "")
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