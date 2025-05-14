namespace ClientNexus.Domain.Exceptions.ServerErrorsExceptions
{
    public class MessageRateExceeded : PushNotificationException
    {
        public MessageRateExceeded() { }

        public MessageRateExceeded(string? message)
            : base(message) { }

        public MessageRateExceeded(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
