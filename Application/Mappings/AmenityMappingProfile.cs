using Application.DTOs.AmenityDTOs;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings;

public class AmenityMappingProfile : Profile
{
    public AmenityMappingProfile()
    {
        CreateMap<Amenity, AmenityDTO>()
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.AmenityName));

        // DTO → Entity
        CreateMap<AmenityDTO, Amenity>()
            .ForMember(dest => dest.AmenityName, opt => opt.MapFrom(src => src.Name));

        // Create DTO → Entity
        CreateMap<CreateAmenityDTO, Amenity>()
            .ForMember(dest => dest.AmenityName, opt => opt.MapFrom(src => src.AmenityName))
            .ForMember(dest => dest.IconURL, opt => opt.MapFrom(src => src.IconUrl));


    }

}
