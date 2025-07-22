using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.ReviewDTOs;
using Application.Interfaces;
using Application.Result;
using AutoMapper;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;


namespace Application.Services
{
    public class ReviewService
    {


        private readonly IUnitOfWork UWU;

        IMapper _map;

        public ReviewService(IUnitOfWork unitOfwork, IMapper mapper)
        {
            this._map = mapper;
            this.UWU = unitOfwork;
        }

        public async Task<Result<List<GuestReviewDTO>>> GetAll()
        {
            try
            {
                Console.WriteLine("there's an error here before get all async ");
                List<Review> reviews = await UWU.ReviewRepo.GetAllAsync();
                Console.WriteLine("there's an error here after get all async ");

                List<GuestReviewDTO> reviewsDTO = _map.Map<List<GuestReviewDTO>>(reviews);

                Console.WriteLine("there's an error here after mapping async ");

                return Result<List<GuestReviewDTO>>.Success(reviewsDTO);
            }
            catch (Exception)
            {
                return Result<List<GuestReviewDTO>>.Fail("Failed to retrieve reviews.", 500);
            }
        }




        public async Task<Result<GuestReviewDTO>> GetById(int id)
        {
            try
            {
                Review? review = await UWU.ReviewRepo.GetByIdAsync(id);



                if (review == null)
                    return Result<GuestReviewDTO>.Fail("Review not found.", 404);

                GuestReviewDTO guestReview = _map.Map<GuestReviewDTO>(review);

                return Result<GuestReviewDTO>.Success(guestReview);
            }
            catch (Exception)
            {
                return Result<GuestReviewDTO>.Fail("An error occurred while retrieving the review.", 500);
            }
        }


        public async Task<Result<GuestReviewDTO>> Add(AddReviewByGuestDTO dto)
        {
            if (dto == null)
                return Result<GuestReviewDTO>.Fail("Review data is required.", 400);

                Review review = await UWU.ReviewRepo.GetByBookingIdAsync(dto.BookingId);   //  _map.Map<Review>(dto);

                if (review == null)
                {
                    //_map.Map(dto, review);
                    
                    review = _map.Map<Review>(dto);

                    await UWU.ReviewRepo.AddAsync(review);


                    

                    await UWU.SaveChangesAsync();

                    GuestReviewDTO reviewDTO = _map.Map<GuestReviewDTO>(review);

                    return Result<GuestReviewDTO>.Success(reviewDTO);

                }

                return Result<GuestReviewDTO>.Fail("Review already exists for this booking.", 400);
            
            //} catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);

            //    return Result<GuestReviewDTO>.Fail("An error occurred while adding the review.", 500);
            //}


        }

        public async Task<Result<GuestReviewDTO>> Edit(int id, EditReviewByGuestDTO dto)
        {
            try
            {
                if (dto == null)
                    return Result<GuestReviewDTO>.Fail("Review data is required.", 400);

                if (dto.Id != id)
                    return Result<GuestReviewDTO>.Fail("Review ID mismatch.", 400);

                Review review = _map.Map<Review>(dto);

                UWU.ReviewRepo.Update(review);
                await UWU.SaveChangesAsync();

                GuestReviewDTO reviewDTO = _map.Map<GuestReviewDTO>(review);
                return Result<GuestReviewDTO>.Success(reviewDTO);
            }
            catch (Exception)
            {
                return Result<GuestReviewDTO>.Fail("An error occurred while updating the review.", 500);
            }
        }

        public async Task<Result<bool>> Delete(int id)
        {
            try
            {
                Review? review = await UWU.ReviewRepo.GetByIdAsync(id);

                if (review == null)
                    return Result<bool>.Fail("Review not found.", 404);

                UWU.ReviewRepo.Delete(review);
                await UWU.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Fail("An error occurred while deleting the review.", 500);
            }
        }



    }
}
