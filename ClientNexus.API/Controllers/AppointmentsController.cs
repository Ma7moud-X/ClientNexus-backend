using ClientNexus.API.Utilities;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;
using ClientNexus.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ClientNexus.API.Controllers
{
    [Route("api/appointments")]
    [ApiController]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }
        /// <summary>
        /// Get an appointment by ID.
        /// </summary>
        [HttpGet("{id:int}", Name = "GetAppointmentById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AppointmentDTO>> GetById(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            return Ok(appointment);
        }
        /// <summary>
        /// Get all appointments for a specific provider.
        /// </summary>
        [HttpGet("provider")]
        [Authorize(Policy = "IsServiceProvider")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetByProviderId(int offset = 0, int limit = 10)
        {
            var userId = User.GetId();
            if (userId is null)
                return Unauthorized();
            
            var appointments = await _appointmentService.GetByProviderIdAsync(userId.Value, offset, limit);
            return Ok(appointments);
        }
        /// <summary>
        /// Get all appointments for a specific client.
        /// </summary>
        [HttpGet("client")]
        [Authorize(Policy = "IsClient")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetByClientId(int offset = 0, int limit = 10)
        {
            var userId = User.GetId();
            if (userId is null)
                return Unauthorized();
            
            var appointments = await _appointmentService.GetByClientIdAsync(userId.Value, offset, limit);
            return Ok(appointments);
        }
        /// <summary>
        /// Create a new appointment.
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "IsClient")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Create([FromBody] AppointmentCreateDTO appointmentDto)
        {
            var userId = User.GetId();
            if (userId is null)
                return Unauthorized();

            var appointment = await _appointmentService.CreateAsync(userId.Value, appointmentDto);
            return CreatedAtRoute("GetAppointmentById", new { id = appointment.Id }, appointment);
        }
        /// <summary>
        /// Update an existing appointment status.
        /// </summary>
        [HttpPut("{id:int}")]
        [Authorize(Policy = "IsServiceProviderOrClient")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateStatusAsync(int id, [FromBody]AppointmentStatusUpdateRequest request)
        {
            var role = User.GetRole();
            var userId = User.GetId();
            if (role is null || userId is null)
                return Unauthorized();

            var updatedAppointment = await _appointmentService.UpdateStatusAsync(id, request.Status,userId.Value, role.Value, request.Reason);
            return NoContent();
        }
        /// <summary>
        /// Delete an appointment by ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        [Authorize(Policy = "IsAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Delete(int id)
        {
            await _appointmentService.DeleteAsync(id);
            return NoContent();
        }
    }
}
