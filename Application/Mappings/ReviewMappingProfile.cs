using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.ReviewDTOs;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    internal class ReviewMappingProfile : Profile
    {
        public ReviewMappingProfile()
        {
            CreateMap<Review, GuestReviewDTO>().ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User)).ReverseMap();
                //.AfterMap((src, dest) => { dest.UserId = src.UserId; })
            CreateMap<Review, AddReviewByGuestDTO>().ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User)).ReverseMap();
            
            CreateMap<Review, EditReviewByGuestDTO>().ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User)).ReverseMap();

        }
    }
}
