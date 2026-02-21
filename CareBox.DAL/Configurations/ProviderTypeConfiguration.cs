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
    public class ProviderTypeConfiguration : IEntityTypeConfiguration<ProviderType>
    {
        public void Configure(EntityTypeBuilder<ProviderType> builder)
        {
            //ProviderTypeID (TinyInt PK)
            builder.HasKey(pt => pt.ProviderTypeId);


            // TypeName (Unique)
            builder.Property(pt => pt.TypeName)
                   .HasColumnType(DBTypes.NvarChar)
                   .HasMaxLength(50)
                   .IsRequired();

            
        }
    }
}
