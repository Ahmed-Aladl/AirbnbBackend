using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chat.MessageReactionDtos;
using Application.DTOs.Chat.ReservationRequestDtos;
using Domain.Enums.Chat;

namespace Application.DTOs.Chat.MessageDtos
{
    public class MessageDto
    {
        public string Id { get; set; }
        public string ChatSessionId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string SenderAvatarUrl { get; set; }
        public string MessageText { get; set; }
        public string MessageType { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? EditedAt { get; set; }
        public bool IsHost { get; set; }
        public bool IsOwnMessage { get; set; }
        public bool IsRead { get; set; }
        public List<MessageReactionDto> Reactions { get; set; } = new();
        public ReservationRequestDto ReservationRequest { get; set; }
    }
}
