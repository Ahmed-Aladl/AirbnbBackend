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
    //public class MessageConfiguration : IEntityTypeConfiguration<Message>
    //{
    //    public void Configure(EntityTypeBuilder<Message> builder)
    //    {
    //        builder
    //            .HasOne<User>(m => m.Sender)
    //            .WithMany()
    //            .HasForeignKey(m => m.SenderId)
    //            .OnDelete(DeleteBehavior.Restrict);

    //        builder
    //            .HasOne<User>(m => m.Receiver)
    //            .WithMany()
    //            .HasForeignKey(m => m.ReceiverId)
    //            .OnDelete(DeleteBehavior.Restrict);
    //    }
    //}
}
