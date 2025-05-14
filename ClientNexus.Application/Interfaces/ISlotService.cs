using ClientNexus.Application.DTO;
using ClientNexus.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace ClientNexus.Application.Interfaces
{
    public interface ISlotService
    {
        Task<IEnumerable<SlotDTO>> GetSlotsAsync(int serviceProviderId, DateTime startDate, DateTime endDate, SlotType type, SlotStatus status);
        Task<SlotDTO> GetSlotByIdAsync(int id);
        Task<SlotDTO> CreateAsync([FromBody] SlotCreateDTO slotDTO, int serviceProviderId);
        Task<IEnumerable<SlotDTO>> GenerateSlotsAsync(int serviceProviderId, DateTime startDate, DateTime endDate);
        Task UpdateDateAsync(int slotId, DateTime date, int serviceProviderId);
        Task UpdateTypeAsync(int slotId, SlotType type, int serviceProviderId);
        Task UpdateStatusAsync(int id, SlotStatus status, int serviceProviderId);
        Task DeleteAsync(int slotId, int userId, UserType role);
    }
}
