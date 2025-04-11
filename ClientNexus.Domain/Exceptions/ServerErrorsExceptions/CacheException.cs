namespace ClientNexus.Domain.Exceptions.ServerErrorsExceptions
{
    public class CacheException : ServerException
    {
        public CacheException(string msg)
            : base(msg) { }

        public CacheException(string msg, Exception innerException)
            : base(msg, innerException) { }
    }
}
