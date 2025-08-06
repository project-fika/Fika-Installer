using Fika_Installer.Models;
using Fika_Installer.Utils;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;
using Timer = System.Threading.Timer;

namespace Fika_Installer
{
    public class FikaHeadless
    {
        private string? _headlessProfileId;
        private string _fikaDirectory;
        
        public FikaHeadless()
        {
            _fikaDirectory = Constants.InstallerDirectory;
        }
        
        public void CopyProfileScript(SptProfile sptProfile, string sptFolder)
        {
            string fikaServerScriptsPath = Path.Combine(sptFolder, @"user\mods\fika-server\assets\scripts");
            string headlessProfileStartScript = $"Start_headless_{sptProfile.ProfileId}.ps1";

            string headlessProfileStartScriptPath = Path.Combine(fikaServerScriptsPath, headlessProfileStartScript);

            if (File.Exists(headlessProfileStartScriptPath))
            {
                string headlessProfileStartScriptDestPath = Path.Combine(_fikaDirectory, headlessProfileStartScript);
                File.Copy(headlessProfileStartScriptPath, headlessProfileStartScriptDestPath, true);
            }
        }

        public void SetupNewProfile(string sptFolder)
        {
            SptProfile headlessProfile = CreateHeadlessProfile(sptFolder);

            if (string.IsNullOrEmpty(headlessProfile.ProfileId))
            {
                return;
            }

            CopyProfileScript(headlessProfile, sptFolder);
        }

        public SptProfile CreateHeadlessProfile(string sptFolder)
        {
            SptProfile headlessProfile = new();
            int cursorTop = Console.CursorTop;

            while (Process.GetProcessesByName("SPT.Server").Length != 0)
            {
                ConUtils.WriteLine(cursorTop, "SPT Server is currently running. Please close it to continue the installation.");
                Thread.Sleep(1000);
            }

            string fikaConfigPath = Path.Combine(sptFolder, @"user\mods\fika-server\assets\configs\fika.jsonc");

            JObject fikaConfig = JsonUtils.ReadJson(fikaConfigPath);

            string sptProfilesPath = Path.Combine(sptFolder, @"user\profiles");
            int sptProfilesCount = SptUtils.GetSptProfiles(sptProfilesPath, true).Length;

            int headlessProfilesAmount = (int)fikaConfig["headless"]["profiles"]["amount"];
            fikaConfig["headless"]["profiles"]["amount"] = sptProfilesCount + 1;

            JsonUtils.WriteJson(fikaConfig, fikaConfigPath);

            Console.WriteLine("Creating headless profile... Please wait. This may take a moment.");

            string sptServerPath = Path.Combine(sptFolder, "SPT.Server.exe");
            StartProcessAndRedirectOutput(sptServerPath, SptConsoleMessageHandler, TimeSpan.FromMinutes(1)); // TODO: is 1 minute too short?

            if (string.IsNullOrEmpty(_headlessProfileId))
            {
                ConUtils.WriteError("An error occurred when creating the headless profile. Check the SPT server logs.", true);
                return headlessProfile;
            }

            string headlessProfilePath = Path.Combine(sptProfilesPath, $"{_headlessProfileId}.json");

            if (File.Exists(headlessProfilePath))
            {
                headlessProfile = SptUtils.GetSptProfileInfo(headlessProfilePath);
            }

            return headlessProfile;
        }

        public void SptConsoleMessageHandler(Process process, string message, Timer cancelTimer)
        {
            Match generatedLaunchScriptRegexMatch = HeadlessRegex.GeneratedLaunchScriptRegex().Match(message);

            if (generatedLaunchScriptRegexMatch.Success)
            {
                _headlessProfileId = generatedLaunchScriptRegexMatch.Groups[1].Value;
                cancelTimer.Dispose();
                process.Kill();
            }

            // TODO: regex to capture SPT errors and kill
        }

        public void StartProcessAndRedirectOutput(string filePath, Action<Process, string, Timer> stdOut, TimeSpan timeout)
        {
            using (var cts = new CancellationTokenSource())
            {
                var token = cts.Token;

                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = filePath,
                    WorkingDirectory = Path.GetDirectoryName(filePath),
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    Timer timeoutTimer = new(_ =>
                    {
                        if (!process.HasExited)
                        {
                            process.Kill();
                        }
                    }, null, timeout, Timeout.InfiniteTimeSpan);

                    process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            stdOut(process, e.Data, timeoutTimer);
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!process.HasExited)
                        {
                            try { process.Kill(); } catch { }
                        }
                    };

                    process.Start();

                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    process.WaitForExit();
                    timeoutTimer.Dispose();
                }
            }
        }
    }
}
