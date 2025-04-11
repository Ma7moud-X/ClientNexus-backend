using ClientNexus.Application.Enums;

namespace ClientNexus.Application.Interfaces;

public interface IHttpService
{
    Task<T> SendRequestAsync<T>(
        string url,
        HttpMethod method,
        IEnumerable<(string headerKey, string headerValue)>? headers = null,
        object? body = null,
        ContentType contentType = ContentType.Json,
        System.Text.Encoding? encoding = null
    );
}
