using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chat.MessageReactionDtos;
using Domain.Enums.Chat;
using Domain.Models.Chat;

namespace Application.Interfaces.IRepositories.Chat
{
    public interface IMessageReactionRepository
    {
        Task<MessageReaction> GetByMessageUserAndTypeAsync(string messageId, string userId, ReactionType reactionType);
        Task<List<MessageReaction>> GetByMessageAsync(string messageId);
        Task<MessageReaction> CreateAsync(MessageReaction reaction);
        Task DeleteAsync(MessageReaction reaction);
        //Task<List<MessageReactionDto>> GetGroupedReactionsAsync(string messageId, string currentUserId);
    }
}
