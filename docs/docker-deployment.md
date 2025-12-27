# Docker Deployment

MockHub provides comprehensive Docker support for easy deployment and containerization. This section covers Docker setup, deployment strategies, configuration, and best practices for running MockHub in containerized environments.

## Overview

Docker deployment includes:
- Multi-stage Docker builds
- SQLite database persistence
- Environment-specific configurations
- Docker Compose orchestration
- Health checks and monitoring

## Docker Architecture

### Container Structure

MockHub uses a multi-stage Docker build:

```dockerfile
# Build stage - .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
# Copy source and build application

# Runtime stage - .NET ASP.NET Core Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
# Run the application
```

### Image Layers

- **Base Layer**: ASP.NET Core runtime
- **Application Layer**: Compiled MockHub binaries
- **Configuration Layer**: Environment-specific settings
- **Data Layer**: SQLite database (mounted volume)

## Quick Start with Docker

### Prerequisites

- Docker Engine 20.10+
- Docker Compose 2.0+
- 2GB available RAM
- 500MB available disk space

### Single Container Deployment

```bash
# Pull and run MockHub
docker run -d \
  --name mockhub \
  -p 5268:5000 \
  -v mockhub-data:/app/data \
  aligoren/mockhub:latest

# Access at http://localhost:5268
```

### Docker Compose Deployment

```bash
# Clone repository
git clone https://github.com/aligoren/mockhub.git
cd mockhub

# Start with Docker Compose
docker-compose up -d

# View logs
docker-compose logs -f mockhub

# Stop services
docker-compose down
```

## Docker Compose Configuration

### Basic Setup

```yaml
version: '3.8'

services:
  mockhub:
    image: aligoren/mockhub:latest
    ports:
      - "5268:5000"
    volumes:
      - mockhub-data:/app/data
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
    restart: unless-stopped
    healthcheck:
      test: ["CMD", "curl", "-f", "http://localhost:5000/health"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s

volumes:
  mockhub-data:
```

### Development Setup

```yaml
version: '3.8'

services:
  mockhub:
    build:
      context: .
      dockerfile: docker/Dockerfile
    ports:
      - "5268:5000"
      - "5100-5200:5100-5200"  # Mock server ports
    volumes:
      - .:/app/data
      - /app/data  # Named volume for data persistence
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - ASPNETCORE_URLS=http://+:5000
    restart: unless-stopped
    profiles:
      - dev
```

### Production Setup

```yaml
version: '3.8'

services:
  mockhub:
    image: aligoren/mockhub:latest
    ports:
      - "80:5000"
    volumes:
      - mockhub-data:/app/data
      - ./logs:/app/logs
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:5000
    restart: always
    healthcheck:
      test: ["CMD-SHELL", "curl -f http://localhost:5000/health || exit 1"]
      interval: 30s
      timeout: 10s
      retries: 3
      start_period: 40s
    logging:
      driver: "json-file"
      options:
        max-size: "10m"
        max-file: "3"

volumes:
  mockhub-data:
```

## Environment Configuration

### Environment Variables

| Variable | Description | Default | Example |
|----------|-------------|---------|---------|
| `ASPNETCORE_ENVIRONMENT` | Runtime environment | Production | Development |
| `ASPNETCORE_URLS` | Server URLs | http://+:5000 | http://+:5000;https://+:5001 |
| `ConnectionStrings__DefaultConnection` | Database connection | Data Source=/app/data/mockhub.db | Custom connection string |
| `Serilog__MinimumLevel` | Logging level | Information | Debug |

### Database Configuration

SQLite database is automatically created in the container:

```bash
# Database location in container
/app/data/mockhub.db

# Access database from host (development)
docker exec -it mockhub sqlite3 /app/data/mockhub.db
```

### Volume Management

```bash
# List volumes
docker volume ls

# Inspect volume
docker volume inspect mockhub-data

# Backup volume
docker run --rm -v mockhub-data:/source -v $(pwd):/backup alpine tar czf /backup/mockhub-backup.tar.gz -C /source .

# Restore volume
docker run --rm -v mockhub-data:/dest -v $(pwd):/backup alpine tar xzf /backup/mockhub-backup.tar.gz -C /dest
```

## Networking

### Port Configuration

MockHub uses specific ports:

- **5000**: Main web application (internal container port)
- **5268**: External access port (configurable)
- **5100-5200**: Reserved for mock servers (future use)

### Network Modes

```yaml
# Bridge network (default)
services:
  mockhub:
    networks:
      - mockhub-network

# Host network
services:
  mockhub:
    network_mode: host

networks:
  mockhub-network:
    driver: bridge
```

## Security

### Container Security

- **Non-root user**: Application runs as non-privileged user
- **Minimal base image**: Uses official Microsoft ASP.NET Core runtime
- **No unnecessary packages**: Only required dependencies installed

### Data Security

- **Volume encryption**: Encrypt sensitive volumes if required
- **Backup security**: Secure database backups
- **Access control**: Restrict container access

### Network Security

```yaml
# Restrict external access
services:
  mockhub:
    ports:
      - "127.0.0.1:5268:5000"  # Localhost only
    networks:
      - internal-network

# Use reverse proxy
services:
  nginx:
    image: nginx:alpine
    ports:
      - "80:80"
      - "443:443"
    volumes:
      - ./nginx.conf:/etc/nginx/nginx.conf
    depends_on:
      - mockhub
    networks:
      - public-network
      - internal-network

  mockhub:
    networks:
      - internal-network
```

## Monitoring and Logging

### Health Checks

MockHub includes built-in health checks:

