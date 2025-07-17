using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces;
using Application.Interfaces.IRepositories;
using Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;



namespace Infrastructure.Common
{
    public class UnitOfWork : IUnitOfWork
    {
        //private IRepository _repo;
        //public IRepository Repo =>
        //    _repo ??= new Repository(_context);
        public UnitOfWork(AirbnbContext _context) 
        {
            Context = _context;
        }

        public AirbnbContext Context { get; }

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
