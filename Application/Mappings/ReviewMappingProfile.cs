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
    internal class ReviewMappingProfile :Profile
    {

        public ReviewMappingProfile()
        {
            CreateMap<Review, GuestReviewDTO>().ReverseMap();
            CreateMap<Review, GuestReviewDTO>().ReverseMap();
            CreateMap<Review, AddReviewByGuestDTO>().ReverseMap();
            CreateMap<Review, EditReviewByGuestDTO>().ReverseMap();
        }

    }
}
