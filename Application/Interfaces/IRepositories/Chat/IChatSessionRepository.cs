using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Chat;

namespace Application.Interfaces.IRepositories.Chat
{
    public interface IChatSessionRepository
    {
        Task<ChatSession> GetByIdAsync(string chatSessionId);
        Task<ChatSession> GetByPropertyAndUserAsync(int propertyId, string userId);
        Task<List<ChatSession>> GetUserChatSessionsAsync(string userId, int page, int pageSize);
        Task<List<ChatSession>> GetHostChatSessionsAsync(string hostId, int page, int pageSize);
        Task<ChatSession> CreateAsync(ChatSession chatSession);
        Task UpdateAsync(ChatSession chatSession);
        Task<bool> ExistsAsync(int propertyId, string userId);
        Task<int> GetUnreadCountAsync(string chatSessionId, string userId);
        Task<int> GetUnreadCountSoftAsync(string chatSessionId, string userId);

    }
}
