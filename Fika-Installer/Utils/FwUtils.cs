namespace Fika_Installer.Utils
{
    public static class FwUtils
    {
        public static void BuildFirewallScript(string installDir, string tempDir)
        {
            string escapeFromTarkovPath = Path.Combine(installDir, "EscapeFromTarkov.exe");
            string sptServerPath = Path.Combine(installDir, "SPT.Server.exe");

            string psScript = @$"
$ErrorActionPreference = 'Stop'

$rules = @(
    @{{
        Name     = 'Fika (SPT) - TCP 6969 (Inbound)'
        Protocol = 'TCP'
        Port     = 6969
        Program  = '{sptServerPath}'
    }},
    @{{
        Name     = 'Fika (Core) - UDP 25565 (Inbound)'
        Protocol = 'UDP'
        Port     = 25565
        Program  = '{escapeFromTarkovPath}'
    }}
)

foreach ($r in $rules) {{
    if (Get-NetFirewallRule -DisplayName $r.Name -ErrorAction SilentlyContinue) {{
        Remove-NetFirewallRule -DisplayName $r.Name
    }}

    New-NetFirewallRule `
        -DisplayName  $r.Name `
        -Direction    Inbound `
        -Action       Allow `
        -Protocol     $r.Protocol `
        -LocalPort    $r.Port `
        -Profile      Any `
        -Program      $r.Program `
        -Enabled      True | Out-Null
}}
";
            string psScriptPath = Path.Combine(tempDir, "FikaFirewall.ps1");
            File.WriteAllText(psScriptPath, psScript);
        }

        public static void ExecuteFirewallScript(string scriptPath, bool elevate = false)
        {
            if (!File.Exists(scriptPath))
            {
                return;
            }

            if (elevate)
            {
                ProcUtils.ExecuteSelfElevate($"-firewall \"{scriptPath}\"");
                return;
            }

            string args = $"-NoProfile -ExecutionPolicy Bypass -File \"{scriptPath}\"";
            ProcUtils.Execute($"PowerShell.exe", args);
        }
    }
}
