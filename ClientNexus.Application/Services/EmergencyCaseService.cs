using System.Globalization;
using System.Linq.Expressions;
using ClientNexus.Application.Constants;
using ClientNexus.Application.DTO;
using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
using ClientNexus.Domain.Interfaces;
using ClientNexus.Domain.ValueObjects;
using NetTopologySuite.Geometries;
using TimeZoneConverter;

namespace ClientNexus.Application.Services;

public class EmergencyCaseService : IEmergencyCaseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly INotificationService _notificationService;

    public EmergencyCaseService(IUnitOfWork unitOfWork, INotificationService notificationService)
    {
        _unitOfWork = unitOfWork;
        _notificationService = notificationService;
    }

    private async Task<EmergencyCase> CreateEmergencyCaseAsync(
        CreateEmergencyCaseDTO emergencyDTO,
        int clientId
    )
    {
        EmergencyCase emergencyCase = new EmergencyCase
        {
            Name = emergencyDTO.Name,
            Description = emergencyDTO.Description,
            MeetingLocation = new MapPoint(
                emergencyDTO.MeetingLongitude,
                emergencyDTO.MeetingLatitude
            ),
            ClientId = clientId,
            Status = ServiceStatus.Pending,
            MeetingTextAddress = emergencyDTO.MeetingTextAddress,
        };

        await _unitOfWork.EmergencyCases.AddAsync(emergencyCase);

        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            throw new Exception("Error creating emergency case", ex);
        }

        return emergencyCase;
    }

    public async Task<ClientEmergencyDTO> InitiateEmergencyCaseAsync(
        CreateEmergencyCaseDTO emergencyDTO,
        int clientId,
        string clientFirstName,
        string clientLastName,
        double notifyServicePorvidersWithinMeters = 3000,
        int allowOffersWithinMinutes = 16
    )
    {
        ArgumentNullException.ThrowIfNull(emergencyDTO);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientFirstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(clientLastName);

        DateTime utcTime = DateTime.UtcNow;
        TimeZoneInfo egyptTimeZone = TZConvert.GetTimeZoneInfo("Egypt Standard Time");
        DateTime egyptTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, egyptTimeZone);

        EmergencyCase emergencyCase = await CreateEmergencyCaseAsync(emergencyDTO, clientId);
        await _notificationService.SendNotificationToServiceProvidersNearLocationAsync(
            emergencyDTO.MeetingLongitude,
            emergencyDTO.MeetingLatitude,
            notifyServicePorvidersWithinMeters,
            $"طلب طارئ بالقرب منك",
            $"عنوان الطلب: {emergencyDTO.Name}\n" +
            $"الوصف: {emergencyDTO.Description}\n" +
            $"الوقت: {egyptTime.ToString("dd/MM/yyyy hh:mm tt", new CultureInfo("ar-EG"))}",
            new Dictionary<string, string>
            {
                { "type", "EmergencyCase" },
                { "id", emergencyCase.Id.ToString() },
            }
        );

        return new ClientEmergencyDTO
        {
            Id = emergencyCase.Id,
            TimeoutInMinutes = allowOffersWithinMinutes - 1,
        };
    }

    public async Task<bool> CheckIfIdExistsAsync(int emergencyCaseId)
    {
        return await _unitOfWork.EmergencyCases.CheckAnyExistsAsync(ec => ec.Id == emergencyCaseId);
    }

    public async Task<bool> HasActiveEmergencyForClientAsync(int clientId)
    {
        var emergencyCase = (
            await _unitOfWork.EmergencyCases.GetByConditionAsync(
                ec =>
                    ec.ClientId == clientId
                    && (
                        ec.CreatedAt >= DateTime.UtcNow.AddHours(-2)
                            && ec.Status == ServiceStatus.InProgress
                        || ec.CreatedAt.AddMinutes(15) >= DateTime.UtcNow
                            && ec.Status == ServiceStatus.Pending
                    ),
                limit: 1
            )
        ).FirstOrDefault();

        return emergencyCase is not null;
    }

    public async Task<bool> HasActiveEmergencyForServiceProviderAsync(int serviceProviderId)
    {
        var emergencyCase = (
            await _unitOfWork.EmergencyCases.GetByConditionAsync(
                ec =>
                    ec.CreatedAt >= DateTime.UtcNow.AddHours(-2)
                    && ec.ServiceProviderId == serviceProviderId
                    && ec.Status == ServiceStatus.InProgress,
                limit: 1
            )
        ).FirstOrDefault();

        return emergencyCase is not null;
    }

    public async Task<bool> IsClientAllowedToCreateEmergencyAsync(int clientId)
    {
        var res = (
            await _unitOfWork.Clients.GetByConditionAsync(
                c => c.Id == clientId,
                c => new
                {
                    c.IsBlocked,
                    c.IsDeleted,
                    c.PhoneNumber,
                    c.NotificationToken,
                },
                limit: 1
            )
        ).FirstOrDefault();

        if (res is null)
        {
            throw new NotFoundException($"Client is not found");
        }

        return !res.IsBlocked
            && !res.IsDeleted
            && res.PhoneNumber != null
            && res.NotificationToken != null
            && !await HasActiveEmergencyForClientAsync(clientId);
    }

    public async Task<(double longitude, double latitude)?> GetMeetingLocationAsync(
        int emergencyCaseId
    )
    {
        Point? location = (
            await _unitOfWork.EmergencyCases.GetByConditionAsync(
                ec => ec.Id == emergencyCaseId,
                ec => ec.MeetingLocation,
                limit: 1
            )
        ).FirstOrDefault();

        if (location is null)
        {
            return null;
        }

        return (location.X, location.Y);
    }

    public async Task<(double longitude, double latitude)?> GetServiceProviderLocationAsync(
        int serviceProviderId
    )
    {
        var location = (
            await _unitOfWork.ServiceProviders.GetByConditionAsync(
                sp => sp.Id == serviceProviderId,
                sp => sp.CurrentLocation,
                limit: 1
            )
        ).FirstOrDefault();

        if (location is null)
        {
            return null;
        }

        return (location.X, location.Y);
    }

    public async Task<bool> SetServiceProviderLocationAsync(
        int serviceProviderId,
        double longitude,
        double latitude
    )
    {
        var affectedCount = await _unitOfWork.SqlExecuteAsync(
            @"
            UPDATE ClientNexusSchema.ServiceProviders SET CurrentLocation = geography::Point(@latitude, @longitude, 4326), LastLocationUpdateTime = @lastLocationUpdateTime
            WHERE Id = @serviceProviderId
            ",
            new Parameter("@latitude", latitude),
            new Parameter("@longitude", longitude),
            new Parameter("@lastLocationUpdateTime", DateTime.UtcNow),
            new Parameter("@serviceProviderId", serviceProviderId)
        );

        return affectedCount != 0;
    }

    public async Task<EmergencyCaseOverviewDTO?> GetOverviewByIdAsync(int id)
    {
        return (
            await _unitOfWork.EmergencyCases.GetByConditionAsync(
                condExp: ec => ec.Id == id,
                selectExp: ec => new EmergencyCaseOverviewDTO
                {
                    Id = ec.Id,
                    Title = ec.Name!,
                    Description = ec.Description!,
                    MeetingLongitude = ec.MeetingLocation!.X,
                    MeetingLatitude = ec.MeetingLocation!.Y,
                    CreatedAt = ec.CreatedAt,
                    Status = (char)ec.Status,
                    ServiceProviderId = ec.ServiceProviderId,
                    ClientId = ec.ClientId,
                    Price = ec.Price,
                },
                limit: 1
            )
        ).FirstOrDefault();
    }

    public async Task<IEnumerable<ServiceProviderEmergencyDTO>> GetAvailableEmergenciesAsync(
        int offsetId = -1,
        int limit = 10,
        double? longitude = null,
        double? latitude = null,
        double? radiusInMeters = null
    )
    {
        Expression<Func<EmergencyCase, bool>> condition;
        if (longitude is not null && latitude is not null && radiusInMeters is not null)
        {
            condition = ec =>
                ec.MeetingLocation != null
                && ec.MeetingLocation.Distance(new MapPoint(longitude.Value, latitude.Value))
                    <= radiusInMeters
                && ec.Status == ServiceStatus.Pending
                && DateTime.UtcNow
                    < ec.CreatedAt.AddMinutes(GlobalConstants.EmergencyCaseTTLInMinutes)
                && ec.Id > offsetId;
        }
        else
        {
            condition = ec =>
                ec.Status == ServiceStatus.Pending
                && DateTime.UtcNow
                    < ec.CreatedAt.AddMinutes(GlobalConstants.EmergencyCaseTTLInMinutes)
                && ec.Id > offsetId;
        }

        return await _unitOfWork.EmergencyCases.GetByConditionAsync(
            condExp: condition,
            ec => new ServiceProviderEmergencyDTO
            {
                ClientFirstName = ec.Client!.FirstName,
                ClientLastName = ec.Client.LastName,
                Name = ec.Name!,
                Description = ec.Description!,
                ServiceId = ec.Id,
                MeetingTextAddress = ec.MeetingTextAddress,
            },
            orderByExp: ec => ec.Id,
            limit: limit
        );
    }

    public async Task<ServiceProviderEmergencyDTO?> GetAvailableEmegencyByIdAsync(int id)
    {
        return (
            await _unitOfWork.EmergencyCases.GetByConditionAsync(
                condExp: ec =>
                    ec.Status == ServiceStatus.Pending
                    && DateTime.UtcNow
                        < ec.CreatedAt.AddMinutes(GlobalConstants.EmergencyCaseTTLInMinutes)
                    && ec.Id == id,
                selectExp: ec => new ServiceProviderEmergencyDTO
                {
                    ClientFirstName = ec.Client!.FirstName,
                    ClientLastName = ec.Client.LastName,
                    Name = ec.Name!,
                    Description = ec.Description!,
                    ServiceId = ec.Id,
                    MeetingTextAddress = ec.MeetingTextAddress,
                },
                limit: 1
            )
        ).FirstOrDefault();
    }
}
