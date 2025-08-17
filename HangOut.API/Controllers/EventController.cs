
using HangOut.API.Services.Implement;
using HangOut.API.Services.Interface;
using HangOut.Domain.Constants;
using HangOut.Domain.Payload.Base;
using HangOut.Domain.Payload.Request.Event;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HangOut.API.Controllers
{
    public class EventController : BaseController<EventController>
    {
        private readonly IEventService _eventService;
        public EventController(ILogger logger, IEventService eventService) : base(logger)
        {
            _eventService = eventService;
        }

        [HttpPost(ApiEndPointConstant.Event.CreateEvent)]
        public async Task<IActionResult> Register([FromForm] CreateEventRequest request)
        {
            var response = await _eventService.CreateEvent(request);
            return CreatedAtAction(nameof(Register), response);
        }

        [HttpGet(ApiEndPointConstant.Event.GetEventByBO)]
        public async Task<IActionResult> GetEventByBO([FromQuery] int? page, [FromQuery] int? size)
        {
            int pageNumber = page ?? 1;
            int pageSize = size ?? 10;
            var response = await _eventService.GetEventsByBO(pageNumber, pageSize);
            return CreatedAtAction(nameof(GetEventByBO), response);
        }

        [HttpGet("get-events")]
        public async Task<IActionResult> GetEvents([FromQuery]int pageNumber, [FromQuery]int pageSize, 
            [FromQuery]string? searchKey, [FromQuery]string? location, [FromQuery]string? businessName)
        {
            var response  = await _eventService.GetEvents(pageNumber,pageSize, searchKey, location, businessName);
            return StatusCode(response.Status, response);
        }

        [HttpGet("get-event")]
        public async Task<IActionResult> GetEvent([FromQuery]Guid eventId)
        {
            var response = await _eventService.GetEvent(eventId);
            return StatusCode(response.Status, response);
        }

        [HttpPatch("edit-event/{eventId}")]
        [Authorize(Roles = "BusinessOwner")]
        public async Task<IActionResult> EditEvent(Guid eventId, [FromForm]EditEventRequest request)
        {
            try
            {
                var response = await _eventService.EditEvent(eventId, request);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500,ex.ToString());
            }
        }

        [HttpDelete("delete-event/{eventId}")]
        [Authorize(Roles = "BusinessOwner")]
        public async Task<IActionResult>DeleteEvent(Guid eventId)
        {
            try
            {
                var response = await _eventService.DeleteEvent(eventId);
                return StatusCode(response.Status, response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
    }
}
