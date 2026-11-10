# GroceryTracker

A modern ASP.NET Core REST API for managing grocery inventory, product pricing, and store locations. Built with .NET 10, FastEndpoints, Entity Framework Core, and cloud-native technologies.

## Features

- Product management with multi-store support
- Automatic price history tracking
- Local ASP.NET Core Identity authentication with JWT tokens
- Interactive API documentation (Scalar UI)
- Global error handling

## Tech Stack

- **.NET 10** - Latest framework with modern language features
- **FastEndpoints** - High-performance REST API framework
- **Entity Framework Core 10** - ORM with SQL Server
- **ASP.NET Core Identity** - Local user accounts and password management
- **Scalar** - Interactive API documentation

## Getting Started

### Prerequisites
- .NET 10 SDK or later
- SQL Server (local or remote)
- Visual Studio Community 2026+ or VS Code

### Installation

```bash
git clone https://github.com/DevPragmatist/GroceryTracker.git
cd GroceryTracker
dotnet restore
dotnet build
dotnet run
```

Start the app in Visual Studio to open the plain HTML login/register page. After signing in, it stores the short-lived access token for the current browser session and opens Scalar at `/scalar/v1`. Visiting Scalar without a session token sends you back to the login page. Scalar uses the token as a Bearer credential for protected requests.

### Configuration

Update `appsettings.Development.json`:
- `ConnectionStrings:GroceryContext` - SQL Server connection string
- JWT signing and token lifetime settings
- SMTP settings for password reset email delivery

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

Requires a valid JWT from the local identity authentication flow. Authenticated user claims are available to API handlers through `HttpContext.User` (including the user ID, username, and email).

## Learning Objectives

This portfolio project demonstrates:
- Modern .NET best practices (.NET 10, nullable reference types, file-scoped namespaces)
- Professional architecture patterns (feature slices, Unit of Work, DI)
- Cloud integration-ready configuration for SQL Server and SMTP, with local identity auth
- High-performance REST APIs (FastEndpoints)
- Enterprise database design (EF Core, SQL Server)
- Clean code principles (SOLID, DRY, separation of concerns)

Future extensions will showcase additional languages (TypeScript, Python, Go) and frameworks.

## License

This project is provided as-is for portfolio and educational purposes.

---

**Author**: DevPragmatist | **Status**: Active Development
