using System.Text.Json.Serialization;

namespace Fika_Installer.Models.Spt
{
    public class SptLauncherConfigModel
    {
        [JsonPropertyName("FirstRun")]
        public bool FirstRun { get; set; }

        [JsonPropertyName("DefaultLocale")]
        public string DefaultLocale { get; set; } = "";

        [JsonPropertyName("LauncherStartGameAction")]
        public int LauncherStartGameAction { get; set; }

        [JsonPropertyName("UseAutoLogin")]
        public bool UseAutoLogin { get; set; }

        [JsonPropertyName("IsDevMode")]
        public bool IsDevMode { get; set; }

        [JsonPropertyName("GamePath")]
        public string GamePath { get; set; } = "";

        [JsonPropertyName("ExcludeFromCleanup")]
        public List<string> ExcludeFromCleanup { get; set; } = new();

        [JsonPropertyName("Server")]
        public ServerConfig Server { get; set; } = new();
    }

    public class ServerConfig
    {
        [JsonPropertyName("AutoLoginCreds")]
        public AutoLoginCreds AutoLoginCreds { get; set; } = new();

        [JsonPropertyName("Name")]
        public string Name { get; set; } = "";

        [JsonPropertyName("Url")]
        public string Url { get; set; } = "";
    }

    public class AutoLoginCreds
    {
        [JsonPropertyName("Username")]
        public string Username { get; set; } = "";

        [JsonPropertyName("Password")]
        public string Password { get; set; } = "";
    }
}
