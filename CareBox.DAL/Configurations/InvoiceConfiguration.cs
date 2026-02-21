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
    public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
    {
        public void Configure(EntityTypeBuilder<Invoice> builder)
        {
            // InvoiceID (BigInt PK)
            builder.HasKey(i => i.InvoiceId);
            builder.Property(i => i.InvoiceId).HasColumnType("bigint");

            // TotalAmount
            builder.Property(i => i.TotalAmount)
                   .HasColumnType(DBTypes.Decimal18_2)
                   .IsRequired();

            // IssueDate
            builder.Property(i => i.IssueDate)
                   .HasColumnType(DBTypes.DateTime)
                   .HasDefaultValueSql("GETDATE()")
                   .IsRequired();

            // Relationship with Booking
            builder.HasOne(i => i.Booking)
                   .WithOne(b => b.Invoice)
                   .HasForeignKey<Invoice>(i => i.BookingId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
