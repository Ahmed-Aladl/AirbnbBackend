using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Common.Repositories
{
    public class HostReplyRepo : IHostReviewRepo
    {
        private readonly AirbnbContext _context;

        public HostReplyRepo(AirbnbContext context)
        {
            _context = context;
        }

        public async Task<HostReply?> GetByReviewIdAsync(int reviewId)
        {
            return await _context.HOST
                .Include(hr => hr.User)
                .Include(hr => hr.Review)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(hr => hr.ReviewId == reviewId);
        }

        public async Task<List<HostReply>> GetByHostIdAsync(string hostId)
        {
            return await _context.HOST
                .Where(hr => hr.UserId == hostId)
                .Include(hr => hr.User)
                .Include(hr => hr.Review)
                    .ThenInclude(r => r.User)
                .Include(hr => hr.Review)
                    .ThenInclude(r => r.Property)
                .OrderByDescending(hr => hr.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<HostReply>> GetByPropertyIdAsync(int propertyId)
        {
            return await _context.HOST
                .Where(hr => hr.Review.PropertyId == propertyId)
                .Include(hr => hr.User)
                .Include(hr => hr.Review)
                    .ThenInclude(r => r.User)
                .OrderByDescending(hr => hr.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<HostReply>> GetAllAsync()
        {
            return await _context.HOST
                .Include(hr => hr.User)
                .Include(hr => hr.Review)
                    .ThenInclude(r => r.User)
                .Include(hr => hr.Review)
                    .ThenInclude(r => r.Property)
                .OrderByDescending(hr => hr.CreatedAt)
                .ToListAsync();
        }

        public async Task<HostReply?> GetByIdAsync(int id)
        {
            return await _context.HOST
                .Include(hr => hr.User)
                .Include(hr => hr.Review)
                    .ThenInclude(r => r.User)
                .Include(hr => hr.Review)
                    .ThenInclude(r => r.Property)
                .FirstOrDefaultAsync(hr => hr.Id == id);
        }

        public async Task AddAsync(HostReply reply)
        {
            await _context.HOST.AddAsync(reply);
        }

        public void Delete(HostReply reply)
        {
            _context.HOST.Remove(reply);
        }

        public List<HostReply> GetAll()
        {
            throw new NotImplementedException();
        }

        public HostReply GetById(int id)
        {
            throw new NotImplementedException();
        }

        public void Add(HostReply entity)
        {
            throw new NotImplementedException();
        }

        public void AddRange(ICollection<HostReply> entities)
        {
            throw new NotImplementedException();
        }

        public void Update(HostReply entity)
        {
            throw new NotImplementedException();
        }
    }
}
