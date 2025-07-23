using System.Collections.Generic;
using System.Threading.Tasks;
using Application.DTOs.PropertyType;
using Application.Interfaces.IRepositories; 
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PropertyTypeController : ControllerBase
    {
        private readonly PropertyTypeService _propertyTypeService;

        public PropertyTypeController(PropertyTypeService propertyTypeService)
        {
            _propertyTypeService = propertyTypeService;
        }


        [HttpPost]
        public async Task<IActionResult> AddAsync([FromBody] PropertyType entity)
        {
            await _propertyTypeService.AddAsync(entity);
            return Ok(new { message = "Property type added successfully" });
        }


        [HttpGet]
        public async Task<ActionResult<List<PropertyTypeDto>>> GetAllAsync()
        {
            var result = await _propertyTypeService.GetAllAsync();
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PropertyTypeDto?>> GetByIdAsync(int id)
        {
            var result = await _propertyTypeService.GetByIdAsync(id);
            if (result == null)
                return NotFound(new { message = $"Property type with ID {id} not found" });

            return Ok(result);
        }



        [HttpPut]
        public IActionResult Update([FromBody] PropertyType entity)
        {
            _propertyTypeService.Update(entity);
            return Ok(new { message = "Property type updated successfully" });
        }

        [HttpDelete]
        public IActionResult Delete([FromBody] PropertyType entity)
        {
            _propertyTypeService.Delete(entity);
            return Ok(new { message = "Property type deleted successfully" });
        }
    }
}
