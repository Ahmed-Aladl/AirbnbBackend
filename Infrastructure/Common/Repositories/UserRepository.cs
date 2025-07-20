using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.UserDto;
using Application.Interfaces.IRepositories;
using Application.Result;
using Domain.Models;
using Infrastructure.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;

namespace Infrastructure.Common.Repositories
{
    public class UserRepository : Repository<User, string>, IUserRepository
    {
        public UserRepository(AirbnbContext _db)
            : base(_db) { }

    }
}
