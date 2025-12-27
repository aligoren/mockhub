# Installation Guide

This guide provides comprehensive instructions for installing and setting up MockHub on various platforms. Whether you're a developer looking to get started quickly or a system administrator deploying to production, this guide covers all installation methods and configurations.

## System Requirements

### Minimum Requirements

| Component | Requirement |
|-----------|-------------|
| Operating System | Windows 10/11, macOS 10.15+, Ubuntu 18.04+, CentOS 7+ |
| Processor | 1 GHz or faster |
| Memory | 2 GB RAM |
| Storage | 500 MB available space |
| Network | Internet connection for initial setup |

### Recommended Requirements

| Component | Recommendation |
|-----------|----------------|
| Operating System | Windows 11, macOS 12+, Ubuntu 20.04+ |
| Processor | 2 GHz dual-core or better |
| Memory | 4 GB RAM or more |
| Storage | 1 GB SSD storage |
| Network | Stable broadband connection |

### Supported Platforms

- **Windows**: 10 (version 1903+), 11
- **macOS**: 10.15 (Catalina) or later
- **Linux**: Ubuntu 18.04+, CentOS 7+, Fedora 30+, Debian 10+
- **Docker**: Any platform supporting Docker Engine 20.10+

## Installation Methods

### Method 1: Source Code Build (Development)

#### Prerequisites

1. **Install .NET 8 SDK**
   ```bash
   # Windows (winget)
   winget install Microsoft.DotNet.SDK.8

   # macOS (Homebrew)
   brew install --cask dotnet-sdk

   # Ubuntu/Debian
   wget https://packages.microsoft.com/config/ubuntu/20.04/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
   sudo dpkg -i packages-microsoft-prod.deb
   sudo apt-get update
   sudo apt-get install -y dotnet-sdk-8.0

   # CentOS/RHEL/Fedora
   sudo dnf install dotnet-sdk-8.0
   ```

2. **Verify Installation**
   ```bash
   dotnet --version
   # Should show 8.0.x
   ```

#### Build and Run

1. **Clone Repository**
   ```bash
   git clone https://github.com/aligoren/mockhub.git
   cd mockhub
   ```

2. **Restore Dependencies**
   ```bash
   dotnet restore
   ```

3. **Build Application**
   ```bash
   dotnet build --configuration Release
   ```

4. **Run Database Migrations** (Optional - runs automatically)
   ```bash
   cd src/MockHub.Web
   dotnet ef database update
   ```

5. **Start Application**
   ```bash
   dotnet run --configuration Release
   ```

6. **Access Application**
   - Open browser to `http://localhost:5268`
   - Complete initial setup

### Method 2: Docker Deployment (Recommended)

#### Prerequisites

1. **Install Docker**
   ```bash
   # Windows/macOS
   # Download and install Docker Desktop from https://www.docker.com/products/docker-desktop

   # Ubuntu/Debian
   sudo apt-get update
   sudo apt-get install ca-certificates curl gnupg lsb-release
   curl -fsSL https://download.docker.com/linux/ubuntu/gpg | sudo gpg --dearmor -o /usr/share/keyrings/docker-archive-keyring.gpg
   echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/docker-archive-keyring.gpg] https://download.docker.com/linux/ubuntu $(lsb_release -cs) stable" | sudo tee /etc/apt/sources.list.d/docker.list > /dev/null
   sudo apt-get update
   sudo apt-get install docker-ce docker-ce-cli containerd.io docker-compose-plugin

   # CentOS/RHEL
   sudo yum install -y yum-utils
   sudo yum-config-manager --add-repo https://download.docker.com/linux/centos/docker-ce.repo
   sudo yum install docker-ce docker-ce-cli containerd.io docker-compose-plugin
   ```

2. **Install Docker Compose** (if not included)
   ```bash
   # Ubuntu/Debian
   sudo apt-get install docker-compose-plugin

   # Or install standalone
   sudo curl -L "https://github.com/docker/compose/releases/download/v2.17.3/docker-compose-$(uname -s)-$(uname -m)" -o /usr/local/bin/docker-compose
   sudo chmod +x /usr/local/bin/docker-compose
   ```

3. **Start Docker Service**
   ```bash
   sudo systemctl start docker
   sudo systemctl enable docker
   ```

#### Docker Deployment

1. **Clone Repository**
   ```bash
   git clone https://github.com/aligoren/mockhub.git
   cd mockhub
   ```

2. **Start with Docker Compose**
   ```bash
   docker-compose up -d
   ```

3. **Monitor Startup**
   ```bash
   docker-compose logs -f mockhub
   ```

4. **Verify Installation**
   ```bash
   curl http://localhost:5268/health
   # Should return {"status":"healthy","timestamp":"..."}
   ```

5. **Access Application**
   - Open browser to `http://localhost:5268`
   - Complete initial setup

#### Docker Configuration

**Basic docker-compose.yml:**
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

volumes:
  mockhub-data:
