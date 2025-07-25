using Newtonsoft.Json.Linq;
using System.Net;

namespace Fika_Installer
{
    public static class GitHub
    {
        public static string[] RetrieveAssetsUrl(string releaseUrl)
        {
            List<string> assetsUrl = new List<string>();

            string json = GetHttpContent(releaseUrl);

            JObject release = JObject.Parse(json);

            JArray assets = (JArray)release["assets"];

            if (assets == null || assets.Count == 0) 
            {
                return [.. assetsUrl];
            }

            foreach (JObject asset in assets)
            {
                string? url = asset["browser_download_url"]?.ToString();
                    
                if (!string.IsNullOrEmpty(url))
                {
                    assetsUrl.Add(url);
                }
            }

            return [.. assetsUrl];
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

        public static void DownloadFile(string downloadUrl, string outputPath)
        {
            using (HttpClient client = new HttpClient
            {
                Timeout = TimeSpan.FromMinutes(30)
            })
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("FikaInstaller");

                using (HttpResponseMessage response = client.GetAsync(downloadUrl, HttpCompletionOption.ResponseHeadersRead).Result)
                {
                    response.EnsureSuccessStatusCode();

                    long? totalBytes = response.Content.Headers.ContentLength;

                    using (Stream contentStream = response.Content.ReadAsStreamAsync().Result)
                    using (FileStream fileStream = new FileStream(outputPath, FileMode.Create, FileAccess.Write, FileShare.None, 8192, true))
                    {
                        byte[] buffer = new byte[8192];
                        long totalRead = 0;
                        int read;
                        double previousProgress = -1;

                        Console.WriteLine();

                        while ((read = contentStream.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            fileStream.Write(buffer, 0, read);
                            totalRead += read;

                            if (totalBytes.HasValue)
                            {
                                double progress = (double)totalRead / totalBytes.Value * 100;
                                if ((int)progress != (int)previousProgress)
                                {
                                    Console.Write($"\rDownloading: {(int)progress}%   ");
                                    previousProgress = progress;
                                }
                            }
                        }

                        Console.WriteLine("\nDownload complete.");
                    }
                }
            }
        }
    }
}
