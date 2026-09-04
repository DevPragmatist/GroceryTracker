# GroceryTracker - AI Coding Agent Instructions

## Project Overview
GroceryTracker is a .NET 10 ASP.NET Core API for managing grocery inventory, prices, and product tracking. It uses FastEndpoints for lightweight REST APIs, Entity Framework Core with SQL Server for data persistence, and supports Azure Key Vault integration with Microsoft Identity authentication.

## Architecture

### Feature Slices Organization
GroceryTracker uses **feature slices architecture** where each domain feature (e.g., Products, Categories) is vertically organized as a cohesive unit:

```
Services/[Feature]/
  ├── [Feature]Service.cs          (I[Feature]Service interface + implementation)
  ├── [Feature]Endpoint.cs         (FastEndpoints handlers)
  └── Models/
      ├── [Entity]AddDto.cs
      ├── [Entity]UpdateDto.cs
      └── [Request|Response]Dto.cs
```

**Benefits**: Clear feature boundaries, easier to extend/test, minimal cross-feature coupling.

### Core Layers
- **Domain** (`Domain/`): Entities, DbContext, Unit of Work contract
- **Services** (`Services/`): Feature-organized business logic
- **Infrastructure** (`Infrastructure/Startup/`): DI configuration, middleware setup

### Key Architectural Patterns

**Entity Base Pattern**: All domain entities inherit from `Entity` base class (`Domain/Entity.cs`) providing:
- Unique GUID `Id`
- `DateCreated` timestamp (auto-set to `DateTimeOffset.Now`)

**Fluent EF Core Configuration**: Entity type configurations use `IEntityTypeConfiguration<T>` pattern (e.g., `ProductEntityTypeConfiguration`). All are auto-registered via:
```csharp
modelBuilder.ApplyConfigurationsFromAssembly(typeof(GroceryContext).Assembly);
```

**Unit of Work Pattern**: `IUnitOfWork` interface abstracts EF Core DbContext:
- `Set<T>()` - tracked DbSets for add/update/delete
- `Query<T>()` - read-only AsNoTracking() queries
- `SaveChangesAsync()` - transaction commit
- Implemented directly by `GroceryContext`

**Auto-Registration of Services**: `AddApplicationServices()` extension method in `Infrastructure/Startup/AddApplicationServicesExtensions.cs` uses reflection to:
1. Find all classes ending with `"Service"` 
2. Register them as scoped by their interface (assumes single interface per service)
3. **New services must follow naming**: `[Feature]Service` + interface `I[Feature]Service`

## Service Patterns

**Service Structure**: Each feature (e.g., `Products/`) contains:
- `I[Feature]Service` interface + `[Feature]Service` implementation in same file
- DTOs in `Models/` subdirectory (DTO naming: `[Entity]AddDto`, `[Entity]UpdateDto`)
- `[Feature]Endpoint(s)` extending FastEndpoints `Endpoint<TRequest>`

**Example Service Layout** (Products):
```
Services/Products/
  ├── AddItemService.cs       (IAddItemService)
  ├── UpdateItemService.cs    (IUpdateItemService)
  ├── UpsertProductEndpoint.cs (POST /product)
  └── Models/
	  ├── ProductAddDto.cs
	  ├── ProductUpdateDto.cs
	  └── UpsertProductRequestDto.cs
```

## Data Model

**Core Entities**:
- `Product`: Name, Size, Price, CategoryId, PriceHistories (1-to-many)
- `Category`: Name, Description, Products (1-to-many)
- `Store`: Name, Location
- `PriceHistory`: Tracks price changes over time

**Decimal Precision**: Prices and sizes use `decimal(18,2)` column type for accuracy.

## Configuration & Startup

**Program.cs Initialization Order**:
1. Azure KeyVault configuration: `builder.Configuration.AddAzureKeyVault()`
   - Uses `DefaultAzureCredential()` in dev (TODO for production: use `ManagedIdentityCredential`)
   - Expects `KeyVaultName` in appsettings
2. JWT authentication via Microsoft Identity Web
3. FastEndpoints registration
4. EF Core DbContext with SQL Server
5. Automatic database migration on app start

**Connection String**: Named `"GroceryContext"` in appsettings.json (required, throws InvalidOperationException if missing)

## Authentication & Authorization

