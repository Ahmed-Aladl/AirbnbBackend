using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.WishlistDTOs;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    public class WishlistProfile : Profile
    {
        public WishlistProfile()
        {
            CreateMap<Wishlist, WishlistDTO>()
           .ForMember(dest => dest.PropertyIds, opt => opt.MapFrom(src => src.WishlistProperties.Select(wp => wp.PropertyId)));

            CreateMap<CreateWishlistDTO, Wishlist>()
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        }
    }
}
