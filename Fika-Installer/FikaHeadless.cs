using Fika_Installer.Models;
using Fika_Installer.Models.Spt;
using Fika_Installer.Spt;
using Fika_Installer.Utils;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace Fika_Installer
{
    public class FikaHeadless
    {
        private string _installDir;
        private SptInstance _sptInstance;
        private string _fikaServerModPath;
        private string _fikaServerScriptsFolder;
        private string _fikaServerConfigPath;
        private string? _headlessProfileId;

        public FikaHeadless(string installDir, SptInstance sptInstance)
        {
            _installDir = installDir;
            _sptInstance = sptInstance;
            _fikaServerModPath = Path.Combine(_sptInstance.SptPath, @"user\mods\fika-server");
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

            if (!IsFikaServerConfigFound())
            {
                Console.WriteLine("Generating Fika config file... This may take a moment.");

                SptServer generateFikaCfgSptServer = new(_sptInstance);

                MatchAction serverIsRunningMatchAction = new(
                    @"Server is running",
                    (process, match) =>
                    {
                        if (!process.HasExited)
                        {
                            process.Kill();
                        }
                    });

                generateFikaCfgSptServer.AddMatchAction(serverIsRunningMatchAction);
                generateFikaCfgSptServer.KillAfter = TimeSpan.FromMinutes(2);

                generateFikaCfgSptServer.Start();

                if (!serverIsRunningMatchAction.Success)
                {
                    return null;
                }

                if (!IsFikaServerConfigFound())
                {
                    return null;
                }
            }

            JsonObject? fikaServerConfig = JsonUtils.DeserializeFromFile(_fikaServerConfigPath);

            if (fikaServerConfig == null)
            {
                return null;
            }

            int sptProfilesCount = _sptInstance.Profiles.Count;

            fikaServerConfig["headless"]["profiles"]["amount"] = sptProfilesCount + 1;

            if (!JsonUtils.SerializeToFile(_fikaServerConfigPath, fikaServerConfig))
            {
                return null;
            }

            Console.WriteLine("Creating headless profile... This may take a moment.");

            SptServer createHeadlessProfileSptServer = new(_sptInstance);

            MatchAction createHeadlessProfileMatchAction = new(
                @"Start_headless_([^.]+)",
                (process, match) =>
                {
                    _headlessProfileId = match.Groups[1].Value;

                    if (!process.HasExited)
                    {
                        process.Kill();
                    }
                });

            createHeadlessProfileSptServer.AddMatchAction(createHeadlessProfileMatchAction);
            createHeadlessProfileSptServer.KillAfter = TimeSpan.FromMinutes(2);

            createHeadlessProfileSptServer.Start();

            if (!createHeadlessProfileMatchAction.Success)
            {
                return null;
            }

            if (string.IsNullOrEmpty(_headlessProfileId))
            {
                return null;
            }

            _sptInstance.LoadProfiles();

            SptProfile? headlessProfile = _sptInstance.GetProfile(_headlessProfileId);

            return headlessProfile;
        }

        public bool CopyProfileScript(string profileId)
        {
            string headlessProfileStartScript = $"Start_headless_{profileId}.ps1";

            string headlessProfileStartScriptPath = Path.Combine(_fikaServerScriptsFolder, headlessProfileStartScript);

            if (File.Exists(headlessProfileStartScriptPath))
            {
                string headlessProfileStartScriptDestPath = Path.Combine(_installDir, headlessProfileStartScript);
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
            // This is a lazy check and is not indicative of a proper Fika-Server installation.
            return Directory.Exists(_fikaServerModPath);
        }

        public bool IsFikaServerConfigFound()
        {
            return File.Exists(_fikaServerConfigPath);
        }
    }
}
