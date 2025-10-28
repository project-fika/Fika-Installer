using System.Diagnostics;

namespace Fika_Installer.Utils
{
    public static class ProcUtils
    {
        public static Process? Execute(string path, string args, ProcessWindowStyle processWindowStyle, bool elevated = false)
        {
            ProcessStartInfo startInfo = new()
            {
                FileName = path,
                WorkingDirectory = Path.GetDirectoryName(path),
                Arguments = args,
                UseShellExecute = true,
                Verb = elevated ? "runas" : "",
                WindowStyle = processWindowStyle
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
                return null;
            }

            process.WaitForExit();

            return process;
        }
    }
}
