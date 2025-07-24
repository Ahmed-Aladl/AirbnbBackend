//using Application.DTOs.Chat.MessageReactionDtos;
//using Application.DTOs.Chat.Requests;
//using Application.DTOs.Chat.ReservationRequestDtos;
//using Application.Interfaces.Services;
//using Application.Shared;
//using Domain.Enums.Chat;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;

//namespace Airbnb.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ReservationController : BaseController
//    {

//        // Controllers/ReservationRequestController.cs
//            private readonly IReservationRequestService _reservationRequestService;
//            private readonly ICurrentUserService _currentUserService;
//            private readonly ILogger<ReservationRequestController> _logger;

//            public ReservationRequestController(
//                IReservationRequestService reservationRequestService,
//                ICurrentUserService currentUserService,
//                ILogger<ReservationRequestController> logger)
//            {
//                _reservationRequestService = reservationRequestService;
//                _currentUserService = currentUserService;
//                _logger = logger;
//            }

//            /// <summary>
//            /// Create a reservation request in a chat session
//            /// </summary>
//            [HttpPost]
//            [ProducesResponseType(typeof(ReservationRequestDto), StatusCodes.Status201Created)]
//            [ProducesResponseType(StatusCodes.Status400BadRequest)]
//            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//            [ProducesResponseType(StatusCodes.Status403Forbidden)]
//            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//            public async Task<ActionResult<ReservationRequestDto>> CreateReservationRequest(
//                [FromBody] CreateReservationRequestDto request)
//            {
//                try
//                {
//                    if (!ModelState.IsValid)
//                    {
//                        return BadRequest(ModelState);
//                    }

//                    var currentUserId = _currentUserService.UserId;
//                    var reservationRequest = await _reservationRequestService.CreateReservationRequestAsync(request, currentUserId);

//                    return CreatedAtAction(nameof(GetReservationRequest),
//                        new { requestId = reservationRequest.RequestId }, reservationRequest);
//                }
//                catch (UnauthorizedAccessException ex)
//                {
//                    _logger.LogWarning(ex, "Unauthorized reservation request creation attempt by user {UserId}", _currentUserService.UserId);
//                    return Forbid();
//                }
//                catch (ArgumentException ex)
//                {
//                    _logger.LogWarning(ex, "Invalid reservation request data from user {UserId}", _currentUserService.UserId);
//                    return BadRequest(new { message = ex.Message });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error creating reservation request for user {UserId}", _currentUserService.UserId);
//                    return StatusCode(500, new { message = "Internal server error occurred while creating reservation request" });
//                }
//            }

//            /// <summary>
//            /// Get reservation request details
//            /// </summary>
//            [HttpGet("{requestId:guid}")]
//            [ProducesResponseType(typeof(ReservationRequestDto), StatusCodes.Status200OK)]
//            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//            [ProducesResponseType(StatusCodes.Status403Forbidden)]
//            [ProducesResponseType(StatusCodes.Status404NotFound)]
//            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//            public async Task<ActionResult<ReservationRequestDto>> GetReservationRequest(Guid requestId)
//            {
//                try
//                {
//                    var currentUserId = _currentUserService.UserId;

//                    // This would be implemented in the service to get a specific request
//                    // For now, we'll return a not implemented response
//                    return StatusCode(501, new { message = "Not implemented yet" });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error getting reservation request {RequestId}", requestId);
//                    return StatusCode(500, new { message = "Internal server error occurred while retrieving reservation request" });
//                }
//            }

//            /// <summary>
//            /// Respond to a reservation request (accept/decline)
//            /// </summary>
//            [HttpPost("{requestId:guid}/respond")]
//            [ProducesResponseType(typeof(ReservationRequestDto), StatusCodes.Status200OK)]
//            [ProducesResponseType(StatusCodes.Status400BadRequest)]
//            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//            [ProducesResponseType(StatusCodes.Status403Forbidden)]
//            [ProducesResponseType(StatusCodes.Status404NotFound)]
//            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//            public async Task<ActionResult<ReservationRequestDto>> RespondToReservationRequest(
//                Guid requestId,
//                [FromBody] RespondToReservationRequestDto request)
//            {
//                try
//                {
//                    if (!ModelState.IsValid)
//                    {
//                        return BadRequest(ModelState);
//                    }

//                    if (request.RequestId != requestId)
//                    {
//                        return BadRequest(new { message = "Request ID mismatch" });
//                    }

