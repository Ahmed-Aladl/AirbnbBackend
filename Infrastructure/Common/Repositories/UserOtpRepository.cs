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
    public class UserOtpRepository : Repository<UserOtp, int>, IUserOtpRepository
    {
        public UserOtpRepository(AirbnbContext context) : base(context)
        {
        }
    }
}

