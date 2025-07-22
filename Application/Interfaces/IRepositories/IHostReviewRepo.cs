using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Application.Interfaces.IRepositories;

namespace Application.Interfaces.IRepositories
{
    internal interface IHostReviewRepo : IRepository<Review, int>
    {
    }
}
