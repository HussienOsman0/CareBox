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
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(c => c.ClientID);

            builder.Property(c=>c.FullName)
                .HasColumnType(DBTypes.NvarChar)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.CreatedAt)
                   .HasColumnType(DBTypes.DateTime2)
                   .HasDefaultValueSql("GETDATE()");

            builder.HasOne(c=>c.AppUser)
                .WithOne(u=>u.Client)
                .HasForeignKey<Client>(c=>c.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
