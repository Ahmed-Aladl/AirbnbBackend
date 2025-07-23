using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Threading.Tasks;
using Airbnb.Extensions;
using Application.DTOs.ReviewDTOs;
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Application.Result;
using Application.Result;
using Application.Services;
using AutoMapper;
using Domain.Models;
using Infrastructure.Common;
using Infrastructure.Common.Repositories;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Application.DTOs.BookingDTOS;
using Domain.Enums.Booking;
namespace Airbnb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostReplyController : ControllerBase
    {

      

    }
}

