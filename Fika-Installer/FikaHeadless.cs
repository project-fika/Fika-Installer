using Fika_Installer.Models;
using Fika_Installer.Utils;
using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.Text.RegularExpressions;


namespace Fika_Installer
{
    public class FikaHeadless
    {
        public List<SptProfile> SptProfiles { get; }

        private string? _headlessProfileId;
        private string _fikaDirectory;
        private string _sptFolder;
        private string _sptServerPath;
        private string _fikaServerModPath;
        private string _fikaServerScriptsFolder;
        private string _fikaServerConfigPath;
        private string _sptProfilesFolder;

        public FikaHeadless(string installDir, string sptFolder)
        {
            _fikaDirectory = installDir;
            _sptFolder = sptFolder;

            _sptServerPath = Path.Combine(_sptFolder, "SPT.Server.exe");
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

            if (!IsFikaConfigFound())
            {
                Console.WriteLine("Generating Fika config file... This may take a moment.");

                SptServerHandler createFikaConfigSptServer = new(_sptServerPath);
                createFikaConfigSptServer.AddMatchAction(new MatchAction(
                    @"Server is running",
                    (process, match) =>
                    {
                        process.Kill();
                    }));

                createFikaConfigSptServer.KillAfter = TimeSpan.FromMinutes(2);
                createFikaConfigSptServer.Start();

                if (!createFikaConfigSptServer.Success)
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

            int sptProfilesCount = SptProfiles.Count;

            int headlessProfilesAmount = (int)fikaConfig["headless"]["profiles"]["amount"];
            fikaConfig["headless"]["profiles"]["amount"] = sptProfilesCount + 1;

            bool writeFikaConfigResult = JsonUtils.WriteJson(fikaConfig, _fikaServerConfigPath);

            if (!writeFikaConfigResult)
            {
                return null;
            }

            Console.WriteLine("Creating headless profile... This may take a moment.");

            SptServerHandler createHeadlessProfileSptServer = new(_sptServerPath);
            createHeadlessProfileSptServer.AddMatchAction(new MatchAction(
                @"Start_headless_([^.]+)",
                (process, match) =>
                {
                    _headlessProfileId = match.Groups[1].Value;
                    process.Kill();
                }));

            createHeadlessProfileSptServer.KillAfter = TimeSpan.FromMinutes(2);
            createHeadlessProfileSptServer.Start();

            if (!createHeadlessProfileSptServer.Success)
            {
                return null;
            }

            if (string.IsNullOrEmpty(_headlessProfileId))
            {
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
