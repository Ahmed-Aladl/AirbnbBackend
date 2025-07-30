using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chat.ChatSessionDtos;
using Application.DTOs.Chat.MessageDtos;
using Application.DTOs.Chat.Requests;
using Application.Result;
using Domain.Models;
using Domain.Models.Chat;

namespace Application.Interfaces.Services
{
    public interface IChatService
    {
        Task<Result<RespondToReservationRequestDto>> GetChatSessionForHostAsync(string hostId, string chatSessionId);
        Task<ChatSession> CreateChatSessionAsync(Property property, string userId);
        //Task<ChatSessionDto> GetChatSessionAsync(string chatSessionId, string currentUserId);
        Task<List<ChatSessionDto>> GetUserChatSessionsAsync(string userId, int page = 1, int pageSize = 20);
        Task<List<MessageDto>> GetChatMessagesAsync(string chatSessionId, string currentUserId, int page = 1, int pageSize = 50);
        Task<MessageDto> SendMessageAsync(SendMessageRequest messageRequest, string senderId);
        //Task<MessageDto> EditMessageAsync(string messageId, string userId, string newText);
        //Task<bool> DeleteMessageAsync(string messageId, string userId);
        Task MarkMessagesAsReadAsync(string chatSessionId, string userId);
        Task<bool> ValidateChatAccessAsync(string chatSessionId, string userId);
        Task<Result<RespondToReservationRequestDto>> Reserve(int propertyId, string userId, CreateReservationRequestDto createReqeust);
        Task<Result<bool>> AcceptReservationAsync(string hostId, string requestId);
        Task<Result<bool>> DeclineReservationAsync(string hostId, string requestId);
    }
}
