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
    public class SavedListingConfiguration : IEntityTypeConfiguration<SavedListing>
    {
        public void Configure(EntityTypeBuilder<SavedListing> builder)
        {
            // Composite PK
            builder.HasKey(sl => new { sl.ClientId, sl.ListingId });

            // Relationship Client
            builder.HasOne(sl => sl.Client)
                   .WithMany(c => c.SavedListings)
                   .HasForeignKey(sl => sl.ClientId)
                   .OnDelete(DeleteBehavior.Cascade);

            //Relationship Listing
            builder.HasOne(sl => sl.Listing)
                   .WithMany(l => l.SavedByUsers)
                   .HasForeignKey(sl => sl.ListingId)
                   .OnDelete(DeleteBehavior.Restrict); // Avoid cascade path conflict
        }
    }
}
