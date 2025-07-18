using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Wishlist
    { 
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId{ get; set; }

        [ForeignKey(nameof(Property))]
        public int? PropertyId{ get; set; }
        
        public User User{ get; set; }
        public Property Property { get; set; }
    }
}
