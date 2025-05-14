using AutoMapper;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Others;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services
{
    public class AvailableDayService : IAvailableDayService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public AvailableDayService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<AvailableDayDTO> CreateAsync(AvailableDayCreateDTO dto, int serviceProviderId)
        {
            if (dto == null)
                throw new ArgumentNullException("Available day data cannot be null");

            if (!await _unitOfWork.ServiceProviders.CheckAnyExistsAsync(p => p.Id == serviceProviderId))
                throw new KeyNotFoundException("Invalid Service Provider Id");

            if (dto.StartTime >= dto.EndTime)
                throw new ArgumentException("Start time must be before end time");

            if (dto.SlotDuration.TotalMinutes <= 0)
                throw new ArgumentException("Slot duration must be positive");
             bool dayExists = await _unitOfWork.AvailableDays.CheckAnyExistsAsync(ad =>
                                    ad.ServiceProviderId == serviceProviderId &&
                                    ad.DayOfWeek == dto.DayOfWeek &&
                                    (
                                        // New start is before existing end AND new end is after existing start
                                        dto.StartTime < ad.EndTime &&
                                        dto.EndTime > ad.StartTime
                                    )
                                    );

            if (dayExists)
                throw new InvalidOperationException($"An overlapping availability already exists on {dto.DayOfWeek}");

            AvailableDay availableDay = _mapper.Map<AvailableDay>(dto);
            availableDay.ServiceProviderId = serviceProviderId;

            var createdAvailableDay = await _unitOfWork.AvailableDays.AddAsync(availableDay);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AvailableDayDTO>(createdAvailableDay);

        }
        public async Task<IEnumerable<AvailableDayDTO>> GetByServiceProviderAsync(int serviceProviderId)
        {
            if (!await _unitOfWork.ServiceProviders.CheckAnyExistsAsync(p => p.Id == serviceProviderId))
                throw new KeyNotFoundException("Invalid Service Provider Id");

            var availableDays = await _unitOfWork.AvailableDays.GetByConditionAsync(ad =>
                ad.ServiceProviderId == serviceProviderId);

            return _mapper.Map<List<AvailableDayDTO>>(availableDays);
        }

        public async Task<AvailableDayDTO> GetByIdAsync(int availableDayId)
        {
            var availableDay = await _unitOfWork.AvailableDays.GetByIdAsync(availableDayId);
            if (availableDay == null)
                throw new KeyNotFoundException("Available day not found");

            return _mapper.Map<AvailableDayDTO>(availableDay);
        }

        public async Task UpdateAsync(int availableDayId, int serviceProviderId, AvailableDayUpdateDTO dto)
        {
            var availableDay = await _unitOfWork.AvailableDays.GetByIdAsync(availableDayId);
            if (availableDay == null)
                throw new KeyNotFoundException("Available day not found");

            if (availableDay.ServiceProviderId != serviceProviderId)
                throw new UnauthorizedAccessException("Cannot modify other service providers' available days");

            if (dto.StartTime >= dto.EndTime)
                throw new ArgumentException("Start time must be before end time");

            if (dto.SlotDuration.TotalMinutes <= 0)
                throw new ArgumentException("Slot duration must be positive");

            // Check if future slots exist for this available day using the direct foreign key relationship
            bool futureSlotExists = await _unitOfWork.Slots.CheckAnyExistsAsync(s =>
                s.AvailableDayId == availableDayId &&
                s.Date >= DateTime.UtcNow);

            // If slots exist for this pattern and LastGenerationEndDate is in the future
            if (futureSlotExists && availableDay.LastGenerationEndDate.HasValue && availableDay.LastGenerationEndDate > DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException(
                    $"Cannot update availability pattern as slots have been generated. Please wait until after {availableDay.LastGenerationEndDate.Value.ToShortDateString()} before updating.");
            }

            // Update the available day
            _mapper.Map(dto, availableDay);
            _unitOfWork.AvailableDays.Update(availableDay);
            await _unitOfWork.SaveChangesAsync();
        }
        public async Task DeleteAsync(int availableDayId, int serviceProviderId)
        {
            var availableDay = await _unitOfWork.AvailableDays.GetByIdAsync(availableDayId);
            if (availableDay == null)
                throw new KeyNotFoundException("Available day not found");

            if (availableDay.ServiceProviderId != serviceProviderId)
                throw new UnauthorizedAccessException("Cannot delete other service providers' available days");

            // Check if future slots exist for this available day using the direct foreign key relationship
            bool futureSlotExists = await _unitOfWork.Slots.CheckAnyExistsAsync(s =>
                s.AvailableDayId == availableDayId &&
                s.Date >= DateTime.UtcNow);

            // If slots exist for this pattern and LastGenerationEndDate is in the future
            if (futureSlotExists && availableDay.LastGenerationEndDate.HasValue && availableDay.LastGenerationEndDate > DateTime.UtcNow.Date)
            {
                throw new InvalidOperationException(
                    $"Cannot delete availability pattern as slots have been generated. Please wait until after {availableDay.LastGenerationEndDate.Value.ToShortDateString()} before deleting.");
            }

            _unitOfWork.AvailableDays.Delete(availableDay);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}
