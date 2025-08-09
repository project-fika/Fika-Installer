namespace Fika_Installer.Models
{
    public class GitHubAsset(string name, string url)
    {
        public string Name { get; set; } = name;
        public string Url { get; set; } = url;
    }
}
