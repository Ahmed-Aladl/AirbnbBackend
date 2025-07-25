﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Application.DTOs.Chat.ChatSessionDtos;
using Application.DTOs.Chat.MessageDtos;
using Application.DTOs.Chat.MessageReactionDtos;
using Application.DTOs.Chat.ReservationRequestDtos;
using Application.Interfaces;
using Application.Interfaces.Services;
using Application.Result;
using AutoMapper;
using Domain.Enums.Chat;
using Domain.Models.Chat;
using Microsoft.Extensions.Logging;


using Application.Interfaces.Hubs;
using Application.DTOs.Chat.Requests;
using Domain.Models;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Application.DTOs.PropertyDTOS;

namespace Application.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly PropertyService _propertyService;
        private readonly IChatNotifier chatNotifier;
        private readonly IMapper _mapper;
        private readonly ILogger<ChatService> _logger;

        public ChatService(
                    IUnitOfWork _unitOfWork,
                    IMapper mapper,
                    ILogger<ChatService> logger,
                    PropertyService propertyService,
                    IChatNotifier chatNotifier
            )
        {
            _propertyService = propertyService;
            this.chatNotifier = chatNotifier;
            UnitOfWork = _unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public IUnitOfWork UnitOfWork { get; }

        public async Task<Result<ChatSessionDto>> GetChatSessionAsync(int propertyId, string userId)
        {
            var existingSession = await UnitOfWork.ChatSessionRepo.GetByPropertyAndUserAsync(propertyId, userId);
            if (existingSession != null)
            {
                return Result<ChatSessionDto>.Success(await MapChatSessionToDto(existingSession, userId));
            }
            return Result<ChatSessionDto>.Fail("Not Found", (int)HttpStatusCode.NotFound);
        }
        public async Task<ChatSession> CreateChatSessionAsync(Property property, string userId)
        {
            try
            {
                // Check if chat session already exists

                // Get property details to get the host
                // Create new chat session
                var newSession = new ChatSession
                {
                    PropertyId = property.Id,
                    UserId = userId,
                    HostId = property.HostId,
                    IsActive = true
                };

                var createdSession = await UnitOfWork.ChatSessionRepo.CreateAsync(newSession);

                _logger.LogInformation("Created new chat session {createdSession.Id} for property {propertyId} and user {userId}",
                    createdSession.Id, property.Id, userId);

                return createdSession;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating/getting chat session for property {propertyId} and user {userId}",
                    property.Id, userId);
                throw;
            }
        }

        public async Task<List<ChatSessionDto>> GetUserChatSessionsAsync(string userId, int page = 1, int pageSize = 20)
        {
            try
            {
                // Get chat sessions where user is either guest or host
                var guestSessions = await UnitOfWork.ChatSessionRepo.GetUserChatSessionsAsync(userId, page, pageSize);
                var hostSessions = await UnitOfWork.ChatSessionRepo.GetHostChatSessionsAsync(userId, page, pageSize);

                var allSessions = guestSessions.Concat(hostSessions)
                    .GroupBy(cs => cs.Id)
                    .Select(g => g.First())
                    .OrderByDescending(cs => cs.LastActivityAt)
                    .Take(pageSize)
                    .ToList();

                var sessionDtos = new List<ChatSessionDto>();
                foreach (var session in allSessions)
                {
                    sessionDtos.Add(await MapChatSessionToDto(session, userId));
                }

                return sessionDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat sessions for user {UserId}", userId);
                throw;
            }
        }

        public async Task<List<MessageDto>> GetChatMessagesAsync(string chatSessionId, string currentUserId, int page = 1, int pageSize = 50)
        {
            try
            {
                // Validate access
                if (!await ValidateChatAccessAsync(chatSessionId, currentUserId))
                {
                    throw new UnauthorizedAccessException("User does not have access to this chat");
                }

                var messages = await UnitOfWork.MessageRepo.GetChatMessagesAsync(chatSessionId, page, pageSize);
                var messageDtos = new List<MessageDto>();

                foreach (var message in messages.OrderBy(m => m.CreatedAt))
                {
                    messageDtos.Add(await MapMessageToDto(message, currentUserId));
                }

                return messageDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting messages for chat session {ChatSessionId}", chatSessionId);
                throw;
            }
        }

        public async Task<MessageDto> SendMessageAsync(SendMessageRequest messageRequest, string senderId)
        {
            try
            {
                // Validate access
                if (!await ValidateChatAccessAsync(messageRequest.ChatSessionId, senderId))
                {
                    return null;
                    throw new UnauthorizedAccessException("User does not have access to this chat");
                }

                var chatSession = await UnitOfWork.ChatSessionRepo.GetByIdAsync(messageRequest.ChatSessionId);
                if (chatSession == null)
                    return null;
                // Create message
                var message = new Message
                {
                    ChatSessionId = messageRequest.ChatSessionId,
                    SenderId = senderId,
                    MessageText = messageRequest.MessageText,
                    MessageType = messageRequest?.MessageType.ToString(),
                    IsHost = chatSession.HostId==senderId,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    IsEdited = false,
                };

                var createdMessage = await UnitOfWork.MessageRepo.CreateAsync(message);

                // Update chat session last activity
                chatSession.LastMessageText = messageRequest?.MessageText;
                chatSession.LastActivityAt = DateTime.UtcNow;
                if(chatSession.UserId == senderId)
                    chatSession.UnreadCountForHost+=1;
                else
                    chatSession.UnreadCountForUser+=1;


                await UnitOfWork.ChatSessionRepo.UpdateAsync(chatSession);
                await UnitOfWork.SaveChangesAsync();

                _logger.LogInformation("Message {MessageId} sent in chat session {ChatSessionId} by user {SenderId}",
                    createdMessage.Id, messageRequest?.ChatSessionId, senderId);

                var messageDto = await MapMessageToDto(createdMessage, senderId);

                var receiverId = chatSession.UserId == senderId ? chatSession.HostId : chatSession.UserId;
                if(receiverId !=null)
                    await chatNotifier.NotifyMessageSentAsync(receiverId, messageDto);

                return messageDto; 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message in chat session {ChatSessionId}", messageRequest.ChatSessionId);
                throw;
            }
        }

        public async Task MarkMessagesAsReadAsync(string chatSessionId, string userId)
        {
            try
            {
                if (!await ValidateChatAccessAsync(chatSessionId, userId))
                {
                    throw new UnauthorizedAccessException("User does not have access to this chat");
                }

                await UnitOfWork.MessageReadRepo.MarkMessagesAsReadAsync(chatSessionId, userId, DateTime.UtcNow);
                await UnitOfWork.SaveChangesAsync();

                _logger.LogInformation("Marked messages as read for user {UserId} in chat session {ChatSessionId}",
                    userId, chatSessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking messages as read for user {UserId} in chat session {ChatSessionId}",
                    userId, chatSessionId);
                throw;
            }
        }

        public async Task<bool> ValidateChatAccessAsync(string chatSessionId, string userId)
        {
            var chatSession = await UnitOfWork.ChatSessionRepo.GetByIdAsync(chatSessionId);
            return chatSession != null && (chatSession.UserId == userId || chatSession.HostId == userId);
        }


        // Reservation
        public async Task<Result<RespondToReservationRequestDto>> Reserve(int propertyId, string userId, CreateReservationRequestDto createReqeust)
        {
            var property = await UnitOfWork.PropertyRepo.GetByIdWithCoverAsync(propertyId);

            if (property == null || property.HostId== userId)
            {
                return Result<RespondToReservationRequestDto>.Fail("Unauthorized", (int)HttpStatusCode.Unauthorized);
            }
            var chatSession = (await UnitOfWork.ChatSessionRepo.GetByPropertyAndUserAsync(propertyId,userId));


            // // // // // // // // 
            // Check availability
            // // // // // // // //
            if (chatSession != null) 
            {
                Console.WriteLine("******************************\n\n\n\n\n\n\n\n\n\n\n\n ChatSession!=null\n\n\n");
                var response = new RespondToReservationRequestDto
                {
                    ChatSession = await MapChatSessionToDto(chatSession, userId),
                    Proeprty = _mapper.Map<PropertyDisplayDTO>(property),
                    Messages = await GetChatMessagesAsync(chatSession.Id, userId)

                };

                var latestRequst = await UnitOfWork.ReservationRepo.GetLatestByChatSessionIdAsync(chatSession.Id);

                if (latestRequst == null || latestRequst.RequestStatus == ReservationRequestStatus.Pending.ToString())
                    latestRequst = await CreateReservationRequest(chatSession, userId, createReqeust);


                response.LatestReservationRequest = _mapper.Map<ReservationRequestDto>(latestRequst);
                Console.WriteLine("******************************\n\n\n\n\n\n\n\n\n\n\n\n return from ChatSession!=null\n\n\n");
                return Result<RespondToReservationRequestDto>.Success(response);
            }

            
            chatSession = await CreateChatSessionAsync(property, userId);
            var latestRequest = await CreateReservationRequest(chatSession,userId,createReqeust);

            var reservationRespond = new RespondToReservationRequestDto
            {
                ChatSession = await MapChatSessionToDto(chatSession, userId),
                LatestReservationRequest = _mapper.Map<ReservationRequestDto>(latestRequest),
                Messages = await GetChatMessagesAsync(chatSession.Id, userId),
                Proeprty = _mapper.Map<PropertyDisplayDTO>(property)
            };

            
            await UnitOfWork.SaveChangesAsync();
            return Result<RespondToReservationRequestDto>.Success(reservationRespond);

        }
        private async Task<ReservationRequest> CreateReservationRequest(ChatSession chatSession, string userId, CreateReservationRequestDto createRequest)
        {
            var messageRequest = new SendMessageRequest
            {
                ChatSessionId = chatSession.Id,
                MessageText = createRequest.Message,
                MessageType = MessageType.ReservationRequest,
            };
            var message = await SendMessageAsync(messageRequest, userId);
            var reserveRequest = new ReservationRequest
            {
                UserId = userId,
                ChatSession = chatSession,
                MessageId = message.Id,
                CheckInDate = createRequest.CheckInDate,
                CheckOutDate = createRequest.CheckOutDate,
                GuestCount = createRequest.GuestCount,
                RequestedAt = DateTime.UtcNow,
                RequestStatus = ReservationRequestStatus.Pending.ToString(),
            };
            return await UnitOfWork.ReservationRepo.CreateAsync(reserveRequest);

        }


        // Private helper methods
        private async Task<ChatSessionDto> MapChatSessionToDto(ChatSession session, string currentUserId)
        {
            var property = (await _propertyService.GetByIdWithCoverAsync(session.PropertyId)).Data;
            var user = UnitOfWork.UserRepo.GetById(session.UserId);
            var host = UnitOfWork.UserRepo.GetById(session.HostId);
            var lastMessage = session.LastMessageText;
            var unreadCount = session.UserId == currentUserId ? session.UnreadCountForUser : session.UnreadCountForHost;
            var hasPendingRequests = await UnitOfWork.ReservationRepo.HasPendingRequestsAsync(session.Id);

            return new ChatSessionDto
            {
                Id = session.Id,
                PropertyId = session.PropertyId,
                PropertyTitle = property?.Title,
                PropertyImageUrl = property?.Images[0]?.ImageUrl,
                UserId = session.UserId,
                UserName = user?.UserName,
                UserAvatarUrl = "",
                HostId = session.HostId,
                HostName = host?.UserName,
                HostAvatarUrl = "",
                LastActivityAt = session.LastActivityAt,
                LastMessageText = lastMessage,// != null ? await MapMessageToDto(lastMessage, currentUserId) : null,
                UnreadCount = unreadCount,
                HasPendingRequests = hasPendingRequests,
                IsActive = session.IsActive
            };
        }

        private async Task<MessageDto> MapMessageToDto(Message message, string currentUserId)
        {
            var sender = UnitOfWork.UserRepo.GetById(message.SenderId);
            var isRead = message.ReadStatuses?.Any(rs => rs.UserId != currentUserId) ?? false;
            
            return new MessageDto
            {
                Id = message.Id,
                ChatSessionId = message.ChatSessionId,
                SenderId = message.SenderId,
                SenderName = sender?.UserName,
                SenderAvatarUrl = "",
                MessageText = message.MessageText,
                MessageType = message.MessageType,
                CreatedAt = message.CreatedAt,
                IsEdited = message.IsEdited,
                EditedAt = message.EditedAt,
                IsHost = message.IsHost,
                IsOwnMessage = message.SenderId == currentUserId,
                IsRead = isRead,
                Reactions = message.Reactions!=null? await MapReactionsToDto(message.Reactions, currentUserId) :null,
                ReservationRequest = message.ReservationRequest != null ?
                    _mapper.Map<ReservationRequestDto>(message.ReservationRequest) : null
            };
        }

        private async Task<List<MessageReactionDto>> MapReactionsToDto(ICollection<MessageReaction> reactions, string currentUserId)
        {
            var list = new List<MessageReactionDto>();
            foreach (var reaction in reactions)
            {
                list.Add(
                        new()
                        {
                            Id = reaction.Id,
                            MessageId = reaction.MessageId,
                            HasCurrentUserReacted = reaction.UserId == currentUserId
                        }
                );
            }
            return list;
            //return reactions
            //    .GroupBy(r => r.ReactionType)
            //    .Select(g => new MessageReactionDto
            //    {
            //        MessageId = g.First().MessageId,
            //        ReactionType = (ReactionType)g.Key,
            //        Count = g.Count(),
            //        HasCurrentUserReacted = g.Any(r => r.UserId == currentUserId),
            //        Users = g.Select(r => new ReactionUserDto
            //        {
            //            UserId = r.UserId,
            //            // UserName and AvatarUrl would be populated from UserService
            //        }).ToList()
            //    }).ToList();
        
        }



        

        //public async Task<MessageDto> EditMessageAsync(Guid messageId, Guid userId, string newText)
        //{
        //    try
        //    {
        //        if (string.IsNullOrWhiteSpace(newText))
        //        {
        //            throw new ArgumentException("Message text cannot be empty");
        //        }

        //        if (newText.Length > 2000)
        //        {
        //            throw new ArgumentException("Message text too long (max 2000 characters)");
        //        }

        //        var message = await _unitOfWork.Messages.GetByIdAsync(messageId);
        //        if (message == null)
        //        {
        //            throw new NotFoundException("Message not found");
        //        }

        //        if (message.SenderId != userId)
        //        {
        //            throw new UnauthorizedAccessException("User can only edit their own messages");
        //        }

        //        // Check if message is too old to edit (e.g., 24 hours)
        //        if (DateTime.UtcNow.Subtract(message.CreatedAt).TotalDays > 1)
        //        {
        //            throw new InvalidOperationException("Message is too old to edit");
        //        }

        //        message.MessageText = newText.Trim();
        //        await _unitOfWork.Messages.UpdateAsync(message);
        //        await _unitOfWork.SaveChangesAsync();

        //        _logger.LogInformation("Message {MessageId} edited by user {UserId}", messageId, userId);

        //        return await MapMessageToDto(message, userId);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error editing message {MessageId} by user {UserId}", messageId, userId);
        //        throw;
        //    }
        //}

        //public async Task<bool> DeleteMessageAsync(Guid messageId, Guid userId)
        //{
        //    try
        //    {
        //        var message = await _unitOfWork.Messages.GetByIdAsync(messageId);
        //        if (message == null)
        //        {
        //            return false;
        //        }

        //        if (message.SenderId != userId)
        //        {
        //            throw new UnauthorizedAccessException("User can only delete their own messages");
        //        }

        //        await _unitOfWork.Messages.DeleteAsync(messageId);
        //        await _unitOfWork.SaveChangesAsync();

        //        _logger.LogInformation("Message {MessageId} deleted by user {UserId}", messageId, userId);

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error deleting message {MessageId} by user {UserId}", messageId, userId);
        //        throw;
        //    }
        //}

        //public async Task<ChatSessionDto> GetChatSessionAsync(Guid chatSessionId, Guid currentUserId)
        //{
        //    try
        //    {
        //        if (!await ValidateChatAccessAsync(chatSessionId, currentUserId))
        //        {
        //            throw new UnauthorizedAccessException("User does not have access to this chat");
        //        }

        //        var chatSession = await _unitOfWork.ChatSessions.GetByIdAsync(chatSessionId);
        //        if (chatSession == null)
        //        {
        //            throw new NotFoundException("Chat session not found");
        //        }

        //        return await MapChatSessionToDto(chatSession, currentUserId);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error getting chat session {ChatSessionId} for user {UserId}", chatSessionId, currentUserId);
        //        throw;
        //    }
        //}


    }
}
