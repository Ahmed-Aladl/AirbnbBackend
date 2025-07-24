using Application.DTOs.Chat.ChatSessionDtos;
using Application.DTOs.Chat.MessageDtos;
using Application.DTOs.Chat.Requests;
using Application.Interfaces.Services;
using Application.Shared;
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
            ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        [HttpGet("sessions")]
        public async Task<ActionResult<List<ChatSessionDto>>> GetChatSessions(
             [FromQuery] int page = 1,
             [FromQuery] int pageSize = 20)
        {
            try
            {
                var currentUserId = userId;
                var sessions = await _chatService.GetUserChatSessionsAsync(currentUserId, page, pageSize);

                _logger.LogInformation("Retrieved {Count} chat sessions for user {UserId}, page {Page}",
                    sessions.Count, currentUserId, page);

                return Ok(sessions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting chat sessions for user {UserId}", userId);
                return StatusCode(500, new { message = "Internal server error occurred while retrieving chat sessions" });
            }
        }



        [HttpPost("sessions")]
        public async Task<ActionResult<ChatSessionDto>> CreateOrGetChatSession([FromBody] int propertyId)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var currentUserId = userId;
                var session = await _chatService.GetOrCreateChatSessionAsync(propertyId, currentUserId);

                // Check if this is a new session or existing one
                var isNew = session?.Data?.LastMessageText == null;

                _logger.LogInformation("Chat session {ChatSessionId} {Action} for property {PropertyId} and user {UserId}",
                    session?.Data?.Id, isNew ? "created" : "retrieved", propertyId, currentUserId);

                return Ok(session);
            }
            catch (NotFoundException ex)
            {
                _logger.LogWarning(ex, "Property {PropertyId} not found for user {UserId}", propertyId, userId);
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Invalid operation for property {PropertyId} and user {UserId}", propertyId, userId);
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating/getting chat session for property {PropertyId}", propertyId);
                return StatusCode(500, new { message = "Internal server error occurred while creating chat session" });
            }
        }




        [HttpGet("sessions/{chatSessionId}/messages")]
        public async Task<ActionResult<List<MessageDto>>> GetChatMessages(
        string chatSessionId,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 50)
        {
            try
            {
                var currentUserId = userId;
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
        public async Task<ActionResult<MessageDto>> SendMessage(
        string chatSessionId,
        [FromBody] SendMessageRequest request)
        {
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

                var currentUserId = userId;
                var message = await _chatService.SendMessageAsync(chatSessionId, currentUserId, request.MessageText);

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


        [HttpPost("sessions/{chatSessionId}/mark-read")]
        public async Task<ActionResult> MarkMessagesAsRead(string chatSessionId)
        {
            try
            {
                var currentUserId = userId;
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
