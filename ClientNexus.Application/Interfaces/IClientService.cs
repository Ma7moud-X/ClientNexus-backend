using ClientNexus.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface IClientService
    {
        public Task UpdateClientAsync(int ClientId, UpdateClientDTO updateDto);
        public  Task<ClientResponseDTO> GetByIdAsync(int clientId);


    }
}
