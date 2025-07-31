using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chat.MessageDtos;
namespace Application.DTOs.Chat.ChatSessionDtos
{
    public class ChatSessionDto
    {
        public string Id { get; set; }
        public int PropertyId { get; set; }
        public string PropertyTitle { get; set; }
        public string PropertyImageUrl { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public string UserAvatarUrl { get; set; }
        public string HostId { get; set; }
        public string HostName { get; set; }
        public string HostAvatarUrl { get; set; }
        public DateTime LastActivityAt { get; set; }
        public string LastMessageText { get; set; }
        public int UnreadCount { get; set; }
        public bool HasPendingRequests { get; set; }
        public bool IsActive { get; set; }
        public bool IsHost {  get; set; }  
    }
}
