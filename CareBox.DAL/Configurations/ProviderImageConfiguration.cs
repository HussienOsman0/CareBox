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
    public class ProviderImageConfiguration : IEntityTypeConfiguration<ProviderImage>
    {
        public void Configure(EntityTypeBuilder<ProviderImage> builder)
        {
            // Primary Key
            builder.HasKey(pi => pi.Id);

            // ImageUrl
            builder.Property(pi => pi.ImageUrl)
                   .HasColumnType("varchar(max)")
                   .IsRequired();

            // Relationship with ServiceProvider (One-to-Many)
            builder.HasOne(pi => pi.ServiceProvider)
                   .WithMany(sp => sp.ProviderImages)
                   .HasForeignKey(pi => pi.ServiceProviderId)
                   .OnDelete(DeleteBehavior.Cascade); // عند حذف مقدم الخدمة، يتم حذف صوره
        }
    }
}
