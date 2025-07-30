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
    public class ReservationRequestConfiguration : IEntityTypeConfiguration<ReservationRequest>
    {
        public void Configure(EntityTypeBuilder<ReservationRequest> builder)
        {
            builder
                .HasOne(r => r.ChatSession)
                .WithMany()
                .HasForeignKey(r => r.ChatSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder
                .HasOne(r => r.Message)
                .WithOne(m=> m.ReservationRequest)
                .HasForeignKey<ReservationRequest>(r => r.MessageId)
                .OnDelete(DeleteBehavior.NoAction);

            builder
                .HasIndex(r => r.ChatSessionId);

            builder
                .HasIndex(r => r.RequestStatus);

            
        }
    }
}
