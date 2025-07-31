using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chat.ChatSessionDtos;
using Application.Interfaces.IRepositories.Chat;
using AutoMapper;
using Domain.Models.Chat;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories.Chat
{
    public class ChatSessionRepository : Repository<ChatSession, string>, IChatSessionRepository
    {
        public ChatSessionRepository(AirbnbContext _db) : base(_db)
        {
        }

        public async Task<ChatSession> GetByIdAsync(string chatSessionId)
        {
            return await Db.ChatSessions
                .Include(cs => cs.Property)
                .Include(cs => cs.User)
                .FirstOrDefaultAsync(cs => cs.Id == chatSessionId);
        }

        public async Task<ChatSession> GetByPropertyAndUserAsync(int propertyId, string userId)
        {
            return await Db.ChatSessions
                .Include(cs => cs.Property)
                .Include(cs => cs.User)
                .FirstOrDefaultAsync(cs => cs.PropertyId == propertyId && cs.UserId == userId);
        }

        public async Task<List<ChatSession>> GetUserChatSessionsAsync(string userId, int page, int pageSize)
        {
            return await Db.ChatSessions
                .Include(cs => cs.Property)
                .Include(cs => cs.User)
                .Where(cs => cs.UserId == userId && cs.IsActive)
                .OrderByDescending(cs => cs.LastActivityAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<ChatSession>> GetHostChatSessionsAsync(string hostId, int page, int pageSize)
        {
            return await Db.ChatSessions
                .Include(cs => cs.Property)
                .Include(cs => cs.User)
                .Where(cs => cs.HostId == hostId && cs.IsActive)
                .OrderByDescending(cs => cs.LastActivityAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }
        public async Task<List<ChatSession>> GetAllUserChatSessionsAsync(string userId, int page, int pageSize)
        {
            return await Db.ChatSessions
                .Include(cs => cs.Property)
                .Where(cs => (cs.HostId == userId || cs.UserId == userId) && cs.IsActive)
                .OrderByDescending(cs => cs.LastActivityAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<ChatSession> CreateAsync(ChatSession chatSession)
        {
            chatSession.Id = chatSession.Id != null ? chatSession.Id : Guid.NewGuid().ToString();
            chatSession.CreatedAt = DateTime.UtcNow;
            chatSession.LastActivityAt = DateTime.UtcNow;

            Db.ChatSessions.Add(chatSession);
            await Db.SaveChangesAsync();
            return chatSession;
        }

        public async Task UpdateAsync(ChatSession chatSession)
        {
            Db.ChatSessions.Update(chatSession);
            await Db.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(int propertyId, string userId)
        {
            return await Db.ChatSessions
                .AnyAsync(cs => cs.PropertyId == propertyId && cs.UserId == userId);
        }

        public async Task<int> GetUnreadCountAsync(string chatSessionId, string userId)
        {
            return await Db.Messages
                .Where(m => m.ChatSessionId == chatSessionId &&
                           m.SenderId != userId &&
                           !m.IsDeleted)
                .Where(m => !Db.MessageReadStatuses
                    .Any(mrs => mrs.MessageId == m.Id && mrs.UserId == userId))
                .CountAsync();
        }
        public async Task<int> GetUnreadCountSoftAsync(string chatSessionId, string userId)
        {
            var chatSession = await Db.ChatSessions.FindAsync(chatSessionId);
            if (chatSession == null)
                return 0;
            return chatSession?.UserId == userId ? chatSession.UnreadCountForUser : chatSession.UnreadCountForHost;
        }


        public async Task<List<ChatSessionWithDataDTO>> GetSessionsWithDataAsync(string userId, int page, int pageSize)
        {
            return await Db.ChatSessions
                    .Include(cs => cs.Property)
                    .Where(cs => (cs.UserId == userId || cs.HostId == userId) && cs.IsActive)
                    .OrderByDescending(cs => cs.LastActivityAt)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c=> new ChatSessionWithDataDTO
                    {
                        ChatSession = c,
                        ProfilePictureURL = c.User.ProfilePictureURL
                    })
                    .ToListAsync();
        }
    }


}
