using ClientNexus.Application.DTO;
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
        Task<IEnumerable<SlotDTO>> GetAvailableSlotsAsync(int serviceProviderId, DateTime startDate, DateTime endDate);
        Task<int> CreateAsync([FromBody] SlotCreateDTO slotDTO);
        Task<SlotDTO> GetSlotByIdAsync(int id);

    }
}
