namespace Database.Models.Content;

public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public ICollection<DocumentCategory>? DocumentsCategories { get; set; }
    public ICollection<Document>? Documents { get; set; }
}
