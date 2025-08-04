using Fika_Installer.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace Fika_Installer
{
    public class Headless
    {
        private string? _headlessProfileId;
        
        public void SetupProfile(SptProfile sptProfile, string sptFolder)
        {
            string sptUserModsPath = Path.Combine(sptFolder, @"user\mods\");
            string fikaServerModPath = Path.Combine(sptUserModsPath, @"fika-server\");
            string fikaServerScriptsPath = Path.Combine(fikaServerModPath, @"assets\scripts\");
            string headlessProfileStartScript = $"Start_headless_{sptProfile.ProfileId}.ps1";

            string headlessProfileStartScriptPath = Path.Combine(fikaServerScriptsPath, headlessProfileStartScript);

            if (File.Exists(headlessProfileStartScriptPath))
            {
                string fikaFolder = Constants.FikaDirectory;
                string headlessProfileStartScriptDestPath = Path.Combine(fikaFolder, headlessProfileStartScript);
                File.Copy(headlessProfileStartScriptPath, headlessProfileStartScriptDestPath, true);
            }
        }

        public void SetupNewProfile(string sptFolder)
        {
            SptProfile headlessProfile = CreateHeadlessProfile(sptFolder);
            SetupProfile(headlessProfile, sptFolder);
        }

        public SptProfile CreateHeadlessProfile(string sptFolder)
        {
            string sptUserModsPath = Path.Combine(sptFolder, @"user\mods\");
            string fikaServerModPath = Path.Combine(sptUserModsPath, @"fika-server\");
            string fikaConfigPath = Path.Combine(fikaServerModPath, @"assets\configs\fika.jsonc");

            string fikaConfig = File.ReadAllText(fikaConfigPath);

            JObject fikaConfigJObject = JObject.Parse(fikaConfig);

            string sptProfilesPath = Path.Combine(sptFolder, @"user\profiles");
            SptProfile[] sptProfiles = SptUtils.GetSptProfiles(sptProfilesPath, true);
            int sptProfilesCount = sptProfiles.Length;

            int headlessProfilesAmount = (int)fikaConfigJObject["headless"]?["profiles"]?["amount"];
            fikaConfigJObject["headless"]["profiles"]["amount"] = sptProfilesCount + 1;
            
            //TODO : \r\n vs \n - is it a problem?
            using (var streamWriter = new StreamWriter(fikaConfigPath))
            using (var jsonWriter = new JsonTextWriter(streamWriter))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.IndentChar = '\t';
                jsonWriter.Indentation = 1;

                fikaConfigJObject.WriteTo(jsonWriter);
            }

            string sptServerPath = Path.Combine(sptFolder, "SPT.Server.exe");

            //TODO: ensure that SPT.Server is not already running

            Console.WriteLine("Creating headless profile... this may take a few seconds.");

            StartProcessAndRedirectOutput(sptServerPath, SptConsoleMessageHandler, TimeSpan.FromSeconds(30));

            if (string.IsNullOrEmpty(_headlessProfileId))
            {
                Utils.WriteLineConfirm("An error occurred when creating the headless profile. Check the SPT server logs.");
            }

            string headlessProfilePath = Path.Combine(sptProfilesPath, $@"{_headlessProfileId}.json");

            SptProfile headlessProfile = new();

            if (File.Exists(headlessProfilePath))
            {
                headlessProfile = SptUtils.GetSptProfileInfo(headlessProfilePath);
            }

            return headlessProfile;
        }

        public void SptConsoleMessageHandler(Process process, string message, System.Threading.Timer cancelTimer)
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


        public void StartProcessAndRedirectOutput(string filePath, Action<Process, string, System.Threading.Timer> stdoutWithCancel, TimeSpan timeout)
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
                    System.Threading.Timer timeoutTimer = new(_ =>
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
                            stdoutWithCancel(process, e.Data, timeoutTimer);
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
