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
    //IEntityTypeConfiguration
    public class ReviewConfiguration : IEntityTypeConfiguration<Domain.Models.Review>
    {


        public void Configure(EntityTypeBuilder<Review> builder)
        {

            builder.HasOne(r => r.Booking)
                   .WithOne()
                   .HasForeignKey<Review>(r => r.BookingId)
                   .OnDelete(DeleteBehavior.NoAction);





        }



    }
}
