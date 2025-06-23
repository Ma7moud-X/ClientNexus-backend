using ClientNexus.Application.DTO;
using ClientNexus.Domain.Entities.Services;


namespace ClientNexus.Application.Interfaces
{
    public interface IGoogleMeetService
    {
        Task<MeetingDetails> CreateMeetingAsync(Appointment appointment, Slot slot);
        Task<string> UpdateMeetingAsync(Appointment appointment, Slot slot);
        Task<bool> DeleteMeetingAsync(string meetingId);

    }
}
