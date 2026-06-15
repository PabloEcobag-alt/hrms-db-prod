# HRMS API

.NET backend API for Human Resource Management System with PostgreSQL and Entity Framework Core.

## Prerequisites

- .NET 10 SDK
- PostgreSQL (local installation or Docker)
- EF Core tools (`dotnet tool install --global dotnet-ef`)

## Installation

1. Restore dependencies:
```bash
dotnet restore
```

2. Configure database connection in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=hrm_db;Username=postgres;Password=postgres"
  }
}
```

3. Run database migrations:
```bash
dotnet ef database update
```

## Environment Variables

Configure the following settings in `appsettings.json`:

### Database Configuration
- `DefaultConnection=<insert_postgres_connection_string>` - PostgreSQL connection string

### JWT Configuration
- `SecretKey=<insert_jwt_secret_key>` - JWT secret key for token generation
- `Issuer=<insert_jwt_issuer>` - JWT issuer (e.g., "api-auth-server")
- `Audience=<insert_jwt_audience>` - JWT audience (e.g., "api-auth-clients")
- `CookieName=<insert_cookie_name>` - Cookie name for auth token (default: "erp_access_token")

### Company Settings
- `Name=<insert_company_name>` - Company name
- `Address=<insert_company_address>` - Company address
- `Contact=<insert_company_contact>` - Company contact number
- `Email=<insert_company_email>` - Company email address

## Run Commands

Start the API server:
```bash
dotnet run
```

The API will run on port 5165 by default.

## Build

```bash
dotnet build
```

## Project Structure

- `Api/` - Controllers and DTOs
- `Applications/` - Services and interfaces
- `Domains/` - Entity models
- `Infrastructures/` - Database context and persistence
- `Migrations/` - EF Core database migrations
