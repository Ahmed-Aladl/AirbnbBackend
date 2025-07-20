using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyImageDTOs;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    public class PropertyImageProfile : Profile
    {
        public PropertyImageProfile()
        {
            CreateMap<PropertyImage, PropertyImageCreateDTO>().ReverseMap();
            CreateMap<PropertyImage, PropertyImageDisplayDTO>().ReverseMap();
        }
    }
}
