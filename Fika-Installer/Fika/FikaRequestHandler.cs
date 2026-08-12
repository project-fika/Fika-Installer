using System.Net;
using System.Text;
using System.Text.Json;
using Fika_Installer.Models.Enums;
using Fika_Installer.Models.Fika;

namespace Fika_Installer.Fika;

public sealed class FikaRequestHandler
{
    private readonly HttpClient _httpClient;

    public FikaRequestHandler(string url, string apiKey)
    {
        HttpClientHandler handler = new()
        {
            ServerCertificateCustomValidationCallback = (httpRequestMessage, cert, cetChain, policyErrors) => true
        };

        _httpClient = new(handler)
        {
            BaseAddress = new Uri(url)
        };
        _httpClient.DefaultRequestHeaders.Authorization = new("Bearer", apiKey);
        _httpClient.DefaultRequestHeaders.Add("requestcompressed", "0");
    }

    public bool WaitForConnection(TimeSpan timeout)
    {
        var checkInterval = TimeSpan.FromMilliseconds(500);
        var startTime = DateTime.Now;

        var success = false;

        while (!success)
        {
            var pingResponse = Ping();

            if (pingResponse.PingResult == FikaPingResult.Success)
            {
                if (pingResponse.HttpStatusCode == HttpStatusCode.OK)
                {
                    success = true;
                }
                else
                {
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
            var httpResponse = GetHttpResponse("fika/api/heartbeat");

            return new(FikaPingResult.Success, httpResponse.StatusCode);
        }
        catch
        {
            return new(FikaPingResult.Failed, HttpStatusCode.RequestTimeout);
        }
    }

    private byte[] EncodeBody<T>(T o)
    {
        var serialized = JsonSerializer.Serialize(o);
        return Encoding.UTF8.GetBytes(serialized);
    }

    private T? DecodeBody<T>(byte[] data)
    {
        var json = Encoding.UTF8.GetString(data);
        return JsonSerializer.Deserialize<T>(json);
    }

    private async Task<T?> GetJsonAsync<T>(string path)
    {
        var response = await _httpClient.GetAsync(path).Result.Content.ReadAsByteArrayAsync();
        return DecodeBody<T>(response);
    }

    private T? GetJson<T>(string path)
    {
        return Task.Run(() => GetJsonAsync<T>(path)).Result;
    }

    private async Task<T2?> PostJsonAsync<T1, T2>(string path, T1 o)
    {
        var data = EncodeBody(o);
        HttpContent httpContent = new ByteArrayContent(data);
        var response = await _httpClient.PostAsync(path, httpContent).Result.Content.ReadAsByteArrayAsync();
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
