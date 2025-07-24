using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.Chat.Requests;
using Application.DTOs.Chat.ReservationRequestDtos;

namespace Application.Interfaces.Services
{
    public interface IReservationRequestService
    {
        Task<ReservationRequestDto> CreateReservationRequestAsync(CreateReservationRequestDto request, string requesterId);
        Task<ReservationRequestDto> RespondToReservationRequestAsync(RespondToReservationRequestDto response, string hostId);
        Task<List<ReservationRequestDto>> GetChatReservationRequestsAsync(string chatSessionId);
        Task<List<ReservationRequestDto>> GetPendingHostRequestsAsync(string hostId);
        Task<ReservationRequestDto> CancelReservationRequestAsync(string requestId, string userId);
        Task<bool> ValidateReservationRequestAccessAsync(string requestId, string userId);
    }

}
