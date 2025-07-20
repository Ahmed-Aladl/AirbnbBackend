using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.NotificationDTOs;
using Application.Interfaces;
using Application.Result;
using AutoMapper;
using Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Application.Services
{
    public class NotificationService
    {
        public IUnitOfWork _unitOfWork;
        public IMapper _mapper;

        public NotificationService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<List<Notification>>> GetUserNotifications(string userId)
        {
            var notifications = (
                await _unitOfWork.NotificationRepo.GetAllByUserIdAsync(userId)
            ).ToList();
            return Result<List<Notification>>.Success(notifications);
        }

        public async Task<Result<NotificationDto>> SendNotification(Notification notification)
        {
            await _unitOfWork.NotificationRepo.AddAsync(notification); // Ensure this method is awaited
            var notificationDto = _mapper.Map<NotificationDto>(notification); // Map the notification to NotificationDto
            return Result<NotificationDto>.Success(notificationDto); // Return the mapped DTO
        }

        public async Task<Result<bool>> MarkAsRead(int id)
        {
            await _unitOfWork.NotificationRepo.MarkAsReadAsync(id); // Await the method call
            return Result<bool>.Success(true); // Return a success result with a boolean value
        }

        public async Task<Result<bool>> DeleteNotification(int id)
        {
            await _unitOfWork.NotificationRepo.DeleteAsync(id); // Await the method call
            return Result<bool>.Success(true); // Return a success result with a boolean value
        }
    }
}
