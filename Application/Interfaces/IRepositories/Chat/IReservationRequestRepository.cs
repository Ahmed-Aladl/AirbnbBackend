using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Chat;

namespace Application.Interfaces.IRepositories.Chat
{
    public interface IReservationRequestRepository
    {
        Task<ReservationRequest> GetByIdAsync(string requestId);
        Task<List<ReservationRequest>> GetByChatSessionAsync(string chatSessionId);
        Task<ReservationRequest> CreateAsync(ReservationRequest request);
        Task UpdateAsync(ReservationRequest request);
        Task<List<ReservationRequest>> GetPendingRequestsAsync(string hostId);
        Task<bool> HasPendingRequestsAsync(string chatSessionId);
        Task<List<ReservationRequest>> GetByChatIdAsync(string chatSessionId);
        Task<ReservationRequest> GetLatestByChatSessionIdAsync(string chatSessionId);
    }

}
