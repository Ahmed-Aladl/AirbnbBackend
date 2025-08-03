using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Application.DTOs.NotificationDTOs;
using Application.Interfaces;
using Application.Result;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class NotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<NotificationDto>>> GetUserNotifications(string userId)
        {
            var notifications = (await _unitOfWork.NotificationRepo.GetAllByUserIdAsync(userId)).ToList();
            var notificationDtos = _mapper.Map<List<NotificationDto>>(notifications);
            return Result<List<NotificationDto>>.Success(notificationDtos);
        }

        public async Task<Result<NotificationDto>> SendNotification(Notification notification)
        {

            await _unitOfWork.NotificationRepo.AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();

            var notificationDto = _mapper.Map<NotificationDto>(notification);
            return Result<NotificationDto>.Success(notificationDto);
        }

        public async Task<Result<bool>> MarkAsRead(int id)
        {

            await _unitOfWork.NotificationRepo.MarkAsReadAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Success(true);

        }

        public async Task<Result<bool>> DeleteNotification(int id)
        {

            await _unitOfWork.NotificationRepo.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
            return Result<bool>.Success(true);

        }
    }
}