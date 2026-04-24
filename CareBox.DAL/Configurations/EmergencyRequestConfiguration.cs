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
    public class EmergencyRequestConfiguration : IEntityTypeConfiguration<EmergencyRequest>
    {
        public void Configure(EntityTypeBuilder<EmergencyRequest> builder)
        {
            // RequestID (BigInt PK)
            builder.HasKey(er => er.RequestId);
            builder.Property(er => er.RequestId).HasColumnType("bigint");

            // RequestType (TinyInt)
            builder.Property(er => er.RequestType)
                   .HasColumnType("tinyint")
                   .IsRequired();

            //RequestLocation (Geography)
            builder.Property(er => er.RequestLocation)
                   .HasColumnType("geography")
                   .IsRequired();

            // <--- جديد: إعداد العنوان اليدوي
            builder.Property(er => er.ManualAddress)
                   .HasMaxLength(500)
                   .IsRequired(false); // ممكن نخليه إجباري حسب الـ Business بتاعك

            // <--- جديد: إعداد المسافة والوقت
            builder.Property(er => er.EstimatedDistance).IsRequired(false);
            builder.Property(er => er.EstimatedTimeInMinutes).IsRequired(false);

            //Status (TinyInt)
            builder.Property(er => er.Status)
                   .HasColumnType("tinyint")
                   .IsRequired();

            // CreatedAt
            builder.Property(er => er.CreatedAt)
                   .HasColumnType(DBTypes.DateTime2)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            // Relationships
            builder.HasOne(er => er.Client)
                   .WithMany(c=>c.EmergencyRequests)
                   .HasForeignKey(er => er.ClientId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.HasOne(er => er.Vehicle)
                   .WithMany(v=>v.EmergencyRequests)
                   .HasForeignKey(er => er.VehicleId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(er => er.ServiceProvider)
                   .WithMany(p => p.EmergencyRequests)
                   .HasForeignKey(er => er.ServiceProviderId)
                   .IsRequired(false) // <--- جديد
                   .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(er => er.AssignedTechnician)
                   .WithMany(t => t.EmergencyRequests)
                   .HasForeignKey(er => er.AssignedTechnicianId)
                   .OnDelete(DeleteBehavior.SetNull);// Optional
        }
    }
}
