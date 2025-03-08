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

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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
            var appointments = await _unitOfWork.Appointments.GetByConditionAsync(a => a.AppointmentProviderId == providerId, offset:offset, limit:limit);
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
        public async Task<AppointmentDTO> CreateAsync([FromBody] AppointmentCreateDTO appointmentDTO)
        {
            if (appointmentDTO == null)
                throw new ArgumentNullException("Appointment data cannot be null");
            //check if foreign key is valid
            if (await _unitOfWork.Slots.FirstOrDefaultAsync(s => s.Id == appointmentDTO.SlotId) == null)
                throw new ArgumentException("Invalid Slot Id");
            if (await _unitOfWork.Clients.GetByIdAsync(appointmentDTO.ClientId) == null)
                throw new ArgumentException("Invalid Client Id");
            if (await _unitOfWork.ServiceProviders.GetByIdAsync(appointmentDTO.AppointmentProviderId) == null)
                throw new ArgumentException("Invalid Provider Id");

            Appointment appoint = _mapper.Map<Appointment>(appointmentDTO);
            appoint.ServiceType = ServiceType.Appointment;

            var createdAppoint = await _unitOfWork.Appointments.AddAsync(appoint);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<AppointmentDTO>(createdAppoint);

        }
        public async Task<AppointmentDTO> UpdateAsync(int id, AppointmentDTO appointmentDTO)
        {
            if (appointmentDTO == null || id != appointmentDTO.Id)
                throw new ArgumentNullException("Invalid Data");

            Appointment? existingAppointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (existingAppointment == null)
                throw new KeyNotFoundException("Appointment not found");

            // Validate foreign keys
            if (await _unitOfWork.Slots.GetByIdAsync(appointmentDTO.SlotId) == null)
                throw new ArgumentException("Invalid Slot Id");
            if (await _unitOfWork.Clients.GetByIdAsync(appointmentDTO.ClientId) == null)
                throw new ArgumentException("Invalid Client Id");
            if (await _unitOfWork.ServiceProviders.GetByIdAsync(appointmentDTO.AppointmentProviderId) == null)
                throw new ArgumentException("Invalid Provider Id");

            Appointment updatedAppointment = _mapper.Map<Appointment>(appointmentDTO);

            updatedAppointment = _unitOfWork.Appointments.Update(existingAppointment, updatedAppointment);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<AppointmentDTO>(updatedAppointment);
        }

        //to add: Patch to update status
        public async Task DeleteAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException("Invalid Appointment ID");

            _unitOfWork.Appointments.Delete(appointment);
            await _unitOfWork.SaveChangesAsync();
        }

    }
}
