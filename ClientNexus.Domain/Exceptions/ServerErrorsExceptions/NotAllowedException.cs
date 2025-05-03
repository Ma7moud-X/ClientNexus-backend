namespace ClientNexus.Domain.Exceptions.ServerErrorsExceptions
{
    public class NotAllowedException : ClientException
    {
        public NotAllowedException() { }

        public NotAllowedException(string? message)
            : base(message) { }

        public NotAllowedException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
