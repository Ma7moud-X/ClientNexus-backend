using Database.Models.Users;

namespace Database.Models.Content;

// public enum DocumentType
//     {
//         Article,
//         template,
//         Other
// }

public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public string? Url { get; set; }
    
    public int DocumentTypeId { get; set; }
    public DocumentType? DocumentType { get; set; }

    public ICollection<DocumentCategory>? DocumentsCategories { get; set; }
    public ICollection<Category>? Categories { get; set; }

    public int UploadedById { get; set; }
    public Admin? UploadedBy { get; set; }
}
