using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.UserDto;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserProfileDto>().ReverseMap();
        }
    }
}
