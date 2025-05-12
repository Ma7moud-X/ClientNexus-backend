using AutoMapper;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Models;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ClientNexus.Application.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IPushNotification _pushNotification;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(IUnitOfWork unitOfWork, IMapper mapper, IPushNotification pushNotification, ILogger<AppointmentService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

            return await _unitOfWork.Appointments.GetByConditionWithIncludesAsync<AppointmentDTO>(
                condExp: a => a.ClientId == clientId,
                selectExp: a => new AppointmentDTO
                {
                    Id = a.Id,
                    Status = a.Status,
                    ClientId = a.ClientId,
                    ServiceProviderId = (int)a.ServiceProviderId!,
                    SlotId = a.SlotId,
                    CheckInTime = a.CheckInTime,
                    CompletionTime = a.CompletionTime,
                    CancellationReason = a.CancellationReason,
                    CancellationTime = a.CancellationTime,
                    ReminderSent = a.ReminderSent,
                    ReminderSentTime = a.ReminderSentTime,
                    // Provider info
                    FirstName = a.ServiceProvider!.FirstName,
                    LastName = a.ServiceProvider.LastName,
                    ServiceProviderMainSpecialization = a.ServiceProvider.MainSpecialization!.Name,
                    ServiceProviderCity = a.ServiceProvider.Addresses!.FirstOrDefault()!.City!.Name,
                    ServiceProviderRate = a.ServiceProvider.Rate,
                    // Slot info
                    SlotType = a.Slot.SlotType,
                    SlotDate = a.Slot.Date
                },
                includeFunc: q => q
                    .Include(a => a.ServiceProvider)
                        .ThenInclude(sp => sp!.MainSpecialization!)
                    .Include(a => a.ServiceProvider)
                        .ThenInclude(sp => sp!.Addresses!)
                            .ThenInclude(addr => addr.City!)
                    .Include(a => a.Slot),
                offset: offset,
                limit: limit,
                orderByExp: a => a.Slot.Date,
                descendingOrdering: false
            );
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

                var slot = await _unitOfWork.Slots.GetByIdWithLockAsync(appointmentDTO.SlotId );
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

                //update slot status to be booked
                slot.Status = SlotStatus.Booked;
                await _unitOfWork.SaveChangesAsync();

                Appointment appoint = _mapper.Map<Appointment>(appointmentDTO);
                appoint.ServiceType = ServiceType.Appointment;
                appoint.Status = ServiceStatus.Pending;
                appoint.ClientId = clientId;
                appoint.ServiceProviderId = slot.ServiceProviderId;

                var createdAppoint = await _unitOfWork.Appointments.AddAsync(appoint);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
                return _mapper.Map<AppointmentDTO>(createdAppoint);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Error occured while Creating the appointment, " + ex.Message, ex);
            }
        }

        public async Task UpdateStatusAsync(int id, ServiceStatus status, int userId, UserType role, string? cancellationReason)
        {
            Appointment? existingAppointment = await _unitOfWork.Appointments.GetByIdAsync(id);
            if (existingAppointment == null)
                throw new KeyNotFoundException("Appointment not found");

            // Prevent invalid transitions
            await ValidateStatusTransition(existingAppointment, status);

            if (!Enum.IsDefined(status))
                throw new ArgumentOutOfRangeException($"Invalid Appointment Status value: {status}");

            await _unitOfWork.BeginTransactionAsync();
            try
            {
                var slot = await _unitOfWork.Slots.GetByIdAsync(existingAppointment.SlotId);
                if (slot == null)
                    throw new KeyNotFoundException($"Invalid slot ID");

                switch (status)
                {
                    case ServiceStatus.InProgress:
                        await HandleInProgressStatusAsync(existingAppointment, slot);
                        break;

                    case ServiceStatus.Done:
                        await HandleDoneStatusAsync(existingAppointment, slot);
                        break;

                    case ServiceStatus.Cancelled:
                        await HandleCancelledStatusAsync(existingAppointment, slot, role, cancellationReason);
                        break;
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Error occured while updating the appointment status!, " + ex.Message, ex);
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
                if (slot != null)
                {
                    if (slot.Date > DateTime.UtcNow)
                        slot.Status = SlotStatus.Available;
                    else
                        slot.Status = SlotStatus.Deleted;
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                throw new Exception("Error occured while deleting the appointment!, " + ex.Message, ex);
            }
        }

        private Task ValidateStatusTransition(Appointment appointment, ServiceStatus newStatus)
        {
            if (appointment.Status == ServiceStatus.Done || appointment.Status == ServiceStatus.Cancelled)
                throw new InvalidOperationException("Cannot change status of a completed or cancelled appointment.");
            return Task.CompletedTask;

        }
        private Task HandleInProgressStatusAsync(Appointment appointment, Slot slot)
        {
            if (slot.Date > DateTime.UtcNow)
            {
                throw new InvalidOperationException("Cannot mark appointment as In Progress before its slot time");
            }

            appointment.Status = ServiceStatus.InProgress;
            appointment.CheckInTime = DateTime.UtcNow;
            appointment.UpdatedAt = DateTime.UtcNow;

            return Task.CompletedTask;
        }
        private Task HandleDoneStatusAsync(Appointment appointment, Slot slot)
        {
            if (slot.Date > DateTime.UtcNow)
            {
                throw new InvalidOperationException("Cannot mark appointment as Done before its slot time");
            }

            appointment.Status = ServiceStatus.Done;
            appointment.CompletionTime = DateTime.UtcNow;
            appointment.UpdatedAt = DateTime.UtcNow;

            return Task.CompletedTask;
        }
        public async Task HandleCancelledStatusAsync(Appointment appointment, Slot slot, UserType role, string? cancellationReason)
        {
            appointment.Status = ServiceStatus.Cancelled;
            appointment.CancellationReason = cancellationReason;
            appointment.CancellationTime = DateTime.UtcNow;
            appointment.UpdatedAt = DateTime.UtcNow;

            // Handle slot and notifications based on who cancelled
            if (role == UserType.Client)
            {
                await HandleClientCancellationAsync(appointment, slot);
            }
            else if (role == UserType.ServiceProvider)
            {
                await HandleProviderCancellationAsync(appointment, slot);
            }
            else
            {
                throw new ArgumentException($"Cancellation handling not implemented for role: {role}");
            }
        }
        private async Task HandleClientCancellationAsync(Appointment appointment, Slot slot)
        {
            // Only return slot to available if appointment is in the future
            if (slot.Date > DateTime.UtcNow)
            {
                slot.Status = SlotStatus.Available;

                // Notify provider of cancellation
                await NotifyProviderOfCancellation(appointment, slot);
            }
        }

        private async Task HandleProviderCancellationAsync(Appointment appointment, Slot slot)
        {
            // Mark slot as deleted
            slot.Status = SlotStatus.Deleted;

            // Notify client of cancellation
            await NotifyClientOfCancellation(appointment, slot);
        }
        private async Task NotifyProviderOfCancellation(Appointment appointment, Slot slot)
        {
            if (appointment == null) throw new ArgumentNullException(nameof(appointment));
            if (slot == null) throw new ArgumentNullException(nameof(slot));

            try
            {
                var tokens = await _unitOfWork.ServiceProviders.GetByConditionAsync(
                                 sp => sp.Id == appointment.ServiceProviderId,
                                 sp => new NotificationToken { Token = sp.NotificationToken! }
             );
                var providerToken = tokens.FirstOrDefault();
                if (providerToken is not null)
                {

                    await _pushNotification.SendNotificationAsync(
                                                                title: "Appointment Cancelled",
                                                                body: $"Your appointment on {slot?.Date} has been cancelled by the client.",
                                                                providerToken.Token);
                    _logger.LogInformation($"Your appointment on {slot?.Date} has been cancelled by the client");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to send reminder for appointment,  {ex.Message}");
            }
        }

        private async Task NotifyClientOfCancellation(Appointment appointment, Slot slot)
        {
            if (appointment == null) throw new ArgumentNullException(nameof(appointment));
            if (slot == null) throw new ArgumentNullException(nameof(slot));

            try
            {
                var tokens = await _unitOfWork.Clients.GetByConditionAsync(
                    c => c.Id == appointment.ClientId,
                    c => new NotificationToken { Token = c.NotificationToken! }
                );
                var clientToken = tokens.FirstOrDefault();
                if (clientToken is not null)
                {
                    await _pushNotification.SendNotificationAsync(
                                                                title: "Appointment Cancelled",
                                                                body: $"Your appointment on {slot?.Date} has been cancelled by the service provider.",
                                                               clientToken.Token);
                    _logger.LogInformation($"Your appointment on {slot?.Date} has been cancelled by the service provider");
                }
            }
            catch (Exception ex)
            {
                _logger.LogInformation($"Failed to send notification for appointment. {ex.Message}");
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
                     includes: new[] { "Slot" });

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
                        _logger.LogInformation($"Failed to send reminder for appointment {appointment.Id}: {ex.Message}");
                    }
                }
            }
        }

    }
}
