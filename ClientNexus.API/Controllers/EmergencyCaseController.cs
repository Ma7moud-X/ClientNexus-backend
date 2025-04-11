using System.Runtime.Serialization;
using System.Text.Json;
using ClientNexus.API.Utilities;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientNexus.API.Controllers
{
    [Route("api/emergency-cases")]
    [ApiController]
    public class EmergencyCaseController : ControllerBase
    {
        private readonly IEmergencyCaseService _emergencyCaseService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGeneralOfferListenerService _generalOfferListener;

        public EmergencyCaseController(
            IEmergencyCaseService emergencyCaseService,
            IUnitOfWork unitOfWork,
            IGeneralOfferListenerService generalOfferListener
        )
        {
            _emergencyCaseService = emergencyCaseService;
            _unitOfWork = unitOfWork;
            _generalOfferListener = generalOfferListener;
        }

        [HttpPost]
        [Authorize(Policy = "IsClient")]
        public async Task<IActionResult> CreateEmergencyCase(
            [FromBody] CreateEmergencyCaseDTO emergencyCaseDTO
        )
        {
            int? userId = User.GetId();
            if (userId is null)
            {
                return Unauthorized();
            }

            if (!await _emergencyCaseService.IsClientAllowedToCreateEmergencyAsync(userId.Value))
            {
                return BadRequest(
                    new
                    {
                        Error = "In order to be able to make a request, you must have a phone number and has no active requests",
                    }
                );
            }

            var clientDetails = (
                await _unitOfWork.Clients.GetByConditionAsync(
                    c => c.Id == userId.Value,
                    c => new { c.FirstName, c.LastName }
                )
            ).FirstOrDefault();
            if (clientDetails is null)
            {
                return Unauthorized();
            }

            ClientEmergencyDTO res = await _emergencyCaseService.InitiateEmergencyCaseAsync(
                emergencyCaseDTO,
                userId.Value,
                clientDetails.FirstName,
                clientDetails.LastName
            );

            return CreatedAtAction(nameof(GetEmergencyCaseById), new { id = res.Id }, res);
        }

        [HttpGet("{id:int}")]
        [Authorize]
        public async Task<IActionResult> GetEmergencyCaseById(int id)
        {
            UserType? role = User.GetRole();
            int? userId = User.GetId();
            if (role is null || userId is null)
            {
                return Unauthorized();
            }

            var emergencyDetails = (
                await _unitOfWork.EmergencyCases.GetByConditionAsync(
                    ec => ec.Id == id,
                    ec => new { ec.ClientId, ec.ServiceProviderId }
                )
            ).FirstOrDefault();
            if (emergencyDetails is null)
            {
                return NotFound();
            }

            if (role.Value == UserType.ServiceProvider)
            {
                if (emergencyDetails.ServiceProviderId != userId)
                {
                    return Unauthorized();
                }
            }
            else if (role.Value == UserType.Client)
            {
                if (emergencyDetails.ClientId != userId)
                {
                    return Unauthorized();
                }
            }

            return Ok(await _emergencyCaseService.GetOverviewByIdAsync(id));
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetEmergencyCases(
            [FromQuery] int? clientId,
            [FromQuery] int? serviceProviderId,
            [FromQuery] ServiceStatus? status,
            [FromQuery] int offset = 0,
            [FromQuery] int limit = 10
        )
        {
            UserType? role = User.GetRole();
            int? userId = User.GetId();
            if (role is null || userId is null)
            {
                return Unauthorized();
            }

            if (clientId is null && serviceProviderId is null)
            {
                return BadRequest(
                    new { Error = "At least one of clientId or serviceProviderId must be provided" }
                );
            }

            if (
                role.Value == UserType.ServiceProvider
                && serviceProviderId is not null
                && serviceProviderId != userId
            )
            {
                return Unauthorized();
            }
            else if (role.Value == UserType.Client && clientId is not null && clientId != userId)
            {
                return Unauthorized();
            }

            List<(string conditionString, object value)> conditions = new();
            if (clientId is not null)
            {
                conditions.Add(("ClientId == @0", clientId));
            }

            if (serviceProviderId is not null)
            {
                conditions.Add(("ServiceProviderId == @0", serviceProviderId));
            }

            if (status is not null)
            {
                conditions.Add(("Status == @0", status));
            }

            return Ok(
                await _unitOfWork.EmergencyCases.GetByConditionAsync(
                    conditions,
                    ec => new
                    {
                        Id = ec.Id,
                        Title = ec.Name,
                        Description = ec.Description,
                        Status = ec.Status,
                        CreatedAt = ec.CreatedAt,
                        Price = ec.Price ?? 0,
                        MeetingLongitude = ec.MeetingLongitude,
                        MeetingLatitude = ec.MeetingLatitude,
                    },
                    offset: offset,
                    limit: limit
                )
            );
        }

        [HttpGet("{id:int}/offers-sse")]
        [Authorize(Policy = "IsClient")]
        public async Task<IActionResult> GetOffersSSE(int id, CancellationToken cancellationToken)
        {
            int? userId = User.GetId();
            if (userId is null)
            {
                return Unauthorized();
            }

            var emergencyDetails = (
                await _unitOfWork.EmergencyCases.GetByConditionAsync(
                    ec => ec.Id == id,
                    ec => new
                    {
                        ec.ClientId,
                        ec.CreatedAt,
                        ec.Status,
                    }
                )
            ).FirstOrDefault();

            if (emergencyDetails is null)
            {
                return NotFound();
            }

            if (emergencyDetails.ClientId != userId)
            {
                return Unauthorized();
            }

            if (emergencyDetails.Status != ServiceStatus.Pending)
            {
                return BadRequest(new { Error = "Only pending requests can have offers" });
            }

            if (DateTime.UtcNow >= emergencyDetails.CreatedAt.AddMinutes(15))
            {
                return BadRequest(new { Error = "Request has expired" });
            }

            Response.Headers["Content-Type"] = "text/event-stream";
            Response.Headers["Cache-Control"] = "no-cache";
            Response.Headers["Connection"] = "keep-alive";

            await _generalOfferListener.SubscribeAsync(id);
            ClientOfferDTO offer;
            bool save = false;
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    offer = await _generalOfferListener.ListenAsync(cancellationToken);
                    await Response.WriteAsync(
                        $"event: offer\ndata: {JsonSerializer.Serialize(offer)}\n\n",
                        cancellationToken
                    );
                    await Response.Body.FlushAsync(cancellationToken);
                }
                catch (OperationCanceledException)
                {
                    save = true;
                    break;
                }
                catch (SerializationException) { }
            }
            await _generalOfferListener.CloseAsync(save);

            return Ok();
        }
    }
}
