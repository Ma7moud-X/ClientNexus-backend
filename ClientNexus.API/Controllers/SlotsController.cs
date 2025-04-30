using Microsoft.AspNetCore.Mvc;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using ClientNexus.API.Utilities;

namespace ClientNexus.API.Controllers
{
    [Route("api/slots")]
    [ApiController]
    public class SlotsController : ControllerBase
    {
        //protected ApiResponseFormat _response;
        private readonly ISlotService _slotService;

        public SlotsController(ISlotService slotService)
        {
            _slotService = slotService;
            //this._response = new();
        }
        /// <summary>
        /// Get available slots for a service provider within a date range
        /// </summary>
        [HttpGet(Name = "GetSlots")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetSlots(
            int serviceProviderId,
            DateTime startDate,
            DateTime endDate,
            SlotType type,
            SlotStatus status = SlotStatus.Available)
        {

            return Ok(await _slotService.GetSlotsAsync(serviceProviderId, startDate, endDate, type, status));
        }

        /// <summary>
        /// Get slot by id
        /// </summary>
        [HttpGet("{id:int}", Name = "GetSlotById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSlotById(int id)
        {
            return Ok(await _slotService.GetSlotByIdAsync(id));
        }

        // <summary>
        /// Service Provider Creates a new slot
        /// </summary>
        [HttpPost]
        [Authorize(Policy = "IsServiceProvider")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateSlot([FromBody] SlotCreateDTO slotDTO)
        {
            var userId = User.GetId();
            if (userId is null)
                return Unauthorized();

            var slot = await _slotService.CreateAsync(slotDTO, userId.Value);
            return CreatedAtRoute("GetSlotById", new { id = slot.Id }, slot);
        }

        // <summary>
        /// Update specific slot date
        /// </summary>
        [HttpPatch("{id:int}/date")]
        [Authorize(Policy = "IsServiceProvider")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateSlotDate(int id, [FromBody] DateTime date)
        {
            var userId = User.GetId();
            if (userId is null)
                return Unauthorized();

            await _slotService.UpdateDateAsync(id, date, userId.Value);
            return NoContent();
        }
        // <summary>
        /// Update specific slot type ()
        /// </summary>
        [HttpPatch("{id:int}/type")]
        [Authorize(Policy = "IsServiceProvider")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> UpdateSlotType(int id, [FromBody] SlotType type)
        {
            var userId = User.GetId();
            if (userId is null)
                return Unauthorized();

            await _slotService.UpdateTypeAsync(id, type, userId.Value);
            return NoContent();
        }

        /// <summary>
        /// Service Provider deletes a slot
        /// </summary>
        [HttpDelete("{id:int}", Name = "DeleteSlot")]
        [Authorize(Policy = "IsServiceProviderOrAdmin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            var role = User.GetRole();
            var userId = User.GetId();
            if (role is null || userId is null)
                return Unauthorized();

            await _slotService.DeleteAsync(id, userId.Value, role.Value);
            return NoContent();
        }

    }
}
