using Airbnb.Extensions;
using Application.DTOs.Chat.ChatSessionDtos;
using Application.DTOs.Chat.MessageDtos;
using Application.DTOs.Chat.Requests;
using Application.Interfaces.Services;
using Application.Shared;
using Domain.Models.Chat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Bcpg;

namespace Airbnb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : BaseController
    {

        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;
        //string userId = "5a6c3d4f-9ca1-4b58-bdf6-a6e19b62218f";// host of property "1" 
        string userId = "1";
        public ChatController(
                            IChatService chatService,
                            ILogger<ChatController> logger
                        )
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpGet("sessions")]
        [Authorize(Roles ="Guest")]
        public async Task<ActionResult<List<ChatSessionDto>>> GetChatSessions(  [FromQuery] int page = 1,
                                                                                [FromQuery] int pageSize = 20)
        {
                var currentUserId = User.GetUserId() ?? userId;
            try
            {
                var sessions = await _chatService.GetUserChatSessionsAsync(currentUserId, page, pageSize);

                _logger.LogInformation("Retrieved {Count} chat sessions for user {UserId}, page {Page}",
                    sessions.Count, currentUserId, page);

                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat sessions for user {UserId}", currentUserId);
                return StatusCode(500, new { message = "Internal server error occurred while retrieving chat sessions" });
            }
        }



        [HttpPost("reserve")]
        [Authorize(Roles ="Guest")]
        public async Task<ActionResult<ChatSessionDto>> CreateOrGetChatSession([FromBody] CreateReservationRequestDto createRequest)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var currentUserId= User.GetUserId() ?? userId;
                


                var responseResult = await _chatService.Reserve(createRequest.propertyId, currentUserId,createRequest);

                // Check if this is a new session or existing one
                var isNew = responseResult?.Data?.ChatSession?.LastMessageText == null;

                _logger.LogInformation("Chat session {ChatSessionId} {Action} for property {PropertyId} and user {UserId}",
                    responseResult?.Data?.ChatSession?.Id, isNew ? "created" : "retrieved", responseResult?.Data?.ChatSession?.PropertyId, currentUserId);

                return Ok(responseResult);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Property {PropertyId} not found for user {UserId}", createRequest.propertyId , userId);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation for property {PropertyId} and user {UserId}", createRequest.propertyId, userId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating/getting chat session for property {PropertyId}", createRequest.propertyId);
                return StatusCode(500, new { message = "Internal server error occurred while creating chat session" });
            }
        }




        [HttpGet("sessions/{chatSessionId}/messages")]
        [Authorize(Roles ="Guest")]
        public async Task<ActionResult<List<MessageDto>>> GetChatMessages(
                                                                    string chatSessionId,
                                                                    [FromQuery] int page = 1,
                                                                    [FromQuery] int pageSize = 50)
        {
            try
            {

                var currentUserId = User.GetUserId() ?? userId;
                
                var messages = await _chatService.GetChatMessagesAsync(chatSessionId, currentUserId, page, pageSize);

                _logger.LogInformation("Retrieved {Count} messages for chat session {ChatSessionId}, page {Page}",
                    messages.Count, chatSessionId, page);

                return Ok(messages);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access to messages in chat session {ChatSessionId} by user {UserId}",
                    chatSessionId, userId);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting messages for chat session {ChatSessionId}", chatSessionId);
                return StatusCode(500, new { message = "Internal server error occurred while retrieving messages" });
            }
        }



        [HttpPost("sessions/{chatSessionId}/messages")]
        [Authorize(Roles ="Guest")]
        public async Task<ActionResult<MessageDto>> SendMessage(
        string chatSessionId,
        [FromBody] SendMessageRequest request,
        bool isUser =false
        )
        {
            //userId = isUser ? (User.GetUserId() ?? userId) : "5a6c3d4f-9ca1-4b58-bdf6-a6e19b62218f";
            userId = User.GetUserId() ?? userId;
            
            

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (request.ChatSessionId != chatSessionId)
                {
                    return BadRequest(new { message = "Chat session ID mismatch" });
                }

                var message = await _chatService.SendMessageAsync(request, userId);


                return CreatedAtAction(nameof(GetChatMessages),
                    new { chatSessionId = chatSessionId }, message);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized message send attempt in chat session {ChatSessionId} by user {UserId}",
                    chatSessionId, userId);
                return Forbid();
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(ex, "Invalid message content in chat session {ChatSessionId}", chatSessionId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending message in chat session {ChatSessionId}", chatSessionId);
                return StatusCode(500, new { message = "Internal server error occurred while sending message" });
            }
        }

        [EndpointSummary("Mark chat as read by user")]
        [HttpPost("sessions/{chatSessionId}/mark-read")]
        [Authorize(Roles ="Guest")]
        public async Task<ActionResult> MarkMessagesAsRead(string chatSessionId)
        {
            try
            {
                var currentUserId = User.GetUserId()?? userId;

                

                await _chatService.MarkMessagesAsReadAsync(chatSessionId, currentUserId);

                return Ok(new { message = "Messages marked as read" });
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized mark as read attempt in chat session {ChatSessionId} by user {UserId}",
                    chatSessionId, userId);
                return Forbid();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking messages as read in chat session {ChatSessionId}", chatSessionId);
                return StatusCode(500, new { message = "Internal server error occurred while marking messages as read" });
            }
        }



    }
}
