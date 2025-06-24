using ClientNexus.Application.DTO;

namespace ClientNexus.Application.Interfaces
{
    public interface IZoomService
    {
        Task<ZoomMeetingDetailsDTO> CreateMeetingAsync(string topic, DateTime startTime, int durationMinutes);
        Task DeleteMeetingAsync(long zoomMeetingId);
        //Task<ZoomMeetingResponse?> GetMeetingAsync(string meetingId);
        //Task<ZoomMeetingResponse> UpdateMeetingAsync(string meetingId, ZoomMeetingRequest request);
    }
}
