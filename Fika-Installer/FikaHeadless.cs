using Fika_Installer.Models;
using Fika_Installer.Spt;
using Fika_Installer.Utils;
using Newtonsoft.Json.Linq;
using System.Diagnostics;

namespace Fika_Installer
{
    public class FikaHeadless
    {
        public SptInstance SptInstance { get; set; }

        private string? _headlessProfileId;
        private string _fikaDirectory;
        private string _sptFolder;
        private string _fikaServerModPath;
        private string _fikaServerScriptsFolder;
        private string _fikaServerConfigPath;

        public FikaHeadless(string installDir, string sptFolder)
        {
            SptInstance = new(sptFolder);

            _fikaDirectory = installDir;
            _sptFolder = sptFolder;
            _fikaServerModPath = Path.Combine(_sptFolder, @"user\mods\fika-server");
            _fikaServerScriptsFolder = Path.Combine(_fikaServerModPath, @"assets\scripts");
            _fikaServerConfigPath = Path.Combine(_fikaServerModPath, @"assets\configs\fika.jsonc");
        }

        public SptProfile? CreateHeadlessProfile()
        {
            string sptProcessName = "SPT.Server";
            bool sptServerRunning = Process.GetProcessesByName(sptProcessName).Length != 0;

            if (sptServerRunning)
            {
                Console.WriteLine("SPT Server is currently running. Please close it to continue the installation.");
            }

            while (sptServerRunning)
            {
                sptServerRunning = Process.GetProcessesByName(sptProcessName).Length != 0;
                Thread.Sleep(1000);
            }

            if (!IsFikaConfigFound())
            {
                Console.WriteLine("Generating Fika config file... This may take a moment.");

                MatchAction serverIsRunningMatchAction = new(
                    @"Server is running",
                    (process, match) =>
                    {
                        process.Kill();
                    });

                SptInstance.SptServer.AddMatchAction(serverIsRunningMatchAction);
                SptInstance.SptServer.KillAfter = TimeSpan.FromMinutes(2);

                SptInstance.SptServer.Start();

                if (!serverIsRunningMatchAction.Success)
                {
                    return null;
                }

                if (!IsFikaConfigFound())
                {
                    return null;
                }
            }

            JObject? fikaConfig = JsonUtils.ReadJson(_fikaServerConfigPath);

            if (fikaConfig == null)
            {
                return null;
            }

            int sptProfilesCount = SptInstance.Profiles.Count;

            int headlessProfilesAmount = (int)fikaConfig["headless"]["profiles"]["amount"];
            fikaConfig["headless"]["profiles"]["amount"] = sptProfilesCount + 1;

            bool writeFikaConfigResult = JsonUtils.WriteJson(fikaConfig, _fikaServerConfigPath);

            if (!writeFikaConfigResult)
            {
                return null;
            }

            Console.WriteLine("Creating headless profile... This may take a moment.");

            MatchAction createHeadlessProfileMatchAction = new(
                @"Start_headless_([^.]+)",
                (process, match) =>
                {
                    _headlessProfileId = match.Groups[1].Value;
                    process.Kill();
                });

            SptInstance.SptServer.AddMatchAction(createHeadlessProfileMatchAction);
            SptInstance.SptServer.KillAfter = TimeSpan.FromMinutes(2);

            SptInstance.SptServer.Start();

            if (!createHeadlessProfileMatchAction.Success)
            {
                return null;
            }

            if (string.IsNullOrEmpty(_headlessProfileId))
            {
                return null;
            }

            SptProfile? headlessProfile = SptInstance.GetSptProfile(_headlessProfileId);

            return headlessProfile;
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
