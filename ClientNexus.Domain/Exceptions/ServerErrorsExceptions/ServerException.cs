namespace ClientNexus.Domain.Exceptions.ServerErrorsExceptions
{
    public class ServerException : Exception
    {
        public ServerException(string msg)
            : base(msg) { }

        public ServerException(string msg, Exception innerException)
            : base(msg, innerException) { }
    }
}
