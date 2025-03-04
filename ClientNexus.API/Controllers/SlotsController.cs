using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ClientNexus.Application.Services;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using ClientNexus.Application.DTO;
using ClientNexus.Application.Interfaces;

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
        /// Get available slots for a service provider within a date range.
        /// </summary>
        [HttpGet("available")]
        public async Task<ActionResult<IEnumerable<SlotDTO>>> GetAvailableSlots(
            int serviceProviderId,
            DateTime startDate,
            DateTime endDate)
        {
            var slots = await _slotService.GetAvailableSlotsAsync(serviceProviderId, startDate, endDate);
            return Ok(slots);
        }

        /// <summary>
        /// Get slot by id
        /// </summary>
        [HttpGet("{id:int}", Name = "GetSlotById")]
        public async Task<ActionResult<SlotDTO>> GetSlotById(int id)
        {
            var slot = await _slotService.GetSlotByIdAsync(id);
            return Ok(slot);
        }

        // <summary>
        /// Create a new slot.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<int>> CreateSlot([FromBody] SlotCreateDTO slotDTO)
        {
            var slotId = await _slotService.CreateAsync(slotDTO);
            return CreatedAtAction(nameof(GetAvailableSlots), new { id = slotId }, slotId);
        }
    }
}
