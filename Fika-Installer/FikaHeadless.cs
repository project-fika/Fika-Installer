using Fika_Installer.Models;
using Fika_Installer.Models.Fika;
using Fika_Installer.Models.Spt;
using Fika_Installer.Spt;
using Fika_Installer.Utils;
using System.Diagnostics;

namespace Fika_Installer
{
    public class FikaHeadless
    {
        private string _fikaDirectory;
        private string _sptFolder;
        private string _fikaServerModPath;
        private string _fikaServerScriptsFolder;
        private string _fikaServerConfigPath;
        private string? _headlessProfileId;

        public SptInstance SptInstance { get; private set; }

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

            if (!IsFikaServerConfigFound())
            {
                Console.WriteLine("Generating Fika config file... This may take a moment.");

                SptServer generateFikaCfgSptServer = new(SptInstance);

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

            FikaServerConfigModel? fikaServerConfig = JsonUtils.ReadJson<FikaServerConfigModel>(_fikaServerConfigPath);

            if (fikaServerConfig == null)
            {
                return null;
            }

            int sptProfilesCount = SptInstance.Profiles.Count;

            fikaServerConfig.Headless.Profiles.Amount = sptProfilesCount + 1;

            bool writeFikaConfigResult = JsonUtils.WriteJson<FikaServerConfigModel>(_fikaServerConfigPath, fikaServerConfig);

            if (!writeFikaConfigResult)
            {
                return null;
            }

            Console.WriteLine("Creating headless profile... This may take a moment.");

            SptServer createHeadlessProfileSptServer = new(SptInstance);

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

            SptInstance.LoadProfiles();

            SptProfile? headlessProfile = SptInstance.GetProfile(_headlessProfileId);

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

        public bool IsFikaServerConfigFound()
        {
            if (File.Exists(_fikaServerConfigPath))
            {
                return true;
            }

            return false;
        }
    }
}
