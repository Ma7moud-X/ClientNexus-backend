using AutoMapper;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Entities.Users;
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
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISlotService _slotService;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, ISlotService slotService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _slotService = slotService;
        }
        public async Task<AppointmentDTO> GetByIdAsync(int id)
        {
            Appointment? appoint = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appoint == null)
                throw new KeyNotFoundException("Invalid ID");
            return _mapper.Map<AppointmentDTO>(appoint);
        }
        public async Task<IEnumerable<AppointmentDTO>> GetByProviderIdAsync(int providerId, int offset, int limit)
        {
            // Validate input
            if (providerId <= 0)
                throw new ArgumentException("Invalid Provider ID");

            // Check if the client exists
            var providerExists = await _unitOfWork.ServiceProviders.GetByIdAsync(providerId);
            if (providerExists == null)
                throw new KeyNotFoundException("Provider not found");
            var appointments = await _unitOfWork.Appointments.GetByConditionAsync(a => a.Slot.ServiceProviderId == providerId, offset: offset, limit: limit, includes: new string[] {"Slot"});
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }
        public async Task<IEnumerable<AppointmentDTO>> GetByClientIdAsync(int clientId, int offset, int limit)
        {
            // Validate input
            if (clientId <= 0)
                throw new ArgumentException("Invalid Client ID");

            // Check if the client exists
            var clientExists = await _unitOfWork.Clients.GetByIdAsync(clientId);
            if (clientExists == null)
                throw new KeyNotFoundException("Client not found");

            // Fetch appointments
            var appointments = await _unitOfWork.Appointments.GetByConditionAsync(a => a.ClientId == clientId, offset: offset, limit: limit);
            /*
            if (!appointments.Any())
                throw new KeyNotFoundException("No appointments found for the given client");
            */
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }
        public async Task<AppointmentDTO> CreateAsync(AppointmentCreateDTO appointmentDTO)
        {
            if (appointmentDTO == null)
                throw new ArgumentNullException("Appointment data cannot be null");

            //for concurrency control
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                //check if foreign key is valid
                var slot = await _unitOfWork.Slots.GetByIdAsync(appointmentDTO.SlotId);
                if (slot == null || slot.Status != SlotStatus.Available)
                    throw new KeyNotFoundException("Slot is not Avaliable");
                if (slot.Status != SlotStatus.Available)
                    throw new ArgumentException("Slot not avaliable!");
                if (await _unitOfWork.Clients.GetByIdAsync(appointmentDTO.ClientId) == null)
                    throw new KeyNotFoundException("Invalid Client Id");
                if (await _unitOfWork.ServiceProviders.GetByIdAsync(slot.ServiceProviderId) == null)
                    throw new KeyNotFoundException("Invalid Provider Id");

                Appointment appoint = _mapper.Map<Appointment>(appointmentDTO);
                appoint.ServiceType = ServiceType.Appointment;
                appoint.Status = ServiceStatus.Pending;

                var createdAppoint = await _unitOfWork.Appointments.AddAsync(appoint);

                //update slot status to be booked
                await _slotService.UpdateStatus(slot.Id, SlotStatus.Booked);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return _mapper.Map<AppointmentDTO>(createdAppoint);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw;  //to preserve origonal error message and stack trace
            }

            
        }
        /*
         public async Task<AppointmentDTO> UpdateAsync(int id, AppointmentDTO appointmentDTO)
         {
             if (appointmentDTO == null || id != appointmentDTO.Id)
                 throw new ArgumentNullException("Invalid Data");

             Appointment? existingAppointment = await _unitOfWork.Appointments.GetByIdAsync(id);
             if (existingAppointment == null)
                 throw new KeyNotFoundException("Appointment not found");

             // Validate foreign keys
             var slot = await _unitOfWork.Slots.FirstOrDefaultAsync(s => s.Id == appointmentDTO.SlotId);
             if (slot == null)
                 throw new ArgumentException("Invalid Slot Id");
             if (await _unitOfWork.Clients.GetByIdAsync(appointmentDTO.ClientId) == null)
                 throw new ArgumentException("Invalid Client Id");
             if (await _unitOfWork.ServiceProviders.GetByIdAsync(slot.ServiceProviderId) == null)
                 throw new ArgumentException("Invalid Provider Id");

             Appointment updatedAppointment = _mapper.Map<Appointment>(appointmentDTO);

             updatedAppointment = _unitOfWork.Appointments.Update(existingAppointment, updatedAppointment);
             await _unitOfWork.SaveChangesAsync();

             return _mapper.Map<AppointmentDTO>(updatedAppointment);
         }
        */
        public async Task<AppointmentDTO> UpdateStatusAsync(int id, ServiceStatus status, string? reason)
        {
            Appointment? existingAppointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (existingAppointment == null)
                throw new KeyNotFoundException("Appointment not found");

            // Prevent invalid transitions
            if (existingAppointment.Status == ServiceStatus.Done || existingAppointment.Status == ServiceStatus.Cancelled)
                throw new InvalidOperationException("Cannot change status of a completed or cancelled appointment.");


            existingAppointment.Status = status;

            if(status == ServiceStatus.InProgress)
            {
                existingAppointment.CheckInTime = DateTime.Now;
            }
            if (status == ServiceStatus.Done)
            {
                existingAppointment.CompletionTime = DateTime.Now;
            }
            //return slot status to avaliable in case of cancelling
            if (status == ServiceStatus.Cancelled)
            {
                await _slotService.UpdateStatus(existingAppointment.SlotId, SlotStatus.Available);
                existingAppointment.CancellationReason = reason;
                existingAppointment.CancellationTime = DateTime.Now;
            }
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<AppointmentDTO>(existingAppointment);
        }


        public async Task DeleteAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException("Invalid Appointment ID");

            _unitOfWork.Appointments.Delete(appointment);
            await _unitOfWork.SaveChangesAsync();
        }

        //for notifications
        public async Task<bool> MarkReminderSentAsync(int appointmentId)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            appointment.ReminderSent = true;
            appointment.ReminderSentTime = DateTime.Now;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

    }
}
