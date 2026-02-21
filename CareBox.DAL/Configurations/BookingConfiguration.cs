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
    public class BookingConfiguration : IEntityTypeConfiguration<Booking>
    {
        public void Configure(EntityTypeBuilder<Booking> builder)
        {
            //BookingID (BigInt PK)
            builder.HasKey(b => b.BookingId);
            builder.Property(b => b.BookingId).HasColumnType("bigint");

            //AppointmentDateTime
            builder.Property(b => b.AppointmentDateTime)
                   .HasColumnType(DBTypes.DateTime)
                   .IsRequired();

            // Status (TinyInt)
            builder.Property(b => b.Status)
                   .HasColumnType("tinyint")
                   .IsRequired();

            //BookingCode (Unique)
            builder.Property(b => b.BookingCode)
                   .HasColumnType("varchar(20)")
                   .IsRequired();
            builder.HasIndex(b => b.BookingCode).IsUnique();

            // Relationship Client
            builder.HasOne(b => b.Client)
                   .WithMany(c => c.Bookings)
                   .HasForeignKey(b => b.ClientId)
                   .OnDelete(DeleteBehavior.Restrict); // Prevent Cascade Cycles

            //Relationship Vehicle
            builder.HasOne(b => b.Vehicle)
                   .WithMany(v => v.Bookings)
                   .HasForeignKey(b => b.VehicleId)
                   .OnDelete(DeleteBehavior.Restrict);

            // Relationship ServiceProvider
            builder.HasOne(b => b.ServiceProvider)
                   .WithMany(sp => sp.Bookings)
                   .HasForeignKey(b => b.ServiceProviderId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
