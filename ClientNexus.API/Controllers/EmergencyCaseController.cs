using System.Runtime.Serialization;
using System.Text.Json;
using ClientNexus.API.Utilities;
using ClientNexus.Application.Constants;
using ClientNexus.Application.DTO;
using ClientNexus.Application.DTOs;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Enums;
using ClientNexus.Domain.Exceptions.ServerErrorsExceptions;
using ClientNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;

namespace ClientNexus.API.Controllers
{
    [Route("api/emergency-cases")]
    [ApiController]
    public class EmergencyCaseController : ControllerBase // TODO: add route to get emergency cases within certain radius away from a point
    {
        private readonly IEmergencyCaseService _emergencyCaseService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGeneralOfferListenerService _generalOfferListener;
        private readonly IBaseServiceService _baseServiceService;
        private readonly IOfferService _offerService;
        private readonly IServiceProviderService _serviceProviderService;
        private readonly IMapService _mapService;

        public EmergencyCaseController(
            IEmergencyCaseService emergencyCaseService,
            IUnitOfWork unitOfWork,
            IGeneralOfferListenerService generalOfferListener,
            IBaseServiceService baseServiceService,
            IOfferService offerService,
            IServiceProviderService serviceProviderService,
            IMapService mapService
        )
        {
            _emergencyCaseService = emergencyCaseService;
            _unitOfWork = unitOfWork;
            _generalOfferListener = generalOfferListener;
            _baseServiceService = baseServiceService;
            _offerService = offerService;
            _serviceProviderService = serviceProviderService;
            _mapService = mapService;
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
                    ec => new EmergencyCaseOverviewDTO
                    {
                        Id = ec.Id,
                        Title = ec.Name!,
                        Description = ec.Description!,
                        Status = (char)ec.Status,
                        CreatedAt = ec.CreatedAt,
                        Price = ec.Price ?? 0,
                        MeetingLongitude = ec.MeetingLocation!.X,
                        MeetingLatitude = ec.MeetingLocation!.Y,
                        ClientId = ec.ClientId,
                        ServiceProviderId = ec.ServiceProviderId,
                    },
                    offset: offset,
                    limit: limit,
                    orderByExp: ec => ec.Id,
                    descendingOrdering: true
                )
            );
        }

        [HttpGet("{id:int}/offers-sse")]
        [Authorize(Policy = "IsClient")]
        public async Task GetOffersSSE(int id, CancellationToken cancellationToken)
        {
            int? userId = User.GetId();
            if (userId is null)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                await Response.WriteAsync("Unauthorized", CancellationToken.None);
                await Response.CompleteAsync();
                return;
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
                Response.StatusCode = StatusCodes.Status404NotFound;
                await Response.WriteAsync("Not Found", CancellationToken.None);
                await Response.CompleteAsync();
                return;
            }

            if (emergencyDetails.ClientId != userId)
            {
                Response.StatusCode = StatusCodes.Status401Unauthorized;
                await Response.WriteAsync("Unauthorized", CancellationToken.None);
                await Response.CompleteAsync();
                return;
            }

            if (emergencyDetails.Status != ServiceStatus.Pending)
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                await Response.WriteAsync(
                    "Only pending requests can have offers",
                    CancellationToken.None
                );
                await Response.CompleteAsync();
                return;
            }

            if (DateTime.UtcNow >= emergencyDetails.CreatedAt.AddMinutes(15))
            {
                Response.StatusCode = StatusCodes.Status400BadRequest;
                await Response.WriteAsync("Request has expired", CancellationToken.None);
                await Response.CompleteAsync();
                return;
            }

            Response.Headers[HeaderNames.ContentType] = "text/event-stream";
            Response.Headers[HeaderNames.CacheControl] = "no-cache";
            Response.Headers[HeaderNames.Connection] = "keep-alive";

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

            return;
        }

        [HttpDelete("{id:int}")]
        [Authorize(Policy = "IsClient")]
        public async Task<IActionResult> CancelEmergencyCase(int id)
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
                        ec.Status,
                        ec.CreatedAt,
                    },
                    limit: 1
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

            if (emergencyDetails.Status == ServiceStatus.Cancelled)
            {
                return NoContent();
            }

            if (emergencyDetails.Status != ServiceStatus.Pending)
            {
                return BadRequest(
                    "Emergency case can't be cancelled as it's either in progress or completed."
                );
            }

            if (DateTime.UtcNow >= emergencyDetails.CreatedAt.AddMinutes(15))
            {
                return BadRequest("Emergency case has expired.");
            }

            await _baseServiceService.CancelAsync(id);
            return NoContent();
        }

        [HttpPost("{id:int}/offers")]
        [Authorize(Policy = "IsServiceProvider")]
        public async Task<IActionResult> CreateOffer(int id, [FromBody] CreateOfferDTO offerDTO)
        {
            int? userId = User.GetId();
            if (userId is null)
            {
                return Unauthorized();
            }

            if (!await _serviceProviderService.CheckIfAllowedToMakeOffersAsync(userId.Value))
            {
                return BadRequest(
                    new
                    {
                        Error = "Not allowed to make offers. Reasons: account blocked, account deleted, no phone number registered, notifications not active or you have an active emergency case.",
                    }
                );
            }

            var emergencyLocation = await _emergencyCaseService.GetMeetingLocationAsync(id);
            if (emergencyLocation is null)
            {
                return NotFound();
            }

            var providerLocation = await _emergencyCaseService.GetServiceProviderLocationAsync(
                userId.Value
            );
            if (providerLocation is null)
            {
                return BadRequest("Please enable location services in your app settings.");
            }

            var providerOverview = await _serviceProviderService.GetServiceProviderOverviewAsync(
                userId.Value
            );
            if (providerOverview is null)
            {
                throw new ServerException("Service provider not found.");
            }

            await _offerService.CreateOfferAsync(
                id,
                offerDTO.Price,
                providerOverview,
                await _mapService.GetTravelDistanceAsync(
                    providerLocation.Value,
                    emergencyLocation.Value,
                    offerDTO.TransportationType
                ),
                TimeSpan.FromMinutes(GlobalConstants.EmergencyCaseOfferTTLInMinutes)
            );

            return NoContent();
        }

        [HttpPut("providers-locations")]
        [Authorize(Policy = "IsServiceProvider")]
        public async Task<IActionResult> UpdateProviderLocation([FromBody] LocationDTO locationDTO)
        {
            int? userId = User.GetId();
            if (userId is null)
            {
                return Unauthorized();
            }

            if (!await _serviceProviderService.CheckIfAllowedToMakeOffersAsync(userId.Value))
            {
                return BadRequest(
                    new
                    {
                        Error = "Not allowed to send your location as you can't make offers. Reasons: account blocked, account deleted, no phone number registered, notifications not active or you have an active emergency case.",
                    }
                );
            }

            bool locationSet = await _emergencyCaseService.SetServiceProviderLocationAsync(
                userId.Value,
                locationDTO.Longitude,
                locationDTO.Latitude
            );
            if (!locationSet)
            {
                return Problem("Unable to set location. Please try again later.");
            }

            return NoContent();
        }

        [HttpPatch("{id:int}/accept")]
        [Authorize(Policy = "IsClient")]
        public async Task<IActionResult> AcceptOffer(int id, [FromBody] AcceptOfferDTO offerDTO)
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
                        ec.Status,
                        ec.CreatedAt,
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
                return BadRequest(
                    "Emergency case can't be accepted as it's either in progress or completed."
                );
            }

            if (DateTime.UtcNow >= emergencyDetails.CreatedAt.AddMinutes(15))
            {
                return BadRequest("Emergency case has expired.");
            }

            PhoneNumberDTO phoneNumberDTO;
            try
            {
                phoneNumberDTO = await _offerService.AcceptOfferAsync(
                    id,
                    userId.Value,
                    offerDTO.ServiceProviderId
                );
            }
            catch (ServerException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(phoneNumberDTO);
        }

        [HttpPut("available-lawyers")]
        [Authorize(Policy = "IsServiceProvider")]
        public async Task<IActionResult> SetAvailablilityForEmergency(
            [FromBody] AvailabilityDTO availability
        )
        {
            int? userId = User.GetId();
            if (userId is null)
            {
                return Unauthorized();
            }

            if (availability.status == false)
            {
                if (await _offerService.HasActiveOfferAsync(userId.Value))
                {
                    return BadRequest(
                        new { Error = "Can't be unavailable after you have just made an offer" }
                    );
                }

                await _serviceProviderService.SetUnvavailableForEmergencyAsync(userId.Value);
                return NoContent();
            }

            if (
                !await _serviceProviderService.CheckIfAllowedToBeAvailableForEmergencyAsync(
                    userId.Value
                )
            )
            {
                return BadRequest(
                    new
                    {
                        Error = "Not allowed to set available lawyers. Reasons: account blocked, account deleted, no phone number registered, notifications not active or you have an active emergency case.",
                    }
                );
            }

            await _serviceProviderService.SetAvailableForEmergencyAsync(userId.Value);
            return NoContent();
        }

        [HttpPatch("{id:int}/status")]
        [Authorize]
        public async Task<IActionResult> UpdateStatus(int id)
        {
            int? userId = User.GetId();
            UserType? userType = User.GetRole();
            if (userId is null || userType is null || userType.Value == UserType.Client)
            {
                return Unauthorized();
            }

            if (userType.Value == UserType.ServiceProvider)
            {
                var emergencyDetails = (
                    await _unitOfWork.EmergencyCases.GetByConditionAsync(
                        ec => ec.Id == id && ec.ServiceProviderId == userId.Value,
                        ec => new { ec.Status },
                        limit: 1
                    )
                ).FirstOrDefault();

                if (emergencyDetails is null)
                {
                    return NotFound();
                }

                if (
                    emergencyDetails.Status != ServiceStatus.InProgress
                    && emergencyDetails.Status != ServiceStatus.Done
                ) // can't happen
                {
                    return BadRequest(
                        "Emergency case can't be marked as done as it's still pending"
                    );
                }
            }

            await _baseServiceService.SetDoneAsync(id);
            return NoContent();
        }

        [HttpGet("available-emergencies")]
        [Authorize(Policy = "IsServiceProvider")]
        public async Task<IActionResult> GetAvailabeEmergencies(
            [FromQuery] double? longitude,
            [FromQuery] double? latitude,
            [FromQuery] double? radiusInMeters,
            [FromQuery] int offsetId = -1,
            [FromQuery] int limit = 10
        )
        {
            return Ok(
                await _emergencyCaseService.GetAvailableEmergenciesAsync(
                    offsetId: offsetId,
                    limit: limit,
                    longitude: longitude,
                    latitude: latitude,
                    radiusInMeters: radiusInMeters
                )
            );
        }
    }
}
