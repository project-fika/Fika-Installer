using System.Diagnostics;

namespace Fika_Installer.Utils
{
    public static class ProcUtils
    {
        public static bool ExecuteSelfElevate(string args)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = Environment.ProcessPath,
                WorkingDirectory = Directory.GetCurrentDirectory(),
                Arguments = args,
                Verb = "runas",
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Minimized
            };

            Process process = new();
            process.StartInfo = startInfo;
            process.Start();

            process.WaitForExit();

            return process.ExitCode == 0;
        }

        public static bool Execute(string path, string args, bool elevate = false)
        {
            string verb = "";

            if (elevate)
            {
                verb = "runas";
            }

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = path,
                WorkingDirectory = Path.GetDirectoryName(path),
                Arguments = args,
                Verb = verb,
                UseShellExecute = true,
                WindowStyle = ProcessWindowStyle.Minimized
            };

            Process process = new();
            process.StartInfo = startInfo;
            process.Start();

            process.WaitForExit();

            return process.ExitCode == 0;
        }
    }
}
