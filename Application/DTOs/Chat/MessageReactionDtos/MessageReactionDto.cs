using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Enums.Chat;

namespace Application.DTOs.Chat.MessageReactionDtos
{
    public class MessageReactionDto
    {
        public string Id { get; set; }
        public string MessageId { get; set; }
        public ReactionType ReactionType { get; set; }
        public List<ReactionUserDto> Users { get; set; } = new();
        public int Count { get; set; }
        public bool HasCurrentUserReacted { get; set; }
    }
}
