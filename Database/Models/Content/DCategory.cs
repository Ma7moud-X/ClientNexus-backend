namespace Database.Models.Content;

public class DCategory
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public ICollection<DocumentCategory>? DocumentCategories { get; set; }
    public ICollection<Document>? Documents { get; set; }
}
