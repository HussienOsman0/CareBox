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
    public class CartConfiguration : IEntityTypeConfiguration<Cart>
    {
        public void Configure(EntityTypeBuilder<Cart> builder)
        {
            builder.HasKey(c => c.Id);

            // علاقة 1-to-1 بين العميل والسلة
            builder.HasOne(c => c.Client)
                   .WithOne(cl => cl.Cart)
                   .HasForeignKey<Cart>(c => c.ClientId)
                   .OnDelete(DeleteBehavior.Cascade); // لو العميل اتمسح، سلته تتمسح
        }
    }
}
