using Fika_Installer.Models;
using Newtonsoft.Json.Linq;

namespace Fika_Installer
{
    public static class SptUtils
    {
        public static SptProfile[] GetSptProfiles(string sptProfilesFolder, bool headlessProfilesOnly = false)
        {
            List<SptProfile> sptProfiles = [];

            if (Directory.Exists(sptProfilesFolder))
            {
                string[] profilesPaths = Directory.GetFiles(sptProfilesFolder);

                if (profilesPaths.Length > 0)
                {
                    foreach (string profilePath in profilesPaths)
                    {
                        SptProfile sptProfile = GetSptProfileInfo(profilePath);

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

            return [.. sptProfiles];
        }

        public static SptProfile GetSptProfileInfo(string sptProfilePath)
        {
            SptProfile sptProfile = new();

            string profileJsonContent = File.ReadAllText(sptProfilePath);
            JObject profileJObject = JObject.Parse(profileJsonContent);

            string? profileId = profileJObject["info"]?["id"]?.ToString();
            string? username = profileJObject["info"]?["username"]?.ToString();
            string? password = profileJObject["info"]?["password"]?.ToString();

            if (profileId != null && username != null && password != null)
            {
                sptProfile = new(profileId, username, password);
            }

            return sptProfile;
        }
    }
}
