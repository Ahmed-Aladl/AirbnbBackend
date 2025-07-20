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
    public class UserOtpConfiguration : IEntityTypeConfiguration<UserOtp>
    {
        public void Configure(EntityTypeBuilder<UserOtp> builder)
        {
            builder.HasKey(o => o.Id);

            builder.Property(o => o.Code)
                .IsRequired()
                .HasMaxLength(6);

            builder.Property(o => o.UserId)
                .IsRequired();

            builder.HasOne(o => o.User)
                .WithMany()
                .HasForeignKey(o => o.UserId);

            builder.Property(o => o.ExpiresAt)
                .IsRequired();

            builder.Property(o => o.IsUsed)
                .IsRequired();
        }
    }
}
