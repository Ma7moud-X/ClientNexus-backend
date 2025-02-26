namespace ClientNexus.Domain.Enums;

public enum FileType
{
    Jpeg,
    Jpg,
    Png,
    MsWord,
    Pdf,
    PlainText
}

public static class FileTypeExtensions
{
    public static string ToMimeType(this FileType fileType)
    {
        switch (fileType)
        {
            case FileType.Jpeg:
            case FileType.Jpg:
                return "image/jpeg";
            case FileType.Png:
                return "image/png";
            case FileType.MsWord:
                return "application/msword";
            case FileType.Pdf:
                return "application/pdf";
            case FileType.PlainText:
                return "text/plain";
            default:
                throw new ArgumentException("Invalid file type");
        }
    }

    public static string ToAbstractType(this FileType fileType)
    {
        switch (fileType)
        {
            case FileType.Jpeg:
            case FileType.Jpg:
            case FileType.Png:
                return "image";
            case FileType.MsWord:
            case FileType.Pdf:
            case FileType.PlainText:
                return "document";
            default:
                throw new ArgumentException("Invalid file type");
        }
    }
}
