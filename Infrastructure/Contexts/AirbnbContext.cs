using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1;

namespace Infrastructure.Contexts
{
    public class AirbnbContext:IdentityDbContext<User>
    {
        public DbSet<User> Users {  get; set; }
        public DbSet<Property> Properties{  get; set; }
        public DbSet<Amenity> Amenities{  get; set; }
        public DbSet<PropertyType> propertyTypes { get; set; }

        public DbSet<Payment> Payments { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<PropertyImage> PropertyImages { get; set; }
        public DbSet<Booking> Bookings {  get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<CalendarAvailability> calendarAvailabilities { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }

        public DbSet<WishlistProperty> WishlistProperties { get; set; }

        public DbSet<PropertyAmenity> PropertyAmenities { get; set; }



        public AirbnbContext(DbContextOptions<AirbnbContext> options)
            :base(options) 
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(typeof(AirbnbContext).Assembly);
        }
    }
}
