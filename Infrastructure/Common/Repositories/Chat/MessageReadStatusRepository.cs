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
    public class MessageReadStatusRepository: Repository<MessageReadStatus,string>, IMessageReadStatusRepository
    {
        public MessageReadStatusRepository(AirbnbContext context) : base(context) { }

        public async Task<MessageReadStatus> GetByMessageAndUserAsync(string messageId, string userId)
        {
            return await Db.MessageReadStatuses.Where(mrs => mrs.UserId == userId && mrs.MessageId == messageId).FirstOrDefaultAsync();

        }
        public async Task CreateAsync(MessageReadStatus readStatus)
        {
            Db.MessageReadStatuses.Add(readStatus);
        }
        public async Task MarkMessagesAsReadAsync(string chatSessionId, string userId, DateTime readAt)
        {
            
            // Get all unread messages for this user in the session
            var unreadMessageIds = await Db.Messages
                .Where(m => m.ChatSessionId == chatSessionId && m.SenderId != userId)
                .Where(m => !Db.MessageReadStatuses
                    .Any(mrs => mrs.MessageId == m.Id && mrs.UserId == userId))
                .Select(m => m.Id)
                .ToListAsync();

            // Insert read statuses
            var newReadStatuses = unreadMessageIds.Select(id => new MessageReadStatus
            {
                MessageId = id,
                UserId = userId,
                ReadAt = readAt
            });

            await Db.MessageReadStatuses.AddRangeAsync(newReadStatuses);

            // Reset the unread count for the current user
            var chatSession = await Db.ChatSessions.FindAsync(chatSessionId);
            if (chatSession != null)
            {
                if (chatSession.UserId == userId)
                    chatSession.UnreadCountForUser = 0;
                else
                    chatSession.UnreadCountForHost = 0;
            }

        }

        //Task<List<MessageReadStatus>> GetByUserAsync(string userId);
    }
}
