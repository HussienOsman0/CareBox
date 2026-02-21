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
    public class ListingImageConfiguration : IEntityTypeConfiguration<ListingImage>
    {
        public void Configure(EntityTypeBuilder<ListingImage> builder)
        {
            // ImageID (BigInt PK)
            builder.HasKey(li => li.ImageId);
            builder.Property(li => li.ImageId).HasColumnType("bigint");

            //ImageURL
            builder.Property(li => li.ImageUrl)
                   .HasColumnType("varchar(max)")
                   .IsRequired();

            // Relationship Listing
            builder.HasOne(li => li.Listing)
                   .WithMany(l => l.Images)
                   .HasForeignKey(li => li.ListingId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
