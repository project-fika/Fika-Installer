using Fika_Installer.Models;
using Fika_Installer.Utils;
using Newtonsoft.Json.Linq;

namespace Fika_Installer.Spt
{
    public class SptInstance
    {
        public string LocationPath { get; private set; }
        public string ServerExePath { get; private set; }
        public string LauncherExePath { get; private set; }
        public SptServer SptServer { get; private set; }
        public List<SptProfile> Profiles => GetSptProfiles(true);

        private string _profilesPath;

        public SptInstance(string locationPath)
        {
            LocationPath = locationPath;
            ServerExePath = Path.Combine(locationPath, "SPT.Server.exe");
            LauncherExePath = Path.Combine(locationPath, "SPT.Launcher.exe");
            SptServer = new(ServerExePath);

            _profilesPath = Path.Combine(locationPath, @"user\profiles");
        }

        public List<SptProfile> GetSptProfiles(bool headlessProfilesOnly = false)
        {
            List<SptProfile> sptProfiles = [];

            if (Directory.Exists(_profilesPath))
            {
                string[] profilesPaths = Directory.GetFiles(_profilesPath);

                if (profilesPaths.Length > 0)
                {
                    foreach (string profilePath in profilesPaths)
                    {
                        SptProfile? sptProfile = GetSptProfileFromJson(profilePath);

                        if (sptProfile != null)
                        {
                            if (headlessProfilesOnly)
                            {
                                if (sptProfile.Password != "fika-headless")
                                {
                                    continue;
                                }
                            }

                            sptProfiles.Add(sptProfile);
                        }
                    }
                }
            }

            return sptProfiles;
        }

        public SptProfile? GetSptProfile(string profileId)
        {
            return Profiles.FirstOrDefault(p => p.ProfileId == profileId);
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
                catch (Exception ex)
                {
                    ConUtils.WriteError($"Failed to read profile: {sptProfilePath}");
                }
            }

            return null;
        }
    }
}
