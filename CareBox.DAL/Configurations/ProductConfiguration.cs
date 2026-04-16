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
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            //ProductID PK
            builder.HasKey(p => p.ProductId);

            // Name
            builder.Property(p => p.Name)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(150)
                   .IsRequired();

            // Description
            builder.Property(p => p.Description)
                   .HasColumnType(DBTypes.NvarCharMax);

            // Price
            builder.Property(p => p.Price)
                   .HasColumnType(DBTypes.Decimal18_2)
                   .IsRequired();

            // ForModel
            builder.Property(p => p.ForModel)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(50)
                   .IsRequired();

            // Make
            builder.Property(p => p.Make)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(50)
                   .IsRequired();

            // Year (SmallInt)
            builder.Property(p => p.Year)
                   .HasColumnType("smallint")
                   .IsRequired();

            // --- Enums ---
            // StockStatus
            builder.Property(p => p.StockStatus)
                   .HasColumnType(DBTypes.Int)
                   .IsRequired();
            // Condition
            builder.Property(p => p.Condition)
                   .HasColumnType(DBTypes.Int)
                   .IsRequired();

            // HorizontalPosition (Nullable Int)
            builder.Property(p => p.HorizontalPosition)
                   .HasColumnType(DBTypes.Int)
                   .IsRequired(false);

            // VerticalPosition (Nullable Int)
            builder.Property(p => p.VerticalPosition)
                   .HasColumnType(DBTypes.Int)
                   .IsRequired(false);


            builder.Property(p => p.StockQuantity)
                   .HasColumnType(DBTypes.Int)
                   .IsRequired()
                   .HasDefaultValue(0);



            // Relationship ServiceProvider
            builder.HasOne(p => p.ServiceProvider)
                   .WithMany(sp => sp.Products)
                   .HasForeignKey(p => p.ServiceProviderId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(p => p.ProductCategory)
                   .WithMany(pc => pc.Products) // تأكد أن كلاس ProductCategory يحتوي على ICollection<Product>
                   .HasForeignKey(p => p.ProductCategoryId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
