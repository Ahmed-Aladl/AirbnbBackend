using Airbnb.Extensions;
using Airbnb.Middleware;
using Application.DTOs.NotificationDTOs;
using Application.Services;
using Domain.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.AspNetCore.Authorization;

namespace Airbnb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseController
    {
        private readonly IHubContext<NotificationHub> _hubContext;
        public NotificationService Notification { get; }

        public NotificationController(NotificationService notification, IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
            Notification = notification;
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

            // إرسال Real-time notification
            await _hubContext.Clients.User(dto.UserId)
                .SendAsync("ReceiveNotification", new
                {
                    message = dto.Message,
                    createdAt = DateTime.UtcNow,
                    id = notification.Id
                });

            return ToActionResult(result);
        }

        [HttpPut("{id}/read")]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var result = await Notification.MarkAsRead(id);
            return ToActionResult(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteNotification(int id)
        {
            var result = await Notification.DeleteNotification(id);
            return ToActionResult(result);
        }
    }
}