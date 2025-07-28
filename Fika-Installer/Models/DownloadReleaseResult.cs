namespace Fika_Installer.Models
{
    public class DownloadReleaseResult
    {
        public string Name { get; set; }
        public string Url { get; set; }
        public bool Result { get; set; }

        public DownloadReleaseResult()
        {
            Name = "";
            Url = "";
            Result = false;
        }

        public DownloadReleaseResult(string name, string url, bool result)
        {
            Name = name;
            Url = url;
            Result = result;
        }
    }
}
