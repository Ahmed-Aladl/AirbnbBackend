using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.UserDto
{
    public class StripeAccountStatusDTO
    {
        public bool Exists { get; set; }
        public bool AccountCompleted { get; set; }
    }

}
