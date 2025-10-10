using System.Diagnostics;

namespace Fika_Installer.Spt
{
    public class SptServer(SptInstance sptInstance)
    {
        public string ExePath { get; set; } = sptInstance.ServerExePath;
        public Process? Process { get; set; }

        public void Start()
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = ExePath,
                WorkingDirectory = Path.GetDirectoryName(ExePath),
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process = new()
            {
                StartInfo = startInfo
            };

            Process.Start();
        }

        public void Stop()
        {
            if (Process != null && !Process.HasExited)
            {
                Process.Kill();
            }
        }
    }
}
