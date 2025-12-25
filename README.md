# Portfolio Tracker API

A .NET 8 Web API for tracking investment portfolios.

## Features

- Customer management
- Portfolio tracking
- Cash balance management
- User authentication
- Multi-currency support

## Technologies

- .NET 8
- Entity Framework Core
- PostgreSQL
- FluentValidation
- BCrypt for password hashing

## Getting Started

### Prerequisites

- .NET 8 SDK
- PostgreSQL database

### Setup

1. Clone the repository
2. Update the connection string in `appsettings.json`
3. Run migrations:
   ```bash
   dotnet ef database update
   ```
4. Run the application:
   ```bash
   dotnet run --project PortfolioTracker.API
   ```

## API Documentation

Once running, visit `/swagger` to view the API documentation.

