using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.ReviewDTOs;
using Application.DTOs.UserDto;
using Application.Interfaces;
using Application.Result;
using AutoMapper;
using Domain.Enums.Booking;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


namespace Application.Services
{
    public class ReviewService
    {


        private readonly IUnitOfWork UOW;

        IMapper _map;

        public ReviewService(IUnitOfWork unitOfwork, IMapper mapper)
        {
            this._map = mapper;
            this.UOW = unitOfwork;
        }


        //public async Task<Result<List<GuestReviewDTO>>> GetReviewsByUserId(string userId)
        //{
        //    try
        //    {
        //        var user = await UOW.UserRepo.GetById(userId); 
        //        if (user == null)
        //            return Result<List<GuestReviewDTO>>.Fail("User not found.", 404);

        //        // Get all reviews written by the specific user
        //        List<Review> reviews = await UOW.ReviewRepo.GetByUserIdAsync(userId);

        //        if (reviews == null || reviews.Count == 0)
        //            return Result<List<GuestReviewDTO>>.Success(new List<GuestReviewDTO>()); 

        //        List<GuestReviewDTO> reviewsDTO = _map.Map<List<GuestReviewDTO>>(reviews);

        //        return Result<List<GuestReviewDTO>>.Success(reviewsDTO);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error retrieving reviews for user {userId}: {ex.Message}");
        //        return Result<List<GuestReviewDTO>>.Fail("Failed to retrieve user reviews.", 500);
        //    }
        //}

        //public async Task<Result<List<GuestReviewDTO>>> GetReviewsByPropertyId(int propertyId)
        //{
        //    try
        //    {

        //        var property = await UOW.Properties.GetByIdAsync(propertyId); 
        //            return Result<List<GuestReviewDTO>>.Fail("Property not found.", 404);

        //        // Get all reviews for the specific property
        //        List<Review> reviews = await UOW.ReviewRepo.GetByPropertyIdAsync(propertyId);

        //        if (reviews == null || reviews.Count == 0)
        //            return Result<List<GuestReviewDTO>>.Success(new List<GuestReviewDTO>());

        //        List<GuestReviewDTO> reviewsDTO = _map.Map<List<GuestReviewDTO>>(reviews);

        //        return Result<List<GuestReviewDTO>>.Success(reviewsDTO);
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine($"Error retrieving reviews for property {propertyId}: {ex.Message}");
        //        return Result<List<GuestReviewDTO>>.Fail("Failed to retrieve property reviews.", 500);
        //    }
        //}



