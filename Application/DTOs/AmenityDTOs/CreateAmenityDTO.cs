using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Application.DTOs.AmenityDTOs;

public class CreateAmenityDTO
{
    [MaxLength(50)]
    [Required(ErrorMessage = "Amenity name is required.")]
    public string AmenityName { get; set; }

    //[MaxLength(200)]
    //[Url(ErrorMessage = "Please provide a valid URL.")]
    [Required(ErrorMessage = "You Should Upload Icon ")]
    public IFormFile IconUrl { get; set; }
}
