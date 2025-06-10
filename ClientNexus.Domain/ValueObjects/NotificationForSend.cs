namespace ClientNexus.Domain.ValueObjects
{
    public class NotificationForSend
    {
        public required string title { get; set; }
        public required string body { get; set; }
        public required string deviceToken { get; set; }
        public Dictionary<string, string>? data { get; set; }
    }
}
