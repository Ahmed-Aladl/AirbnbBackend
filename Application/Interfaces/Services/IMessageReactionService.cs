using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chat.MessageReactionDtos;
using Domain.Enums.Chat;

namespace Application.Interfaces.Services
{
    public interface IMessageReactionService
    {
        Task<MessageReactionDto> ToggleReactionAsync(string messageId, string userId, ReactionType reactionType);
        Task<List<MessageReactionDto>> GetMessageReactionsAsync(string messageId, string currentUserId);
        Task<bool> RemoveReactionAsync(string messageId, string userId, ReactionType reactionType);
    }
}
