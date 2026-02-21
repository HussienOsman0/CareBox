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
    public class InvoiceDetailConfiguration : IEntityTypeConfiguration<InvoiceDetail>
    {
        public void Configure(EntityTypeBuilder<InvoiceDetail> builder)
        {
            //InvoiceDetailID (BigInt PK)
            builder.HasKey(id => id.InvoiceDetailId);
            builder.Property(id => id.InvoiceDetailId).HasColumnType("bigint");

            //ItemDescription
            builder.Property(id => id.ItemDescription)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(200)
                   .IsRequired();

            //Price
            builder.Property(id => id.Price)
                   .HasColumnType(DBTypes.Decimal18_2)
                   .IsRequired();

            // Relationship Invoice
            builder.HasOne(id => id.Invoice)
                   .WithMany(i => i.InvoiceDetails)
                   .HasForeignKey(id => id.InvoiceId)
                   .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
