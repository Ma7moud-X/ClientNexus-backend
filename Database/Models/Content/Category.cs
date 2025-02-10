namespace Database.Models.Content;

public class Category
{
    public int Id { get; set; }
    public string Name { get; set; }

    public List<Document>? Documents { get; set; }
}
