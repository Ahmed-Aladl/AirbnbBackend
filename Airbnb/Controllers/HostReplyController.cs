using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using System.Threading.Tasks;
using Airbnb.Extensions;
using Application.DTOs.BookingDTOS;
using Application.DTOs.HostReply;
using Application.DTOs.ReviewDTOs;
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Application.Result;
using Application.Result;
using Application.Services;
using AutoMapper;
using Domain.Enums.Booking;
using Domain.Models;
using Infrastructure.Common;
using Infrastructure.Common.Repositories;
using Infrastructure.Contexts;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe;
namespace Airbnb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HostReplyController : ControllerBase
    {
        public HostReplyController(HostReplyService hostr)
        {
            Hostr = hostr;
        }

        public HostReplyService Hostr { get; }


        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
        
            var result = await Hostr.GetAll();
            return result.ToActionResult();
        
        
        
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        { 
        
            var result = await Hostr.GetById(id);
            if(!result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? 500, result.Message);
            }
            else
                return result.ToActionResult();
        }

        [HttpGet("review/{reviewId}")]
        public async Task<IActionResult> GetByReviewId(int reviewId)
        {
        var result = await Hostr.GetByReviewId(reviewId);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? 500, result.Message);
            }
            else
                return result.ToActionResult();
        }


        [HttpGet("host/{hostId}")]
        public async Task<IActionResult> GetByHostId(string hostId)
        {
        var result = await Hostr.GetByHostId(hostId);
            if (!result.IsSuccess)
            {
                return StatusCode(result.StatusCode ?? 500, result.Message);
            }
            else
                return result.ToActionResult();
        }


            //[Authorize]  
            //[HttpGet("{id}")]
            //public async Task<IActionResult> GetByPropertyAndUserId(int id)
            //{
            //    try
            //    {
            //        // Get user ID from JWT token claims
            //        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            //        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            //        {
            //            return Unauthorized("Invalid user token.");
            //        }

            //        var result = await _hostReviewReplyService.GetByPropertyAndUserId(id);

            //        if (!result.IsSuccess)
            //        {
            //            return StatusCode(result.StatusCode, new { message = result.ErrorMessage });
            //        }

            //        return Ok(result.Data);
            //    }
            //    catch (Exception ex)
            //    {
            //        // Log the exception here if you have a logging framework
            //        return StatusCode(500, new { message = "An internal server error occurred." });
            //    }
            //}
  

        [HttpPost]
        public async Task<IActionResult> Add(AddHostReply dto)
        {

            if (dto == null)
                return BadRequest("DTO cannot be null");
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await Hostr.Add(dto);


            if (result.IsSuccess)
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Id }, result.Data);
            else
                return StatusCode(result.StatusCode ?? 500, result.Message);



        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, HostReviewEditReplyDTO dto)
        {
            if (dto == null)
                return BadRequest();

            if (dto.Id != id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await Hostr.Edit(id, dto); 

            if (result.IsSuccess)
                return Ok(result.Data);
            else
                return StatusCode(result.StatusCode ?? 500, result.Message);
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id,  string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("UserId is required");

            var result = await Hostr.Delete(id, userId); 

            if (result.IsSuccess)
                return NoContent();
            else
                return StatusCode(result.StatusCode ?? 500, result.Message);
        }





    }
}

