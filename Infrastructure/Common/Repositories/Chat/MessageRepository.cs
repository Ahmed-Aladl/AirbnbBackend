using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories.Chat;
using Domain.Models.Chat;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories.Chat
{
    public class MessageRepository :Repository<Message,string>, IMessageRepository
    {
        public MessageRepository(AirbnbContext context):base(context)
        {
        }

        public async Task<Message> GetByIdAsync(string messageId)
        {
            return await Db.Messages
                .Include(m => m.Reactions)
                .Include(m => m.ReadStatuses)
                .Include(m => m.ReservationRequest)
                .FirstOrDefaultAsync(m => m.Id == messageId && !m.IsDeleted);
        }

        public async Task<List<Message>> GetChatMessagesAsync(string chatSessionId, int page, int pageSize)
        {
            return await Db.Messages
                .Include(m => m.Reactions)
                .Include(m => m.ReadStatuses)
                .Include(m => m.ReservationRequest)
                .Where(m => m.ChatSessionId == chatSessionId && !m.IsDeleted)
                .OrderByDescending(m => m.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Message> CreateAsync(Message message)
        {
            message.Id = message.Id != null? message.Id : Guid.NewGuid().ToString();
            message.CreatedAt = DateTime.UtcNow;

            Db.Messages.Add(message);
            await Db.SaveChangesAsync();
            return message;
        }

        public async Task UpdateAsync(Message message)
        {
            message.IsEdited = true;
            message.EditedAt = DateTime.UtcNow;

            Db.Messages.Update(message);
            await Db.SaveChangesAsync();
        }

        public async Task DeleteAsync(string messageId)
        {
            var message = await GetByIdAsync(messageId);
            if (message != null)
            {
                message.IsDeleted = true;
                await Db.SaveChangesAsync();
            }
        }

        public async Task<Message> GetLatestMessageAsync(string chatSessionId)
        {
            return await Db.Messages
                    .Where(m => m.ChatSessionId == chatSessionId && !m.IsDeleted)
                    .OrderByDescending(m => m.CreatedAt)
                    .FirstOrDefaultAsync();
        }

        public async Task<List<Message>> GetUnreadMessagesAsync(string chatSessionId, string userId)
        {
            return await Db.Messages
                .Where(m => m.ChatSessionId == chatSessionId &&
                           m.SenderId != userId &&
                           !m.IsDeleted)
                .Where(m => !Db.MessageReadStatuses
                    .Any(mrs => mrs.MessageId == m.Id && mrs.UserId == userId))
                .OrderBy(m => m.CreatedAt)
                .ToListAsync();
        }
    }
}
