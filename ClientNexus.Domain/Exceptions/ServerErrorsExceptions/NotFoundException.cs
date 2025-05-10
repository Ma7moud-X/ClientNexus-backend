namespace ClientNexus.Domain.Exceptions.ServerErrorsExceptions
{
    public class NotFoundException : ClientException
    {
        public NotFoundException() { }

        public NotFoundException(string? message)
            : base(message) { }

        public NotFoundException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
