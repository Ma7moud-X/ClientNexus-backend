namespace Database.Models.Content;

public class Category
{
    public int Id { get; set; }
    public string? Name { get; set; }

    public int DocumentId { get; set; }
    public Document Document { get; set; }
}
