using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Domain.DbConfigs
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
        {
            builder.HasMany(u=> u.Bookings)
                   .WithOne(b=> b.User)
                   .HasForeignKey(b=> b.UserId);

            builder.HasMany(u => u.OwnedProps).WithOne(p => p.Host).HasForeignKey(p => p.HostId);

            builder.HasMany(u => u.ReservedProps).WithMany().UsingEntity<Booking>();

            builder.HasMany(u => u.Reviews).WithOne(r=> r.User).HasForeignKey(r=> r.UserId);

        }
    }
}
