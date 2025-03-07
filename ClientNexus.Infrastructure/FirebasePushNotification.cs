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
        string? googleAppCredententials = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
        string? projectId = Environment.GetEnvironmentVariable("GOOGLE_PROJECT_ID");

        if (string.IsNullOrWhiteSpace(googleAppCredententials))
        {
            throw new ArgumentException("GOOGLE_APPLICATION_CREDENTIALS environment variable is not set.", nameof(googleAppCredententials));
        }

        if (string.IsNullOrWhiteSpace(projectId))
        {
            throw new ArgumentException("GOOGLE_PROJECT_ID environment variable is not set.", nameof(projectId));
        }

        _firebaseApp = FirebaseApp.Create(new AppOptions()
        {
            Credential = GoogleCredential.FromFile(googleAppCredententials),
            ProjectId = projectId
        });
    }

    public async Task<string> SendNotificationAsync(string title, string body, string deviceToken, Dictionary<string, string>? data = null)
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
            throw new ArgumentException("Device token cannot be null or whitespace.", nameof(deviceToken));
        }

        var message = new Message()
        {
            Notification = new Notification
            {
                Title = title,
                Body = body
            },
            Data = data,
            Token = deviceToken
        };

        try
        {
            string messageId = await FirebaseMessaging.DefaultInstance.SendAsync(message);
            return messageId;
        }
        catch (FirebaseException ex)
        {
            throw new Exception($"Failed to send notification: {ex.Message}", ex);
        }
    }
}
