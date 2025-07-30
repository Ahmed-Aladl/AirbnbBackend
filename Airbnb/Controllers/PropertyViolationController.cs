using Airbnb.Extensions;
using Application.DTOs.PropertyViolationDTOs;
using Application.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ViolationController : ControllerBase
    {
        private readonly PropertyViolationService _violationService;

        public ViolationController(PropertyViolationService violationService)
        {
            _violationService = violationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _violationService.GetAllPropertyViolationsAsync();
            return result.ToActionResult();
        }

        [HttpGet("violation/{id:int}")]
        public async Task<IActionResult> GetPropertyViolationById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid violation ID");

            var result = await _violationService.GetPropertyViolationByIdAsync(id);
            return result.ToActionResult();
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetViolationsByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("User ID is required");

            var result = await _violationService.GetViolationsByUserIdAsync(userId);
            return result.ToActionResult();
        }

        [HttpGet("property/{propertyId:int}")]
        public async Task<IActionResult> GetViolationsByPropertyId(int propertyId)
        {
            if (propertyId <= 0)
                return BadRequest("Invalid property ID");

            var result = await _violationService.GetViolationsByPropertyIdAsync(propertyId);
            return result.ToActionResult();
        }

        [HttpPost]
        public async Task<IActionResult> CreateViolation([FromBody] CreateViolationDTO createDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _violationService.AddViolationAsync(createDto);
            return result.ToActionResult();
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateViolation(int id, [FromBody] UpdateViolationDTO updateDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (id != updateDto.Id)
                return BadRequest("Route ID doesn't match DTO ID");

            var result = await _violationService.UpdateViolationAsync(updateDto);
            return result.ToActionResult();
        }
    }
}