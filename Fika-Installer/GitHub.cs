using Fika_Installer.Models.GitHub;
using System.Text.Json;

namespace Fika_Installer
{
    public static class GitHub
    {
        public static GitHubRelease? GetReleaseFromUrl(string url)
        {
            try
            {
                string releaseJson = GetHttpContent(url);

                GitHubRelease? gitHubRelease = JsonSerializer.Deserialize<GitHubRelease>(releaseJson);

                return gitHubRelease;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }

        public static string GetHttpContent(string url)
        {
            using (HttpClient client = new())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("FikaInstaller");

                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
