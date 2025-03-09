using System.Text.Json;
using ClientNexus.Application.Enums;
using ClientNexus.Application.Interfaces;

namespace ClientNexus.Application.Services;

public class HttpService : IHttpService
{
    private readonly HttpClient _httpClient;

    public HttpService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<T> SendRequestAsync<T>(
        string url,
        HttpMethod method,
        IEnumerable<(string headerKey, string headerValue)>? headers = null,
        object? body = null,
        ContentType contentType = ContentType.Json,
        System.Text.Encoding? encoding = null
    )
    {
        StringContent? content = null;
        if (body is not null)
        {
            encoding ??= System.Text.Encoding.UTF8;

            var json = JsonSerializer.Serialize(body);
            content = new StringContent(
                json,
                encoding ?? System.Text.Encoding.UTF8,
                contentType.ToContentType()
            );
        }

        HttpRequestMessage request = new HttpRequestMessage(
            method,
            url
        )
        {
            Content = content,
        };
        if (headers != null)
        {
            foreach (var (headerKey, headerValue) in headers)
            {
                request.Headers.Add(headerKey, headerValue);
            }
        }

        HttpResponseMessage response;
        try
        {
            response = await _httpClient.SendAsync(request);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error sending request: {ex.Message}", ex);
        }

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error: {response.StatusCode}");
        }

        string strResponse;
        try
        {
            strResponse = await response.Content.ReadAsStringAsync();
        }
        catch (Exception ex)
        {
            throw new Exception($"Error reading response: {ex.Message}", ex);
        }

        return JsonSerializer.Deserialize<T>(strResponse)
            ?? throw new Exception("Error deserializing response");
    }
}
