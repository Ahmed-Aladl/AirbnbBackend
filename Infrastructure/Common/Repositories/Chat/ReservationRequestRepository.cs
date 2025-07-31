using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories.Chat;
using Domain.Enums.Chat;
using Domain.Models.Chat;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories.Chat
{
    public class ReservationRequestRepository : Repository<ReservationRequestRepository, string>, IReservationRequestRepository
    {
        public ReservationRequestRepository(AirbnbContext _db) : base(_db)
        {
        }

        public async Task<ReservationRequest> CreateAsync(ReservationRequest request)
        {
            return (await Db.ReservationRequests.AddAsync(request)).Entity;
        }

        public async Task<List<ReservationRequest>> GetByChatSessionAsync(string chatSessionId)
        {
            return await Db.ReservationRequests
                        .Where(req=> req.ChatSessionId == chatSessionId )
                        .OrderByDescending(req=> req.ChatSessionId)
                        .ToListAsync();
        }

        public async Task<ReservationRequest> GetByIdAsync(string requestId)
        {
            return await Db.ReservationRequests.FindAsync(requestId);
        }

        public async Task<List<ReservationRequest>> GetPendingRequestsAsync(string hostId)
        {
            return await Db.ReservationRequests
                        .Where(req => req.ChatSession.HostId == hostId && req.RequestStatus == ReservationRequestStatus.Pending.ToString())
                        .OrderByDescending(req => req.ChatSessionId)
                        .ToListAsync();
        }

        public async Task<ReservationRequest> GetByIdWithDataAsync(string reservationId)
        {
            return await Db.ReservationRequests
                            .Include(r => r.ChatSession)
                            .Include(r => r.User)
                            .FirstOrDefaultAsync(r => r.Id == reservationId);
        }

        public async Task<bool> HasPendingRequestsAsync(string chatSessionId)
        {
            return await Db.ReservationRequests.AnyAsync(req=> req.ChatSessionId == chatSessionId);
        }

        public async Task UpdateAsync(ReservationRequest request)
        {
            Db.ReservationRequests.Update(request);
            return;
        }
        public async Task<List<ReservationRequest>> GetByChatIdAsync(string chatSessionId)
        {
            return await Db.ReservationRequests.Where(req=> req.Id == chatSessionId).ToListAsync();
        }
        public async Task<ReservationRequest> GetLatestByChatSessionIdAsync(string chatSessionId)
        {

           return await Db.ReservationRequests
                    .OrderByDescending(req => req.RequestedAt)
                    .FirstOrDefaultAsync(req => req.ChatSessionId == chatSessionId);
        }
    }
}
