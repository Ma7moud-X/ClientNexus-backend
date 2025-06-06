namespace ClientNexus.Domain.Exceptions.ServerErrorsExceptions
{
    public class DeviceNotRegisteredException : PushNotificationException
    {
        public DeviceNotRegisteredException() { }

        public DeviceNotRegisteredException(string? message)
            : base(message) { }

        public DeviceNotRegisteredException(string? message, Exception? innerException)
            : base(message, innerException) { }
    }
}
