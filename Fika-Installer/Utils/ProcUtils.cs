using System.Diagnostics;

namespace Fika_Installer.Utils
{
    public static class ProcUtils
    {
        private const string _psExeName = "Powershell.exe";
        private const string _psCmdArgs = "-NoProfile -ExecutionPolicy Bypass -Command";

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
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }

            process.WaitForExit();

            return process;
        }

        public static Process? ExecuteElevateSelf(string args)
        {
            return Execute(Application.ExecutablePath, args, ProcessWindowStyle.Minimized, true);
        }

        public static Process? ExecutePsCmd(string cmd)
        {
            return Execute(_psExeName, $"{_psCmdArgs} \"{cmd}\"", ProcessWindowStyle.Hidden);
        }
    }
}
