using ClientNexus.Domain.Enums;

namespace ClientNexus.Application.DTO
{
    public class AvailableDayDTO
    {
        public int Id { get; set; }
        public int ServiceProviderId { get; set; }
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan SlotDuration { get; set; }
        public SlotType SlotType { get; set; }
        public DateTime? LastGenerationEndDate { get; set; }
    }
    public class AvailableDayCreateDTO
    {
        public DayOfWeek DayOfWeek { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan SlotDuration { get; set; }
        public SlotType SlotType { get; set; }
    }
    public class AvailableDayUpdateDTO
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public TimeSpan SlotDuration { get; set; }
        public SlotType SlotType { get; set; }
    }
}
