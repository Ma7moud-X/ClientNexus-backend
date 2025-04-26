using AutoMapper;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;


namespace ClientNexus.Application.Services
{
    public class SlotService : ISlotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IAppointmentService _appointService;
        private readonly IPushNotification _pushNotification;

        public SlotService(IUnitOfWork unitOfWork, IMapper mapper, IAppointmentService appointService, IPushNotification pushNotification)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appointService = appointService;
            _pushNotification = pushNotification;
        }

        //pagination by date range for calendar display
        public async Task<IEnumerable<SlotDTO>> GetSlotsAsync(int serviceProviderId, DateTime startDate, DateTime endDate, SlotType type, SlotStatus status)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date must be before end date.");

            if (!await _unitOfWork.ServiceProviders.CheckAnyExistsAsync(p => p.Id == serviceProviderId))
                throw new KeyNotFoundException("Invalid Service Provider Id");

            // Validate the passed enum values
            if (!Enum.IsDefined(type))
                throw new ArgumentOutOfRangeException($"Invalid SlotType value: {type}");

            if (!Enum.IsDefined(status))
                throw new ArgumentOutOfRangeException($"Invalid SlotStatus value: {status}");

            if (startDate < DateTime.UtcNow && status == SlotStatus.Available)
                startDate = DateTime.UtcNow;  //don't retrieve past avaliable slots

            IEnumerable<Slot> slots = await _unitOfWork.Slots.GetByConditionAsync(s =>
                s.ServiceProviderId == serviceProviderId &&
                s.Date >= startDate &&
                s.Date <= endDate &&
                s.SlotType == type &&
                s.Status == status);

            return _mapper.Map<List<SlotDTO>>(slots);
        }
        public async Task<SlotDTO> GetSlotByIdAsync(int slotId)
        {
            Slot? slot = await _unitOfWork.Slots.GetByIdAsync(slotId);
            if (slot == null)
                throw new KeyNotFoundException("Invalid slot ID");

            return _mapper.Map<SlotDTO>(slot);
        }
        public async Task<SlotDTO> CreateAsync(SlotCreateDTO slotDTO, int serviceProviderId)
        {
            if (slotDTO == null)
                throw new ArgumentNullException("Slot data cannot be null");
            //check if foreign key is valid
            if (!await _unitOfWork.ServiceProviders.CheckAnyExistsAsync(p => p.Id == serviceProviderId))
                throw new KeyNotFoundException("Invalid Service Provider Id");
            //check if it is a valid date
            if (slotDTO.Date < DateTime.UtcNow)
                throw new ArgumentException("Slot date cannot be in the past");

            Slot slot = _mapper.Map<Slot>(slotDTO);
            slot.Status = SlotStatus.Available;
            slot.ServiceProviderId = serviceProviderId;
            var createdSlot = await _unitOfWork.Slots.AddAsync(slot);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<SlotDTO>(createdSlot);


        }

        public async Task UpdateDateAsync(int slotId, DateTime date, int serviceProviderId)
        {
            var slot = await _unitOfWork.Slots.GetByIdAsync(slotId);
            if (slot == null)
                throw new KeyNotFoundException("Invalid slot ID");

            if (slot.ServiceProviderId != serviceProviderId)
                throw new UnauthorizedAccessException("Cannot modify other service providers slots!");

            if (date < DateTime.UtcNow)
                throw new ArgumentException("Cannot modify a slot in the past.");

            if (slot.Status == SlotStatus.Deleted || slot.Status == SlotStatus.Booked)
                throw new InvalidOperationException("Cannot change date of a deleted or booked slot.");

            slot.Date = date;
            await _unitOfWork.SaveChangesAsync();

        }
        public async Task UpdateTypeAsync(int slotId, SlotType type, int serviceProviderId)
        {

            var slot = await _unitOfWork.Slots.GetByIdAsync(slotId);
            if (slot == null)
                throw new KeyNotFoundException("Invalid slot ID");

            if (!Enum.IsDefined(type))
                throw new ArgumentOutOfRangeException($"Invalid SlotType value: {type}");

            if (slot.ServiceProviderId != serviceProviderId)
                throw new UnauthorizedAccessException("Cannot modify other service providers slots!");

            if (slot.Status == SlotStatus.Deleted || slot.Status == SlotStatus.Booked)
                throw new InvalidOperationException("Cannot change type of a deleted or booked slot.");

            slot.SlotType = type;
            await _unitOfWork.SaveChangesAsync();

        }

        public async Task UpdateStatusAsync(int slotId, SlotStatus status, int serviceProviderId)
        {
            if (status == SlotStatus.Deleted)
            {
                await DeleteAsync(slotId, serviceProviderId, UserType.ServiceProvider);
            }

            else
            {
                var slot = await _unitOfWork.Slots.GetByIdAsync(slotId);
                if (slot == null)
                    throw new KeyNotFoundException("Invalid slot ID");

                if (!Enum.IsDefined(status))
                    throw new ArgumentOutOfRangeException($"Invalid SlotStatus value: {status}");

                if (slot.ServiceProviderId != serviceProviderId)
                    throw new UnauthorizedAccessException("Cannot modify other service providers slots!");

                slot.Status = status;
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int slotId, int userId, UserType role)
        {
            Slot? slot = await _unitOfWork.Slots.GetByIdAsync(slotId);
            if (slot == null)
                throw new KeyNotFoundException("Invalid slot ID");

            // Only allow deletion of future slots
            if (slot.Date <= DateTime.UtcNow)
                throw new InvalidOperationException("Cannot delete past or ongoing slots.");
            if (role == UserType.ServiceProvider && slot.ServiceProviderId != userId)
                throw new UnauthorizedAccessException("Cannot delete other service providers slots!");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                if (slot.Status == SlotStatus.Available)
                {
                    //delete from the DB
                    _unitOfWork.Slots.Delete(slot);
                }
                else if (slot.Status == SlotStatus.Pending || slot.Status == SlotStatus.Booked)
                {

                    // Cancel appointment at this slot
                    var appointments = await _unitOfWork.Appointments.GetByConditionAsync(
                                                                a => a.SlotId == slotId && a.Status != ServiceStatus.Cancelled);
                    //var appointmentIds = appointments.Cast<int>().ToList();
                    var appoint = appointments.FirstOrDefault();
                    if (appoint is not null)
                    {
                        await _appointService.HandleCancelledStatusAsync(appoint, slot, UserType.ServiceProvider, "Provider cancelled this slot");
                        _unitOfWork.Appointments.Update(appoint);
                    }
                }
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Error occured while deleting the slot, " + ex.Message, ex);
            }
        }
    }
}
