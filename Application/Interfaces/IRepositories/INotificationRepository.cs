using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface INotificationRepository
    {
        Task<IEnumerable<Notification>> GetAllByUserIdAsync(string userId);
        Task<Notification> GetByIdAsync(int id);
        Task AddAsync(Notification notification);
        Task MarkAsReadAsync(int id);
    }
}
