namespace Fika_Installer.Models.GitHub
{
    public class DownloadReleaseResult(string name, string url, bool result)
    {
        public string Name { get; set; } = name;
        public string Url { get; set; } = url;
        public bool Result { get; set; } = result;
    }
}
