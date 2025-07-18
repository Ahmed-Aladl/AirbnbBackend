using System.ComponentModel.DataAnnotations;

namespace Application.DTOs.AmenityDTOs;

public class CreateAmenityDTO
{
    [MaxLength(50)]
    public string AmenityName { get; set; }

    [MaxLength(200)]
    //[Url(ErrorMessage = "Please provide a valid URL.")]
    public string IconUrl { get; set; }
}
