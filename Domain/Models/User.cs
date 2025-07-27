using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Domain.Models
{
    public class User : IdentityUser
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public virtual ICollection<IdentityRole> Roles { get; set; } = new HashSet<IdentityRole>();
        public DateTime CreateAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? ProfilePictureURL { get; set; }
        public string? Bio { get; set; }
        public string? Country { get; set; }
        public DateOnly? BirthDate { get; set; }
        public bool? IsDeleted { get; set; }
        public bool? IsConfirmed { get; set; } = false;

        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public ICollection<Wishlist>? Wishlists { get; set; }
        public ICollection<UserOtp> Otps { get; set; }
        public ICollection<Property>? OwnedProps { get; set; }
        public ICollection<Property>? ReservedProps { get; set; }
        public ICollection<Booking>? Bookings { get; set; }
        public ICollection<Review>? Reviews { get; set; }
        public ICollection<Payment>? Payments { get; set; }
    }
}
