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
    public class AmenityConfiguration : IEntityTypeConfiguration<Amenity>
    {
        public void Configure(EntityTypeBuilder<Amenity> builder)
        {
            builder
                    .HasMany(a => a.Properties)
                    .WithMany(p => p.Amenities)
                    .UsingEntity<PropertyAmenity>(
                                r => r.HasOne(pa => pa.Property)
                                        .WithMany(p => p.PropertyAmenities)
                                        .HasForeignKey(pa => pa.PropertyId),
                                l => l.HasOne(pa => pa.Amenity)
                                        .WithMany(a => a.PropertyAmenities)
                                        .HasForeignKey(pa => pa.AmenityId),
                                j => {
                                    j.HasKey(j => new {j.PropertyId, j.AmenityId});
                                    j.HasIndex(j => new { j.PropertyId, j.AmenityId }).IsUnique();

                                    j.Property(j => j.PropertyId).IsRequired();
                                    j.Property(j => j.AmenityId).IsRequired();

                                }
                    );
        }
    }
}
