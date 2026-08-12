using System.Text.Json.Serialization;

namespace Fika_Installer.Models.GitHub;

public sealed record GitHubAsset(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("browser_download_url")] string BrowserDownloadUrl
);
