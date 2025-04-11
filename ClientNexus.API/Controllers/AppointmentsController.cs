using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;
using ClientNexus.Domain.Enums;
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
        // authorize: admin, provider
        [HttpGet("provider/{providerId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetByProviderId(int providerId, int offset, int limit)
        {
            var appointments = await _appointmentService.GetByProviderIdAsync(providerId, offset, limit);
            return Ok(appointments);
        }
        /// <summary>
        /// Get all appointments for a specific client.
        /// </summary>
        // authorize: admin, client
        [HttpGet("client/{clientId:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetByClientId(int clientId, int offset, int limit)
        {
            var appointments = await _appointmentService.GetByClientIdAsync(clientId, offset, limit);
            return Ok(appointments);
        }
        /// <summary>
        /// Create a new appointment.
        /// </summary>
        // authorize: client
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create([FromBody] AppointmentCreateDTO appointmentDto)
        {
            var appointment = await _appointmentService.CreateAsync(appointmentDto);
            return CreatedAtRoute("GetAppointmentById", new { id = appointment.Id }, appointment);
        }
        /// <summary>
        /// Update an existing appointment status.
        /// </summary>
        // authorize: admin, provider, client

        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateStatusAsync(int id, AppointmentStatusUpdateRequest request)
        {
            // Get the user's role from the claims
            var role = User.FindFirstValue(ClaimTypes.Role);

            var updatedAppointment = await _appointmentService.UpdateStatusAsync(id, request.Status, request.Reason, role);
            return NoContent();
        }
        /// <summary>
        /// Delete an appointment by ID.
        /// </summary>
        // authorize: admin 

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            await _appointmentService.DeleteAsync(id);
            return NoContent();
        }
    }
}
