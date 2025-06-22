using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ClientNexus.Domain.Entities.Services;
using Google.Apis.Auth.OAuth2.Flows;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.DTO;

namespace Infrastructure.Services
{
    public class GoogleMeetService : IGoogleMeetService
    {
        private readonly CalendarService _calendarService;
        private readonly ILogger<GoogleMeetService> _logger;
        private readonly string _hostEmail;

        public GoogleMeetService(IConfiguration configuration, ILogger<GoogleMeetService> logger)
        {
            _logger = logger;
            _hostEmail = Environment.GetEnvironmentVariable("GOOGLE_MEET_HOST_EMAIL")
                ?? throw new InvalidOperationException("GOOGLE_MEET_HOST_EMAIL environment variable is required.");

            var credential = CreateCredential();
            _calendarService = new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = Environment.GetEnvironmentVariable("GOOGLE_MEET_APP_NAME") ?? "Meeting Scheduler"
            });
        }

        private UserCredential CreateCredential()
        {
            var clientId = Environment.GetEnvironmentVariable("GOOGLE_MEET_CLIENT_ID");
            var clientSecret = Environment.GetEnvironmentVariable("GOOGLE_MEET_CLIENT_SECRET");
            var refreshToken = Environment.GetEnvironmentVariable("GOOGLE_MEET_REFRESH_TOKEN");

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret) || string.IsNullOrEmpty(refreshToken))
            {
                throw new InvalidOperationException("Google Meet credentials environment variables are not properly configured. Required: GOOGLE_MEET_CLIENT_ID, GOOGLE_MEET_CLIENT_SECRET, GOOGLE_MEET_REFRESH_TOKEN");
            }

            // Create the authorization flow
            var flow = new GoogleAuthorizationCodeFlow(
                new GoogleAuthorizationCodeFlow.Initializer
                {
                    ClientSecrets = new ClientSecrets
                    {
                        ClientId = clientId,
                        ClientSecret = clientSecret
                    },
                    Scopes = new[] { CalendarService.Scope.Calendar }
                }
            );

            // Create token response with refresh token
            var tokenResponse = new Google.Apis.Auth.OAuth2.Responses.TokenResponse
            {
                RefreshToken = refreshToken
            };

            // Create user credential
            var userCredential = new UserCredential(flow, "user", tokenResponse);

            return userCredential;
        }
        /*
        public async Task<string> CreateMeetingAsync(Appointment appointment, Slot slot)
        {
            try
            {
                var calendarEvent = new Event
                {
                    Summary = appointment.Name ?? "Video Appointment",
                    Description = appointment.Description ?? "Scheduled video appointment",
                    Start = new EventDateTime
                    {
                        DateTimeDateTimeOffset = new DateTimeOffset(slot.Date, TimeSpan.FromHours(3)),
                        TimeZone = "Africa/Cairo"
                    },
                    End = new EventDateTime
                    {
                        DateTimeDateTimeOffset = new DateTimeOffset(slot.Date + slot.SlotDuration, TimeSpan.FromHours(3)),
                        TimeZone = "Africa/Cairo"
                    },
                    ConferenceData = new ConferenceData
                    {
                        CreateRequest = new CreateConferenceRequest
                        {
                            RequestId = Guid.NewGuid().ToString(),
                            ConferenceSolutionKey = new ConferenceSolutionKey
                            {
                                Type = "hangoutsMeet"
                            }
                        }
                    },
                    Attendees = new List<EventAttendee>
                    {
                        new EventAttendee
                        {
                            Email = appointment.Client?.Email,
                            ResponseStatus = "needsAction"
                        }
                    }
                };

                var request = _calendarService.Events.Insert(calendarEvent, "primary");
                request.ConferenceDataVersion = 1;
                request.SendUpdates = EventsResource.InsertRequest.SendUpdatesEnum.All;

                var createdEvent = await request.ExecuteAsync();
                var meetingLink = createdEvent.ConferenceData?.EntryPoints?.FirstOrDefault(ep => ep.EntryPointType == "video")?.Uri;

                // Update the appointment with the meeting link and Google Calendar event ID
                appointment.MeetingLink = meetingLink;
                appointment.GoogleCalendarEventId = createdEvent.Id;

                return meetingLink;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating Google Meet for appointment ID: {AppointmentId}", appointment.Id);
                throw;
            }
        }*/
        public async Task<MeetingDetails> CreateMeetingAsync(Appointment appointment, Slot slot)
        {
            try
            {
                // Create the Google Calendar event
                var eventRequest = new Event
                {
                    Summary = $"Online Appointment Meeting",
                    Description = $"Online appointment scheduled via the platform.",
                    Start = new EventDateTime
                    {
                        DateTimeDateTimeOffset = new DateTimeOffset(slot.Date, TimeSpan.FromHours(2)), // Egypt timezone UTC+2
                        TimeZone = "Africa/Cairo"
                    },
                    End = new EventDateTime
                    {
                        DateTimeDateTimeOffset = new DateTimeOffset(slot.Date + slot.SlotDuration, TimeSpan.FromHours(2)),
                        TimeZone = "Africa/Cairo"
                    },
                    // Set the host as the organizer (no attendees needed for guests)
                    Organizer = new Event.OrganizerData
                    {
                        Email = Environment.GetEnvironmentVariable("GOOGLE_MEET_HOST_EMAIL"), // Your host email
                        DisplayName = "Meeting Host"
                    },
                    // Enable Google Meet
                    ConferenceData = new ConferenceData
                    {
                        CreateRequest = new CreateConferenceRequest
                        {
                            RequestId = Guid.NewGuid().ToString(),
                            ConferenceSolutionKey = new ConferenceSolutionKey
                            {
                                Type = "hangoutsMeet"
                            }
                        }
                    },
                    // Make it visible to guests who have the link
                    Visibility = "public",
                    // Allow guests to join
                    GuestsCanModify = true,
                    //AnyoneCanAddSelf = true,
                    GuestsCanInviteOthers = false,
                    GuestsCanSeeOtherGuests = true,


                };

                // Insert the event
                var insertRequest = _calendarService.Events.Insert(eventRequest, Environment.GetEnvironmentVariable("GOOGLE_CALENDAR_ID"));
                insertRequest.ConferenceDataVersion = 1; // Required for Google Meet
                insertRequest.SendUpdates = EventsResource.InsertRequest.SendUpdatesEnum.None; // Don't send emails since no attendees

                var createdEvent = await insertRequest.ExecuteAsync();

                // Extract the Google Meet link
                string meetingLink = createdEvent.ConferenceData?.EntryPoints?
                    .FirstOrDefault(ep => ep.EntryPointType == "video")?.Uri;

                if (string.IsNullOrEmpty(meetingLink))
                {
                    throw new InvalidOperationException("Failed to create Google Meet link");
                }

                return new MeetingDetails
                {
                    MeetingLink = meetingLink,
                    GoogleCalendarEventId = createdEvent.Id
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"The service calendar has thrown an exception. {ex.Message}", ex);
            }
        }

        public async Task<string> UpdateMeetingAsync(Appointment appointment, Slot slot)
        {
            try
            {
                if (string.IsNullOrEmpty(appointment.GoogleCalendarEventId))
                {
                    throw new InvalidOperationException("Cannot update meeting: GoogleCalendarEventId is missing");
                }

                var existingEvent = await _calendarService.Events.Get("primary", appointment.GoogleCalendarEventId).ExecuteAsync();

                existingEvent.Summary = appointment.Name ?? "Video Appointment";
                existingEvent.Description = appointment.Description ?? "Scheduled video appointment";
                existingEvent.Start = new EventDateTime
                {
                    DateTimeDateTimeOffset = new DateTimeOffset(slot.Date, TimeSpan.FromHours(3)),
                    TimeZone = "Africa/Cairo"
                };
                existingEvent.End = new EventDateTime
                {
                    DateTimeDateTimeOffset = new DateTimeOffset(slot.Date + slot.SlotDuration, TimeSpan.FromHours(3)),
                    TimeZone = "Africa/Cairo"
                };
                existingEvent.Attendees = new List<EventAttendee>
                {
                    new EventAttendee
                    {
                        Email = appointment.Client?.Email,
                        ResponseStatus = "needsAction"
                    }
                };

                var request = _calendarService.Events.Update(existingEvent, "primary", appointment.GoogleCalendarEventId);
                request.SendUpdates = EventsResource.UpdateRequest.SendUpdatesEnum.All;

                var updatedEvent = await request.ExecuteAsync();
                var meetingLink = updatedEvent.ConferenceData?.EntryPoints?.FirstOrDefault(ep => ep.EntryPointType == "video")?.Uri;

                appointment.MeetingLink = meetingLink;
                return meetingLink;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating Google Meet for appointment ID: {AppointmentId}", appointment.Id);
                throw;
            }
        }

        public async Task<bool> DeleteMeetingAsync(string googleCalendarEventId)
        {
            try
            {
                if (string.IsNullOrEmpty(googleCalendarEventId))
                {
                    return false;
                }

                var request = _calendarService.Events.Delete("primary", googleCalendarEventId);
                request.SendUpdates = EventsResource.DeleteRequest.SendUpdatesEnum.All;
                await request.ExecuteAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting Google Meet with calendar event ID: {EventId}", googleCalendarEventId);
                return false;
            }
        }
    }
}