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
    public class NotificationRepository : Repository<Notification, int>, INotificationRepository
    {
        public NotificationRepository(AirbnbContext context)
            : base(context) { }

        public async Task<IEnumerable<Notification>> GetAllByUserIdAsync(string userId)
        {
            return await Db
                .Notifications.Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<Notification> GetByIdAsync(int id)
        {
            return await Db.Notifications.FindAsync(id);
        }

        public async Task AddAsync(Notification notification)
        {
            await Db.Notifications.AddAsync(notification);
            await Db.SaveChangesAsync();
        }

        public async Task MarkAsReadAsync(int id)
        {
            var notification = await Db.Notifications.FindAsync(id);
            if (notification != null)
            {
                notification.isRead = true;
                await Db.SaveChangesAsync();
            }
        }

        public async Task DeleteAsync(int id)
        {
            var notification = await Db.Notifications.FindAsync(id);
            if (notification != null)
            {
                Db.Notifications.Remove(notification);
                await Db.SaveChangesAsync();
            }
        }
    }
}
