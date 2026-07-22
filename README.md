# Shop Manager (.NET 10)

REST API for managing products, customers and orders, built with ASP.NET Core.

## Features

- CRUD for Products, Customers and Orders
- Repository Pattern implementation
- Global exception handling middleware
- Result pattern using ErrorOr
- Dependency Injection
- Unit tests for service classes

## Architecture

The project follows a clean structure:

- Controllers → handle HTTP requests
- Services → business logic
- Repositories → data access
- DTOs → data transfer between layers

## Tech Stack

- .NET 10 / ASP.NET Core
- Entity Framework Core
- SQL Server
- ErrorOr
- xUnit
- Moq
- FluentAssertions

## Getting Started

1. Clone the repository
2. Configure the connection string in `appsettings.json`
3. Run the project:

```bash
dotnet run
```
4. Access swagger at:
https://localhost:7207/swagger

*(Port may vary depending on your configuration)*

## Roadmap

- Add JWT authentication
- Add role-based authorization
- Add Integration test
- Add logs

## Notes

This project is part of my backend learning journey, focused on applying clean architecture
