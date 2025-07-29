using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.HostReply;
using Application.DTOs.ReviewDTOs;
using Application.Interfaces;
using Application.Result;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class HostReplyService
    {



        public IUnitOfWork UnitOfWork { get; }
        public IMapper Mapper { get; }
        public HostReplyService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            UnitOfWork = unitOfWork;
            Mapper = mapper;
        }

        public async Task<Result<List<HostReviewReplyDto>>> GetAll()
        {
            try
            {
                var replies = await UnitOfWork.HostReviewRepo.GetAllAsync();
                var repliesDTO = Mapper.Map<List<HostReviewReplyDto>>(replies);
                return Result<List<HostReviewReplyDto>>.Success(repliesDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetAll: {ex.Message}");
                return Result<List<HostReviewReplyDto>>.Fail("Failed to retrieve host replies.", 500);
            }
        }

        public async Task<Result<HostReviewReplyDto>> GetById(int id)
        {
            try
            {
                var reply = await UnitOfWork.HostReviewRepo.GetByIdAsync(id);
                if (reply == null)
                    return Result<HostReviewReplyDto>.Fail("Host reply not found.", 404);

                var replyDTO = Mapper.Map<HostReviewReplyDto>(reply);
                return Result<HostReviewReplyDto>.Success(replyDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetById: {ex.Message}");
                return Result<HostReviewReplyDto>.Fail("An error occurred while retrieving the host reply.", 500);
            }
        }


        public async Task<Result<List<HostReviewReplyDto>>> GetByPropertyId(int propertyId)
        {
            try
            {
                var replies = await UnitOfWork.HostReviewRepo.GetByPropertyIdAsync(propertyId);
                if (replies == null || !replies.Any())
                    return Result<List<HostReviewReplyDto>>.Fail("No host replies found for this property.", 404);

                var replyDTOs = Mapper.Map<List<HostReviewReplyDto>>(replies);
                return Result<List<HostReviewReplyDto>>.Success(replyDTOs);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByPropertyId: {ex.Message}");
                return Result<List<HostReviewReplyDto>>.Fail("An error occurred while retrieving the host replies.", 500);
            }
        }


        public async Task<Result<HostReviewReplyDto>> GetByReviewId(int reviewId)
        {
            try
            {
                var reply = await UnitOfWork.HostReviewRepo.GetByReviewIdAsync(reviewId);
                if (reply == null)
                    return Result<HostReviewReplyDto>.Fail("No reply found for this review.", 404);

                var replyDTO = Mapper.Map<HostReviewReplyDto>(reply);
                return Result<HostReviewReplyDto>.Success(replyDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByReviewId: {ex.Message}");
                return Result<HostReviewReplyDto>.Fail("An error occurred while retrieving the host reply.", 500);
            }
        }

        public async Task<Result<List<HostReviewReplyDto>>> GetByHostId(string hostId)
        {
            try
            {
                var replies = await UnitOfWork.HostReviewRepo.GetByHostIdAsync(hostId);
                var repliesDTO = Mapper.Map<List<HostReviewReplyDto>>(replies);
                return Result<List<HostReviewReplyDto>>.Success(repliesDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in GetByHostId: {ex.Message}");
                return Result<List<HostReviewReplyDto>>.Fail("Failed to retrieve host replies.", 500);
            }
        }

        public async Task<Result<HostReviewReplyDto>> Add(AddHostReply dto)
        {
            try
            {
                if (dto == null)
                    return Result<HostReviewReplyDto>.Fail("Reply data is required.", 400);

                var review = await UnitOfWork.ReviewRepo.GetByIdAsync(dto.ReviewId);
                if (review == null)
                    return Result<HostReviewReplyDto>.Fail("Review not found.", 404);

                var property = await UnitOfWork.PropertyRepo.GetByIdAsync(review.PropertyId);
                if (property == null || property.HostId!= dto.HostId)
                    return Result<HostReviewReplyDto>.Fail("You can only reply to reviews of your own properties.", 403);

                var existingReply = await UnitOfWork.HostReviewRepo.GetByReviewIdAsync(dto.ReviewId);
                if (existingReply != null)
                    return Result<HostReviewReplyDto>.Fail("You have already replied to this review.", 400);

                var reply = Mapper.Map<HostReply>(dto);
                await UnitOfWork.HostReviewRepo.AddAsync(reply);
                await UnitOfWork.SaveChangesAsync();

                var fullReply = await UnitOfWork.HostReviewRepo.GetByIdAsync(reply.Id);
                var replyDTO = Mapper.Map<HostReviewReplyDto>(fullReply);

                return Result<HostReviewReplyDto>.Success(replyDTO);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Add: {ex.Message}");
                return Result<HostReviewReplyDto>.Fail("An error occurred while adding the reply.", 500);
            }
        }

        public async Task<Result<HostReviewReplyDto>> Edit(int id, HostReviewEditReplyDTO dto)
        {
            try
            {
                if (dto == null)
                    return Result<HostReviewReplyDto>.Fail("Reply data is required.", 400);

                if (dto.Id != id)
                    return Result<HostReviewReplyDto>.Fail("Reply ID mismatch.", 400);

                var existingReply = await UnitOfWork.HostReviewRepo.GetByIdAsync(id);
                if (existingReply == null)
                    return Result<HostReviewReplyDto>.Fail("Host reply not found.", 404);

                if (existingReply.UserId != dto.UserId)
                    return Result<HostReviewReplyDto>.Fail("You can only edit your own replies.", 403);

                existingReply.Comment = dto.Comment;

                UnitOfWork.HostReviewRepo.Update(existingReply);
                await UnitOfWork.SaveChangesAsync();

                var replyDTO = Mapper.Map<HostReviewReplyDto>(existingReply);
                return Result<HostReviewReplyDto>.Success(replyDTO);

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Edit: {ex.Message}");
                return Result<HostReviewReplyDto>.Fail("An error occurred while updating the reply.", 500);
            }
        }

        public async Task<Result<bool>> Delete(int id, string userId)
        {
            try
            {
                var reply = await UnitOfWork.HostReviewRepo.GetByIdAsync(id);
                if (reply == null)
                    return Result<bool>.Fail("Host reply not found.", 404);

                if (reply.UserId != userId)
                    return Result<bool>.Fail("You can only delete your own replies.", 403);

                UnitOfWork.HostReviewRepo.Delete(reply);
                await UnitOfWork.SaveChangesAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Delete: {ex.Message}");
                return Result<bool>.Fail("An error occurred while deleting the reply.", 500);
            }
        }




    }
}
