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
    public class OrderDetailConfiguration : IEntityTypeConfiguration<OrderDetail>
    {
        public void Configure(EntityTypeBuilder<OrderDetail> builder)
        {
            // Composite PK
            builder.HasKey(od => new { od.OrderId, od.ProductId });

            // Quantity
            builder.Property(od => od.Quantity)
                   .HasColumnType(DBTypes.Int)
                   .IsRequired();

            // PriceAtPurchase
            builder.Property(od => od.PriceAtPurchase)
                   .HasColumnType(DBTypes.Decimal18_2)
                   .IsRequired();

            // Relationship Order
            builder.HasOne(od => od.Order)
                   .WithMany(o => o.OrderDetails)
                   .HasForeignKey(od => od.OrderId)
                   .OnDelete(DeleteBehavior.Cascade);

            //Relationship Product
            builder.HasOne(od => od.Product)
                   .WithMany(p => p.OrderDetails)
                   .HasForeignKey(od => od.ProductId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
