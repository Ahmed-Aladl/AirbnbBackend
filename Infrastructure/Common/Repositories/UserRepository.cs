using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Interfaces.IRepositories;
using Domain.Models;
using Infrastructure.Contexts;

namespace Infrastructure.Common.Repositories
{
    public class UserRepository : Repository<User, string>, IUserRepository
    {
        public UserRepository(AirbnbContext _db) : base(_db)
        {

        }
    }
}
