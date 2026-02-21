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
    public class TechnicianConfiguration : IEntityTypeConfiguration<Technician>
    {
        public void Configure(EntityTypeBuilder<Technician> builder)
        {
           // TechnicianID PK
            builder.HasKey(t => t.TechnicianId);

            //Name
            builder.Property(t => t.Name)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(100)
                   .IsRequired();

            //IsAvailable
            builder.Property(t => t.IsAvailable)
                   .HasColumnType(DBTypes.Bit)
                   .IsRequired();

            // Relationship ServiceProvider
            builder.HasOne(t => t.ServiceProvider)
                   .WithMany(sp => sp.Technicians)
                   .HasForeignKey(t => t.ServiceProviderId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
