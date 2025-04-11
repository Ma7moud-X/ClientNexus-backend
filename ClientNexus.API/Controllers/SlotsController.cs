using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClientNexus.Application.Services;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;
using ClientNexus.Domain.Enums;
using System.Security.Claims;

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
        public async Task<ActionResult<IEnumerable<SlotDTO>>> GetSlots(
            int serviceProviderId,
            DateTime startDate,
            DateTime endDate,
            SlotType type,
            SlotStatus? status)
        {

            return Ok(await _slotService.GetSlotsAsync(serviceProviderId, startDate, endDate, type, status));
        }

        /// <summary>
        /// Get slot by id
        /// </summary>
        [HttpGet("{id:int}", Name = "GetSlotById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<SlotDTO>> GetSlotById(int id)
        {
            return Ok(await _slotService.GetSlotByIdAsync(id));
        }

        // <summary>
        /// Service Provider Creates a new slot
        /// </summary>
        /// 
        //authorize: admin , provider
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<SlotDTO>> CreateSlot([FromBody] SlotCreateDTO slotDTO)
        {
            var slot = await _slotService.CreateAsync(slotDTO);
            return CreatedAtRoute("GetSlotById", new { id = slot.Id }, slot);
        }

        // <summary>
        /// Service Provider Updates a slot
        /// </summary>
        /// 
        //authorize: admin, provider
        [HttpPut("{id:int}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSlot(int id, [FromBody] SlotDTO slotDTO)
        {
            return Ok(await _slotService.Update(id, slotDTO));
        }

        // <summary>
        /// Update specific slot status
        /// </summary>
        
        //authorize: admin, provider, client 'to cancel'
        [HttpPatch("{id:int}/status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateSlotStatus(int id, [FromBody] SlotStatus status)
        {
            return Ok( await _slotService.UpdateStatus(id, status));
        }
        /// <summary>
        /// Service Provider deletes a slot
        /// </summary>
         
        //authorize: provider, admin
        [HttpDelete("{id:int}", Name = "DeleteSlot")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteSlot(int id)
        {
            // Get the user's role from the claims
            var role = User.FindFirstValue(ClaimTypes.Role);

            await _slotService.DeleteAsync(id, role);
            return NoContent();
        }

    }
}
