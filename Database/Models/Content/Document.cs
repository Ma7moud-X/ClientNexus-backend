using Database.Models.Users;

namespace Database.Models.Content;

public class Document
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string? Url { get; set; }

    public int? CategoryId { get; set; }
    public Category? Category { get; set; }

    public int UploadedById { get; set; }
    public Admin? UploadedBy { get; set; }
}
