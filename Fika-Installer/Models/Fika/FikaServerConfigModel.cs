namespace Fika_Installer.Models.Fika
{
    using System.Text.Json.Serialization;
    using System.Collections.Generic;

    public class FikaServerConfigModel
    {
        [JsonPropertyName("client")]
        public FikaServerClientConfig Client { get; set; } = new();

        [JsonPropertyName("server")]
        public FikaServerServerConfig Server { get; set; } = new();

        [JsonPropertyName("natPunchServer")]
        public FikaServerNatPunchServerConfig NatPunchServer { get; set; } = new();

        [JsonPropertyName("headless")]
        public FikaServerHeadlessConfig Headless { get; set; } = new();

        [JsonPropertyName("background")]
        public FikaServerBackgroundConfig Background { get; set; } = new();
    }

    public class FikaServerClientConfig
    {
        [JsonPropertyName("useBtr")]
        public bool UseBtr { get; set; }

        [JsonPropertyName("friendlyFire")]
        public bool FriendlyFire { get; set; }

        [JsonPropertyName("dynamicVExfils")]
        public bool DynamicVExfils { get; set; }

        [JsonPropertyName("allowFreeCam")]
        public bool AllowFreeCam { get; set; }

        [JsonPropertyName("allowSpectateFreeCam")]
        public bool AllowSpectateFreeCam { get; set; }

        [JsonPropertyName("blacklistedItems")]
        public List<string> BlacklistedItems { get; set; } = new();

        [JsonPropertyName("forceSaveOnDeath")]
        public bool ForceSaveOnDeath { get; set; }

        [JsonPropertyName("mods")]
        public ModsConfig Mods { get; set; } = new();

        [JsonPropertyName("useInertia")]
        public bool UseInertia { get; set; }

        [JsonPropertyName("sharedQuestProgression")]
        public bool SharedQuestProgression { get; set; }

        [JsonPropertyName("canEditRaidSettings")]
        public bool CanEditRaidSettings { get; set; }

        [JsonPropertyName("enableTransits")]
        public bool EnableTransits { get; set; }

        [JsonPropertyName("anyoneCanStartRaid")]
        public bool AnyoneCanStartRaid { get; set; }
    }

    public class ModsConfig
    {
        [JsonPropertyName("required")]
        public List<string> Required { get; set; } = new();

        [JsonPropertyName("optional")]
        public List<string> Optional { get; set; } = new();
    }

    public class FikaServerServerConfig
    {
        [JsonPropertyName("SPT")]
        public SptConfig SPT { get; set; } = new();

        [JsonPropertyName("allowItemSending")]
        public bool AllowItemSending { get; set; }

        [JsonPropertyName("sentItemsLoseFIR")]
        public bool SentItemsLoseFIR { get; set; }

        [JsonPropertyName("launcherListAllProfiles")]
        public bool LauncherListAllProfiles { get; set; }

        [JsonPropertyName("sessionTimeout")]
        public int SessionTimeout { get; set; }

        [JsonPropertyName("showDevProfile")]
        public bool ShowDevProfile { get; set; }

        [JsonPropertyName("showNonStandardProfile")]
        public bool ShowNonStandardProfile { get; set; }

        [JsonPropertyName("logClientModsInConsole")]
        public bool LogClientModsInConsole { get; set; }
    }

    public class SptConfig
    {
        [JsonPropertyName("http")]
        public HttpConfig Http { get; set; } = new();

        [JsonPropertyName("disableSPTChatBots")]
        public bool DisableSPTChatBots { get; set; }
    }

    public class HttpConfig
    {
        [JsonPropertyName("ip")]
        public string Ip { get; set; } = "";

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("backendIp")]
        public string BackendIp { get; set; } = "";

        [JsonPropertyName("backendPort")]
        public int BackendPort { get; set; }
    }

    public class FikaServerNatPunchServerConfig
    {
        [JsonPropertyName("enable")]
        public bool Enable { get; set; }

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("natIntroduceAmount")]
        public int NatIntroduceAmount { get; set; }
    }

    public class FikaServerHeadlessConfig
    {
        [JsonPropertyName("profiles")]
        public ProfilesConfig Profiles { get; set; } = new();

        [JsonPropertyName("scripts")]
        public ScriptsConfig Scripts { get; set; } = new();

        [JsonPropertyName("setLevelToAverageOfLobby")]
        public bool SetLevelToAverageOfLobby { get; set; }

        [JsonPropertyName("restartAfterAmountOfRaids")]
        public int RestartAfterAmountOfRaids { get; set; }
    }

    public class ProfilesConfig
    {
        [JsonPropertyName("amount")]
        public int Amount { get; set; }

        [JsonPropertyName("aliases")]
        public Dictionary<string, string> Aliases { get; set; } = new();
    }

    public class ScriptsConfig
    {
        [JsonPropertyName("generate")]
        public bool Generate { get; set; }

        [JsonPropertyName("forceIp")]
        public string ForceIp { get; set; } = "";
    }

    public class FikaServerBackgroundConfig
    {
        [JsonPropertyName("enable")]
        public bool Enable { get; set; }

        [JsonPropertyName("easteregg")]
        public bool EasterEgg { get; set; }
    }
}
