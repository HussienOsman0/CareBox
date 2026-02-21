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
    public class ServiceProviderConfiguration : IEntityTypeConfiguration<ServiceProvider>
    {
        public void Configure(EntityTypeBuilder<ServiceProvider> builder)
        {
            //ServiceProviderID PK
            builder.HasKey(sp => sp.ServiceProviderId);

            //Name
            builder.Property(sp => sp.Name)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(100)
                   .IsRequired();


            // Address
            builder.Property(sp => sp.Address)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(255)
                   .IsRequired();

            // (Geography Point)
            builder.Property(sp => sp.Location)
                   .HasColumnType("geography")
                   .IsRequired();

            // WorkingHours
            builder.Property(sp => sp.WorkingHours)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(100)
                   .IsRequired();

            // LogoImageURL
            builder.Property(sp => sp.LogoImageUrl)
                   .HasColumnType("varchar(max)");

            //CreatedAt
            builder.Property(sp => sp.CreatedAt)
                   .HasColumnType(DBTypes.DateTime)
                   .HasDefaultValueSql("GETDATE()");


            // AppUser
            builder.HasOne(sp => sp.AppUser)
                   .WithOne(u => u.ServiceProvider)
                   .HasForeignKey<ServiceProvider>(sp => sp.AppUserId)
                   .OnDelete(DeleteBehavior.Cascade);

            //Relationship ProviderType
            builder.HasOne(sp => sp.ProviderType)
                   .WithMany(pt => pt.ServiceProviders)
                   .HasForeignKey(sp => sp.ProviderTypeId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
