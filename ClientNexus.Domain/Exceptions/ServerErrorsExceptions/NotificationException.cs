namespace ClientNexus.Domain.Exceptions.ServerErrorsExceptions
{
    public class NotificationException : ServerException
    {
        public NotificationException(string message)
            : base(message) { }

        public NotificationException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
