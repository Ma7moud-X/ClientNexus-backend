namespace ClientNexus.Domain.Exceptions.ServerErrorsExceptions
{
    public class ConnectionEstablishmentException : ServerException
    {
        public ConnectionEstablishmentException() { }

        public ConnectionEstablishmentException(string? message)
            : base(message) { }

        public ConnectionEstablishmentException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
