using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Infrastructure.Common.Repositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;



namespace Infrastructure.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        //private IRepository _repo;
        //public IRepository Repo =>
        //    _repo ??= new Repository(_context);

        private IBookingRepo _bookingRepo;

        public UnitOfWork(AirbnbContext _context) 
        {
            Context = _context;
        }

        public AirbnbContext Context { get; }

        public IBookingRepo Bookings
        {
            get
            {
                if (_bookingRepo != null) return _bookingRepo;
                _bookingRepo = new BookingRepository(Context);
                return _bookingRepo;
            }
        }


        public int SaveChanges()
        {
            return Context
                        .SaveChanges();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await Context
                                .SaveChangesAsync();
        }
    }
}
