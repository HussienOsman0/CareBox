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
    public class ListingConfiguration : IEntityTypeConfiguration<Listing>
    {
        public void Configure(EntityTypeBuilder<Listing> builder)
        {
            // ListingID (BigInt PK)
            builder.HasKey(l => l.ListingId);
            builder.Property(l => l.ListingId).HasColumnType("bigint");

            //[cite: 118] Title
            builder.Property(l => l.Title)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(150)
                   .IsRequired();

            //[cite: 119] Description
            builder.Property(l => l.Description)
                   .HasColumnType(DBTypes.NvarCharMax)
                   .IsRequired();

            // [cite: 120] Price
            builder.Property(l => l.Price)
                   .HasColumnType(DBTypes.Decimal18_2)
                   .IsRequired();

            // Status (TinyInt)
            builder.Property(l => l.Status)
                   .HasColumnType("tinyint")
                   .IsRequired();

            // Location
            builder.Property(l => l.Location)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(200)
                   .IsRequired();

            // SellerPhoneNumber
            builder.Property(l => l.SellerPhoneNumber)
                   .HasColumnType("varchar(20)")
                   .IsRequired();

            //Relationship Client (Seller)
            builder.HasOne(l => l.SellerClient)
                   .WithMany(c => c.Listings)
                   .HasForeignKey(l => l.SellerClientId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Relationship Vehicle
            builder.HasOne(l => l.Vehicle)
                   .WithMany()
                   .HasForeignKey(l => l.VehicleId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
