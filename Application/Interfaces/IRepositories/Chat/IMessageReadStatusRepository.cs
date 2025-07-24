using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Chat;

namespace Application.Interfaces.IRepositories.Chat
{
    public interface IMessageReadStatusRepository
    {
        Task<MessageReadStatus> GetByMessageAndUserAsync(string messageId, string userId);
        Task CreateAsync(MessageReadStatus readStatus);
        Task MarkMessagesAsReadAsync(string chatSessionId, string userId, DateTime readAt);
        //Task<List<MessageReadStatus>> GetByUserAsync(string userId);
    }
}
