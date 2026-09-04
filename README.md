# GroceryTracker

A modern ASP.NET Core REST API for managing grocery inventory, product pricing, and store locations. Built with .NET 10, FastEndpoints, Entity Framework Core, and cloud-native technologies.

## Features

- Product management with multi-store support
- Automatic price history tracking
- Microsoft Entra ID authentication
- Azure KeyVault integration
- Interactive API documentation (Scalar UI)
- Global error handling

## Tech Stack

- **.NET 10** - Latest framework with modern language features
- **FastEndpoints** - High-performance REST API framework
- **Entity Framework Core 10** - ORM with SQL Server
- **Microsoft Entra ID** - Cloud identity authentication
- **Azure KeyVault** - Secrets management
- **Scalar** - Interactive API documentation

## Getting Started

### Prerequisites
- .NET 10 SDK or later
- SQL Server (local or remote)
- Azure subscription (for KeyVault and Entra ID)
- Visual Studio Community 2026+ or VS Code

### Installation

```bash
git clone https://github.com/DevPragmatist/GroceryTracker.git
cd GroceryTracker
dotnet restore
dotnet build
dotnet run
```

Access the API docs at `http://localhost:5000/scalar/v1`

### Configuration

Update `appsettings.Development.json`:
- `ConnectionStrings:GroceryContext` - SQL Server connection string
- `KeyVaultName` - Azure KeyVault name
- Azure Entra ID settings (tenant ID, client ID)

Database migrations run automatically on startup.

## Project Structure

```
GroceryTracker/
├── Domain/              # Core entities and data access
├── Services/            # Feature-organized business logic
│   └── Products/        # Product management feature
├── Infrastructure/      # DI configuration and setup
├── Migrations/          # EF Core migrations
└── Program.cs           # Application entry point
```

## Architecture

Uses **feature slices** organization - each domain feature is vertically organized with its own service, endpoint, and DTOs:

```
Services/[Feature]/
├── [Feature]Service.cs      # Business logic
├── [Feature]Endpoint.cs     # HTTP handlers
└── Models/                  # DTOs
```

**Patterns used:**
- Unit of Work for data access abstraction
- Dependency Injection with auto-registration via reflection
- Entity base class with GUID identity and audit timestamp
- Fluent EF Core configuration with `IEntityTypeConfiguration<T>`

## API Example

**POST** `/product` - Create or update a product

```json
{
  "productId": null,
  "name": "Organic Bananas",
  "categoryId": "550e8400-e29b-41d4-a716-446655440000",
  "categoryName": "Produce",
  "storeId": "660e8400-e29b-41d4-a716-446655440000",
  "storeName": "Whole Foods",
  "location": "Downtown",
  "size": 1.5,
  "sizeUnit": "lb",
  "price": 0.59,
  "purchaseDate": "2025-02-10T14:30:00Z"
}
```

Requires Microsoft Entra ID authentication.

## Learning Objectives

This portfolio project demonstrates:
- Modern .NET best practices (.NET 10, nullable reference types, file-scoped namespaces)
- Professional architecture patterns (feature slices, Unit of Work, DI)
- Cloud integration (Azure KeyVault, Entra ID, Managed Identity)
- High-performance REST APIs (FastEndpoints)
- Enterprise database design (EF Core, SQL Server)
- Clean code principles (SOLID, DRY, separation of concerns)

Future extensions will showcase additional languages (TypeScript, Python, Go) and frameworks.

## License

This project is provided as-is for portfolio and educational purposes.

---

**Author**: DevPragmatist | **Status**: Active Development
