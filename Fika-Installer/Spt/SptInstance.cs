using System.Text.Json.Nodes;
using Fika_Installer.Models.Spt;
using Fika_Installer.Utils;

namespace Fika_Installer.Spt;

public sealed class SptInstance
{
    private readonly string _profilesPath;
    private readonly string _launcherConfigPath;

    public string ClientPath { get; }
    public string SptPath { get; }
    public string ServerExePath { get; }
    public List<SptProfile> Profiles { get; private set; } = [];

    public SptInstance(string path)
    {
        ClientPath = path;
        SptPath = Path.Combine(path, "SPT_Runtime");
        ServerExePath = Path.Combine(SptPath, SptConstants.ServerExeName);

        _profilesPath = Path.Combine(SptPath, @"user\profiles");
        _launcherConfigPath = Path.Combine(SptPath, @"user\launcher\config.json");

        LoadProfiles();
    }

    public void LoadProfiles()
    {
        List<SptProfile> sptProfiles = [];

        if (Directory.Exists(_profilesPath))
        {
            foreach (var profilePath in Directory.GetFiles(_profilesPath))
            {
                var sptProfile = GetProfileFromJson(profilePath);

                if (sptProfile != null)
                {
                    sptProfiles.Add(sptProfile);
                }
            }
        }

        Profiles = sptProfiles;
    }

    public SptProfile? GetProfileFromJson(string sptProfilePath)
    {
        if (File.Exists(sptProfilePath))
        {
            try
            {
                var profile = JsonUtils.DeserializeFromFile<JsonObject>(sptProfilePath);

                if (profile != null)
                {
                    var headless = false;

                    var id = profile["info"]?["id"]?.GetValue<string>();
                    var username = profile["info"]?["username"]?.GetValue<string>();

                    if (id != null && username != null)
                    {
                        headless = username.StartsWith("headless_");

                        return new SptProfile(id, username, headless);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"Failed to read profile: {sptProfilePath}. {ex.Message}");
            }
        }

        return null;
    }

    public SptProfile? GetProfile(string profileId)
    {
        return Profiles.FirstOrDefault(p => p.ProfileId == profileId);
    }

    public List<SptProfile> GetHeadlessProfiles()
    {
        return [.. Profiles.Where(p => p.Headless)];
    }

    public JsonObject? GetLauncherConfig()
    {
        if (File.Exists(_launcherConfigPath))
        {
            return JsonUtils.DeserializeFromFile<JsonObject>(_launcherConfigPath);
        }

        return null;
    }

    public bool SetLauncherConfig(JsonObject launcherConfig)
    {
        try
        {
            JsonUtils.SerializeToFile(_launcherConfigPath, launcherConfig);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
