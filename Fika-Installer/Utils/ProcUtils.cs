using System.Diagnostics;

namespace Fika_Installer.Utils
{
    public static class ProcUtils
    {
        public static bool ExecuteSilent(string path, string args)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = path,
                WorkingDirectory = Path.GetDirectoryName(path),
                Arguments = args,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            Process process = new()
            {
                StartInfo = startInfo
            };

            try
            {
                process.Start();
            }
            catch
            {
                return false;
            }

            process.WaitForExit();

            return process.ExitCode == 0;
        }
    }
}
