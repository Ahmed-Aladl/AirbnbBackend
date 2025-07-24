using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfigs.Chat
{
    public class MessageReadStatusConfiguration : IEntityTypeConfiguration<MessageReadStatus>
    {
        public void Configure(EntityTypeBuilder<MessageReadStatus> builder)
        {
            builder
                .HasOne(rs => rs.Message)
                .WithMany(m=> m.ReadStatuses)
                .HasForeignKey(rs => rs.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
            builder
                .HasKey(rs => new { rs.MessageId, rs.UserId });
        }
    }
}