**Authentication**: Microsoft Entra ID via `Microsoft.Identity.Web`
- JWT bearer tokens validated against Entra ID tenant
- `[Authorize]` attribute on endpoints requiring authentication
- `User.GetObjectId()` and `User.GetTenantId()` available in endpoints

**Authorization**: Patterns are still being determined and should be added once decided.
- Will update with specific patterns once authorization requirements are finalized
- Consider: role-based (RBAC), policy-based, or custom claims-based approaches

## Error Handling

**Global Error Handling via FastEndpoints**:
- Centralized error handling through FastEndpoints' built-in error handler mechanism
- Errors are caught globally and mapped to standardized responses
- Avoid try-catch in individual endpoints; let FastEndpoints handle exceptions
- Define custom exception handlers by extending FastEndpoints' error handling pipeline

**Standard Error Response Pattern** (TBD):
- Structure and error codes to be finalized
- Will include HTTP status codes, error messages, and correlation IDs

## FastEndpoints Usage

**Endpoint Pattern**:
```csharp
public class UpsertProductEndpoint : Endpoint<UpsertProductRequestDto>
{
	public override void Configure()
	{
		Post("/product");
		AllowAnonymous(); // or [Authorize] via attribute
	}
	public override async Task HandleAsync(UpsertProductRequestDto req, CancellationToken ct)
	{
		// Service injection via constructor
		// No explicit response mapping (handled by FastEndpoints)
	}
}
```
- Endpoints are auto-discovered in `Program.cs` via `AddFastEndpoints()`
- DTOs are automatically serialized/deserialized
- Use `await _service.MethodAsync(ct)` with CancellationToken in handlers
- Errors propagate to global error handler (avoid try-catch at endpoint level)

## Common Workflows

**Adding a New Feature**:
1. Create entity class in `Domain/Entities/[Entity].cs` inheriting from `Entity`
2. Add `IEntityTypeConfiguration<[Entity]>` in same file for EF Core mappings
3. Register DbSet in `GroceryContext.cs`
4. Create service folder `Services/[Feature]/`
5. Implement `I[Feature]Service` interface + `[Feature]Service` class
6. Create endpoint `[Feature]Endpoint.cs` extending `Endpoint<RequestDto>`
7. Create request/response DTOs in `Services/[Feature]/Models/`
8. Create and apply migration: `Add-Migration [MigrationName]`

**Entity Queries via Unit of Work**:
```csharp
// Read-only (recommended)
var products = _unitOfWork.Query<Product>().Where(p => p.CategoryId == id).ToList();

// Tracked (for modifications)
var product = _unitOfWork.Set<Product>().FirstOrDefault(p => p.Id == id);
_unitOfWork.Set<Product>().Update(product);
await _unitOfWork.SaveChangesAsync(ct);
```

## Development Commands

- **Build**: `dotnet build`
- **Run**: `dotnet run` or `dotnet watch run` (auto-reload)
- **Migrations**: 
  - Create: `Add-Migration [MigrationName]`
  - Apply: `Update-Database` (automatic on app start)
- **API Docs**: Available at `/scalar/v1` in development (Scalar UI)

## Testing

**Unit Testing**: TBD (to be implemented after authentication setup)
- Will define patterns for service and endpoint testing
- FastEndpoints provides testing utilities for endpoint validation
- Consider Moq or NSubstitute for mocking IUnitOfWork dependencies

## Code Style Guidelines

- **Nullable Reference Types**: Enabled (`<Nullable>enable</Nullable>`)
- **Implicit Usings**: Enabled (`<ImplicitUsings>enable</ImplicitUsings>`)
- **File-scoped Namespaces**: Use `namespace GroceryTracker.Domain;` (no braces)
- **Record/Required Keywords**: Use for request DTOs and entity properties
- **Async-only**: Methods should be async with `CancellationToken` parameters

## Dependencies

Core NuGet packages:
- **FastEndpoints 8.3.0**: Lightweight REST API framework
- **EF Core 10.0.11**: ORM with SQL Server provider
- **Azure.Identity 1.21.0**: Managed identity / KeyVault auth
- **Microsoft.Identity.Web 4.14.2**: JWT / OpenID Connect
- **Scalar.AspNetCore 2.17.2**: Interactive API documentation

---
**Last Updated**: .NET 10 project with Microsoft Entra ID auth, feature slices architecture, and FastEndpoints global error handling (Feb 2025)
