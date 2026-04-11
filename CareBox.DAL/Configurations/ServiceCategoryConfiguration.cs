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
    public class ServiceCategoryConfiguration : IEntityTypeConfiguration<ServiceCategory>
    {
        public void Configure(EntityTypeBuilder<ServiceCategory> builder)
        {
            // تحديد الـ Primary Key
            builder.HasKey(c => c.Id);

            // إعدادات حقل الاسم
            builder.Property(c => c.Name)
                   .IsRequired()
                   .HasMaxLength(100); // أقصى طول لاسم التصنيف

            builder.HasOne(c => c.ServiceProvider)
                   .WithMany(p => p.ServiceCategories)
                   .HasForeignKey(c => c.ServiceProviderId)
                   .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
