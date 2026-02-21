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
    public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
    {
        public void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            // Primary Key
            builder.HasKey(rt => rt.Id);

            // Required fields
            builder.Property(rt => rt.TokenHash)
                   .IsRequired()
                   .HasMaxLength(500); 

            builder.Property(rt => rt.DeviceId)
                   .IsRequired()
                   .HasMaxLength(200);

            builder.Property(rt => rt.ExpiryDate)
                   .IsRequired();

            builder.Property(rt => rt.IsRevoked)
                   .HasDefaultValue(false);

            // Relation with AppUser
            builder.HasOne(rt => rt.User)
                   .WithMany(u => u.RefreshTokens)  
                   .HasForeignKey(rt => rt.UserId)   
                   .OnDelete(DeleteBehavior.Cascade);


        }
    }
}
