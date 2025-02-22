namespace ClientNexus.Domain.Entities.Content
{
    public class DocumentCategory
    {
        public int DocumentId { get; set; }
        public Document? Document { get; set; }

        public int DCategoryId { get; set; }
        public DCategory? Category { get; set; }
    }
}