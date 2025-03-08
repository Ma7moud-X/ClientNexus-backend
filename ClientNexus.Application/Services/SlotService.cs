using AutoMapper;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
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

        public SlotService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //pagination by date range for calendar display
        public async Task<IEnumerable<SlotDTO>> GetAvailableSlotsAsync(int serviceProviderId, DateTime startDate, DateTime endDate)
        {
            if (startDate > endDate)
                throw new ArgumentException("Start date must be before end date.");
            //no past slots
            /*
            if (startDate < DateTime.Now)
                throw new ArgumentException("Start date cannot be in the past");
            */
            var providerExists = await _unitOfWork.ServiceProviders.GetByIdAsync(serviceProviderId);
            if (providerExists == null)
                throw new ArgumentException("Invalid Service Provider ID.");

            IEnumerable<Slot> slots = await _unitOfWork.Slots.GetByConditionAsync(s => 
                s.ServiceProviderId == serviceProviderId && 
                s.Status == SlotStatus.Available && 
                s.Date >= startDate && 
                s.Date <= endDate);

            return _mapper.Map<List<SlotDTO>>(slots);
        }
        public async Task<SlotDTO> GetSlotByIdAsync (int slotId)
        {
            Slot? slot = await _unitOfWork.Slots.FirstOrDefaultAsync(s => s.Id == slotId); 
            if (slot == null)
            {
                throw new KeyNotFoundException("Invalid ID");
            }
            return _mapper.Map<SlotDTO>(slot);
        }
        public async Task<SlotDTO> CreateAsync([FromBody] SlotCreateDTO slotDTO)
        {
            if (slotDTO == null)
            {
                throw new ArgumentNullException("Slot data cannot be null");
            }
            //check if foreign key is valid
            if(await _unitOfWork.ServiceProviders.GetByIdAsync(slotDTO.ServiceProviderId) == null)
            {
                throw new ArgumentException("Invalid Service Provider Id");
            }
            //check if it is a valid date
            if (slotDTO.Date < DateTime.UtcNow)
            {
                throw new ArgumentException("Slot date cannot be in the past");
            }
            Slot slot = _mapper.Map<Slot>(slotDTO);
            slot.Status = SlotStatus.Available;
            var createdSlot = await _unitOfWork.Slots.AddAsync(slot);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<SlotDTO>(createdSlot);
            
        }

        public async Task<SlotDTO> Update(int id, [FromBody] SlotDTO slotDTO)
        {
            if (slotDTO == null || id!= slotDTO.Id)
                throw new ArgumentNullException("Invalid Data");
            //check if foreign key is valid
            var existingSlot = await _unitOfWork.Slots.FirstOrDefaultAsync(s => s.Id == id);
            if (existingSlot == null)
                throw new KeyNotFoundException("Slot not found.");
            //check if updated foreign key is valid
            if (await _unitOfWork.ServiceProviders.GetByIdAsync(slotDTO.ServiceProviderId) == null)
                 throw new ArgumentException("Invalid Service Provider Id");
            //check if updated date is a valid date
            if (slotDTO.Date < DateTime.UtcNow)
                throw new ArgumentException("Slot date cannot be in the past");

            Slot updatedSlot = _mapper.Map<Slot>(slotDTO);
            updatedSlot = _unitOfWork.Slots.Update(existingSlot, updatedSlot);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<SlotDTO>(updatedSlot);

        }

        public async Task DeleteAsync (int slotId)
        {
            Slot? slot = await _unitOfWork.Slots.FirstOrDefaultAsync(s => s.Id == slotId);
            if (slot == null)
            {
                throw new KeyNotFoundException("Invalid slot ID");
            }
            _unitOfWork.Slots.Delete(slot);
            await _unitOfWork.SaveChangesAsync();

        }
    }
}
