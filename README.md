# StoockerMT - Multi-Tenant SaaS Platform

[![Build Status](https://github.com/yourusername/StoockerMT/workflows/CI-CD/badge.svg)](https://github.com/yourusername/StoockerMT/actions)
[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)](https://dotnet.microsoft.com)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

StoockerMT is a modern, scalable multi-tenant SaaS platform built with .NET 9 and Clean Architecture principles. It provides a comprehensive business management solution with modules for CRM, Inventory Management, Accounting, and HR.

## 🚀 Features

- **Multi-Tenant Architecture**: Secure data isolation with separate databases per tenant
- **Modular Design**: Enable/disable modules per tenant (CRM, Inventory, Accounting, HR)
- **Clean Architecture**: Domain-driven design with clear separation of concerns
- **Modern Tech Stack**: .NET 9, Entity Framework Core, SQL Server, Redis
- **Security First**: JWT authentication, role-based authorization, data encryption
- **API-First**: RESTful API with Swagger documentation
- **Performance**: Caching, async operations, optimized queries
- **Monitoring**: Health checks, logging with Serilog, metrics

## 📋 Prerequisites

- .NET 9 SDK
- SQL Server 2019+ (or Docker)
- Redis (optional, for caching)
- Docker & Docker Compose (for containerized deployment)
- Visual Studio 2022 or VS Code

## 🛠️ Quick Start

### 1. Clone the repository
```bash
git clone https://github.com/yourusername/StoockerMT.git
cd StoockerMT
```

### 2. Set up the database
```bash
# Using Docker
docker-compose up -d sqlserver redis

# Or update connection strings in appsettings.json for your local SQL Server
```

### 3. Run database migrations
```bash
dotnet ef database update --project StoockerMT.Persistence --context MasterDbContext
```

### 4. Run the application
```bash
dotnet run --project StoockerMT.API
```

The API will be available at:
- HTTP: `http://localhost:5000`
- HTTPS: `https://localhost:5001`
- Swagger UI: `http://localhost:5000/swagger`

## 🐳 Docker Deployment

### Build and run with Docker Compose
```bash
docker-compose up -d
```

This will start:
- SQL Server
- Redis
- StoockerMT API
- Nginx (reverse proxy)
- Monitoring stack (Prometheus, Grafana, Jaeger)

## 🏗️ Architecture

```
StoockerMT/
├── src/
│   ├── Core/
│   │   ├── StoockerMT.Domain/          # Entities, Value Objects, Interfaces
│   │   └── StoockerMT.Application/     # Use Cases, CQRS, DTOs
│   ├── Infrastructure/
│   │   ├── StoockerMT.Infrastructure/  # External Services
│   │   ├── StoockerMT.Persistence/     # EF Core, Repositories
│   │   └── StoockerMT.Identity/        # Authentication/Authorization
│   └── Presentation/
│       └── StoockerMT.API/             # Web API, Controllers
├── tests/
│   ├── StoockerMT.Domain.Tests/
│   ├── StoockerMT.Application.Tests/
│   └── StoockerMT.Integration.Tests/
└── docker/
```

## 📦 Modules

### CRM Module
- Customer management
- Lead tracking
- Sales pipeline
- Contact management

### Inventory Module
- Product catalog
- Stock tracking
- Warehouse management
- Reorder automation

### Accounting Module
- Chart of accounts
- Journal entries
- Financial reporting
- Multi-currency support

### HR Module
- Employee records
- Leave management
- Timesheet tracking
- Department/Position hierarchy

## 🔧 Configuration

### Application Settings
Key configuration options in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "MasterConnection": "Your master DB connection",
    "TenantConnection": "Your tenant DB template"
  },
  "JwtSettings": {
    "Secret": "Your-256-bit-secret",
    "ExpirationInMinutes": 60
  },
  "TenantSettings": {
    "TrialDays": 30,
    "DefaultModules": ["CRM", "INV"]
  }
}
```

### Environment Variables
For production, use environment variables:
- `ConnectionStrings__MasterConnection`
- `ConnectionStrings__TenantConnection`
- `JwtSettings__Secret`
- `ASPNETCORE_ENVIRONMENT`

## 🧪 Testing

### Run all tests
```bash
dotnet test
```

### Run with coverage
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Integration tests with Docker
```bash
docker-compose -f docker-compose.test.yml up --abort-on-container-exit
```

## 📚 API Documentation

### Authentication
All API endpoints (except `/api/auth/*`) require JWT authentication:

```bash
curl -X POST https://localhost:5001/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@example.com","password":"Admin123!"}'
```

### Create a Tenant
```bash
curl -X POST https://localhost:5001/api/v1/tenants \
  -H "Authorization: Bearer {token}" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Acme Corporation",
    "code": "ACME001",
    "adminEmail": "admin@acme.com",
    "selectedModules": ["CRM", "INV"]
  }'
```

## 🔐 Security

- JWT token-based authentication
- Role-based authorization (Super Admin, Tenant Admin, User)
- Data encryption at rest
- HTTPS enforcement
- Rate limiting
- CORS configuration
- SQL injection protection via parameterized queries

## 📊 Monitoring

- **Health Checks**: `/health`
- **Metrics**: Prometheus endpoint at `/metrics`
- **Tracing**: Jaeger UI at `http://localhost:16686`
- **Logs**: Structured logging with Serilog

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

### Coding Standards
- Follow C# coding conventions
- Write unit tests for new features
- Update documentation
- Ensure all tests pass
- Add integration tests for API endpoints

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👥 Team

- Your Name - Initial work - [YourGitHub](https://github.com/yourusername)

## 🙏 Acknowledgments

- Built with Clean Architecture principles by Uncle Bob
- Inspired by Domain-Driven Design by Eric Evans
- Uses MediatR for CQRS pattern implementation

---

For more information, please check our [Wiki](https://github.com/yourusername/StoockerMT/wiki) or contact support@stoockermt.com