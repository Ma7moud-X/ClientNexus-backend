namespace Database.Models.Content;

public class DocumentType
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;

    public ICollection<Document>? Documents { get; set; }
}
