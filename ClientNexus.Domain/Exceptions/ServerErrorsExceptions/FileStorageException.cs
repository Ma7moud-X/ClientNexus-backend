namespace ClientNexus.Domain.Exceptions.ServerErrorsExceptions
{
    public class FileStorageException : ServerException
    {
        public FileStorageException(string message)
            : base(message) { }

        public FileStorageException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