```bash
# Check container health
docker ps
docker inspect mockhub | grep -A 10 "Health"

# Manual health check
curl http://localhost:5268/health
```

### Application Logs

```bash
# View container logs
docker logs mockhub

# Follow logs
docker logs -f mockhub

# View logs with timestamps
docker logs --timestamps mockhub

# Export logs
docker logs mockhub > mockhub.log
```

### Log Configuration

Serilog configuration in `appsettings.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}"
        }
      }
    ]
  }
}
```

## Scaling and Performance

### Resource Limits

```yaml
services:
  mockhub:
    deploy:
      resources:
        limits:
          cpus: '1.0'
          memory: 1G
        reservations:
          cpus: '0.5'
          memory: 512M
```

### Performance Optimization

- **Database optimization**: SQLite WAL mode for better concurrency
- **Memory management**: .NET GC tuning for containers
- **Connection pooling**: Database connection optimization

### High Availability

*Future considerations:*
- Load balancing multiple instances
- Database replication
- Session sharing across instances
- Health check orchestration

## Backup and Recovery

### Database Backup

```bash
# Backup database
docker exec mockhub sqlite3 /app/data/mockhub.db ".backup /tmp/backup.db"
docker cp mockhub:/tmp/backup.db ./mockhub-backup.db

# Automated backup script
#!/bin/bash
BACKUP_DIR="./backups"
TIMESTAMP=$(date +"%Y%m%d_%H%M%S")
BACKUP_FILE="$BACKUP_DIR/mockhub_$TIMESTAMP.db"

docker exec mockhub sqlite3 /app/data/mockhub.db ".backup /tmp/backup.db"
docker cp mockhub:/tmp/backup.db "$BACKUP_FILE"
```

### Configuration Backup

```bash
# Backup volumes
docker run --rm -v mockhub-data:/source -v $(pwd)/backups:/backup alpine tar czf /backup/mockhub-data.tar.gz -C /source .

# Restore volumes
docker run --rm -v mockhub-data:/dest -v $(pwd)/backups:/backup alpine tar xzf /backup/mockhub-data.tar.gz -C /dest
```

## Troubleshooting

### Common Issues

**Container won't start:**
```bash
# Check logs
docker logs mockhub

# Check container status
docker ps -a

# Check port conflicts
netstat -tulpn | grep 5268
```

**Database issues:**
```bash
# Check database file
docker exec -it mockhub ls -la /app/data/

# Access database
docker exec -it mockhub sqlite3 /app/data/mockhub.db ".tables"
```

**Performance problems:**
```bash
# Check resource usage
docker stats mockhub

# Check .NET diagnostics
docker exec mockhub dotnet-counters monitor --process-id 1
```

### Debug Mode

```yaml
# Debug configuration
services:
  mockhub:
    environment:
      - ASPNETCORE_ENVIRONMENT=Development
      - Logging__LogLevel__Default=Debug
    ports:
      - "5268:5000"
    volumes:
      - .:/app/data  # Mount for easy access
```

## CI/CD Integration

### GitHub Actions Example

```yaml
name: Build and Push Docker Image

on:
  push:
    branches: [ main ]

jobs:
  build:
    runs-on: ubuntu-latest
    
    steps:
    - uses: actions/checkout@v3
    
    - name: Login to Docker Hub
      uses: docker/login-action@v2
      with:
        username: ${{ secrets.DOCKER_USERNAME }}
        password: ${{ secrets.DOCKER_PASSWORD }}
    
    - name: Build and push
      uses: docker/build-push-action@v4
      with:
        context: .
        push: true
        tags: aligoren/mockhub:latest,aligoren/mockhub:${{ github.sha }}
        file: docker/Dockerfile
```

### Automated Deployment

```yaml
# docker-compose.prod.yml
version: '3.8'

services:
  mockhub:
    image: aligoren/mockhub:${TAG}
    # ... production configuration
```

## Best Practices

### Development
- Use development Docker Compose for local development
- Mount source code for hot reload capabilities
- Use debug logging levels
- Enable development-specific features

### Production
- Use production Docker Compose configuration
- Implement proper logging and monitoring
- Set up automated backups
- Use reverse proxy for SSL termination
- Implement health checks and restart policies

### Security
- Regularly update base images
- Scan images for vulnerabilities
- Use secrets management for sensitive data
- Implement network segmentation
- Restrict container capabilities

### Monitoring
- Monitor container resource usage
- Set up log aggregation
- Implement health check endpoints
- Use container orchestration monitoring
- Track application performance metrics

### Backup Strategy
- Regular automated backups
- Test backup restoration
- Store backups in multiple locations
- Encrypt sensitive backup data
- Document backup and recovery procedures

## Migration Guide

### From Local Installation

1. **Backup existing data**
   ```bash
   cp mockhub.db mockhub-backup.db
   ```

2. **Create Docker volume**
   ```bash
   docker volume create mockhub-migration
   ```

3. **Copy data to volume**
   ```bash
   docker run --rm -v mockhub-migration:/dest -v $(pwd):/src alpine cp /src/mockhub-backup.db /dest/
   ```

4. **Update Docker Compose**
   ```yaml
   volumes:
     - mockhub-migration:/app/data
   ```

### From Other Platforms

- **Windows**: Use Docker Desktop for Windows
- **macOS**: Use Docker Desktop for Mac
- **Linux**: Use native Docker installation
- **Cloud**: Use cloud container services (ECS, AKS, etc.)

## Future Enhancements

*Planned Docker features:*
- Multi-container setups with database separation
- Kubernetes deployment manifests
- Docker Swarm support
- Advanced monitoring integration
- Automated scaling configurations
