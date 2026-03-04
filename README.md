CareBox - Automotive Services & Marketplace Platform
📖 About the Project
CareBox is a comprehensive, all-in-one automotive ecosystem designed to bridge the gap between car owners and automotive service providers. Built with ASP.NET Core, it offers a robust backend infrastructure that handles everything from vehicle maintenance bookings and spare parts e-commerce to emergency roadside assistance and a peer-to-peer car marketplace.

✨ Key Features
👤 Dual-User Architecture & Security

Custom Identity system supporting both Clients (car owners) and Service Providers (workshops, stores).

Secure authentication utilizing JWT with Refresh Tokens and OTP verification.

🚗 Garage & Vehicle Management

Clients can register and manage multiple vehicles.

Tracks vehicle details including Make, Model, Year, Plate Number, and Kilometers driven.

🛠️ Service Providers & Technician Tracking

Supports multiple provider types: Maintenance, Spare Parts, Emergency, and Car Care.

Geospatial Integration: Uses NetTopologySuite to track the exact real-time locations of service providers and mobile technicians.

🛒 E-commerce & Service Bookings

Service providers can list specific services and physical auto parts (Products) filtered by car model.

Complete Order Management System (OMS) for purchasing parts.

Booking system for scheduling maintenance appointments, complete with automated invoice generation.

🆘 Emergency Roadside Assistance

On-demand SOS requests for Towing, Fuel Delivery, Battery Jump-starts, and Flat Tires.

Location-based matching to assign available technicians to the exact client's geographic coordinates.

🏷️ Peer-to-Peer Car Marketplace

Integrated classifieds section where clients can list their vehicles for sale.

Features include image galleries, pricing, descriptions, and a "Saved Listings" (Wishlist) functionality for buyers.

⭐ Review & Rating System

Transparent feedback system allowing clients to rate and review service providers to ensure quality control.

🛠️ Technical Highlights
Framework: .NET / ASP.NET Core Web API

Database ORM: Entity Framework Core

Spatial Data: NetTopologySuite for mapping and distance calculations (Geography/Point).

Authentication: ASP.NET Core Identity with custom Token Management.

Architecture: Modular Domain-Driven Design approach separating Identity, Marketplace, Orders, and Emergency contexts.
