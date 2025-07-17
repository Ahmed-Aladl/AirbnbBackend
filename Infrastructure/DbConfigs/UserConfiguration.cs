using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbConfigs
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<User> builder)
        {
            builder.HasMany(u=> u.Bookings)
                   .WithOne(b=> b.User)
                   .HasForeignKey(b=> b.UserId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.OwnedProps)
                .WithOne(p => p.Host)
                .HasForeignKey(p => p.HostId).OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(u => u.ReservedProps)
                .WithMany()
                .UsingEntity<Booking>();

            builder.HasMany(u => u.Reviews)
                .WithOne(r=> r.User).OnDelete(DeleteBehavior.Restrict)
                .HasForeignKey(r=> r.UserId);

            builder.HasMany(u=> u.WishlistedProps)
                .WithMany()
                .UsingEntity<Wishlist>(
                            r=> r.HasOne(r=> r.Property)
                                 .WithMany()
                                 .HasForeignKey(r=> r.PropertyId),

                            l=> l.HasOne(l=> l.User)
                                    .WithMany(u=> u.Wishlist)
                                    .HasForeignKey(l=> l.UserId),
                            j=> {
                                    j.HasKey(j => j.Id);
                                    j.HasIndex(j => new { j.UserId, j.PropertyId }).IsUnique();

                                    j.Property(j => j.UserId).IsRequired();
                                    j.Property(j => j.PropertyId).IsRequired();
                                }
                );

           


        }
    }
}
