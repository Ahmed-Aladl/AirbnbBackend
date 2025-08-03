using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using Application.DTOs.Chat.ChatSessionDtos;
using Application.DTOs.Chat.MessageDtos;
using Application.DTOs.Chat.MessageReactionDtos;
using Application.DTOs.Chat.Requests;
using Application.DTOs.Chat.ReservationRequestDtos;
using Application.DTOs.PropertyDTOS;
using Application.Interfaces;
using Application.Interfaces.Hubs;
using Application.Interfaces.Services;
using Application.Result;
using AutoMapper;
using Azure;
using Domain.Enums.Chat;
using Domain.Models;
using Domain.Models.Chat;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Logging;
//using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Application.Services.Chat
{
    public class ChatService : IChatService
    {
        private readonly PropertyService _propertyService;
        private readonly IChatNotifier chatNotifier;
        private readonly CalendarService calendarService;
        private readonly IMapper _mapper;
        private readonly ILogger<ChatService> _logger;

        // In-memory cache for translations: (messageText, targetLang) -> translatedText
        private static readonly ConcurrentDictionary<string, string> _translationCache = new();

        public ChatService(
                    IUnitOfWork _unitOfWork,
                    IMapper mapper,
                    ILogger<ChatService> logger,
                    PropertyService propertyService,
                    IChatNotifier chatNotifier,
                    CalendarService _calendarService

            )
        {
            _propertyService = propertyService;
            this.chatNotifier = chatNotifier;
            calendarService = _calendarService;
            UnitOfWork = _unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public IUnitOfWork UnitOfWork { get; }

        public async Task<Result<RespondToReservationRequestDto>> GetChatSessionForHostAsync(string hostId, string chatSessionId, string? targetLang = null)
        {
            var stopwatch = Stopwatch.StartNew();
            var chatSession = await UnitOfWork.ChatSessionRepo.GetByIdAsync( chatSessionId );

            if (chatSession == null)
                return Result<RespondToReservationRequestDto>.Fail("Not found", (int)HttpStatusCode.NotFound);

            if (chatSession.HostId != hostId && chatSession.UserId != hostId)
                return Result<RespondToReservationRequestDto>.Fail("Unauthorized", (int)HttpStatusCode.Unauthorized);

            var latestRequest = await UnitOfWork.ReservationRepo.GetLatestByChatSessionIdAsync(chatSessionId);
            var response = new RespondToReservationRequestDto
            {
                ChatSession = await MapChatSessionToDto(chatSession, hostId),
                Proeprty = chatSession.Property != null ? _mapper.Map<PropertyDisplayDTO>(chatSession.Property) : null,
                LatestReservationRequest = _mapper.Map<ReservationRequestDto>(latestRequest)

            };
            stopwatch.Stop();
            Console.WriteLine($"\n\n\n\nExecution Time for Get sessions for host: {stopwatch.ElapsedMilliseconds} ms\n\n\n\n");
            var ChatSessionDtoTask = MapChatSessionToDto(chatSession, hostId);

            var MessageDtosTask = GetChatMessagesAsync(chatSession.Id, hostId, targetLang: targetLang);

            response.ChatSession = await ChatSessionDtoTask;
            response.Messages = await MessageDtosTask;
            return Result<RespondToReservationRequestDto>.Success(response);


        }
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
                //var guestSessions = await UnitOfWork.ChatSessionRepo.GetUserChatSessionsAsync(userId, page, pageSize);
                //var hostSessions = await UnitOfWork.ChatSessionRepo.GetHostChatSessionsAsync(userId, page, pageSize);

                var allSessions = await UnitOfWork.ChatSessionRepo.GetAllUserChatSessionsAsync(userId, page, pageSize);

                allSessions = allSessions
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

        public async Task<List<MessageDto>> GetChatMessagesAsync(string chatSessionId, string currentUserId, int page = 1, int pageSize = 50, string? targetLang = null)
        {
            try
            {
                // Validate access
                if (!await ValidateChatAccessAsync(chatSessionId, currentUserId))
                {
                    throw new UnauthorizedAccessException("User does not have access to this chat");
                }

                var messages = await UnitOfWork.MessageRepo.GetChatMessagesAsync(chatSessionId, page, pageSize);
                var messageTasks = messages
                .OrderBy(m => m.CreatedAt)
                .Select(m => MapMessageToDto(m, currentUserId))
                .ToList();

                var messageDtos = (await Task.WhenAll(messageTasks)).ToList();

                var textsToTranslate = new List<string>();
                var indicesToReplace = new List<int>();

                for (int i = 0; i < messageDtos.Count; i++)
                {
                    var dto = messageDtos[i];
                    if (!string.IsNullOrWhiteSpace(targetLang) && !string.IsNullOrWhiteSpace(dto.MessageText) && !dto.IsOwnMessage)
                    {
                        string cacheKey = $"{dto.MessageText}|{targetLang}";
                        if (_translationCache.TryGetValue(cacheKey, out var cachedTranslation))
                        {
                            dto.MessageText = cachedTranslation;
                        }
                        else
                        {
                            textsToTranslate.Add(dto.MessageText);
                            indicesToReplace.Add(i);
                        }
                    }
                }

                // Translate only uncached messages in bulk
                if (textsToTranslate.Count > 0)
                {
                    var translatedTexts = await TranslateTextsAsync(textsToTranslate, targetLang);
                    for (int i = 0; i < indicesToReplace.Count; i++)
                    {
                        int targetIndex = indicesToReplace[i];
                        messageDtos[targetIndex].MessageText = translatedTexts[i];
                        // Cache the translation
                        string cacheKey = $"{textsToTranslate[i]}|{targetLang}";
                        _translationCache[cacheKey] = translatedTexts[i];
                    }
                }

                return messageDtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting messages for chat session {ChatSessionId}", chatSessionId);
                throw;
            }
        }

        public async Task<MessageDto> SendMessageAsync(SendMessageRequest messageRequest, string senderId, string? targetLang = null)
        {
            try
            {
                Task<List<string>> translationTask = null;
                if (targetLang != null)
                    translationTask = TranslateTextsAsync(new() { messageRequest.MessageText }, targetLang);

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
                    IsHost = chatSession.HostId == senderId,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false,
                    IsEdited = false,
                };

                var createdMessage = await UnitOfWork.MessageRepo.CreateAsync(message);

                // Update chat session last activity
                chatSession.LastMessageText = messageRequest?.MessageText;
                chatSession.LastActivityAt = DateTime.UtcNow;
                if (chatSession.UserId == senderId)
                    chatSession.UnreadCountForHost += 1;
                else
                    chatSession.UnreadCountForUser += 1;


                await UnitOfWork.ChatSessionRepo.UpdateAsync(chatSession);
                await UnitOfWork.SaveChangesAsync();


                _logger.LogInformation("Message {MessageId} sent in chat session {ChatSessionId} by user {SenderId}",
                    createdMessage.Id, messageRequest?.ChatSessionId, senderId);

                var messageDto = await MapMessageToDto(createdMessage, senderId);

                var receiverId = chatSession.UserId == senderId ? chatSession.HostId : chatSession.UserId;
                Console.WriteLine($"****\n\n\n\nReceiver Id {receiverId} {chatSession?.Id}");

                //if (translationTask != null)
                //    messageDto.MessageText = (await translationTask)[0];
                if (receiverId != null)
                    chatNotifier.NotifyMessageSentAsync(receiverId, messageDto);

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
        public async Task<Result<RespondToReservationRequestDto>> Reserve(int propertyId, string userId, CreateReservationRequestDto createReqeust, string? targetLang = null)
        {
            var property = await UnitOfWork.PropertyRepo.GetByIdWithCoverAsync(propertyId);

            if (property == null)
                return Result<RespondToReservationRequestDto>.Fail("Not found", (int)HttpStatusCode.NotFound);

            if (property.HostId == userId)
                return Result<RespondToReservationRequestDto>.Fail("Host can't make a request on thier own property", (int)HttpStatusCode.Unauthorized);

            var chatSession = (await UnitOfWork.ChatSessionRepo.GetByPropertyAndUserAsync(propertyId, userId));



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

                if (latestRequst == null || latestRequst.RequestStatus != ReservationRequestStatus.Pending.ToString())
                {
                    latestRequst = await CreateReservationRequest(chatSession, userId, createReqeust);
                    await UnitOfWork.SaveChangesAsync();
                }

                response.LatestReservationRequest = _mapper.Map<ReservationRequestDto>(latestRequst);
                Console.WriteLine("******************************\n\n\n\n\n\n\n\n\n\n\n\n return from ChatSession!=null\n\n\n");
                return Result<RespondToReservationRequestDto>.Success(response);
            }


            chatSession = await CreateChatSessionAsync(property, userId);
            var latestRequest = await CreateReservationRequest(chatSession, userId, createReqeust);


            var reservationRespond = new RespondToReservationRequestDto
            {
                ChatSession = await MapChatSessionToDto(chatSession, userId),
                LatestReservationRequest = _mapper.Map<ReservationRequestDto>(latestRequest),
                Messages = await GetChatMessagesAsync(chatSession.Id, userId, targetLang: targetLang),
                Proeprty = _mapper.Map<PropertyDisplayDTO>(property)
            };


            await UnitOfWork.SaveChangesAsync();
            return Result<RespondToReservationRequestDto>.Success(reservationRespond);

        }


        public async Task<Result<bool>> AcceptReservationAsync(string hostId, string reservationId)
        {
            var request = await UnitOfWork.ReservationRepo.GetByIdWithDataAsync(reservationId);
            Console.WriteLine($"***\n\n\n\n\n\n\nrequest == null{request == null}\n\n\n");
            if (request == null)
                return Result<bool>.Fail("not found", (int)HttpStatusCode.NotFound);
            if (request.ChatSession.HostId != hostId)
                return Result<bool>.Fail("Unauthorized", (int)HttpStatusCode.Unauthorized);

            var isAvailable = (await calendarService
                                    .IsPropertyBookableAsync(
                                                request.ChatSession.PropertyId,
                                                request.CheckInDate,
                                                request.CheckOutDate
                                    )).Data;


            var message = new SendMessageRequest
            {
                ChatSessionId = request.ChatSessionId,
                MessageType = MessageType.ReservationResponse
            };


            var response = CloneReservation(request);

            if (!isAvailable)
            {
                message.MessageText = $"The host can't afford your request.";
                await UnitOfWork.ReservationRepo.UpdateAsync(request);

                var declineMessageDto = await SendMessageAsync(message, request.ChatSession.HostId);
                response.RequestStatus = ReservationRequestStatus.Declined.ToString();
                response.MessageId = declineMessageDto.Id;
                await UnitOfWork.ReservationRepo.CreateAsync(response);

                await UnitOfWork.SaveChangesAsync();
                return Result<bool>.Fail("Not Available in this period", (int)HttpStatusCode.Conflict);
            }

            message.MessageText = $"The host Accepted your request.";
            request.RequestStatus = ReservationRequestStatus.Accepted.ToString();
            await UnitOfWork.ReservationRepo.UpdateAsync(request);

            var acceptessageDto = await SendMessageAsync(message, request.ChatSession.HostId);
            response.MessageId = acceptessageDto.Id;
            await UnitOfWork.ReservationRepo.CreateAsync(response);

            await UnitOfWork.SaveChangesAsync();

            return Result<bool>.Success(true, (int)HttpStatusCode.NoContent, "The host accepted you request.");
        }


        public async Task<Result<bool>> DeclineReservationAsync(string hostId, string requestId)
        {
            var request = await UnitOfWork.ReservationRepo.GetByIdWithDataAsync(requestId);
            if (request == null)
                return Result<bool>.Fail("Not found", (int)HttpStatusCode.NotFound);
            if (request.ChatSession.HostId != hostId)
                return Result<bool>.Fail("Unauthorized", (int)HttpStatusCode.Unauthorized);

            if (request.RequestStatus != ReservationRequestStatus.Pending.ToString())
                return Result<bool>.Fail("Request already has been closed.", (int)HttpStatusCode.BadRequest);


            var message = new SendMessageRequest
            {
                ChatSessionId = request.ChatSessionId,
                MessageType = MessageType.ReservationResponse,
                MessageText = $"The host Declined your request."
            };

            request.RequestStatus = ReservationRequestStatus.Declined.ToString();
            await UnitOfWork.ReservationRepo.UpdateAsync(request);

            var response = CloneReservation(request);
            var messageDto = await SendMessageAsync(message, request.ChatSession.HostId);
            response.MessageId = messageDto.Id;
            await UnitOfWork.ReservationRepo.CreateAsync(response);

            await UnitOfWork.SaveChangesAsync();
            return Result<bool>.Success(true, (int)HttpStatusCode.NoContent, "Request has been rejected");

        }

        // Private helper methods
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
        private ReservationRequest CloneReservation(ReservationRequest request)
        {
            return new()
            {
                GuestCount = request.GuestCount,
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                RequestStatus = request.RequestStatus,
                RequestedAt = request.RequestedAt,
                RespondedAt = request.RespondedAt,
                TotalAmount = request.TotalAmount,
                UserId = request.UserId,
                ResponseMessage = request.ResponseMessage,
                ChatSessionId = request.ChatSessionId,
            };
        }
        private async Task<ChatSessionDto> MapChatSessionToDto(ChatSession session, string currentUserId)
        {
            var lastMessage = session.LastMessageText;
            var unreadCount = session.UserId == currentUserId ? session.UnreadCountForUser : session.UnreadCountForHost;
            //var hasPendingRequests = await UnitOfWork.ReservationRepo.HasPendingRequestsAsync(session.Id);

            return new ChatSessionDto
            {
                Id = session.Id,
                PropertyId = session.PropertyId,
                PropertyTitle = session?.Property?.Title,
                PropertyImageUrl = session.Property?.Images.FirstOrDefault()?.ImageUrl,
                UserId = session.UserId,
                UserName = session?.User?.UserName,
                UserAvatarUrl = session?.User?.ProfilePictureURL,
                HostId = session.HostId,
                HostName = session?.Host?.UserName,
                HostAvatarUrl = "",
                LastActivityAt = session.LastActivityAt,
                LastMessageText = lastMessage,// != null ? await MapMessageToDto(lastMessage, currentUserId) : null,
                UnreadCount = unreadCount,
                //HasPendingRequests = hasPendingRequests,
                IsActive = session.IsActive,
                IsHost = session.HostId == currentUserId,
            };
        }

        private async Task<MessageDto> MapMessageToDto(Message message, string currentUserId)
        {
            var isRead = message.ReadStatuses?.Any(rs => rs.UserId != currentUserId) ?? false;

            return new MessageDto
            {
                Id = message.Id,
                ChatSessionId = message.ChatSessionId,
                SenderId = message.SenderId,
                SenderName = message?.Sender?.UserName,
                SenderAvatarUrl = "",
                MessageText = message.MessageText,
                MessageType = message.MessageType,
                CreatedAt = message.CreatedAt,
                IsEdited = message.IsEdited,
                EditedAt = message.EditedAt,
                IsHost = message.IsHost,
                IsOwnMessage = message.SenderId == currentUserId,
                IsRead = isRead,
                Reactions = message.Reactions != null ? await MapReactionsToDto(message.Reactions, currentUserId) : null,
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


        private async Task<List<string>> TranslateTextsAsync(List<string> texts, string targetLang)
        {

            var stopWatch = Stopwatch.StartNew();

            if (texts == null || texts.Count == 0)
                return texts;

            try
            {
                var request = new
                {
                    texts = texts,
                    tgt_lang = targetLang
                };

                var json = JsonSerializer.Serialize(request);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var innerStopwatch = Stopwatch.StartNew();

                using var httpClient = new HttpClient();
                httpClient.DefaultRequestHeaders.ExpectContinue = false;
                var response = await httpClient.PostAsync("https://ahmedaladl-transliation.hf.space/translate", content);
                innerStopwatch.Stop();
                Console.WriteLine($"\n\n\n\nExecution Time Translation Request: {innerStopwatch.ElapsedMilliseconds} ms\n\n\n\n");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Translation batch failed with status {StatusCode}", response.StatusCode);
                    return texts; // Fallback: return original
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<Dictionary<string, List<string>>>(responseBody);
                stopWatch.Stop();
                Console.WriteLine($"******\n\n\nExecution Time For translation method: {stopWatch.ElapsedMilliseconds} ms\n\n\n****");

                return result?["translated_texts"] ?? texts;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Translation batch exception occurred");
                return texts;
            }
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
        //    }B
        //}


    }
}
