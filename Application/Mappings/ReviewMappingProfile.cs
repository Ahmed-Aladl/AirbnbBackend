using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PropertyDTOS;
using Application.DTOs.ReviewDTOs;
using Application.DTOs.UserDto;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    internal class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<Review, GuestReviewDTO>()
    .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
    .AfterMap((src, dest) => {
        if (dest.User == null)
            dest.User = new UserProfileDto();
        dest.User.UserId = src.UserId;
    })
    .ReverseMap();


            CreateMap<Review, AddReviewByGuestDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ReverseMap();

            CreateMap<Review, HostReviewDTO>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Property, opt => opt.MapFrom(src => src.Property))
                .AfterMap((src, dest) => {
                    if (dest.User == null)
                    {
                        dest.User = new UserProfileDto();
                    }
                    if (dest.Property == null)
                    {
                        dest.Property = new PropertyDisplayDTO();
                    }
                })
                .ReverseMap();

            CreateMap<Review, EditReviewByGuestDTO>()
                //.ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ReverseMap();


        }
    }
}
