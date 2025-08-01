﻿using Airbnb.Extensions;
using Airbnb.Middleware;
using Application.DTOs.NotificationDTOs;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;

namespace Airbnb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseController
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService Notification { get; }

        public NotificationController(NotificationService _notification, IHubContext<NotificationHub> hubContext)

        {
            _hubContext = hubContext;
            Notification = _notification;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserNotifications(string userId)
        {
            var notifications = await Notification.GetUserNotifications(userId);
            return ToActionResult(notifications);
        }

        [HttpPost]
        public async Task<IActionResult> SendNotification([FromBody] NotificationDto dto)
        {
            var notification = new Notification
            {
                Message = dto.Message,
                CreatedAt = DateTime.UtcNow,
                isRead = false,
                UserId = dto.UserId
            };

            var result = await Notification.SendNotification(notification);

            await _hubContext.Clients.User(dto.UserId)
                .SendAsync("ReceiveNotification", dto.Message);
            return ToActionResult(result);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await Notification.MarkAsRead(id);
            return result.ToActionResult();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var result = await Notification.DeleteNotification(id);
            return result.ToActionResult();
        }
    }
}
