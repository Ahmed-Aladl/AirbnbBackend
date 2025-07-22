using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    internal class HostReply
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey(nameof(User))]
        public string UserId { get; set; } 

        [ForeignKey(nameof(Review))]
        public int ReviewId { get; set; } 

        [ForeignKey(nameof(Property))]
        public int PropertyId { get; set; }
        public User User { get; set; } 
        public Review Review { get; set; }
        public Property Property { get; set; }
    }
}
