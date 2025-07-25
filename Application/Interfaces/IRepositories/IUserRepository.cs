﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.DTOs.UserDto;
using Domain.Models;

namespace Application.Interfaces.IRepositories
{
    public interface IUserRepository : IRepository<User, string> { }
}