//                    var currentUserId = _currentUserService.UserId;
//                    var response = await _reservationRequestService.RespondToReservationRequestAsync(request, currentUserId);

//                    return Ok(response);
//                }
//                catch (UnauthorizedAccessException ex)
//                {
//                    _logger.LogWarning(ex, "Unauthorized reservation request response attempt for request {RequestId} by user {UserId}",
//                        requestId, _currentUserService.UserId);
//                    return Forbid();
//                }
//                catch (NotFoundException ex)
//                {
//                    _logger.LogWarning(ex, "Reservation request {RequestId} not found", requestId);
//                    return NotFound(new { message = ex.Message });
//                }
//                catch (InvalidOperationException ex)
//                {
//                    _logger.LogWarning(ex, "Invalid reservation request operation for request {RequestId}", requestId);
//                    return BadRequest(new { message = ex.Message });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error responding to reservation request {RequestId}", requestId);
//                    return StatusCode(500, new { message = "Internal server error occurred while responding to reservation request" });
//                }
//            }

//            /// <summary>
//            /// Get reservation requests for a specific chat session
//            /// </summary>
//            [HttpGet("chat/{chatSessionId:guid}")]
//            [ProducesResponseType(typeof(List<ReservationRequestDto>), StatusCodes.Status200OK)]
//            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//            [ProducesResponseType(StatusCodes.Status403Forbidden)]
//            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//            public async Task<ActionResult<List<ReservationRequestDto>>> GetChatReservationRequests(Guid chatSessionId)
//            {
//                try
//                {
//                    var requests = await _reservationRequestService.GetChatReservationRequestsAsync(chatSessionId);

//                    return Ok(requests);
//                }
//                catch (UnauthorizedAccessException ex)
//                {
//                    _logger.LogWarning(ex, "Unauthorized access to reservation requests for chat session {ChatSessionId} by user {UserId}",
//                        chatSessionId, _currentUserService.UserId);
//                    return Forbid();
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error getting reservation requests for chat session {ChatSessionId}", chatSessionId);
//                    return StatusCode(500, new { message = "Internal server error occurred while retrieving reservation requests" });
//                }
//            }

//            /// <summary>
//            /// Get pending reservation requests for the current host
//            /// </summary>
//            [HttpGet("pending")]
//            [ProducesResponseType(typeof(List<ReservationRequestDto>), StatusCodes.Status200OK)]
//            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//            public async Task<ActionResult<List<ReservationRequestDto>>> GetPendingHostRequests()
//            {
//                try
//                {
//                    var currentUserId = _currentUserService.UserId;
//                    var requests = await _reservationRequestService.GetPendingHostRequestsAsync(currentUserId);

//                    return Ok(requests);
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error getting pending reservation requests for host {HostId}", _currentUserService.UserId);
//                    return StatusCode(500, new { message = "Internal server error occurred while retrieving pending requests" });
//                }
//            }

//            /// <summary>
//            /// Cancel a reservation request
//            /// </summary>
//            [HttpPost("{requestId:guid}/cancel")]
//            [ProducesResponseType(typeof(ReservationRequestDto), StatusCodes.Status200OK)]
//            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//            [ProducesResponseType(StatusCodes.Status403Forbidden)]
//            [ProducesResponseType(StatusCodes.Status404NotFound)]
//            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//            public async Task<ActionResult<ReservationRequestDto>> CancelReservationRequest(Guid requestId)
//            {
//                try
//                {
//                    var currentUserId = _currentUserService.UserId;
//                    var cancelledRequest = await _reservationRequestService.CancelReservationRequestAsync(requestId, currentUserId);

//                    return Ok(cancelledRequest);
//                }
//                catch (UnauthorizedAccessException ex)
//                {
//                    _logger.LogWarning(ex, "Unauthorized cancellation attempt for reservation request {RequestId} by user {UserId}",
//                        requestId, _currentUserService.UserId);
//                    return Forbid();
//                }
//                catch (NotFoundException ex)
//                {
//                    _logger.LogWarning(ex, "Reservation request {RequestId} not found for cancellation", requestId);
//                    return NotFound(new { message = ex.Message });
//                }
//                catch (InvalidOperationException ex)
//                {
//                    _logger.LogWarning(ex, "Invalid cancellation operation for reservation request {RequestId}", requestId);
//                    return BadRequest(new { message = ex.Message });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error cancelling reservation request {RequestId}", requestId);
//                    return StatusCode(500, new { message = "Internal server error occurred while cancelling reservation request" });
//                }
//            }
//        }

