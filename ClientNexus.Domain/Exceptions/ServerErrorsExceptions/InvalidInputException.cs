namespace ClientNexus.Domain.Exceptions.ServerErrorsExceptions
{
    public class InvalidInputException : Exception
    {
        public InvalidInputException() { }

        public InvalidInputException(string? message)
            : base(message) { }

        public InvalidInputException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
