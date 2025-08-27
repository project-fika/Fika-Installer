using Fika_Installer.Models.Spt;
using Fika_Installer.Utils;
using System.Diagnostics;
using System.Text.Json;

namespace Fika_Installer.Spt
{
    public class SptInstance
    {
        private string _profilesPath;
        private string _launcherConfigPath;

        public string SptPath { get; private set; }
        public string ServerExePath { get; private set; }
        public string LauncherExePath { get; private set; }
        public string TarkovExePath { get; private set; }
        public string EftVersion { get; private set; } = "";
        public List<SptProfile> Profiles { get; private set; } = [];

        public SptInstance(string sptPath)
        {
            _profilesPath = Path.Combine(sptPath, @"user\profiles");
            _launcherConfigPath = Path.Combine(sptPath, @"user\launcher\config.json");

            SptPath = sptPath;
            ServerExePath = Path.Combine(sptPath, "SPT.Server.exe");
            LauncherExePath = Path.Combine(sptPath, "SPT.Launcher.exe");
            TarkovExePath = Path.Combine(sptPath, "EscapeFromTarkov.exe");

            LoadProfiles();

            if (File.Exists(TarkovExePath))
            {
                FileVersionInfo? tarkovVersionInfo = FileVersionInfo.GetVersionInfo(TarkovExePath);

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

            Profiles = sptProfiles;
        }

        public SptProfile? GetProfile(string profileId)
        {
            return Profiles.FirstOrDefault(p => p.ProfileId == profileId);
        }

        public List<SptProfile> GetHeadlessProfiles()
        {
            return [.. Profiles.Where(p => p.Headless)];
        }

        public SptLauncherConfigModel? GetLauncherConfig()
        {
            if (File.Exists(_launcherConfigPath))
            {
                SptLauncherConfigModel? launcherConfig = JsonUtils.ReadJson<SptLauncherConfigModel>(_launcherConfigPath);
                
                return launcherConfig;
            }

            return null;
        }

        public bool SetLauncherConfig(SptLauncherConfigModel launcherConfig)
        {
            try
            {
                JsonUtils.WriteJson(_launcherConfigPath, launcherConfig);
                
                return true;
            }
            catch
            {
                return false;
            }
        }

        private SptProfile? GetProfileFromJson(string sptProfilePath)
        {
            if (File.Exists(sptProfilePath))
            {
                try
                {
                    SptProfileModel? profile = JsonUtils.ReadJson<SptProfileModel>(sptProfilePath);

                    if (profile != null)
                    {
                        bool headlessProfile = false;

                        if (profile.Info.Password == "fika-headless")
                        {
                            headlessProfile = true;
                        }

                        SptProfile sptProfile = new(profile.Info.Id, profile.Info.Username, profile.Info.Password, headlessProfile);

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
