using System.Text.Json.Serialization;

namespace Fika_Installer.Models.GitHub;

public sealed record GitHubAsset
{
    [JsonPropertyName("name")]
    public string Name { get; set; } = "";

    [JsonPropertyName("browser_download_url")]
    public string BrowserDownloadUrl { get; set; } = "";
}
