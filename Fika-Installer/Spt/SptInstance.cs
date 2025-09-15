using Fika_Installer.Models.Spt;
using Fika_Installer.Utils;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace Fika_Installer.Spt
{
    public class SptInstance
    {
        private string _profilesPath;
        private string _launcherConfigPath;
        private CompositeLogger _logger;

        public string SptPath { get; private set; }
        public string ServerExePath { get; private set; }
        public string EftExePath { get; private set; }
        public string EftVersion { get; private set; } = "";
        public List<SptProfile> Profiles { get; private set; } = [];

        public SptInstance(string sptPath, CompositeLogger logger)
        {
            _profilesPath = Path.Combine(sptPath, @"user\profiles");
            _launcherConfigPath = Path.Combine(sptPath, @"user\launcher\config.json");
            _logger = logger;

            SptPath = sptPath;
            ServerExePath = Path.Combine(sptPath, SptConstants.ServerExeName);
            EftExePath = Path.Combine(sptPath, EftConstants.GameExeName);

            LoadProfiles();

            if (File.Exists(EftExePath))
            {
                FileVersionInfo? tarkovVersionInfo = FileVersionInfo.GetVersionInfo(EftExePath);

                if (tarkovVersionInfo.FileVersion != null)
                {
                    EftVersion = tarkovVersionInfo.FileVersion;
                }
            }
        }

        public void LoadProfiles()
        {
            List<SptProfile> sptProfiles = [];

            if (Directory.Exists(_profilesPath))
            {
                string[] profilesPaths = Directory.GetFiles(_profilesPath);

                foreach (string profilePath in profilesPaths)
                {
                    SptProfile? sptProfile = GetProfileFromJson(profilePath);

                    if (sptProfile != null)
                    {
                        sptProfiles.Add(sptProfile);
                    }
                }
            }

            Profiles = sptProfiles;
        }

        public SptProfile? GetProfile(string profileId)
        {
            return Profiles.FirstOrDefault(p => p.ProfileId == profileId);
        }

        public SptProfile? GetProfileFromJson(string sptProfilePath)
        {
            if (File.Exists(sptProfilePath))
            {
                try
                {
                    JsonObject? profile = JsonUtils.DeserializeFromFile(sptProfilePath, _logger);

                    if (profile != null)
                    {
                        bool headless = false;

                        string? id = profile["info"]?["id"]?.GetValue<string>();
                        string? username = profile["info"]?["username"]?.GetValue<string>();
                        string? password = profile["info"]?["password"]?.GetValue<string>();

                        if (id != null && username != null && password != null)
                        {
                            headless = password == "fika-headless";

                            SptProfile sptProfile = new(id, username, password, headless);

                            return sptProfile;
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger?.Error($"Failed to read profile: {sptProfilePath}. {ex.Message}");
                }
            }

            return null;
        }

        public List<SptProfile> GetHeadlessProfiles()
        {
            return [.. Profiles.Where(p => p.Headless)];
        }

        public JsonObject? GetLauncherConfig()
        {
            if (File.Exists(_launcherConfigPath))
            {
                JsonObject? launcherConfig = JsonUtils.DeserializeFromFile(_launcherConfigPath, _logger);

                return launcherConfig;
            }

            return null;
        }

        public bool SetLauncherConfig(JsonObject launcherConfig)
        {
            try
            {
                JsonUtils.SerializeToFile(_launcherConfigPath, launcherConfig, _logger);

                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
