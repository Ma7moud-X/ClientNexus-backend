using ClientNexus.Application.DTO;
using ClientNexus.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface ISlotService
    {
        Task<IEnumerable<SlotDTO>> GetSlotsAsync(int serviceProviderId, DateTime startDate, DateTime endDate, SlotType type, SlotStatus? status);
        Task<SlotDTO> CreateAsync([FromBody] SlotCreateDTO slotDTO);
        Task<SlotDTO> GetSlotByIdAsync(int id);
        Task<SlotDTO> Update(int id, [FromBody] SlotDTO slotDTO);
        Task<SlotDTO> UpdateStatus(int id, SlotStatus status);
        Task DeleteAsync(int slotId);
    }
}
