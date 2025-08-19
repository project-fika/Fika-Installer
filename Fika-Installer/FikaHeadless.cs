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
        public List<SptProfile> SptProfiles { get; }

        private string? _headlessProfileId;
        private string _fikaDirectory;
        private string _sptFolder;
        private string _fikaServerModPath;
        private string _fikaServerScriptsFolder;
        private string _fikaServerConfigPath;
        private string _sptProfilesFolder;

        public FikaHeadless(string installDir, string sptFolder)
        {
            _fikaDirectory = installDir;
            _sptFolder = sptFolder;

            _fikaServerModPath = Path.Combine(_sptFolder, @"user\mods\fika-server");
            _fikaServerScriptsFolder = Path.Combine(_fikaServerModPath, @"assets\scripts");
            _fikaServerConfigPath = Path.Combine(_fikaServerModPath, @"assets\configs\fika.jsonc");
            _sptProfilesFolder = Path.Combine(_sptFolder, @"user\profiles");

            SptProfiles = GetSptProfiles(true);
        }

        public List<SptProfile> GetSptProfiles(bool headlessProfilesOnly = false)
        {
            List<SptProfile> sptProfiles = [];

            if (Directory.Exists(_sptProfilesFolder))
            {
                string[] profilesPaths = Directory.GetFiles(_sptProfilesFolder);

                if (profilesPaths.Length > 0)
                {
                    foreach (string profilePath in profilesPaths)
                    {
                        SptProfile? sptProfile = GetSptProfileFromJson(profilePath);

                        if (sptProfile != null)
                        {
                            if (headlessProfilesOnly)
                            {
                                if (sptProfile.Password == "fika-headless")
                                {
                                    sptProfiles.Add(sptProfile);
                                }
                            }
                            else
                            {
                                sptProfiles.Add(sptProfile);
                            }
                        }
                    }
                }
            }

            return sptProfiles;
        }

        public SptProfile? GetSptProfileFromJson(string sptProfilePath)
        {
            if (File.Exists(sptProfilePath))
            {
                try
                {
                    string profileJsonContent = File.ReadAllText(sptProfilePath);
                    JObject profileJObject = JObject.Parse(profileJsonContent);

                    string? profileId = profileJObject["info"]?["id"]?.ToString();
                    string? username = profileJObject["info"]?["username"]?.ToString();
                    string? password = profileJObject["info"]?["password"]?.ToString();

                    if (profileId != null && username != null && password != null)
                    {
                        SptProfile sptProfile = new(profileId, username, password);

                        return sptProfile;

                    }
                }
                catch(Exception ex)
                {
                    ConUtils.WriteError($"Failed to read profile: {sptProfilePath}");
                }
            }

            return null;
        }

        public bool CopyProfileScript(string profileId)
        {
            string headlessProfileStartScript = $"Start_headless_{profileId}.ps1";

            string headlessProfileStartScriptPath = Path.Combine(_fikaServerScriptsFolder, headlessProfileStartScript);

            if (File.Exists(headlessProfileStartScriptPath))
            {
                string headlessProfileStartScriptDestPath = Path.Combine(_fikaDirectory, headlessProfileStartScript);
                File.Copy(headlessProfileStartScriptPath, headlessProfileStartScriptDestPath, true);
            }
            else
            {
                ConUtils.WriteError($"Couldn't find {headlessProfileStartScript}!", true);
                return false;
            }

            return true;
        }

        public SptProfile? CreateHeadlessProfile()
        {
            bool sptServerRunning = Process.GetProcessesByName("SPT.Server").Length != 0;

            if (sptServerRunning)
            {
                Console.WriteLine("SPT Server is currently running. Please close it to continue the installation.");
            }

            while (sptServerRunning)
            {
                sptServerRunning = Process.GetProcessesByName("SPT.Server").Length != 0;
                Thread.Sleep(1000);
            }

            JObject? fikaConfig = JsonUtils.ReadJson(_fikaServerConfigPath);

            if (fikaConfig == null)
            {
                return null;
            }

            int sptProfilesCount = SptProfiles.Count;

            int headlessProfilesAmount = (int)fikaConfig["headless"]["profiles"]["amount"];
            fikaConfig["headless"]["profiles"]["amount"] = sptProfilesCount + 1;

            bool writeFikaConfigResult = JsonUtils.WriteJson(fikaConfig, _fikaServerConfigPath);

            if (!writeFikaConfigResult)
            {
                return null;
            }

            Console.WriteLine("Creating headless profile... Please wait. This may take a moment.");

            string sptServerPath = Path.Combine(_sptFolder, "SPT.Server.exe");

            StartProcessAndRedirectOutput(sptServerPath, SptConsoleMessageHandler, TimeSpan.FromMinutes(1)); // TODO: is 1 minute too short?

            if (string.IsNullOrEmpty(_headlessProfileId))
            {
                ConUtils.WriteError("An error occurred when creating the headless profile. Check the SPT server logs.", true);
                return null;
            }

            string headlessProfilePath = Path.Combine(_sptProfilesFolder, $"{_headlessProfileId}.json");

            if (!File.Exists(headlessProfilePath))
            {
                return null;
            }

            SptProfile? headlessProfile = GetSptProfileFromJson(headlessProfilePath);

            return headlessProfile;
        }

        public void SptConsoleMessageHandler(Process process, string message)
        {
            Match generatedLaunchScriptRegexMatch = HeadlessRegex.GeneratedLaunchScriptRegex().Match(message);

            if (generatedLaunchScriptRegexMatch.Success)
            {
                _headlessProfileId = generatedLaunchScriptRegexMatch.Groups[1].Value;
                process.Kill();
            }

            Match sptErrorRegexMatch = HeadlessRegex.SptErrorRegex().Match(message);

            if (sptErrorRegexMatch.Success)
            {
                process.Kill();
            }
        }

        public void StartProcessAndRedirectOutput(string filePath, Action<Process, string> stdOut, TimeSpan timeout)
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
                        stdOut(process, e.Data);
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

        public bool IsFikaServerInstalled()
        {
            if (Directory.Exists(_fikaServerModPath))
            {
                return true;
            }

            return false;
        }

        public bool IsFikaConfigFound()
        {

            if (File.Exists(_fikaServerConfigPath))
            {
                return true;
            }

            return false;
        }
    }
}
