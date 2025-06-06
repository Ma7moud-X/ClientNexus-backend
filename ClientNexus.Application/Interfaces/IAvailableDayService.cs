using ClientNexus.Application.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface IAvailableDayService
    {
        Task<AvailableDayDTO> CreateAsync(AvailableDayCreateDTO dto, int serviceProviderId);
        Task<IEnumerable<AvailableDayDTO>> GetByServiceProviderAsync(int serviceProviderId);
        Task<AvailableDayDTO> GetByIdAsync(int availableDayId);
        Task UpdateAsync(int availableDayId, int serviceProviderId, AvailableDayUpdateDTO dto);
        Task DeleteAsync(int availableDayId, int serviceProviderId);
    }
}
