using Application.DTOs.AmenityDTOs;
using Application.Result;
using Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace Airbnb.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AmenityController : ControllerBase
{
    private readonly AmenityService _amenityService;

    public AmenityController(AmenityService amenityService)
    {
        _amenityService = amenityService;
    }


    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetAmenityById(int id)
    {
        var result = await _amenityService.GetAmenityById(id);

        if (!result.IsSuccess)
        {
            return StatusCode(result.StatusCode ?? 400, result);
        }

        return Ok(result);
    }

    [HttpPost("Create")]
    [Consumes("multipart/form-data")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<Result<AmenityDTO>>> CreateAmenity([FromForm] CreateAmenityDTO createAmenityDto)
    {
        var result = await _amenityService.CreateAsync(createAmenityDto);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode ?? 400, result);

        return Ok(result);
    }


    //GEt All Amenities related to a specific property   
    [HttpGet("property/{propertyId}/amenities")]
    public async Task<ActionResult<Result<IEnumerable<AmenityDTO>>>> GetAmenitiesByPropertyId(int propertyId)
    {
        var result = await _amenityService.GetAllAmenitiesByPropertyIdAsync(propertyId);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode ?? 500, result);

        return Ok(result);
    }

    //display all amenities and host select from 
    [HttpGet("all")]
    public async Task<ActionResult<Result<IEnumerable<AmenityDTO>>>> GetAllAmenities()
    {
        var result = await _amenityService.GetAllAmenitiesAsync();

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode ?? 500, result);

        return Ok(result);
    }



    [HttpPut("{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<Result<AmenityDTO>>> Edit(int id, AmenityDTO amenityDTO)
    {
        var result = await _amenityService.Update(amenityDTO);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode ?? 400, result);

        return Ok(result);
    }


    [HttpDelete("{id}")]
    //[Authorize(Roles = "Admin")]
    public async Task<ActionResult<Result<string>>> DeleteAmenity(int id)
    {
        var result = await _amenityService.DeleteAsync(id);

        if (!result.IsSuccess)
            return StatusCode(result.StatusCode ?? 400, result);

        return Ok(result);
    }

}
