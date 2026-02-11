using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfigs
{
    public class PropertyConfiguration : IEntityTypeConfiguration<Property>
    {
        public void Configure(EntityTypeBuilder<Property> builder)
        {
            builder
                .HasMany<Review>()
                .WithOne(r => r.Property)
                .HasForeignKey(r => r.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Images)
                   .WithOne(i => i.Property)
                   .HasForeignKey(i => i.PropertyId);

            builder.HasMany(p => p.Bookings)
                   .WithOne(p => p.Property)
                   .HasForeignKey(b => b.PropertyId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasMany(p => p.CalendarAvailabilities)
                   .WithOne(c => c.Property)
                   .HasForeignKey(c => c.PropertyId);

            builder.Property(p => p.WeekendPrice)
                   .HasColumnType("decimal(18,2)");
        }
    }
}
