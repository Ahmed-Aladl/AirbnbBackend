using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IBookingRepo:IRepository<Booking,int> 
    {
        Task<List<Booking>> GetAllAsync();
        Task<Booking?> GetByIdAsync(int id);
        Task AddAsync(Booking entity);
        void Delete(Booking entity);
        void Update(Booking entity);


    }
}
