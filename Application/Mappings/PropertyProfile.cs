using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyDTOS;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    public class PropertyProfile : Profile
    {
        public PropertyProfile()
        {
            CreateMap<Property, PropertyDisplayDTO>().ReverseMap();
        }
    }
}
