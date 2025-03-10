using System;
using ClientNexus.Application.DTO;
using ClientNexus.Domain.Entities.Services;

namespace ClientNexus.Application.Interfaces;

public interface IEmergencyCaseService
{
    Task<EmergencyCase> CreateEmergencyCaseAsync(EmergencyCaseDTO emergencyDTO);
}
