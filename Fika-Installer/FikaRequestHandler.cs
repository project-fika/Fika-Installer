using Fika_Installer.Models.Fika;
using System.Text;
using System.Text.Json;

namespace Fika_Installer
{
    public class FikaRequestHandler
    {
        private HttpClient _httpClient;

        public FikaRequestHandler(string url, string apiKey)
        {
            HttpClientHandler handler = new()
            {
                ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
            };

            _httpClient = new(handler);
            _httpClient.BaseAddress = new Uri(url);
            _httpClient.DefaultRequestHeaders.Add("Auth", apiKey);
            _httpClient.DefaultRequestHeaders.Add("requestcompressed", "0");
        }

        public bool TestConnection(TimeSpan timeout)
        {
            TimeSpan checkInterval = TimeSpan.FromMilliseconds(500);

            DateTime startTime = DateTime.Now;

            bool success = false;

            while (!success)
            {
                try
                {
                    success = Ping();
                }
                catch { }

                if (DateTime.Now - startTime > timeout)
                {
                    return false;
                }

                Thread.Sleep(checkInterval);
            }

            Thread.Sleep(TimeSpan.FromSeconds(1));

            return true;
        }

        public CreateHeadlessProfileResponse CreateHeadlessProfile()
        {
            return PostJson<object, CreateHeadlessProfileResponse>("/post/createheadlessprofile", null);
        }

        public bool Ping()
        {
            return GetNoBodyResponse("/get/heartbeat");
        }

        private byte[] EncodeBody<T>(T o)
        {
            string serialized = JsonSerializer.Serialize(o);
            return Encoding.UTF8.GetBytes(serialized);
        }

        private T DecodeBody<T>(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(json);
        }

        private async Task<T> GetJsonAsync<T>(string path)
        {
            byte[] response = await _httpClient.GetAsync(path).Result.Content.ReadAsByteArrayAsync();
            return DecodeBody<T>(response);
        }

        private T GetJson<T>(string path)
        {
            return Task.Run(() => GetJsonAsync<T>(path)).Result;
        }

        private async Task<T2> PostJsonAsync<T1, T2>(string path, T1 o)
        {
            byte[] data = EncodeBody<T1>(o);
            HttpContent httpContent = new ByteArrayContent(data);
            byte[] response = await _httpClient.PostAsync(path, httpContent).Result.Content.ReadAsByteArrayAsync();
            return DecodeBody<T2>(response);
        }

        private T2 PostJson<T1, T2>(string path, T1? o)
        {
            return Task.Run(() => PostJsonAsync<T1, T2>(path, o)).Result;
        }

        private async Task<bool> GetNoBodyResponseAsync(string path)
        {
            try
            {
                var result = await _httpClient.GetAsync(path);

                result.EnsureSuccessStatusCode();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool GetNoBodyResponse(string path)
        {
            return Task.Run(() => GetNoBodyResponseAsync(path)).Result;
        }
    }
}
