using Fika_Installer.Models;
using Fika_Installer.Utils;
using Newtonsoft.Json.Linq;

namespace Fika_Installer
{
    public static class GitHub
    {
        public static GitHubAsset[] FetchGitHubAssets(string releaseUrl)
        {
            List<GitHubAsset> githubAssets = [];

            try
            {
                string json = GetHttpContent(releaseUrl);

                JObject release = JObject.Parse(json);

                JArray assets = (JArray)release["assets"];

                if (assets == null || assets.Count == 0)
                {
                    return [.. githubAssets];
                }

                foreach (JObject asset in assets)
                {
                    string? name = asset["name"]?.ToString();
                    string? url = asset["browser_download_url"]?.ToString();

                    if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(url))
                    {
                        GitHubAsset gitHubAsset = new(name, url);
                        githubAssets.Add(gitHubAsset);
                    }
                }
            }
            catch (Exception ex)
            {
                ConUtils.WriteError($"An error occurred when fetching GitHub assets: {ex.Message}", true);
            }

            return [.. githubAssets];
        }

        public static string GetHttpContent(string url)
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("FikaInstaller");

                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                return response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
