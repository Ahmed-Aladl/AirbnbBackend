using System;
using System.Threading.Tasks;
using Application.DTOs.Calendar;
using Application.Result;
using Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CalendarController : ControllerBase
    {
        private readonly CalendarService _calendarService;

        public CalendarController(CalendarService calendarService)
        {
            _calendarService = calendarService;
        }

        [HttpGet("property/{propertyId}")]
        public async Task<ActionResult<Result<List<CalendarAvailabilityDto>>>> GetPropertyCalendar(
            int propertyId, [FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate)
        {
            var result = await _calendarService.GetPropertyCalendar(propertyId, startDate, endDate);
            return StatusCode(result.StatusCode ?? 200, result);
        }

        [Authorize(Roles = "Host")]
        [HttpPut("property/{propertyId}")]
        public async Task<ActionResult<Result<bool>>> UpdatePropertyCalendar(
            int propertyId, [FromBody] List<CalendarUpdateDto> updates)
        {
            var result = await _calendarService.UpdatePropertyCalendar(propertyId, updates);
            return StatusCode(result.StatusCode ?? 200, result);
        }
    }
}