//        // Controllers/MessageReactionController.cs
//        [ApiController]
//        [Route("api/[controller]")]
//        [Authorize]
//        public class MessageReactionController : ControllerBase
//        {
//            private readonly IMessageReactionService _messageReactionService;
//            private readonly ICurrentUserService _currentUserService;
//            private readonly ILogger<MessageReactionController> _logger;

//            public MessageReactionController(
//                IMessageReactionService messageReactionService,
//                ICurrentUserService currentUserService,
//                ILogger<MessageReactionController> logger)
//            {
//                _messageReactionService = messageReactionService;
//                _currentUserService = currentUserService;
//                _logger = logger;
//            }

//            /// <summary>
//            /// Toggle a reaction on a message
//            /// </summary>
//            [HttpPost("messages/{messageId:guid}/toggle")]
//            [ProducesResponseType(typeof(MessageReactionDto), StatusCodes.Status200OK)]
//            [ProducesResponseType(StatusCodes.Status400BadRequest)]
//            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//            [ProducesResponseType(StatusCodes.Status404NotFound)]
//            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//            public async Task<ActionResult<MessageReactionDto>> ToggleReaction(
//                Guid messageId,
//                [FromBody] ToggleReactionRequest request)
//            {
//                try
//                {
//                    if (!ModelState.IsValid)
//                    {
//                        return BadRequest(ModelState);
//                    }

//                    var currentUserId = _currentUserService.UserId;
//                    var reaction = await _messageReactionService.ToggleReactionAsync(messageId, currentUserId, request.ReactionType);

//                    return Ok(reaction);
//                }
//                catch (NotFoundException ex)
//                {
//                    _logger.LogWarning(ex, "Message {MessageId} not found for reaction toggle", messageId);
//                    return NotFound(new { message = ex.Message });
//                }
//                catch (ArgumentException ex)
//                {
//                    _logger.LogWarning(ex, "Invalid reaction type for message {MessageId}", messageId);
//                    return BadRequest(new { message = ex.Message });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error toggling reaction on message {MessageId}", messageId);
//                    return StatusCode(500, new { message = "Internal server error occurred while toggling reaction" });
//                }
//            }

//            /// <summary>
//            /// Get all reactions for a message
//            /// </summary>
//            [HttpGet("messages/{messageId:guid}")]
//            [ProducesResponseType(typeof(List<MessageReactionDto>), StatusCodes.Status200OK)]
//            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//            [ProducesResponseType(StatusCodes.Status404NotFound)]
//            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//            public async Task<ActionResult<List<MessageReactionDto>>> GetMessageReactions(Guid messageId)
//            {
//                try
//                {
//                    var currentUserId = _currentUserService.UserId;
//                    var reactions = await _messageReactionService.GetMessageReactionsAsync(messageId, currentUserId);

//                    return Ok(reactions);
//                }
//                catch (NotFoundException ex)
//                {
//                    _logger.LogWarning(ex, "Message {MessageId} not found for getting reactions", messageId);
//                    return NotFound(new { message = ex.Message });
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error getting reactions for message {MessageId}", messageId);
//                    return StatusCode(500, new { message = "Internal server error occurred while retrieving reactions" });
//                }
//            }

//            /// <summary>
//            /// Remove a specific reaction from a message
//            /// </summary>
//            [HttpDelete("messages/{messageId:guid}/reactions/{reactionType}")]
//            [ProducesResponseType(StatusCodes.Status204NoContent)]
//            [ProducesResponseType(StatusCodes.Status401Unauthorized)]
//            [ProducesResponseType(StatusCodes.Status404NotFound)]
//            [ProducesResponseType(StatusCodes.Status500InternalServerError)]
//            public async Task<ActionResult> RemoveReaction(Guid messageId, ReactionType reactionType)
//            {
//                try
//                {
//                    var currentUserId = _currentUserService.UserId;
//                    var success = await _messageReactionService.RemoveReactionAsync(messageId, currentUserId, reactionType);

//                    if (!success)
//                    {
//                        return NotFound(new { message = "Reaction not found" });
//                    }

//                    return NoContent();
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error removing reaction from message {MessageId}", messageId);
//                    return StatusCode(500, new { message = "Internal server error occurred while removing reaction" });
//                }
//            }
        

//    }
//}
