using AutoMapper;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Roles;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Services
{
    public class SlotService : ISlotService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly Lazy<IAppointmentService> _appointService;

        public SlotService(IUnitOfWork unitOfWork, IMapper mapper, Lazy<IAppointmentService> appointService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _appointService = appointService;
        }

        //pagination by date range for calendar display
        public async Task<IEnumerable<SlotDTO>> GetSlotsAsync(int serviceProviderId, DateTime startDate, DateTime endDate, SlotType type, SlotStatus? status)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date must be before end date.");

            var providerExists = await _unitOfWork.ServiceProviders.GetByIdAsync(serviceProviderId);
            if (providerExists == null)
                throw new KeyNotFoundException("Invalid Service Provider ID.");

            // Validate the passed enum values
            if (!Enum.IsDefined(type))
                throw new ArgumentOutOfRangeException($"Invalid SlotType value: {type}");

            status ??= SlotStatus.Available;

            if (!Enum.IsDefined(status.Value))
                throw new ArgumentOutOfRangeException($"Invalid SlotStatus value: {status}");

            if (startDate < DateTime.Now && status == SlotStatus.Available)
                startDate = DateTime.Now;  //don't retrieve past avaliable slots

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
            Slot? slot = await _unitOfWork.Slots.FirstOrDefaultAsync(s => s.Id == slotId);
            if (slot == null)
                throw new KeyNotFoundException("Invalid slot ID");

            return _mapper.Map<SlotDTO>(slot);
        }
        public async Task<SlotDTO> CreateAsync([FromBody] SlotCreateDTO slotDTO)
        {
            if (slotDTO == null)
                throw new ArgumentNullException("Slot data cannot be null");
            //check if foreign key is valid
            if (await _unitOfWork.ServiceProviders.GetByIdAsync(slotDTO.ServiceProviderId) == null)
                throw new KeyNotFoundException("Invalid Service Provider Id");
            //check if it is a valid date
            if (slotDTO.Date < DateTime.UtcNow)
                throw new ArgumentException("Slot date cannot be in the past");

            Slot slot = _mapper.Map<Slot>(slotDTO);
            slot.Status = SlotStatus.Available;
            var createdSlot = await _unitOfWork.Slots.AddAsync(slot);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<SlotDTO>(createdSlot);

        }

        public async Task<SlotDTO> Update(int id, [FromBody] SlotDTO slotDTO)
        {
            if (slotDTO == null || id != slotDTO.Id)
                throw new ArgumentNullException("Invalid Data");
            //check if foreign key is valid
            var existingSlot = await _unitOfWork.Slots.FirstOrDefaultAsync(s => s.Id == id);
            if (existingSlot == null)
                throw new KeyNotFoundException("Invalid slot ID");
            //check if updated foreign key is valid
            if (await _unitOfWork.ServiceProviders.GetByIdAsync(slotDTO.ServiceProviderId) == null)
                throw new KeyNotFoundException("Invalid Service Provider Id");

            Slot updatedSlot = _mapper.Map<Slot>(slotDTO);

            if (!Enum.IsDefined(updatedSlot.Status))
                throw new ArgumentOutOfRangeException($"Invalid SlotStatus value");

            updatedSlot = _unitOfWork.Slots.Update(existingSlot, updatedSlot);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<SlotDTO>(updatedSlot);

        }

        public async Task<SlotDTO> UpdateStatus(int id, SlotStatus status)
        {
            var slot = await _unitOfWork.Slots.GetByIdAsync(id);
            if (slot == null)
                throw new KeyNotFoundException("Invalid slot ID");

            if (!Enum.IsDefined(status))
                throw new ArgumentOutOfRangeException($"Invalid SlotStatus value: {status}");

            slot.Status = status;
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<SlotDTO>(slot);
        }

        public async Task DeleteAsync(int slotId, string role)
        {
            Slot? slot = await _unitOfWork.Slots.FirstOrDefaultAsync(s => s.Id == slotId);
            if (slot == null)
                throw new KeyNotFoundException("Invalid slot ID");

            // Only allow deletion of future slots
            if (slot.Date <= DateTime.Now)
                throw new InvalidOperationException("Cannot delete past or ongoing slots.");

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
                    // Cancel all appointments for this slot
                    var appointments = await _unitOfWork.Appointments.GetByConditionAsync(
                                                                a => a.SlotId == slotId && a.Status != ServiceStatus.Cancelled);
                    //var appointmentIds = appointments.Cast<int>().ToList();

                    foreach (var appoint in appointments)
                    {
                        appoint.Status = ServiceStatus.Cancelled;
                        appoint.CancellationReason = "Service Provider cancelled this slot";
                        appoint.CancellationTime = DateTime.Now;
                    }

                    //mark the slot as deleted
                    slot.Status = SlotStatus.Deleted;
                }
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Error occured while Creating the appointment");
            }
        }
    }
}