        public async Task<Result<List<GuestReviewDTO>>> GetAll()
        {
            try
            {
                Console.WriteLine("there's an error here before get all async ");
                List<Review> reviews = await UOW.ReviewRepo.GetAllAsync();
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
                Review? review = await UOW.ReviewRepo.GetByIdAsync(id);

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
            try
            {
            
            if (dto == null)
                return Result<GuestReviewDTO>.Fail("Review data is required.", 400);



                //var userBookings = await UOW.Bookings.GetBookingByUserIdAsync(dto.User.UserId);
                var userBookings = await UOW.Bookings.GetBookingByUserIdAsync(dto.UserId);

                var booking = userBookings
                    .Where(b => b.PropertyId == dto.PropertyId && b.BookingStatus == BookingStatus.Completed)
                    .OrderByDescending(b => b.CheckOutDate) // or b.CreatedAt
                    .FirstOrDefault();

                Console.WriteLine($"{booking.Id} booking id is here wheres the error ");

                
                // var booking = await UOW.Bookings.GetByIdAsync(dto.BookingId); //no need for now cause i got it above

                if (booking == null)
                return Result<GuestReviewDTO>.Fail("Booking not found.", 404);


                Review existingReview = await UOW.ReviewRepo.GetByBookingIdAsync(booking.Id);
           
                if (existingReview != null)
                return Result<GuestReviewDTO>.Fail("Review already exists for this booking.", 400);


            // if (booking.UserId != dto.User.UserId)
            if (booking.UserId != dto.UserId)
                return Result<GuestReviewDTO>.Fail("You are not authorized to review this booking.", 403);


            if (booking.BookingStatus != BookingStatus.Completed)
                return Result<GuestReviewDTO>.Fail("You can only review completed bookings.", 400);

            Review review = _map.Map<Review>(dto);

             review.BookingId = booking.Id; 


                await UOW.ReviewRepo.AddAsync(review);
            
                // Update property rating statistics if review has a rating
                if (dto.Rating > 0)
                {
                    // Get the property associated with this booking
                    var property = await UOW.PropertyRepo.GetByIdAsync(booking.PropertyId);
                    if (property != null)
                    {
                        // Get all existing reviews for this property to calculate new average
                        var existingPropertyReviews = await UOW.ReviewRepo.GetByPropertyIdAsync(booking.PropertyId);

                        // Calculate new average rating
                        var totalRating = existingPropertyReviews.Sum(r => r.Rating) + dto.Rating;
                        var totalReviewsCount = existingPropertyReviews.Count + 1;
                        var newAverageRating = (float)totalRating / totalReviewsCount;

                        // Update property statistics
                        property.ReviewCount = totalReviewsCount;
                        property.AverageRating = newAverageRating;

                        // Update the property
                        // maybe need to update the property repo 
                         UOW.PropertyRepo.Update(property);
                    }
                }


                await UOW.SaveChangesAsync();



                //var reviewWithUser = await UOW.ReviewRepo.getbyid(review.Id);

                //GuestReviewDTO reviewDTO = _map.Map<GuestReviewDTO>(review);

                // var reviewWithUser = await UOW.ReviewRepo.GetByIdWithUserAsync(review.Id);

                // var reviewWithUser = await UOW.ReviewRepo.GetByIdWithUserAsync(review.Id);

                //reviewDTO.User = _map.Map<UserProfileDto>(booking.User);


                GuestReviewDTO reviewDTO = _map.Map<GuestReviewDTO>(review);
                reviewDTO.User = _map.Map<UserProfileDto>(booking.User); //i can get it from user too 
                return Result<GuestReviewDTO>.Success(reviewDTO);
            
            } 
                catch (Exception e)
                 {
                   Console.WriteLine(e.Message);
                   return Result<GuestReviewDTO>.Fail("An error occurred while adding the review.", 500);
            }


        }

        public async Task<Result<GuestReviewDTO>> Edit(int id, EditReviewByGuestDTO dto)
        {
            try
            {
                if (dto == null)
                    return Result<GuestReviewDTO>.Fail("Review data is required.", 400);

                //if (dto.Id != id)
                //    return Result<GuestReviewDTO>.Fail("Review ID mismatch.", 400);

                Review existingReview = await UOW.ReviewRepo.GetByIdAsync(id); // _map.Map<Review>(dto);

                if (existingReview == null)
                    return Result<GuestReviewDTO>.Fail("Review not found.", 404);

                var oldRating = existingReview.Rating;

                _map.Map(dto, existingReview);

                // Update property rating statistics if rating changed
                if (oldRating != existingReview.Rating)
                {
                    // Get the booking to find the property
                    var booking = await UOW.Bookings.GetByIdAsync(existingReview.BookingId);
                    if (booking != null)
                    {
                        var property = await UOW.PropertyRepo.GetByIdAsync(booking.PropertyId);
                        if (property != null)
                        {
                            // Get all reviews for this property
                            var propertyReviews = await UOW.ReviewRepo.GetByPropertyIdAsync(booking.PropertyId);

                            // Calculate new average rating with the updated review
                            var reviewsWithRating = propertyReviews.Where(r => r.Rating > 0).ToList();

                            if (reviewsWithRating.Any())
                            {
                                var totalRating = reviewsWithRating.Sum(r => r.Rating);
                                var newAverageRating = (float)totalRating / reviewsWithRating.Count;

                                property.ReviewCount = reviewsWithRating.Count;
                                property.AverageRating = newAverageRating;

                                UOW.PropertyRepo.Update(property);
                            }
                        }
                    }
                }

                await UOW.SaveChangesAsync();

                GuestReviewDTO reviewDTO = _map.Map<GuestReviewDTO>(existingReview);
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
                Review? review = await UOW.ReviewRepo.GetByIdAsync(id);

                if (review == null)
                    return Result<bool>.Fail("Review not found.", 404);

                UOW.ReviewRepo.Delete(review);
                await UOW.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception)
            {
                return Result<bool>.Fail("An error occurred while deleting the review.", 500);
            }
        }


        public async Task<Result<List<GuestReviewDTO>>> GetReviewsByUserId(string userId)
        {
            try
            {                
                List<Review> reviews = await UOW.ReviewRepo.GetByUserIdAsync(userId);

                if (reviews == null || reviews.Count == 0)
                    return Result<List<GuestReviewDTO>>.Success(new List<GuestReviewDTO>()); 

                List<GuestReviewDTO> reviewsDTO = _map.Map<List<GuestReviewDTO>>(reviews);

                return Result<List<GuestReviewDTO>>.Success(reviewsDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving reviews for user {userId}: {ex.Message}");
                return Result<List<GuestReviewDTO>>.Fail("Failed to retrieve user reviews.", 500);
            }
        }

        public async Task<Result<List<GuestReviewDTO>>> GetReviewsByPropertyId(int propertyId)
        {
            try
            {
                var property = await UOW.PropertyRepo.GetByIdAsync(propertyId);
                if (property == null)
                    return Result<List<GuestReviewDTO>>.Fail("Property not found.", 404);

                List<Review> reviews = await UOW.ReviewRepo.GetByPropertyIdAsync(propertyId);

                if (reviews == null || reviews.Count == 0)
                    return Result<List<GuestReviewDTO>>.Success(new List<GuestReviewDTO>()); // Return empty list instead of error

                List<GuestReviewDTO> reviewsDTO = _map.Map<List<GuestReviewDTO>>(reviews);

                return Result<List<GuestReviewDTO>>.Success(reviewsDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving reviews for property {propertyId}: {ex.Message}");
                return Result<List<GuestReviewDTO>>.Fail("Failed to retrieve property reviews.", 500);
            }
        }

        public async Task<Result<bool>> CanUserReview(string userId, int propertyId)
        {
            try
            {
                var userBookings = await UOW.Bookings.GetBookingByUserIdAsync(userId);

                var booking = userBookings
                    .Where(b => b.PropertyId == propertyId && b.BookingStatus == BookingStatus.Completed)
                    .OrderByDescending(b => b.CheckOutDate)
                    .FirstOrDefault();

                if (booking == null)
                    return Result<bool>.Success(false); // No eligible booking

                var existingReview = await UOW.ReviewRepo.GetByBookingIdAsync(booking.Id);

                if (existingReview != null)
                    return Result<bool>.Success(false); // Already reviewed

                return Result<bool>.Success(true); // Eligible to review
            }
            catch (Exception)
            {
                return Result<bool>.Fail("An error occurred while checking review eligibility.", 500);
            }
        }







        //public async Task<Result<List<GuestReviewDTO>>> GetByHostId(int hostId)
        //{
        //    try
        //    {
        //        List<Review> reviews = await UOW.ReviewRepo.GetByHostIdAsync(hostId);
        //        if (reviews == null || !reviews.Any())
        //            return Result<List<GuestReviewDTO>>.Fail("No reviews found for this host.", 404);
        //        List<GuestReviewDTO> reviewsDTO = _map.Map<List<GuestReviewDTO>>(reviews);
        //        return Result<List<GuestReviewDTO>>.Success(reviewsDTO);
        //    }
        //    catch (Exception)
        //    {
        //        return Result<List<GuestReviewDTO>>.Fail("An error occurred while retrieving the reviews.", 500);
        //    }
        //}







    }
}
