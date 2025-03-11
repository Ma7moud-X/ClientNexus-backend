using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Entities.Services;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;

namespace ClientNexus.Application.Services;

public class EmergencyCaseService : IEmergencyCaseService
{
    private readonly IUnitOfWork _unitOfWork;

    public EmergencyCaseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<EmergencyCase> CreateEmergencyCaseAsync(
        CreateEmergencyCaseDTO emergencyDTO,
        int clientId
    )
    {
        EmergencyCase emergencyCase = new EmergencyCase
        {
            Name = emergencyDTO.Name,
            Description = emergencyDTO.Description,
            MeetingLatitude = emergencyDTO.MeetingLatitude,
            MeetingLongitude = emergencyDTO.MeetingLongitude,
            ClientId = clientId,
            ServiceType = ServiceType.Emergency,
            Status = ServiceStatus.Pending,
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

    // public async Task<bool> AssignServiceProviderAsync(
    //     int emergencyCaseId,
    //     int clientID,
    //     int serviceProviderId,
    //     decimal price
    // )
    // {
    //     var emergencyCase = (
    //         await _unitOfWork.EmergencyCases.GetByConditionAsync(
    //             condExp: ec => ec.Id == emergencyCaseId && ec.ClientId == clientID,
    //             selectExp: sp => new { sp.CreatedAt, sp.ServiceProviderId }
    //         )
    //     ).FirstOrDefault();

    //     bool serviceProviderExists = await _unitOfWork.ServiceProviders.CheckAnyExistsAsync(sp =>
    //         sp.Id == serviceProviderId
    //     );
    //     if (!serviceProviderExists)
    //     {
    //         throw new ArgumentException(
    //             "Invalid service provider ID. Service provider does not exist.",
    //             nameof(serviceProviderId)
    //         );
    //     }
    // }
}
