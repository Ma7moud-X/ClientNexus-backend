using System.Runtime.Serialization;

namespace ClientNexus.Domain.Exceptions.ServerErrorsExceptions
{
    public class ServerException : Exception
    {
        public ServerException() { }

        public ServerException(string? message)
            : base(message) { }

        public ServerException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
