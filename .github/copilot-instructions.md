# Copilot Instructions for BookLAB

## Architecture Overview
This solution follows **Clean Architecture** principles with **CQRS** using MediatR.

- **src/BookLAB.API**: Entry point, Controllers. Depends on Application and Infrastructure.
- **src/BookLAB.Application**: Core business logic, MediatR Handlers, Interfaces, Validators. No direct DB dependencies.
- **src/BookLAB.Domain**: Enterprise entities, Enums, Value Objects. No dependencies.
- **src/BookLAB.Infrastructure**: Implementation of interfaces (EF Core, Services). depends on Application and Domain.

## Tech Stack
- **Framework**: .NET 10.0
- **Database**: PostgreSQL (`Npgsql`)
- **ORM**: EF Core 10.0
- **Mediator**: MediatR
- **Validation**: FluentValidation
- **Mapping**: AutoMapper

## Developer Workflow & Patterns

### 1. Creating New Features (CQRS)
Organize code by feature in `src/BookLAB.Application/Features/<FeatureName>`.
- **Commands**: Modify state. Return `Unit` or Resource ID (`Guid`).
- **Queries**: Read state. Return DTOs.
- **Structure**:
  ```text
  Features/SomeFeature/
  ├── Commands/
  │   └── CreateBooking/
  │       ├── CreateBookingCommand.cs (record)
  │       ├── CreateBookingCommandValidator.cs
  │       └── CreateBookingCommandHandler.cs
  └── Queries/
  ```

### 2. Database Access
- Use `IBookLABDbContext` in handlers for distinct separation.
- **Queries**: Use `ProjectTo<Dto>(_mapper.ConfigurationProvider)` for efficient projection.
- **Commands**: 
  - Use async methods: `ToListAsync`, `FirstOrDefaultAsync`.
  - Always accept and pass `CancellationToken`.

### 3. Validation
- Define validators in the same namespace as the Command/Query.
- Inherit from `AbstractValidator<T>`.
- Validation is handled automatically by `ValidationBehavior`. Do not manually validate in controllers.

### 4. Domain & Entities
- Entities inherit from `BaseEntity` (provides `Id` as `Guid`).
- Implement interfaces like `IAuditable` or `ISoftDeletable` where appropriate.
- Configure EF mappings in `src/BookLAB.Infrastructure/Persistence/Configurations`.

### 5. Dependency Injection
- Register Application services in `src/BookLAB.Application/DependencyInjection.cs`.
- Register Infrastructure services in `src/BookLAB.Infrastructure/DependencyInjection.cs`.

## Common Commands
- **Run API**: `dotnet run --project src/BookLAB.API`
- **Add Migration**: `dotnet ef migrations add <Name> --project src/BookLAB.Infrastructure --startup-project src/BookLAB.API`
- **Update DB**: `dotnet ef database update --project src/BookLAB.Infrastructure --startup-project src/BookLAB.API`

## Rules
- **Async/Await**: Use async/await for all I/O bound operations.
- **Controller Implementation**: Controllers should be thin wrappers sending MediatR requests.
- **Formatting**: Follow standard C# naming conventions (PascalCase for classes/methods, camelCase for variables).
