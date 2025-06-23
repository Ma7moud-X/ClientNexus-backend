using ZoomNet;
using ZoomNet.Models;
using Microsoft.Extensions.Logging;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.DTO;

namespace ClientNexus.Infrastructure
{
    public class ZoomService : IZoomService
    {
        private readonly ILogger<ZoomService> _logger;
        private readonly ZoomClient _zoomClient;
        private readonly string _serviceHostEmail;

        public ZoomService(ILogger<ZoomService> logger)
        {
            _logger = logger;

            // Retrieve Zoom API credentials directly from environment variables
            var clientId = Environment.GetEnvironmentVariable("ZOOM_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("ZOOM_CLIENT_SECRET");
            var accountId = Environment.GetEnvironmentVariable("ZOOM_ACCOUNT_ID");
            _serviceHostEmail = Environment.GetEnvironmentVariable("ZOOM_SERVICE_HOST_EMAIL") ?? throw new InvalidOperationException("ZOOM_SERVICE_HOST_EMAIL environment variable is not set.");

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(accountId))
            {
                _logger.LogError("Zoom API credentials (ZOOM_CLIENT_ID, ZOOM_CLIENT_SECRET, ZOOM_ACCOUNT_ID) are not set as environment variables.");
                throw new InvalidOperationException("Zoom API credentials are missing from environment variables.");
            }
            if (string.IsNullOrEmpty(_serviceHostEmail))
            {
                _logger.LogError("ZOOM_SERVICE_HOST_EMAIL environment variable is not set.");
                throw new InvalidOperationException("Zoom service host email is missing from environment variables.");
            }

            // Initialize ZoomClient with Server-to-Server OAuth
            var connectionInfo = OAuthConnectionInfo.ForServerToServer(clientId, clientSecret, accountId);
            _zoomClient = new ZoomClient(connectionInfo);
        }

        public async Task<ZoomMeetingDetailsDTO> CreateMeetingAsync(string topic, DateTime startTime, int durationMinutes)
        {
            try
            {
                var meeting = new ScheduledMeeting()
                {
                    Topic = topic,
                    Type = MeetingType.Scheduled,
                    StartTime = startTime,
                    Duration = durationMinutes,
                    Timezone = TimeZones.UTC,
                    Settings = new MeetingSettings()
                    {
                        StartVideoWhenParticipantsJoin = true,
                        JoinBeforeHost = true,
                        MuteUponEntry = true,
                        ApprovalType = ApprovalType.Automatic,
                        Audio = AudioType.Both,
                        AutoRecording = AutoRecordingType.Disabled,
                        WaitingRoom = false
                    }
                };
                
                string agenda = $"Appointment with {topic}"; 

                
                var createdMeeting = await _zoomClient.Meetings.CreateScheduledMeetingAsync(
                    userId: _serviceHostEmail,          
                    topic: meeting.Topic,        
                    agenda: agenda,               
                    start: meeting.StartTime,   
                    duration: meeting.Duration,     
                    timeZone: meeting.Timezone,     
                    settings: meeting.Settings);

                _logger.LogInformation($"Zoom meeting created: ID={createdMeeting.Id}, Join URL={createdMeeting.JoinUrl}");

                return new ZoomMeetingDetailsDTO
                {
                    MeetingId = createdMeeting.Id,
                    JoinUrl = createdMeeting.JoinUrl,
                    HostStartUrl = createdMeeting.StartUrl
                };
            }
            catch (Exception ex)
            {
                //await CreateOrInviteZoomUserAsync(hostEmail, firstName, lastName);
                _logger.LogError(ex, $"Error creating Zoom meeting for topic '{topic}' using service host '{_serviceHostEmail}'.");
                throw new Exception("Failed to create Zoom meeting.", ex);
            }
        }
        public async Task DeleteMeetingAsync(long zoomMeetingId)
        {
            if (string.IsNullOrEmpty(zoomMeetingId.ToString()))
            {
                _logger.LogWarning("Attempted to delete Zoom meeting with empty ID.");
                return;
            }

            try
            {
                await _zoomClient.Meetings.DeleteAsync(zoomMeetingId);
                _logger.LogInformation($"Zoom meeting deleted: ID={zoomMeetingId}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting Zoom meeting with ID: {zoomMeetingId}.");

            }
        }
        public async Task<string> CreateOrInviteZoomUserAsync(string email, string firstName, string lastName)
        {
            try
            {

                // ZoomNet's CreateUserAsync or AddUserAsync method might take parameters like:
                // email, type (e.g., UserType.Basic or UserType.Licensed depending on your desired default),
                // optional: password, firstName, lastName etc.
                // The specific 'type' might correspond to licensing.
                // For assigning a *paid* license, it often involves updating the user or ensuring the account has available paid seats.

                // Example: Create a new licensed user (might vary based on ZoomNet version/API)
                // Zoom API's POST /users endpoint supports inviting users as 'licensed' or 'basic'
                var newUser = await _zoomClient.Users.CreateAsync(
                    email: email,
                    type: UserType.Licensed, 
                    firstName: firstName,
                    lastName: lastName
                );

                _logger.LogInformation($"Zoom user '{email}' invited/created with ID: {newUser.Id}. Invitation sent.");
                return newUser.Id;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to create or invite Zoom user '{email}'.");
                throw new Exception($"Failed to create or invite Zoom user '{email}'.", ex);
            }
        }

    }
}
