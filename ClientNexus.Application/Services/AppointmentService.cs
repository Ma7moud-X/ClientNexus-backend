using AutoMapper;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Models;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace ClientNexus.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ISlotService _slotService;
        private readonly IPushNotification _pushNotification;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, ISlotService slotService, IPushNotification pushNotification, ILogger<AppointmentService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _slotService = slotService;
            _pushNotification = pushNotification;
            _logger = logger;
        }
        public async Task<AppointmentDTO> GetByIdAsync(int id)
        {
            Appointment? appoint = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appoint == null)
                throw new KeyNotFoundException("Invalid Appointment ID");
            return _mapper.Map<AppointmentDTO>(appoint);
        }
        public async Task<IEnumerable<AppointmentDTO>> GetByProviderIdAsync(int providerId, int offset, int limit)
        {
            // Check if the provider exists
            if (!await _unitOfWork.ServiceProviders.CheckAnyExistsAsync(p => p.Id == providerId))
                throw new KeyNotFoundException("Invalid Service Provider Id");

            var appointments = await _unitOfWork.Appointments.GetByConditionAsync(a => a.Slot.ServiceProviderId == providerId, offset: offset, limit: limit); //, includes: new string[] { "Slot" }
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }
        public async Task<IEnumerable<AppointmentDTO>> GetByClientIdAsync(int clientId, int offset, int limit)
        {
            // Check if the client exists
            if (!await _unitOfWork.Clients.CheckAnyExistsAsync(c => c.Id == clientId))
                throw new KeyNotFoundException("Invalid Client Id");


            // Fetch appointments
            var appointments = await _unitOfWork.Appointments.GetByConditionAsync(a => a.ClientId == clientId, offset: offset, limit: limit);
            /*
            if (!appointments.Any())
                throw new KeyNotFoundException("No appointments found for the given client");
            */
            return _mapper.Map<IEnumerable<AppointmentDTO>>(appointments);
        }
        public async Task<AppointmentDTO> CreateAsync(int clientId, AppointmentCreateDTO appointmentDTO)
        {
            if (appointmentDTO == null)
                throw new ArgumentNullException("Appointment data cannot be null");

            //for concurrency control
            await _unitOfWork.BeginTransactionAsync();
            try
            {
                //check if foreign key is valid

                var slot = await _unitOfWork.Slots.GetByIdAsync(appointmentDTO.SlotId);
                if (slot == null)
                    throw new KeyNotFoundException("Invalid Slot Id");
                if (slot.Status != SlotStatus.Available || slot.Date < DateTime.UtcNow)
                    throw new InvalidOperationException("Slot not avaliable!");
                if (!await _unitOfWork.Clients.CheckAnyExistsAsync(c => c.Id == clientId))
                    throw new KeyNotFoundException("Invalid Client Id");
                if (!await _unitOfWork.ServiceProviders.CheckAnyExistsAsync(p => p.Id == slot.ServiceProviderId))
                    throw new KeyNotFoundException("Invalid Service Provider Id");

                if (await HasConflictAsync(clientId, slot.Date))
                    throw new InvalidOperationException("Client already has an appointment at this time.");

                Appointment appoint = _mapper.Map<Appointment>(appointmentDTO);
                appoint.ServiceType = ServiceType.Appointment;
                appoint.Status = ServiceStatus.Pending;
                appoint.ClientId = clientId;
                appoint.ServiceProviderId = slot.ServiceProviderId;

                var createdAppoint = await _unitOfWork.Appointments.AddAsync(appoint);

                //update slot status to be booked
                await _slotService.UpdateStatus(slot.Id, SlotStatus.Booked, slot.ServiceProviderId);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return _mapper.Map<AppointmentDTO>(createdAppoint);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Error occured while Creating the appointment" + ex.Message, ex);
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
        public async Task<AppointmentDTO> UpdateStatusAsync(int id, ServiceStatus status, int userId, UserType role, string? cancellationReason)
        {
            Appointment? existingAppointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (existingAppointment == null)
                throw new KeyNotFoundException("Appointment not found");

            // Prevent invalid transitions
            if (existingAppointment.Status == ServiceStatus.Done || existingAppointment.Status == ServiceStatus.Cancelled)
                throw new InvalidOperationException("Cannot change status of a completed or cancelled appointment.");

            if (!Enum.IsDefined(status))
                throw new ArgumentOutOfRangeException($"Invalid Appointment Status value: {status}");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var slot = await _unitOfWork.Slots.GetByIdAsync(existingAppointment.SlotId);

                existingAppointment.Status = status;

                if (status == ServiceStatus.InProgress)
                {
                    if (slot?.Date < DateTime.UtcNow)
                        existingAppointment.CheckInTime = DateTime.UtcNow;
                    else
                        throw new InvalidOperationException("Cannot mark appointment as In Progress before its slot time!");
                }
                if (status == ServiceStatus.Done)
                {
                    if (slot?.Date < DateTime.UtcNow)
                        existingAppointment.CompletionTime = DateTime.UtcNow;
                    else
                        throw new InvalidOperationException("Cannot mark appointment as Done before its slot time!");
                }
                //return slot status to avaliable in case of cancelling
                if (status == ServiceStatus.Cancelled)
                {
                    existingAppointment.CancellationReason = cancellationReason;
                    existingAppointment.CancellationTime = DateTime.UtcNow;


                    //handle slot status depending on the role
                    if (role == UserType.Client)  //if appointment cancelled by the client
                    {
                        if (slot != null && slot.Date > DateTime.UtcNow)
                        {
                            await _slotService.UpdateStatus(existingAppointment.SlotId, SlotStatus.Available, slot.ServiceProviderId);
                            //Notify the provider

                            var tokens = await _unitOfWork.ServiceProviders.GetByConditionAsync(
                                sp => sp.Id == existingAppointment.ServiceProviderId,
                                sp => new NotificationToken { Token = sp.NotificationToken! }
            );
                            var providerToken = tokens.FirstOrDefault();
                            if (providerToken is not null)
                            {
                                try
                                {
                                    await _pushNotification.SendNotificationAsync(
                                                                                title: "Appointment Cancelled",
                                                                                body: $"Your appointment on {slot?.Date} has been cancelled by the client.",
                                                                                providerToken.Token);
                                    _logger.LogInformation($"Your appointment on {slot?.Date} has been cancelled by the client");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Failed to send reminder for appointment. {ex.Message}");
                                }
                            }
                        }
                    }
                    else if (role == UserType.ServiceProvider)  //if appointment is cancelled by the provider
                    {
                        if (slot != null)
                        {
                            await _slotService.UpdateStatus(existingAppointment.SlotId, SlotStatus.Deleted, slot.ServiceProviderId);
                            // Notify the client
                            var tokens = await _unitOfWork.Clients.GetByConditionAsync(
                                    c => c.Id == existingAppointment.ClientId,
                                    c => new NotificationToken { Token = c.NotificationToken! }
                                );
                            var clientToken = tokens.FirstOrDefault();
                            if (clientToken is not null)
                            {
                                try
                                {
                                    await _pushNotification.SendNotificationAsync(
                                                                                title: "Appointment Cancelled",
                                                                                body: $"Your appointment on {slot?.Date} has been cancelled by the service provider.",
                                                                               clientToken.Token);
                                    _logger.LogInformation($"Your appointment on {slot?.Date} has been cancelled by the service provider");
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine($"Failed to send notification for appointment. {ex.Message}");
                                }
                            }
                        }
                    }
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return _mapper.Map<AppointmentDTO>(existingAppointment);

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Error occured while updating the appointment status!" + ex.Message, ex);
            }
        }


        public async Task DeleteAsync(int id)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (appointment == null)
                throw new KeyNotFoundException("Invalid Appointment ID");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                _unitOfWork.Appointments.Delete(appointment);

                var slot = await _unitOfWork.Slots.GetByIdAsync(appointment.SlotId);
                if (slot != null && slot.Date > DateTime.UtcNow)
                {
                    await _slotService.UpdateStatus(appointment.SlotId, SlotStatus.Available, slot.ServiceProviderId);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Error occured while deleting the appointment!" + ex.Message, ex);
            }
        }

        private async Task<bool> HasConflictAsync(int clientId, DateTime appointmentDate)
        {
            return await _unitOfWork.Appointments.CheckAnyExistsAsync(a => a.ClientId == clientId && a.Slot.Date == appointmentDate && a.Status != ServiceStatus.Cancelled);
        }

        //for notifications
        private async Task<bool> MarkReminderSentAsync(int appointmentId)
        {
            var appointment = await _unitOfWork.Appointments.GetByIdAsync(appointmentId);
            if (appointment == null)
                throw new KeyNotFoundException("Appointment not found.");

            appointment.ReminderSent = true;
            appointment.ReminderSentTime = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();
            return true;
        }
        public async Task SendAppointmentReminderAsync()
        {
            var targetTime = DateTime.UtcNow.AddHours(24);
            var startRange = targetTime.AddMinutes(-25);
            var endRange = targetTime.AddMinutes(25);

            var upcomingAppointments = await _unitOfWork.Appointments.GetByConditionAsync(
                a => a.Slot.Date > startRange &&
                     a.Slot.Date < endRange &&
                     a.Status == ServiceStatus.Pending &&
                     !a.ReminderSent,
                     includes: new[] {"Slot"});

            foreach (var appointment in upcomingAppointments)
            {
                var tokens = await _unitOfWork.Clients.GetByConditionAsync(
                                    c => c.Id == appointment.ClientId,
                                    c => new NotificationToken { Token = c.NotificationToken! }
                                );
                var clientToken = tokens.FirstOrDefault();
                if (clientToken is not null)
                {
                    try
                    {
                        var appointmentTimeString = appointment.Slot.Date.ToString("MMM dd, yyyy at h:mm tt");   // user-friendly format time

                        await _pushNotification.SendNotificationAsync(
                            title: "Appointment Reminder",
                            body: $"You have an appointment scheduled for tomorrow, {appointmentTimeString}.",
                            clientToken.Token);

                        _logger.LogInformation($"Successfully sent reminder for appointment ID: {appointment.Id}");

                        // Mark reminder as sent
                        await MarkReminderSentAsync(appointment.Id);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Failed to send reminder for appointment {appointment.Id}: {ex.Message}");
                    }
                }
            }
        }

    }
}
