using CareBox.BLL.Repositories.Interfaces;
using CareBox.DAL.Contexts;
using CareBox.DAL.Models;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CareBox.BLL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly CareBoxContext _context;

        public UnitOfWork(CareBoxContext context)
        {
            _context = context;

            // Identity
            AppUsers = new GenericRepository<AppUser>(_context);

            // Client & Vehicles
            Clients = new GenericRepository<Client>(_context);
            Vehicles = new GenericRepository<Vehicle>(_context);

            // Providers
            ProviderTypes = new GenericRepository<ProviderType>(_context);
            ServiceProviders = new GenericRepository<ServiceProvider>(_context);
            Technicians = new GenericRepository<Technician>(_context);

            // Services & Products
            Services = new GenericRepository<Service>(_context);
            Products = new GenericRepository<Product>(_context);

            // Bookings & Orders
            Bookings = new GenericRepository<Booking>(_context);
            Invoices = new GenericRepository<Invoice>(_context);
            InvoiceDetails = new GenericRepository<InvoiceDetail>(_context);
            Orders = new GenericRepository<Order>(_context);
            OrderDetails = new GenericRepository<OrderDetail>(_context);

            // Emergency & Reviews
            EmergencyRequests = new GenericRepository<EmergencyRequest>(_context);
            Reviews = new GenericRepository<Review>(_context);

            // Marketplace
            Listings = new GenericRepository<Listing>(_context);
            ListingImages = new GenericRepository<ListingImage>(_context);
            SavedListings = new GenericRepository<SavedListing>(_context);

            RefreshTokens = new GenericRepository<RefreshToken>(_context);
        }

        // Properties Implementation
        public IGenericRepository<AppUser> AppUsers { get; }

        public IGenericRepository<Client> Clients { get; }
        public IGenericRepository<Vehicle> Vehicles { get; }

        public IGenericRepository<ProviderType> ProviderTypes { get; }
        public IGenericRepository<ServiceProvider> ServiceProviders { get; }
        public IGenericRepository<Technician> Technicians { get; }

        public IGenericRepository<Service> Services { get; }
        public IGenericRepository<Product> Products { get; }

        public IGenericRepository<Booking> Bookings { get; }
        public IGenericRepository<Invoice> Invoices { get; }
        public IGenericRepository<InvoiceDetail> InvoiceDetails { get; }
        public IGenericRepository<Order> Orders { get; }
        public IGenericRepository<OrderDetail> OrderDetails { get; }

        public IGenericRepository<EmergencyRequest> EmergencyRequests { get; }
        public IGenericRepository<Review> Reviews { get; }

        public IGenericRepository<Listing> Listings { get; }
        public IGenericRepository<ListingImage> ListingImages { get; }
        public IGenericRepository<SavedListing> SavedListings { get; }

        public IGenericRepository<RefreshToken> RefreshTokens { get; }

        public async Task<int> SaveAsync()
        {
            return await _context.SaveChangesAsync();
        }
        public IDbContextTransaction BeginTransaction()
        {
            return _context.Database.BeginTransaction();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
