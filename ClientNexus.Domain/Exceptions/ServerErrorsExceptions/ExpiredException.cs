namespace ClientNexus.Domain.Exceptions.ServerErrorsExceptions
{
    public class ExpiredException : ClientException
    {
        public ExpiredException() { }

        public ExpiredException(string? message)
            : base(message) { }

        public ExpiredException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
