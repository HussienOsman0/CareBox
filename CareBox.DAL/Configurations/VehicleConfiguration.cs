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
    public class VehicleConfiguration : IEntityTypeConfiguration<Vehicle>
    {
        public void Configure(EntityTypeBuilder<Vehicle> builder)
        {
            //VehicleID PK
            builder.HasKey(v => v.VehicleId);

            // VehicleType (Enum as TinyInt)// removed


            // add new
            builder.Property(v => v.PlateNumber)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(50)
                   .IsRequired();

            // Make
            builder.Property(v => v.Make)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(50)
                   .IsRequired();

            //Model
            builder.Property(v => v.Model)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(50)
                   .IsRequired();

            // Year (SmallInt)
            builder.Property(v => v.Year)
                   .HasColumnType("smallint")
                   .IsRequired();


            // Kilometers
            builder.Property(v => v.Kilometers)
                   .HasColumnType(DBTypes.Int)
                   .IsRequired();

            // Relationship Client (One-to-Many)
            builder.HasOne(v => v.Client)
                   .WithMany(c => c.Vehicles)
                   .HasForeignKey(v => v.ClientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
