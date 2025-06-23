using ClientNexus.Application.DTO;
using ClientNexus.Domain.Entities.Services;
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
        Task<IEnumerable<AppointmentDTO3>> GetByProviderIdAsync(int providerId, int offset, int limit);
        Task<IEnumerable<AppointmentDTO2>> GetByClientIdAsync(int clientId, int offset, int limit);
        Task<AppointmentDTO> CreateAsync(int clientId, AppointmentCreateDTO appointmentDTO);
        //Task<AppointmentDTO> UpdateAsync(int id, AppointmentDTO appointmentDTO);
        Task UpdateStatusAsync(int id, ServiceStatus status, int userId, UserType role, string? reason);
        Task DeleteAsync(int id);
        Task SendAppointmentReminderAsync(int targetHours = 24, int rangeMinutes = 25);
        Task HandleCancelledStatusAsync(Appointment appointment, Slot slot, UserType role, string? cancellationReason);

    }
}
