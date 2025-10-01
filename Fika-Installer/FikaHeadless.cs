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
        private SptInstance _sptInstance;
        private string _fikaServerModPath;
        private string _fikaServerScriptsFolder;
        private string _fikaServerConfigPath;
        private string? _headlessProfileId;
        private CompositeLogger _logger;

        public FikaHeadless(SptInstance sptInstance, CompositeLogger logger)
        {
            _sptInstance = sptInstance;
            _fikaServerModPath = Path.Combine(_sptInstance.SptPath, @"user\mods\fika-server");
            _fikaServerScriptsFolder = Path.Combine(_fikaServerModPath, @"assets\scripts");
            _fikaServerConfigPath = Path.Combine(_fikaServerModPath, @"assets\configs\fika.jsonc");
            _logger = logger;
        }

        public SptProfile? CreateHeadlessProfile()
        {
            string sptProcessName = "SPT.Server";
            bool sptServerRunning = Process.GetProcessesByName(sptProcessName).Length != 0;

            if (sptServerRunning)
            {
                _logger?.Warning("SPT Server is currently running. Please close it to continue the installation.");
            }

            while (sptServerRunning)
            {
                Thread.Sleep(500);
                sptServerRunning = Process.GetProcessesByName(sptProcessName).Length != 0;
            }

            if (!IsFikaServerConfigFound())
            {
                _logger?.Log("Generating Fika config file... This may take a moment.");

                MatchAction serverIsRunningMatchAction = new(
                    @"Server is running",
                    (process, match) =>
                    {
                        if (!process.HasExited)
                        {
                            process.Kill();
                        }
                    });

                SptServer generateFikaCfgSptServer = new(_sptInstance);

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

            JsonObject? fikaServerConfig = JsonUtils.DeserializeFromFile(_fikaServerConfigPath, _logger);

            if (fikaServerConfig == null)
            {
                return null;
            }

            int headlessProfileCount = _sptInstance.GetHeadlessProfiles().Count;

            fikaServerConfig["headless"]["profiles"]["amount"] = headlessProfileCount + 1;

            if (!JsonUtils.SerializeToFile(_fikaServerConfigPath, fikaServerConfig, _logger))
            {
                return null;
            }

            _logger?.Log("Creating headless profile... This may take a moment.");

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

            SptServer createHeadlessProfileSptServer = new(_sptInstance);

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

        public bool CopyProfileScript(string profileId, string installDir)
        {
            string headlessProfileStartScript = $"Start_headless_{profileId}.ps1";

            string headlessProfileStartScriptPath = Path.Combine(_fikaServerScriptsFolder, headlessProfileStartScript);

            if (File.Exists(headlessProfileStartScriptPath))
            {
                string headlessProfileStartScriptDestPath = Path.Combine(installDir, headlessProfileStartScript);
                File.Copy(headlessProfileStartScriptPath, headlessProfileStartScriptDestPath, true);
            }
            else
            {
                _logger?.Error($"Couldn't find {headlessProfileStartScript}!", true);
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
