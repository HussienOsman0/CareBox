using CareBox.DAL.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Configurations
{
    public class ReviewConfiguration : IEntityTypeConfiguration<Review>
    {
        public void Configure(EntityTypeBuilder<Review> builder)
        {
            //ReviewID (BigInt PK)
            builder.HasKey(r => r.ReviewId);
            builder.Property(r => r.ReviewId).HasColumnType("bigint");

           //Rating (TinyInt)
            builder.Property(r => r.Rating)
                   .HasColumnType("tinyint")
                   .IsRequired();

            // Comment
            builder.Property(r => r.Comment)
                   .HasColumnType(DBTypes.NvarCharMax);

            //Relationship Client
            builder.HasOne(r => r.Client)
                   .WithMany(c => c.Reviews)
                   .HasForeignKey(r => r.ClientId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relationship ServiceProvider
            builder.HasOne(r => r.ServiceProvider)
                   .WithMany(sp => sp.Reviews)
                   .HasForeignKey(r => r.ServiceProviderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
