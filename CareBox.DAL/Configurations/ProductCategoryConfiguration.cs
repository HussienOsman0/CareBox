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
    public class ProductCategoryConfiguration : IEntityTypeConfiguration<ProductCategory>
    {
        public void Configure(EntityTypeBuilder<ProductCategory> builder)
        {
            // إعداد الـ Primary Key
            builder.HasKey(pc => pc.Id);

            // إعداد خاصية Name
            builder.Property(pc => pc.Name)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(100)
                   .IsRequired();

           
        }
    }
}
