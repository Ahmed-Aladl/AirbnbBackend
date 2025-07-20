using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class UserOtp
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string UserId { get; set; }
        public User User { get; set; }
        public DateTime ExpiresAt { get; set; }
        public bool IsUsed { get; set; }
    }
}
