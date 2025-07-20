using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.PaymentDTOs;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    public class PaymentProfile : Profile
    {
        public PaymentProfile()
        {
            CreateMap<Payment, PaymentDTO>()
                .ForMember(dest => dest.PaymentIntentId, opt => opt.MapFrom(src => src.StripePaymentIntentId));
        }
    }
}
