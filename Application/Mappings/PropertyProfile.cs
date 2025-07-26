using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyDTOS;
using Application.Shared;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    public class PropertyProfile : Profile
    {
        public PropertyProfile()
        {
            CreateMap<Property, PropertyDisplayDTO>()
                .ForMember(dest => dest.isFavourite, opt => opt.MapFrom((src, dest, destMember, context) =>
                {
                    return src.WishlistProperties?.Count> 0;
                })
                ).ReverseMap();
            CreateMap<PaginatedResult<Property>, PaginatedResult<PropertyDisplayDTO>>()
                .ReverseMap();
        }
    }
}
