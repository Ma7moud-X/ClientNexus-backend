namespace ClientNexus.Application.Enums;

public enum ContentType
{
    Json,
    Xml,
}

public static class ContentTypeExtensions
{
    public static string ToContentType(this ContentType contentType)
    {
        return contentType switch
        {
            ContentType.Json => "application/json",
            ContentType.Xml => "application/xml",
            _ => throw new ArgumentOutOfRangeException(nameof(contentType), contentType, null),
        };
    }
}
