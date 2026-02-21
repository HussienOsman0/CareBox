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
    public class ServiceConfiguration : IEntityTypeConfiguration<Service>
    {
        public void Configure(EntityTypeBuilder<Service> builder)
        {
            //ServiceID PK
            builder.HasKey(s => s.ServiceId);

            //ServiceName
            builder.Property(s => s.ServiceName)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(100)
                   .IsRequired();

            // Description
            builder.Property(s => s.Description)
                   .HasColumnType(DBTypes.NvarCharMax);

            // Price
            builder.Property(s => s.Price)
                   .HasColumnType(DBTypes.Decimal18_2)
                   .IsRequired();

            // Relationship ServiceProvider
            builder.HasOne(s => s.ServiceProvider)
                   .WithMany(sp => sp.Services)
                   .HasForeignKey(s => s.ServiceProviderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
