using System.Text.Json;
using Fika_Installer.Models.GitHub;

namespace Fika_Installer;

public static class GitHub
{
    public static GitHubRelease? GetReleaseFromUrl(string url)
    {
        try
        {
            var releaseJson = GetHttpContent(url);
            return JsonSerializer.Deserialize<GitHubRelease>(releaseJson);
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

            var response = client.GetAsync(url).Result;
            response.EnsureSuccessStatusCode();

            return response.Content.ReadAsStringAsync().Result;
        }
    }
}
