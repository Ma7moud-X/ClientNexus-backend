using ClientNexus.Domain.Entities.Users;

namespace ClientNexus.Domain.Entities.Content;

// public enum DocumentType
//     {
//         Article,
//         template,
//         Other
// }

public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Content { get; set; } = default!;
    //public string? Url { get; set; }
    public string? ImageUrl { get; set; }


    public int DocumentTypeId { get; set; }
    public DocumentType? DocumentType { get; set; }

    public ICollection<DocumentCategory>? DocumentCategories { get; set; }
    public ICollection<DCategory>? Categories { get; set; }

    public int? UploadedById { get; set; }
    public Admin? UploadedBy { get; set; }
}
