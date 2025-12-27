# MockHub

MockHub is a professional mock API management platform designed for teams and individual developers. It provides a web-based interface for creating, managing, and monitoring mock APIs, with support for dynamic responses, team collaboration, and real-time request logging.

## Overview

MockHub enables developers to create realistic mock APIs without writing custom server code. It supports path-based routing, allowing multiple projects to run simultaneously on the same server instance. The platform is built with ASP.NET Core and Blazor Server, providing a modern web interface for managing mock endpoints.

## Key Features

### Core Capabilities
- **Path-Based Routing**: Projects are accessible via unique URL paths (e.g., `/project-slug/api/users`)
- **Team Support**: Create team workspaces with shared mock projects
- **Individual Projects**: Personal projects for solo development
- **Real-Time Logging**: Monitor API requests in real-time using SignalR
- **Request Logging**: Persistent request logs with detailed request/response information

### API Features
- All HTTP methods (GET, POST, PUT, PATCH, DELETE, HEAD, OPTIONS)
- Route parameters (`/api/users/{id}`, `/api/users/:id`)
- Query string parsing and validation
- Request body parsing (JSON, XML, form data)
- Required parameter validation
- Custom response headers
- Configurable response delays

### Dynamic Variables
- Random data generation (names, emails, addresses, dates, UUIDs)
- Request parameter access (route params, query strings, headers, body)
- Template-based responses using Scriban
- Conditional response logic
- Faker library integration for realistic test data

### Security & Authentication
- Admin-based user management
- Team role-based access control
- JWT token simulation and validation
- CORS configuration per project
- Secure password management

## Technology Stack

| Layer | Technology |
|-------|-----------|
| UI | Blazor Server, Bootstrap 5 |
| Backend | ASP.NET Core 8 |
| Database | SQLite with Entity Framework Core 8 |
| Authentication | ASP.NET Core Identity |
| Real-Time | SignalR |
| Template Engine | Scriban |
| Fake Data | Bogus |
| Logging | Serilog |

## Quick Start

### Prerequisites
- .NET 8 SDK
- (Optional) Docker and Docker Compose

### Development Setup

```bash
# Clone the repository
git clone https://github.com/your-repo/MockHub.git
cd MockHub

# Restore packages and build
dotnet restore
dotnet build

# Run the application
cd src/MockHub.Web
dotnet run
```

Navigate to `http://localhost:5268` in your browser. On first launch, you'll be prompted to create an administrator account.

### Docker Deployment

```bash
# Using Docker Compose
cd docker
docker-compose up -d

# Or build and run manually
docker build -t mockhub -f docker/Dockerfile .
docker run -p 5268:5268 mockhub
```

## Project Structure

```
MockHub/
├── src/
│   ├── MockHub.Domain/         # Domain entities and enums
│   ├── MockHub.Application/    # DTOs and service interfaces
│   ├── MockHub.Infrastructure/ # EF Core, Identity, service implementations
│   ├── MockHub.MockEngine/     # Route matching, template processing
│   ├── MockHub.HostManager/    # Mock server hosting management
│   └── MockHub.Web/            # Blazor Server UI and API controllers
├── tests/
│   ├── MockHub.UnitTests/
│   └── MockHub.IntegrationTests/
├── docker/
│   ├── Dockerfile
│   └── docker-compose.yml
└── MockHub.sln
```

## Basic Usage

### Creating a Project

1. Log in to the application
2. Navigate to Projects from the sidebar
3. Click "New Project"
4. Enter project name and description
5. Configure CORS and logging settings
6. Save the project

### Adding Endpoints

1. Open your project from the Projects page
2. Click "Add Endpoint"
3. Select HTTP method and define the route
4. Configure response status code and body
5. Add custom headers if needed
6. Set up validation rules for request parameters
7. Save the endpoint

### Using Dynamic Variables

In your response body, use template variables:

```json
{
  "id": "{{faker.random.uuid}}",
  "name": "{{faker.name.fullName}}",
  "email": "{{faker.internet.email}}",
  "userId": "{{request.params.id}}",
  "page": "{{request.query.page}}",
  "createdAt": "{{now}}"
}
```

### Accessing Mock APIs

Personal projects are accessible at:
```
http://localhost:5268/{project-slug}/api/{endpoint}
```

Team projects are accessible at:
```
http://localhost:5268/{team-slug}/{project-slug}/api/{endpoint}
```

## Configuration

### Database

The application uses SQLite by default. The database file is created automatically on first run at `src/MockHub.Web/mockhub.db`.

### Application Settings

Key settings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=mockhub.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information"
    }
  }
}
```

## Documentation

For detailed documentation, including installation guides, API reference, and advanced usage, see [DOCUMENTATION.md](DOCUMENTATION.md).

## License

MIT License - See LICENSE file for details.
