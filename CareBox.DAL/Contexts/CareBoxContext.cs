using CareBox.DAL.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.DAL.Contexts
{
    public class CareBoxContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public CareBoxContext(DbContextOptions<CareBoxContext> options) : base(options)
        {
        }

        #region Authentication & Tokens
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        #endregion

        #region Client & Vehicles Module
        public DbSet<Client> Clients { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
        #endregion

        #region Service Providers Module
        public DbSet<ProviderType> ProviderTypes { get; set; }
        public DbSet<ServiceProvider> ServiceProviders { get; set; }
        public DbSet<Technician> Technicians { get; set; }
        #endregion

        #region Services & Products Module
        public DbSet<Service> Services { get; set; }
        public DbSet<Product> Products { get; set; }
        #endregion

        #region Bookings & Orders Module
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        #endregion

        #region Emergency & Reviews Module
        public DbSet<EmergencyRequest> EmergencyRequests { get; set; }
        public DbSet<Review> Reviews { get; set; }
        #endregion

        #region Marketplace Module
        public DbSet<Listing> Listings { get; set; }
        public DbSet<ListingImage> ListingImages { get; set; }
        public DbSet<SavedListing> SavedListings { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            // === بداية الكود النضيف (Data Seeding) ===
            modelBuilder.Entity<ProviderType>().HasData(
                new ProviderType { ProviderTypeId = 1, TypeName = "Maintenance" },
                new ProviderType { ProviderTypeId = 2, TypeName = "Spare Parts" },
                new ProviderType { ProviderTypeId = 3, TypeName = "Emergency" },
                new ProviderType { ProviderTypeId = 4, TypeName = "Car Care" }
                );
        

                    modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
