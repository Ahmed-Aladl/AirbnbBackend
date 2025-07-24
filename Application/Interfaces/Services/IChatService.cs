using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chat.ChatSessionDtos;
using Application.DTOs.Chat.MessageDtos;
using Application.Result;

namespace Application.Interfaces.Services
{
    public interface IChatService
    {
        Task<Result<ChatSessionDto>> GetOrCreateChatSessionAsync(int propertyId, string userId);
        //Task<ChatSessionDto> GetChatSessionAsync(string chatSessionId, string currentUserId);
        Task<List<ChatSessionDto>> GetUserChatSessionsAsync(string userId, int page = 1, int pageSize = 20);
        Task<List<MessageDto>> GetChatMessagesAsync(string chatSessionId, string currentUserId, int page = 1, int pageSize = 50);
        Task<MessageDto> SendMessageAsync(string chatSessionId, string senderId, string messageText);
        //Task<MessageDto> EditMessageAsync(string messageId, string userId, string newText);
        //Task<bool> DeleteMessageAsync(string messageId, string userId);
        Task MarkMessagesAsReadAsync(string chatSessionId, string userId);
        Task<bool> ValidateChatAccessAsync(string chatSessionId, string userId);
    }
}
