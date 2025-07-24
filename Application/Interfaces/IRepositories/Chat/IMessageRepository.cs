using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Chat;

namespace Application.Interfaces.IRepositories.Chat
{
        public interface IMessageRepository
        {
            Task<Message> GetByIdAsync(string messageId);
            Task<List<Message>> GetChatMessagesAsync(string chatSessionId, int page, int pageSize);
            Task<Message> CreateAsync(Message message);
            Task UpdateAsync(Message message);
            Task DeleteAsync(string messageId);
            Task<Message> GetLatestMessageAsync(string chatSessionId);
            Task<List<Message>> GetUnreadMessagesAsync(string chatSessionId, string userId);
        }


    
}
