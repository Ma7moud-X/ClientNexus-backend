using ClientNexus.API.Filters.AuthFilters;
using ClientNexus.API.Utilities;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClientNexus.API.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class BaseUserController : ControllerBase
    {
        private readonly IBaseUserService _baseUserService;

        public BaseUserController(IBaseUserService baseUserService)
        {
            _baseUserService = baseUserService;
        }

        [HttpPatch("notification-token")]
        [Authorize]
        [TypeFilter(
            typeof(OrAuthorizationFilter),
            Arguments = new object[] { new string[] { "IsServiceProvider", "IsClient" } }
        )]
        public async Task<IActionResult> UpdateNotificationToken(
            [FromBody] SetNotificationDTO notificationDto
        )
        {
            int? userId = User.GetId();
            if (userId is null)
            {
                return Unauthorized();
            }

            if (string.IsNullOrWhiteSpace(notificationDto.token))
            {
                return BadRequest("Token can't be empty or white space");
            }

            await _baseUserService.SetNotificationTokenAsync(userId.Value, notificationDto.token);
            return NoContent();
        }
    }
}
