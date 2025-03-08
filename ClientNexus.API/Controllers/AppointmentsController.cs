using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public async Task<ActionResult<AppointmentDTO>> GetById(int id)
        {
            var appointment = await _appointmentService.GetByIdAsync(id);
            return Ok(appointment);
        }
        /// <summary>
        /// Get all appointments for a specific provider.
        /// </summary>
        [HttpGet("provider/{providerId:int}")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetByProviderId(int providerId, int offset, int limit)
        {
            var appointments = await _appointmentService.GetByProviderIdAsync(providerId, offset, limit);
            return Ok(appointments);
        }
        /// <summary>
        /// Get all appointments for a specific client.
        /// </summary>
        [HttpGet("client/{clientId:int}")]
        public async Task<ActionResult<IEnumerable<AppointmentDTO>>> GetByClientId(int clientId, int offset, int limit)
        {
            var appointments = await _appointmentService.GetByClientIdAsync(clientId, offset, limit);
            return Ok(appointments);
        }
        /// <summary>
        /// Create a new appointment.
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] AppointmentCreateDTO appointmentDto)
        {
            var appointment = await _appointmentService.CreateAsync(appointmentDto);
            return CreatedAtRoute("GetAppointmentById", new { id = appointment.Id }, appointment);
        }
        /// <summary>
        /// Update an existing appointment.
        /// </summary>
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] AppointmentDTO appointmentDto)
        {
            var updatedAppointment = await _appointmentService.UpdateAsync(id, appointmentDto);
            return NoContent();
        }

        /// <summary>
        /// Delete an appointment by ID.
        /// </summary>
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _appointmentService.DeleteAsync(id);
            return NoContent();
        }
    }
}
