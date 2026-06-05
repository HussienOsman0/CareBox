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
    public class IgnoredEmergencyRequestConfiguration : IEntityTypeConfiguration<IgnoredEmergencyRequest>
    {
        public void Configure(EntityTypeBuilder<IgnoredEmergencyRequest> builder)
        {
            // 1. تعريف الـ Composite Primary Key (مفتاح أساسي مركب)
            builder.HasKey(ir => new { ir.EmergencyRequestId, ir.ServiceProviderId });

            // 2. ربط العلاقة مع جدول الطلبات (EmergencyRequest)
            builder.HasOne(ir => ir.EmergencyRequest)
                .WithMany(e => e.IgnoredRequests) // الخاصية اللي ضفناها في كلاس الطلب
                .HasForeignKey(ir => ir.EmergencyRequestId)
                .OnDelete(DeleteBehavior.Cascade); // لو الطلب اتمسح، سجل التجاهل بتاعه يتمسح معاه

            // 3. ربط العلاقة مع جدول الورش (ServiceProvider)
            builder.HasOne(ir => ir.ServiceProvider)
                .WithMany() // الورشة مش محتاجين نحطلها لستة بالتجاهلات جواها
                .HasForeignKey(ir => ir.ServiceProviderId)
                .OnDelete(DeleteBehavior.Restrict); // يفضل Restrict عشان نتفادى إيرور الـ Multiple Cascade Paths في الـ SQL
        }
    }
}
