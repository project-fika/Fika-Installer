using Fika_Installer.Models;
using Fika_Installer.Utils;
using Newtonsoft.Json.Linq;

namespace Fika_Installer.Spt
{
    public class SptInstance
    {
        private string _profilesPath;
        private string _launcherConfigPath;

        public string LocationPath { get; private set; }
        public string ServerExePath { get; private set; }
        public string LauncherExePath { get; private set; }
        public List<SptProfile> Profiles { get; private set; }

        public SptInstance(string locationPath)
        {
            _profilesPath = Path.Combine(locationPath, @"user\profiles");
            _launcherConfigPath = Path.Combine(locationPath, @"user\launcher\config.json");

            LocationPath = locationPath;
            ServerExePath = Path.Combine(locationPath, "SPT.Server.exe");
            LauncherExePath = Path.Combine(locationPath, "SPT.Launcher.exe");
            Profiles = LoadProfiles();
        }

        public SptProfile? GetProfile(string profileId)
        {
            return Profiles.FirstOrDefault(p => p.ProfileId == profileId);
        }

        public List<SptProfile> GetHeadlessProfiles()
        {
            return [.. Profiles.Where(p => p.Headless)];
        }

        public JObject? GetLauncherConfig()
        {
            JObject? launcherConfig = JsonUtils.ReadJson(_launcherConfigPath);

            if (launcherConfig == null)
            {
                return null;
            }

            return launcherConfig;
        }

        public bool SetLauncherConfig(JObject launcherConfig)
        {
            try
            {
                JsonUtils.WriteJson(launcherConfig, _launcherConfigPath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public void ReloadProfiles()
        {
            Profiles = LoadProfiles();
        }

        private List<SptProfile> LoadProfiles()
        {
            List<SptProfile> sptProfiles = [];

            if (Directory.Exists(_profilesPath))
            {
                string[] profilesPaths = Directory.GetFiles(_profilesPath);

                if (profilesPaths.Length > 0)
                {
                    foreach (string profilePath in profilesPaths)
                    {
                        SptProfile? sptProfile = GetProfileFromJson(profilePath);

                        if (sptProfile != null)
                        {
                            sptProfiles.Add(sptProfile);
                        }
                    }
                }
            }

            return sptProfiles;
        }

        private SptProfile? GetProfileFromJson(string sptProfilePath)
        {
            if (File.Exists(sptProfilePath))
            {
                try
                {
                    string profileJson = File.ReadAllText(sptProfilePath);
                    JObject profile = JObject.Parse(profileJson);

                    string? profileId = profile["info"]?["id"]?.ToString();
                    string? username = profile["info"]?["username"]?.ToString();
                    string? password = profile["info"]?["password"]?.ToString();

                    if (profileId != null && username != null && password != null)
                    {
                        bool headlessProfile = false;

                        if (password == "fika-headless")
                        {
                            headlessProfile = true;
                        }

                        SptProfile sptProfile = new(profileId, username, password, headlessProfile);

                        return sptProfile;

                    }
                }
                catch (Exception ex)
                {
                    ConUtils.WriteError($"Failed to read profile: {sptProfilePath}");
                }
            }

            return null;
        }
    }
}
