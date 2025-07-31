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
            string headlessProfileStartScript = $"Start_headless_{sptProfile.ProfileId}";

            string headlessProfileStartScriptPath = Path.Combine(fikaServerScriptsPath, headlessProfileStartScript);

            if (File.Exists(headlessProfileStartScriptPath))
            {
                string fikaFolder = Constants.FikaDirectory;
                File.Copy(headlessProfileStartScriptPath, fikaFolder, true);

                string headlessProfileStartScriptName = Path.GetFileName(headlessProfileStartScriptPath);

                Utils.WriteLineConfirm($"{headlessProfileStartScriptName} has been copied to the root of your Fika install!");
            }
        }

        public void SetupNewProfile(string sptFolder)
        {
            CreateHeadlessProfile(sptFolder);
        }

        public void CreateHeadlessProfile(string sptFolder)
        {
            string sptUserModsPath = Path.Combine(sptFolder, @"user\mods\");
            string fikaServerModPath = Path.Combine(sptUserModsPath, @"fika-server\");
            string fikaConfigPath = Path.Combine(fikaServerModPath, @"assets\configs\fika.jsonc");

            string fikaConfig = File.ReadAllText(fikaConfigPath);

            JObject fikaConfigJObject = JObject.Parse(fikaConfig);

            int headlessProfilesAmount = (int)fikaConfigJObject["headless"]?["profiles"]?["amount"];
            fikaConfigJObject["headless"]["profiles"]["amount"] = headlessProfilesAmount + 1;
            
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

            StartProcessAndRedirectOutput(sptServerPath, SptConsoleMessageHandler);

            if (string.IsNullOrEmpty(_headlessProfileId))
            {
                Utils.WriteLineConfirm("An error occurred when creating the headless profile. Check the SPT server logs.");
            }
        }

        public void SptConsoleMessageHandler(Process process, string message)
        {
            Match generatedLaunchScriptRegexMatch = HeadlessRegex.GeneratedLaunchScriptRegex().Match(message);

            if (generatedLaunchScriptRegexMatch.Success)
            {
                _headlessProfileId = generatedLaunchScriptRegexMatch.Groups[1].Value;
                process.Kill();
            }

            Match serverIsRunningRegexMatch = HeadlessRegex.ServerIsRunning().Match(message);

            if (serverIsRunningRegexMatch.Success)
            {
                // TODO: check if closing process this way can lead to issues
                //process.Kill();
            }

            // TODO: regex to capture SPT errors and kill
        }

        public void StartProcessAndRedirectOutput(string filePath, Action<Process, string> stdout)
        {
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
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                    {
                        stdout(process, e.Data);
                    }
                };

                process.ErrorDataReceived += (sender, e) =>
                {
                    // TODO: does SPT even outputs in STDERR?
                    process.Kill();
                };

                process.Start();

                process.BeginOutputReadLine();
                process.BeginErrorReadLine();

                process.WaitForExit();
            }
        }
    }
}
