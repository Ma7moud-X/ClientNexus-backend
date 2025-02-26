namespace ClientNexus.Application.Enums;

public enum FileType {
    Jpeg,
    Png,
    MsWord,
    Pdf,
    PlainText
}

public static class FileTypeExtensions {
    public static string GetMimeType(this FileType fileType) {
        switch (fileType) {
            case FileType.Jpeg:
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
}