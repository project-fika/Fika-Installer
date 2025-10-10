using Fika_Installer.Models.Fika;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace Fika_Installer.Fika
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
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            _httpClient.DefaultRequestHeaders.Add("requestcompressed", "0");
        }

        public bool TestConnection(TimeSpan timeout)
        {
            TimeSpan checkInterval = TimeSpan.FromMilliseconds(500);
            DateTime startTime = DateTime.Now;

            bool success = false;

            while (!success)
            {
                FikaPingResponse pingResponse = Ping();

                if (pingResponse.PingResult == PingResult.Success)
                {
                    switch (pingResponse.HttpStatusCode)
                    {
                        case HttpStatusCode.OK:
                            success = true;
                            break;
                        default:
                            return false;
                    }
                }

                if (DateTime.Now - startTime > timeout)
                {
                    return false;
                }

                Thread.Sleep(checkInterval);
            }

            Thread.Sleep(TimeSpan.FromSeconds(1));

            return true;
        }

        public CreateHeadlessProfileResponse? CreateHeadlessProfile()
        {
            return PostJson<object, CreateHeadlessProfileResponse>("fika/api/createheadlessprofile", null);
        }

        public FikaPingResponse Ping()
        {
            try
            {
                HttpResponseMessage httpResponse = GetHttpResponse("fika/api/heartbeat");
                
                return new(PingResult.Success, httpResponse.StatusCode);
            }
            catch
            {
                return new(PingResult.Failed, HttpStatusCode.RequestTimeout);
            }
        }

        private byte[] EncodeBody<T>(T o)
        {
            string serialized = JsonSerializer.Serialize(o);
            return Encoding.UTF8.GetBytes(serialized);
        }

        private T? DecodeBody<T>(byte[] data)
        {
            string json = Encoding.UTF8.GetString(data);
            return JsonSerializer.Deserialize<T>(json);
        }

        private async Task<T?> GetJsonAsync<T>(string path)
        {
            byte[] response = await _httpClient.GetAsync(path).Result.Content.ReadAsByteArrayAsync();
            return DecodeBody<T>(response);
        }

        private T? GetJson<T>(string path)
        {
            return Task.Run(() => GetJsonAsync<T>(path)).Result;
        }

        private async Task<T2?> PostJsonAsync<T1, T2>(string path, T1 o)
        {
            byte[] data = EncodeBody(o);
            HttpContent httpContent = new ByteArrayContent(data);
            byte[] response = await _httpClient.PostAsync(path, httpContent).Result.Content.ReadAsByteArrayAsync();
            return DecodeBody<T2>(response);
        }

        private T2? PostJson<T1, T2>(string path, T1? o)
        {
            return Task.Run(() => PostJsonAsync<T1, T2>(path, o)).Result;
        }

        private async Task<HttpResponseMessage> GetHttpResponseAsync(string path)
        {
            return await _httpClient.GetAsync(path);
        }

        private HttpResponseMessage GetHttpResponse(string path)
        {
            return Task.Run(() => GetHttpResponseAsync(path)).Result;
        }
    }
}
