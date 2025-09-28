namespace Fika_Installer.Models
{
    public class FikaRelease(string releaseName, string releaseUrl)
    {
        public string Name { get; set; } = releaseName;
        public string Url { get; set; } = releaseUrl;
    }
}
