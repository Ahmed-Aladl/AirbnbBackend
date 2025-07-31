using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Security.Claims;
using System.Threading.Tasks;
using Airbnb.Extensions;
using Application.DTOs.BookingDTOS;
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
   
    public class ReviewController : BaseController
    {
        //UnitOfWork _unit
        //public UnitOfWork Unit { get; }

        public BookingService book { get; }
        public Application.Services.ReviewService reviewservice { get; }
        public ReviewController(Application.Services.ReviewService rev , BookingService bookingservice)
        {
            this.reviewservice = rev;
            book = bookingservice;
            //Unit = _unit;
            //_Map = _map;
        }



        //[HttpGet("user/{userId}")]
        //public async Task<IActionResult> GetReviewsByUserId(int userId)
        //{
        //    Result<List<GuestReviewDTO>> result = await reviewservice.GetReviewsByUserId(userId);

        //    if (!result.IsSuccess)
        //        return StatusCode(result.StatusCode ?? 500, result.Message);
        //    else
        //        return Ok(result);
        //}

        //[HttpGet("property/{propertyId}")]
        //public async Task<IActionResult> GetReviewsByPropertyId(int propertyId)
        //{
        //    Result<List<GuestReviewDTO>> result = await reviewservice.GetReviewsByPropertyId(propertyId);

        //    if (!result.IsSuccess)
        //        return StatusCode(result.StatusCode ?? 500, result.Message);
        //    else
        //        return Ok(result);
        //}

        [HttpGet("all")]
        public async Task<IActionResult> GetAll()
        {
            Result<List<GuestReviewDTO>>  result = await reviewservice.GetAll();

            return ToActionResult(result);

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {

            Result<GuestReviewDTO> result = await reviewservice.GetById(id);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message); 
            else
            {
                return ToActionResult(result);
            }
        }


       //For private use 
        [HttpGet("user/{userId}")]
        [Authorize]
        public async Task<IActionResult> GetReviewsByUserId()
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Result<List<GuestReviewDTO>> result = await reviewservice.GetReviewsByUserId(UserId);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);
            else
                return ToActionResult(result);
        }

        //For public use
        [HttpGet("user/show/{userId}")]
        public async Task<IActionResult> GetPublicReviewsByUserId()
        {
            string UserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            Result<List<GuestReviewDTO>> result = await reviewservice.GetReviewsByUserId(UserId);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);
            else
                return ToActionResult(result);
        }





        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetReviewsByPropertyId(int propertyId)
        {
            Result<List<GuestReviewDTO>> result = await reviewservice.GetReviewsByPropertyId(propertyId);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);
            else
                return ToActionResult(result);
        }

        [Authorize]
        [HttpGet("user/{userId}/{propertyid}")]
        public async Task<IActionResult> GetReviewsByPropertyIdAndUser(int propertyId)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid user token.");
            }

            Result<List<GuestReviewDTO>> result = await reviewservice.GetReviewsByPropertyIdAndUser(propertyId, userId);
            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);
            else
                return ToActionResult(result);
        }

        //check

        [HttpGet("is-host/{userId}")]
        public async Task<IActionResult> IsUserHost(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            var result = await reviewservice.IsUserAHostAsync(userId);
            return ToActionResult(result);
        }


        //host

        [HttpGet("host/my-reviews")]
    // [Authorize]
        public async Task<IActionResult> GetReviewsForHostProperties()
        {
            string hostId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(hostId))
            {
                return Unauthorized("User not authenticated");
            }

            var result = await reviewservice.GetReviewsForHostProperties(hostId);
            return ToActionResult(result);
        }
        //For pivate use
        [HttpGet("host/{hostId}/reviews-with-properties")]
        [Authorize]
        public async Task<IActionResult> GetReviewsForHostPropertiesWithPropertyData()
        {
           string hostId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(hostId))
            {
                return Unauthorized("User not authenticated");
            }

            var result = await reviewservice.GetReviewsForHostPropertiesWithPropertyData(hostId);
            return ToActionResult(result);
        }


        //For public use
        [HttpGet("host/{hostId}/reviews-with-properties/public")]
        public async Task<IActionResult> GetPublicHostReviewsWithProperties(string hostId)
        {
            if (string.IsNullOrEmpty(hostId))
            {
                return BadRequest("User not authenticated");
            }

            var result = await reviewservice.GetReviewsForHostPropertiesWithPropertyData(hostId);
            return ToActionResult(result);
        }





        [HttpGet("host/property/{propertyId}/reviews")]
         [Authorize]
        public async Task<IActionResult> GetReviewsForSpecificHostProperty(int propertyId)
        {
            string hostId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(hostId))
            {
                return Unauthorized("User not authenticated");
            }

            var result = await reviewservice.GetReviewsForSpecificHostProperty(hostId, propertyId);
            return ToActionResult(result);
        }


        [HttpPost]
        //[Authorize]
        public async Task<IActionResult> Add(AddReviewByGuestDTO dto)
        {
            if (dto == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);


            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User not authenticated");
            }

            var result = await reviewservice.Add(dto,userId);

            return ToActionResult(result);

            //if (result.IsSuccess)
            //    return ToActionResult(result);
            //else
            //    return StatusCode(result.StatusCode ?? 500, result.Message);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, EditReviewByGuestDTO dto)
        {
            if (dto == null)
                return BadRequest("Review dto is required");

            //if (dto.Id != id)
            //    return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var result = await reviewservice.Edit(id, dto,userId);

            return ToActionResult(result);

        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var result = await reviewservice.Delete(id,userId);

            if (result.IsSuccess)
                return NoContent();
            else
                return StatusCode(result.StatusCode ?? 500, result.Message);
        }

        [HttpGet("/can-review/{propertyId}")]
        [Authorize]
        public async Task<IActionResult> CanUserReview(int propertyId)
        {
            string userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized("User not authenticated");

            var result = await reviewservice.CanUserReview(userId, propertyId);
            return ToActionResult(result);
        }

    }
}




