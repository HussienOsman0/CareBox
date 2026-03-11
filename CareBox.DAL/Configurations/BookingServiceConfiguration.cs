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
    public class BookingServiceConfiguration : IEntityTypeConfiguration<BookingService>
    {
        public void Configure(EntityTypeBuilder<BookingService> builder)
        {
            // تحديد المفتاح الأساسي المركب (Composite Key)
            builder.HasKey(bs => new { bs.BookingId, bs.ServiceId });

            // علاقة الحجز بالجدول الوسيط
            builder.HasOne(bs => bs.Booking)
                   .WithMany(b => b.BookingServices)
                   .HasForeignKey(bs => bs.BookingId)
                   .OnDelete(DeleteBehavior.Cascade); // لو اتمسح الحجز، تتمسح خدماته

            // علاقة الخدمة بالجدول الوسيط
            builder.HasOne(bs => bs.Service)
                   .WithMany(s => s.BookingServices)
                   .HasForeignKey(bs => bs.ServiceId)
                   .OnDelete(DeleteBehavior.Restrict); // نمنع مسح خدمة لو مربوطة بحجز سابق
        }
    }
}
