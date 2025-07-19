using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; }

        [ForeignKey(nameof(User))]
        public string UserId { get; set; }

        [ForeignKey(nameof(Property))]
        public int PropertyId { get; set; }

        [ForeignKey(nameof(Booking))]
        public int BookingId { get; set; }

        public User User { get; set; }
        public Property Property { get; set; }
        public Booking Booking { get; set; }
    }
}
