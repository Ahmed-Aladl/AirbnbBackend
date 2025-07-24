using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chat.MessageReactionDtos;
using Application.Interfaces.IRepositories.Chat;
using Domain.Enums.Chat;
using Domain.Models.Chat;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories.Chat
{
    public class MessageReactionRepository : Repository<MessageReaction, string>, IMessageReactionRepository
    {
        public MessageReactionRepository(AirbnbContext _db) : base(_db)
        {
        }

        public async Task<MessageReaction> CreateAsync(MessageReaction reaction)
        {
            return (await Db.AddAsync(reaction)).Entity;
        }

        public async Task DeleteAsync(MessageReaction reaction)
        {
            Db.Remove(reaction);
        }

        public async Task<List<MessageReaction>> GetByMessageAsync(string messageId)
        {
            return await Db.MessageReactions
                                .Where(mr=> mr.MessageId ==  messageId)
                                .ToListAsync();
        }

        public async Task<MessageReaction> GetByMessageUserAndTypeAsync(string messageId, string userId, ReactionType reactionType)
        {
            return await  Db.MessageReactions
                .FirstOrDefaultAsync(mr => mr.MessageId == messageId &&
                                          mr.UserId == userId &&
                                          mr.ReactionType == reactionType.ToString()
                                );
        }

        //public Task<List<MessageReactionDto>> GetGroupedReactionsAsync(string messageId, string currentUserId)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
