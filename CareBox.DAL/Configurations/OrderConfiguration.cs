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
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            // OrderID (BigInt PK)
            builder.HasKey(o => o.OrderId);
            builder.Property(o => o.OrderId).HasColumnType("bigint");

            // OrderDate
            builder.Property(o => o.OrderDate)
                   .HasColumnType(DBTypes.DateTime2)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            // Status (TinyInt)
            builder.Property(o => o.Status)
                   .HasColumnType("tinyint")
                   .IsRequired();

            // TotalAmount
            builder.Property(o => o.TotalAmount)
                   .HasColumnType(DBTypes.Decimal18_2)
                   .IsRequired();

            //Relationship Client
            builder.HasOne(o => o.Client)
                   .WithMany(c => c.Orders)
                   .HasForeignKey(o => o.ClientId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
