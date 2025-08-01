﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.HostReply;
using AutoMapper;
using Domain.Models;

namespace Application.Mappings
{
    internal class HostReplyMappingProfile : Profile
    {

        public HostReplyMappingProfile()
        {
            CreateMap<HostReviewReplyDto, HostReply>()
                .AfterMap((src, dest) => { dest.UserId = src.HostId; })
                .AfterMap((src, dest) => { dest.ReviewId = src.ReviewId; }).ReverseMap();

            CreateMap<HostReviewEditReplyDTO, HostReply>().ReverseMap();

            CreateMap<AddHostReply, HostReply>().AfterMap((src, dest) => { dest.UserId = src.HostId; }).ReverseMap();

        }

    }
}
