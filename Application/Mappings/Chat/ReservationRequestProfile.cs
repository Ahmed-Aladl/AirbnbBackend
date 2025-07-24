using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chat.ReservationRequestDtos;
using AutoMapper;
using Domain.Models.Chat;

namespace Application.Mappings.Chat
{
    public class ReservationRequestProfile:Profile
    {
        public ReservationRequestProfile() 
        {
            CreateMap<ReservationRequest, ReservationRequestDto>().ReverseMap();
        }
    }
}
