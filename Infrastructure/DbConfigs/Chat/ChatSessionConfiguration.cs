using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Models.Chat;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfigs.Chat
{
    public class ChatSessionConfiguration : IEntityTypeConfiguration<ChatSession>
    {
        public void Configure(EntityTypeBuilder<ChatSession> builder)
        {
            builder.HasOne(cs => cs.Property)
                .WithMany(p => p.ChatSessions) 
                .HasForeignKey(cs => cs.PropertyId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(cs => cs.Host)
                .WithMany() 
                .HasForeignKey(cs => cs.HostId)
                .OnDelete(DeleteBehavior.Restrict);
            
            builder.HasOne(cs => cs.User)
                .WithMany() 
                .HasForeignKey(cs => cs.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            builder
                .HasIndex(cs => new { cs.PropertyId, cs.UserId })
                .IsUnique();

            builder
                .HasIndex(cs => new { cs.UserId, cs.LastActivityAt });

            builder
                .HasIndex(cs => new { cs.HostId, cs.LastActivityAt });

            


        }
    }
}
