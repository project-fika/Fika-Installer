using System.Text.Json.Serialization;

namespace Fika_Installer.Models.GitHub;

public sealed record GitHubRelease(
    [property: JsonPropertyName("name")] string Name = "",
    [property: JsonPropertyName("tag_name")] string TagName = "",
    [property: JsonPropertyName("body")] string Body = "",
    [property: JsonPropertyName("assets")] List<GitHubAsset> Assets = null!
);