#region before service

//public IMapper _Map { get; }

//Services.AddScoped<ReviewRepo>();

//[HttpGet]


//public async Task<IActionResult> getall()
//{
//    List<Review> r = await Unit.ReviewRepo.GetAllAsync();
//    List<GuestReviewDTO> reviewsDTO = new List<GuestReviewDTO>();
//    foreach (Review rr in r)
//    {
//        GuestReviewDTO reviewDTO = new GuestReviewDTO
//        {
//            UserId = rr.UserId,
//            PropertyId = rr.PropertyId,
//            BookingId = rr.BookingId,
//            Comment = rr.PublicReview,
//            Cleanliness = rr.Cleanliness ?? 0,
//            Accuracy = rr.Accuracy ?? 0,
//            Communication = rr.Communication ?? 0,
//            CheckIn = rr.CheckIn ?? 0,
//            Location = rr.Location ?? 0,
//            Value = rr.Value ?? 0
//        };
//        reviewsDTO.Add(reviewDTO);
//    }
//    return Ok(reviewsDTO);
//}



//List<Review> reviews = await Unit.ReviewRepo.GetAllAsync();


//[HttpGet("{id}")]
//public async Task<IActionResult> GetById(int id)
//{
//    Result<GuestReviewDTO> result = await reviewser.GetById(id);
//    Review? r = await Unit.ReviewRepo.GetByIdAsync(id);
//    if (r == null) return NotFound();
//    else
//    {
//        GuestReviewDTO reviewDTO = _Map.Map<GuestReviewDTO>(r);
//        return Ok(reviewDTO);
//    }
//} 




//[HttpPost]
//public async Task<IActionResult> Add(AddReviewByGuestDTO dto)
//{
//    if (dto == null) return BadRequest();
//    if (ModelState.IsValid)
//    {
//        Review r = new Review
//        {
//            PublicReview = dto.Comment,
//            Cleanliness = dto.Cleanliness,
//            Accuracy = dto.Accuracy,
//            Communication = dto.Communication,
//            CheckIn = dto.CheckIn,
//            Location = dto.Location,
//            Value = dto.Value
//        };

//        await Unit.ReviewRepo.AddAsync(r);
//        await Unit.SaveChangesAsync();
//        return Ok(r);
//    }
//    return BadRequest(ModelState);
//}



//[HttpPut("{id}")]
//public async Task<IActionResult> edit(int id, EditReviewByGuestDTO dto)
//{
//    if (dto == null) return BadRequest();
//    if (dto.Id != id) return BadRequest();

//    if (ModelState.IsValid)
//    {
//        Review r = new Review()
//        {

//            // Add these properties to the Review object in the edit method
//            Id = dto.Id,
//            UserId = dto.UserId,
//            PropertyId = dto.PropertyId,
//            BookingId = dto.BookingId,

//             PublicReview = dto.Comment,
//            Cleanliness = dto.Cleanliness,
//            Accuracy = dto.Accuracy,
//            Communication = dto.Communication,
//            CheckIn = dto.CheckIn,
//            Location = dto.Location,
//            Value = dto.Value
//        };



//        Unit.ReviewRepo.Update(r);
//       await Unit.SaveChangesAsync();    
//        return NoContent();
//    }
//    else
//        return BadRequest(ModelState);
//}


//[HttpDelete("{id}")]
//public async Task<IActionResult> delete(int id)
//{
//    Review r = Unit.ReviewRepo.GetById(id);
//    if (r == null) return NotFound();

//    Unit.ReviewRepo.Delete(r);
//    await Unit.SaveChangesAsync();
//    return NoContent();

//}

#endregion