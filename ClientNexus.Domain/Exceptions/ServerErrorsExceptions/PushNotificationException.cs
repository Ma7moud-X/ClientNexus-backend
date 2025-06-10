namespace ClientNexus.Domain.Exceptions.ServerErrorsExceptions
{
    public class PushNotificationException : ServerException
    {
        public PushNotificationException() { }

        public PushNotificationException(string? message)
            : base(message) { }

        public PushNotificationException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