```

### Method 3: Pre-built Binaries (Windows/macOS)

#### Windows Installation

1. **Download Release**
   - Go to GitHub releases
   - Download `MockHub-win-x64.zip`

2. **Extract Files**
   ```cmd
   unzip MockHub-win-x64.zip
   cd MockHub-win-x64
   ```

3. **Run Application**
   ```cmd
   MockHub.Web.exe
   ```

4. **Access Application**
   - Open browser to `http://localhost:5268`

#### macOS Installation

1. **Download Release**
   - Download `MockHub-osx-x64.tar.gz`

2. **Extract Files**
   ```bash
   tar -xzf MockHub-osx-x64.tar.gz
   cd MockHub-osx-x64
   ```

3. **Make Executable**
   ```bash
   chmod +x MockHub.Web
   ```

4. **Run Application**
   ```bash
   ./MockHub.Web
   ```

### Method 4: Linux Package Installation

#### Ubuntu/Debian (.deb)

1. **Download Package**
   ```bash
   wget https://github.com/aligoren/mockhub/releases/download/v1.0.0/mockhub_1.0.0_amd64.deb
   ```

2. **Install Package**
   ```bash
   sudo dpkg -i mockhub_1.0.0_amd64.deb
   ```

3. **Start Service**
   ```bash
   sudo systemctl start mockhub
   sudo systemctl enable mockhub
   ```

#### CentOS/RHEL (.rpm)

1. **Download Package**
   ```bash
   wget https://github.com/aligoren/mockhub/releases/download/v1.0.0/mockhub-1.0.0-1.x86_64.rpm
   ```

2. **Install Package**
   ```bash
   sudo rpm -i mockhub-1.0.0-1.x86_64.rpm
   ```

3. **Start Service**
   ```bash
   sudo systemctl start mockhub
   sudo systemctl enable mockhub
   ```

## Initial Setup

### First Run Setup

1. **Access Application**
   - Open browser to `http://localhost:5268`
   - You'll be redirected to setup page

2. **Create Administrator Account**
   - **Email**: Administrator email address
   - **Password**: Strong password (8+ characters)
   - **First Name**: Administrator first name
   - **Last Name**: Administrator last name

3. **Complete Setup**
   - Click "Complete Setup"
   - You'll be automatically logged in

### Post-Setup Configuration

1. **Create Your First Project**
   - Click "New Project"
   - Enter project name and description
   - Configure CORS and logging settings

2. **Add Endpoints**
   - Open your project
   - Click "Add Endpoint"
   - Configure HTTP method, route, and response

## Configuration

### Application Settings

MockHub uses `appsettings.json` for configuration:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=mockhub.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "MockHub": {
    "PortRange": {
      "Min": 5100,
      "Max": 6000
    }
  },
  "Serilog": {
    "MinimumLevel": "Information"
  }
}
```

### Environment Variables

Override settings with environment variables:

```bash
# Database
ConnectionStrings__DefaultConnection="Data Source=/custom/path/mockhub.db"

# Logging
Logging__LogLevel__Default="Debug"

# Application
ASPNETCORE_ENVIRONMENT="Production"
ASPNETCORE_URLS="http://+:5000"
```

### Database Configuration

MockHub uses SQLite by default. The database file is created automatically at:

- **Windows**: `%APPDATA%\MockHub\mockhub.db`
- **macOS**: `~/Library/Application Support/MockHub/mockhub.db`
- **Linux**: `~/.config/MockHub/mockhub.db`
- **Docker**: `/app/data/mockhub.db`

## Firewall and Network Configuration

### Port Requirements

MockHub requires these ports:

- **5268**: Main web interface (configurable)
- **5000**: Internal application port (Docker)
- **5100-6000**: Reserved for future mock server ports

### Firewall Configuration

#### Windows Firewall

```cmd
# Allow port 5268
netsh advfirewall firewall add rule name="MockHub" dir=in action=allow protocol=TCP localport=5268

