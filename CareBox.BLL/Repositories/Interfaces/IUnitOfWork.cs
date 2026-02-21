using CareBox.DAL.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Repositories.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        // Identity & Users
        IGenericRepository<AppUser> AppUsers { get; }

        // Client & Vehicles Module
        IGenericRepository<Client> Clients { get; }
        IGenericRepository<Vehicle> Vehicles { get; }

        // Service Providers Module
        IGenericRepository<ProviderType> ProviderTypes { get; }
        IGenericRepository<ServiceProvider> ServiceProviders { get; }
        IGenericRepository<Technician> Technicians { get; }

        // Services & Products Module
        IGenericRepository<Service> Services { get; }
        IGenericRepository<Product> Products { get; }

        // Bookings & Orders Module
        IGenericRepository<Booking> Bookings { get; }
        IGenericRepository<Invoice> Invoices { get; }
        IGenericRepository<InvoiceDetail> InvoiceDetails { get; }
        IGenericRepository<Order> Orders { get; }
        IGenericRepository<OrderDetail> OrderDetails { get; }

        // Emergency & Reviews Module
        IGenericRepository<EmergencyRequest> EmergencyRequests { get; }
        IGenericRepository<Review> Reviews { get; }

        // Car Marketplace Module
        IGenericRepository<Listing> Listings { get; }
        IGenericRepository<ListingImage> ListingImages { get; }
        IGenericRepository<SavedListing> SavedListings { get; }


        //Token Management
        IGenericRepository<RefreshToken> RefreshTokens { get; }


        Task<int> SaveAsync();
        IDbContextTransaction BeginTransaction();

    }
}
