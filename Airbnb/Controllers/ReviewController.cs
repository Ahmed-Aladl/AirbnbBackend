﻿using System.Collections.Generic;
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
        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetReviewsByUserId(string userId)
        {
            Result<List<GuestReviewDTO>> result = await reviewservice.GetReviewsByUserId(userId);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);
            else
                return ToActionResult(result);
        }
        //property
        [HttpGet("property/{propertyId}")]
        public async Task<IActionResult> GetReviewsByPropertyId(int propertyId)
        {
            Result<List<GuestReviewDTO>> result = await reviewservice.GetReviewsByPropertyId(propertyId);

            if (!result.IsSuccess)
                return StatusCode(result.StatusCode ?? 500, result.Message);
            else
                return ToActionResult(result);
        }



        [HttpPost]
        public async Task<IActionResult> Add(AddReviewByGuestDTO dto)
        {
            
            if (dto == null)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await reviewservice.Add(dto);

            if (result.IsSuccess)
                return ToActionResult(result);
            else
                return StatusCode(result.StatusCode ?? 500, result.Message);


        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Edit(int id, EditReviewByGuestDTO dto)
        {
            if (dto == null)
                return BadRequest();

            if (dto.Id != id)
                return BadRequest("ID mismatch");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await reviewservice.Edit(id, dto);

            if (result.IsSuccess)
                return Ok(result.Data);
            else
                return StatusCode(result.StatusCode ?? 500, result.Message);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await reviewservice.Delete(id);

            if (result.IsSuccess)
                return NoContent();
            else
                return StatusCode(result.StatusCode ?? 500, result.Message);
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