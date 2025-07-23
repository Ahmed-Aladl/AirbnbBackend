using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs;
using Application.DTOs.PropertyDTOS;
using Application.DTOs.PropertyType;
using Application.Shared;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    internal class PropertyTypeMappingProfile : Profile
    {
        public PropertyTypeMappingProfile()
        {
            CreateMap<PropertyType, PropertyTypeDto>().ReverseMap();
        }
    }
}
