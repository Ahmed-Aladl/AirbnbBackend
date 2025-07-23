using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Application.Result;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IHostReviewRepo : IRepository<HostReply, int>
    {
        Task<HostReply?> GetByReviewIdAsync(int reviewId);
        Task AddAsync(HostReply reply);
        Task<List<HostReply>> GetByHostIdAsync(string hostId);
        Task<List<HostReply>> GetByPropertyIdAsync(int propertyId);
        Task<List<HostReply>> GetAllAsync();
        Task<HostReply?> GetByIdAsync(int id);
    }
}