# Or use Windows Defender Firewall UI
```

#### Linux (ufw)

```bash
sudo ufw allow 5268/tcp
sudo ufw reload
```

#### Linux (firewalld)

```bash
sudo firewall-cmd --permanent --add-port=5268/tcp
sudo firewall-cmd --reload
```

### Reverse Proxy Setup

#### Nginx Configuration

```nginx
server {
    listen 80;
    server_name mockhub.example.com;

    location / {
        proxy_pass http://localhost:5268;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

#### Apache Configuration

```apache
<VirtualHost *:80>
    ServerName mockhub.example.com

    ProxyPreserveHost On
    ProxyPass / http://localhost:5268/
    ProxyPassReverse / http://localhost:5268/

    ErrorLog ${APACHE_LOG_DIR}/mockhub_error.log
    CustomLog ${APACHE_LOG_DIR}/mockhub_access.log combined
</VirtualHost>
```

## SSL/TLS Configuration

### HTTPS with Reverse Proxy

#### Nginx with Let's Encrypt

```nginx
server {
    listen 80;
    server_name mockhub.example.com;
    return 301 https://$server_name$request_uri;
}

server {
    listen 443 ssl http2;
    server_name mockhub.example.com;

    ssl_certificate /etc/letsencrypt/live/mockhub.example.com/fullchain.pem;
    ssl_certificate_key /etc/letsencrypt/live/mockhub.example.com/privkey.pem;

    location / {
        proxy_pass http://localhost:5268;
        proxy_set_header Host $host;
        proxy_set_header X-Real-IP $remote_addr;
        proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header X-Forwarded-Proto $scheme;
    }
}
```

### Self-Signed Certificate (Development)

```bash
# Generate self-signed certificate
openssl req -x509 -newkey rsa:4096 -keyout key.pem -out cert.pem -days 365 -nodes -subj "/CN=localhost"

# Configure Kestrel (appsettings.Development.json)
{
  "Kestrel": {
    "Certificates": {
      "Default": {
        "Path": "cert.pem",
        "KeyPath": "key.pem"
      }
    },
    "Endpoints": {
      "Https": {
        "Url": "https://localhost:7268"
      }
    }
  }
}
```

## Backup and Recovery

### Database Backup

```bash
# Stop application first
docker-compose down

# Backup SQLite database
cp mockhub.db mockhub-backup-$(date +%Y%m%d).db

# Or with Docker
docker run --rm -v mockhub-data:/source -v $(pwd):/backup alpine cp /source/mockhub.db /backup/mockhub-backup.db
```

### Configuration Backup

```bash
# Backup configuration files
cp appsettings.json appsettings.json.backup
cp appsettings.Development.json appsettings.Development.json.backup
```

### Full System Backup

```bash
# Create backup directory
BACKUP_DIR="mockhub-backup-$(date +%Y%m%d)"
mkdir "$BACKUP_DIR"

# Copy all data
cp -r . "$BACKUP_DIR/"
cp mockhub.db "$BACKUP_DIR/"

# Create archive
tar -czf "${BACKUP_DIR}.tar.gz" "$BACKUP_DIR"
```

## Troubleshooting Installation

### Common Issues

**Application won't start:**

```bash
# Check logs
dotnet run --verbose

# Check port availability
netstat -tulpn | grep 5268

# Check .NET installation
dotnet --info
```

**Database errors:**

```bash
# Check database file permissions
ls -la mockhub.db

# Check SQLite installation
sqlite3 --version

# Recreate database
rm mockhub.db
dotnet ef database update
```

**Permission errors:**

```bash
# Fix file permissions (Linux/macOS)
chmod 644 mockhub.db
chmod 755 .

# Run as correct user
sudo -u www-data dotnet MockHub.Web.dll
```

**Network issues:**

```bash
# Test local connectivity
curl http://localhost:5268/health

# Check firewall
sudo ufw status
sudo firewall-cmd --list-all
```

### Log Locations

- **Windows**: `%APPDATA%\MockHub\logs\`
- **macOS**: `~/Library/Logs/MockHub/`
- **Linux**: `/var/log/mockhub/`
- **Docker**: `docker logs mockhub`

### Health Check

MockHub provides a health check endpoint:

```bash
curl http://localhost:5268/health
# Expected: {"status":"healthy","timestamp":"2024-12-24T18:30:00Z"}
```

## Upgrade Guide

### Minor Version Updates

1. **Backup Data**
   ```bash
   cp mockhub.db mockhub-backup.db
   ```

2. **Stop Application**
   ```bash
   docker-compose down
   ```

3. **Update Image**
   ```bash
   docker-compose pull
   ```

4. **Start Application**
   ```bash
   docker-compose up -d
   ```

5. **Verify Upgrade**
   ```bash
   curl http://localhost:5268/health
   ```

### Major Version Updates

1. **Review Release Notes**
   - Check breaking changes
   - Review migration guide

2. **Database Migration**
   ```bash
   dotnet ef database update
   ```

3. **Configuration Updates**
   - Update `appsettings.json`
   - Review environment variables

4. **Test Thoroughly**
   - Test all endpoints
   - Verify user permissions
   - Check data integrity

## Production Deployment Checklist

- [ ] Install on dedicated server
- [ ] Configure reverse proxy (Nginx/Apache)
- [ ] Set up SSL/TLS certificates
- [ ] Configure firewall rules
- [ ] Set up log rotation
- [ ] Configure automated backups
- [ ] Set up monitoring and alerts
- [ ] Test high availability (if applicable)
- [ ] Document deployment procedure
- [ ] Set up CI/CD pipeline (optional)

## Support

### Getting Help

- **Documentation**: Check docs/ directory
- **GitHub Issues**: Report bugs and request features
- **Community**: Join discussions
- **Logs**: Check application logs for errors

### System Information

When reporting issues, include:

```bash
# System info
uname -a
dotnet --info
docker --version

# Application logs
tail -n 50 logs/mockhub.log

# Database status
sqlite3 mockhub.db ".schema"
```

This comprehensive installation guide should help you get MockHub running on your preferred platform. For advanced configurations or specific deployment scenarios, refer to the additional documentation files in the docs/ directory.
