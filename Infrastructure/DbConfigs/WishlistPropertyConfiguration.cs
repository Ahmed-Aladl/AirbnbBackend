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
    public class WishlistPropertyConfiguration : IEntityTypeConfiguration<WishlistProperty>
    {
        public void Configure(EntityTypeBuilder<WishlistProperty> builder)
        {
            builder.HasKey(wp => new { wp.WishlistId, wp.PropertyId });

            builder.HasOne(wp => wp.Wishlist)
                   .WithMany(w => w.WishlistProperties)
                   .HasForeignKey(wp => wp.WishlistId);

            builder.HasOne(wp => wp.Property)
                   .WithMany(p => p.WishlistProperties)
                   .HasForeignKey(wp => wp.PropertyId);
        }
    }
}
