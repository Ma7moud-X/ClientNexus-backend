﻿using ClientNexus.Application.DTO;
using ClientNexus.Domain.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClientNexus.Application.Interfaces
{
    public interface IAppointmentService
    {
        Task<AppointmentDTO> GetByIdAsync(int id);
        Task<IEnumerable<AppointmentDTO>> GetByProviderIdAsync(int providerId, int offset, int limit);
        Task<IEnumerable<AppointmentDTO>> GetByClientIdAsync(int clientId, int offset, int limit);
        Task<AppointmentDTO> CreateAsync([FromBody] AppointmentCreateDTO appointmentDTO);
        //Task<AppointmentDTO> UpdateAsync(int id, AppointmentDTO appointmentDTO);
        Task<AppointmentDTO> UpdateStatusAsync(int id, ServiceStatus status, string? reason, string role);
        Task DeleteAsync(int id);

    }
}
