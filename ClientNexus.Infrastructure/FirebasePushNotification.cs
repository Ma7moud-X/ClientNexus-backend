using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
using ClientNexus.Domain.Interfaces;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;

namespace ClientNexus.Infrastructure;

public class FirebasePushNotification : IPushNotification
{
    private readonly FirebaseApp _firebaseApp;

    public FirebasePushNotification()
    {
        string? googleCredentialsPath = Environment.GetEnvironmentVariable(
            "GOOGLE_APPLICATION_CREDENTIALS"
        );
        string? projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        if (string.IsNullOrEmpty(googleCredentialsPath))
        {
            googleCredentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "clientnexus-791b7-firebase-adminsdk-fbsvc-0ff0233d14.json");

            // If that doesn't exist, try with wwwroot
            if (!File.Exists(googleCredentialsPath))
            {
                googleCredentialsPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,
                    "wwwroot", "clientnexus-791b7-firebase-adminsdk-fbsvc-0ff0233d14.json");
            }
            // One more fallback - sometimes the base directory itself is wwwroot
            if (!File.Exists(googleCredentialsPath))
            {
                googleCredentialsPath = Path.Combine(
                    Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory).FullName,
                    "clientnexus-791b7-firebase-adminsdk-fbsvc-0ff0233d14.json");
            }
        }

        // Log the path we're trying to use (helpful for debugging)
        Console.WriteLine($"Attempting to use Firebase credentials at: {googleCredentialsPath}");


        if (string.IsNullOrWhiteSpace(googleCredentialsPath))
        {
            Console.WriteLine($"failed to use Firebase credentials at: {googleCredentialsPath}");

            throw new ArgumentException(
                "GOOGLE_APPLICATION_CREDENTIALS environment variable is not set, , and fallback path is also invalid.",
                nameof(googleCredentialsPath)
            );
        }

        if (string.IsNullOrWhiteSpace(projectId))
        {
            throw new ArgumentException(
                "GOOGLE_PROJECT_ID environment variable is not set.",
                nameof(projectId)
            );
        }

        _firebaseApp = FirebaseApp.Create(
            new AppOptions()
            {
                Credential = GoogleCredential.FromFile(googleCredentialsPath),
                ProjectId = projectId,
            }
        );
    }

    public async Task<string> SendNotificationAsync(
        string title,
        string body,
        string deviceToken,
        Dictionary<string, string>? data = null
    )
    {
        if (string.IsNullOrWhiteSpace(title))
        {
            throw new ArgumentException("Title cannot be null or whitespace.", nameof(title));
        }

        if (string.IsNullOrWhiteSpace(body))
        {
            throw new ArgumentException("Body cannot be null or whitespace.", nameof(body));
        }

        if (string.IsNullOrWhiteSpace(deviceToken))
        {
            throw new ArgumentException(
                "Device token cannot be null or whitespace.",
                nameof(deviceToken)
            );
        }

        var message = new Message()
        {
            Notification = new Notification { Title = title, Body = body },
            Data = data,
            Token = deviceToken,
        };

        try
        {
            string messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            return messageId;
        }
        catch (FirebaseException ex)
        {
            throw new NotificationException(
                $"Failed to send notification with message: `{message}`",
                ex
            );
        }
    }
}
