# 🏨 HotelDruid Docker LAMP Stack

Complete Docker containerization setup for the HotelDruid hotel management system with a modern LAMP stack.

## 🎯 Quick Start Guide phpDesktop

- Get clone of this repository
- Run ```./build_deployment_package.ps1```  
- On target machine unzip the package from ```./out/``` folder
- On target machine in the unzipped folder run ```./install_release.ps1 -Language en```
- Run the PHPDesktop to run HotelDruid

- One can use the alternative winget: 
  ```powershell
  
  pwsh -ExecutionPolicy Bypass -File [install_release.ps1](http://_vscodecontentref_/2) -Language en -Quiet
  ```


## 🎯 Quick Start Guide Docker

### 1. Prerequisites

- Docker Desktop installed and running
- At least 2GB free disk space
- Ports 8080, 8081, and 3306 available

### 2. One-Command Setup

```powershell
# Windows PowerShell - Start containers
.\start-containers.ps1

# Or manually:
docker-compose up -d
```

### 3. Access Your Application

- **HotelDruid**: <http://localhost:8080>
- **phpMyAdmin**: <http://localhost:8081>
- **Database**: localhost:3306

## 📋 What's Included

### 📚 Documentation Index

For consolidated documentation and navigation, see [INDEX.md](INDEX.md).

- Purpose: A single entry point to setup, configuration, workflows, and developer notes
- Quick links for developers:
  - [hoteldruid/SETUP.md](hoteldruid/SETUP.md) — setup
  - [hoteldruid/CONFIGURATION.md](hoteldruid/CONFIGURATION.md) — configuration
  - [hoteldruid/DEVELOPER.md](hoteldruid/DEVELOPER.md) — developer notes

### 🐳 Docker Services

| Service | Container | Purpose | Port |
| - | - | - | - |
| `hoteldruid-web` | Apache + PHP 8.1 | Main application | 8080 |
| `hoteldruid-db` | MySQL 8.0 | Database server | 3306 |
| `phpmyadmin` | phpMyAdmin | Database management | 8081 |

### 🔧 Technical Stack

- **OS**: Debian-based Linux
- **Web Server**: Apache 2.4 with mod_rewrite
- **PHP**: Version 8.1 with extensions:
  - mysqli, pdo_mysql (database)
  - gd (image processing)
  - mbstring (string handling)
  - zip (file compression)
- **Database**: MySQL 8.0 with UTF8MB4 support
- **Management**: phpMyAdmin for database operations

## 🚀 Deployment Options

### Option 1: Windows PowerShell Script

```powershell
# Run validation and setup
./validate-docker-setup.ps1
./start-containers.ps1
```

### Option 2: Manual Docker Commands

```bash
# Copy environment configuration
cp .env.example .env

# Build and start containers
docker-compose build
docker-compose up -d

# View logs
docker-compose logs -f
```

### Option 3: Production Deployment

```bash
# For production, modify .env file first
# Then start with production settings
docker-compose -f docker-compose.yml up -d
```

### Option 4: use the phpdesktop

```powershell
setup-phpdesktop.ps1
start-hoteldruid-desktop.ps1
package-release.ps1
```

## 📁 Project Structure

```text
hotelDroid/
├── 📁 hoteldruid/                    # HotelDruid application files
│   ├── 📁 dati/                      # Database connection config
│   │   └── 📄 dati_connessione.php   # Docker MySQL config
│   ├── 📁 includes/                  # Modern enhancements
│   │   ├── 📄 security.php           # CSRF protection & rate limiting
│   │   ├── 📄 modern.css             # Responsive design framework
│   │   ├── 📄 template.php           # Template engine
│   │   └── 📄 menu_generator.php     # Modern menu system
│   └── 📄 inizio.php                 # Main entry point (modernized)
├── 📁 mysql/
│   └── 📁 init/                      # Database initialization
│       └── 📄 01-init-hoteldruid.sql # Auto-setup script
├── 📁 logs/apache/                   # Apache log files
├── 📄 docker-compose.yml             # Container orchestration
├── 📄 Dockerfile                     # Web server image
├── 📄 .env                           # Environment configuration
├── 📄 start-containers.ps1           # Container startup script
├── 📄 validate-docker-setup.ps1      # Validation script
└── 📄 DOCKER-README.md               # This file
```

## ⚙️ Configuration

### Database Settings

The application automatically connects to the Docker MySQL container using:

```php
// Auto-configured in hoteldruid/dati/dati_connessione.php
$PHPR_DB_HOST = "hoteldruid-db";     // Container name
$PHPR_DB_NAME = "hoteldruid";        // Database name
$PHPR_DB_USER = "hoteldruid_user";   // Username
$PHPR_DB_PASS = "hoteldruid_pass_2024"; // Password (change in production!)
```

### Environment Variables (.env)

```env
# Database Configuration
DB_HOST=hoteldruid-db
DB_NAME=hoteldruid
DB_USER=hoteldruid_user
DB_PASSWORD=hoteldruid_pass_2024

# Application Settings
APP_URL=http://localhost:8080
PHP_MEMORY_LIMIT=256M
PHP_UPLOAD_MAX_FILESIZE=50M
```

### Port Mapping

```yaml
# docker-compose.yml ports
hoteldruid-web:   8080:80   # Web application
hoteldruid-db:    3306:3306 # MySQL database
phpmyadmin:       8081:80   # Database admin
```

## 🛠️ Management Commands

### Container Operations

```bash
# Start all services
docker-compose up -d

# Stop all services
docker-compose down

# Restart specific service
docker-compose restart hoteldruid-web

# View logs
docker-compose logs -f hoteldruid-web
docker-compose logs -f hoteldruid-db

# Check container status
docker-compose ps
```

### Database Operations

```bash
# Access MySQL command line
docker-compose exec hoteldruid-db mysql -u hoteldruid_user -p hoteldruid

# Create backup
docker-compose exec hoteldruid-db mysqldump -u root -p hoteldruid > backup.sql

# Restore backup
docker-compose exec -T hoteldruid-db mysql -u root -p hoteldruid < backup.sql

# Reset database (WARNING: Deletes all data!)
docker-compose down -v
docker-compose up -d
```

### Application Operations

```bash
# Access web container shell
docker-compose exec hoteldruid-web bash

# Check PHP configuration
docker-compose exec hoteldruid-web php -m

# View Apache configuration
docker-compose exec hoteldruid-web apache2ctl -S

# Restart Apache
docker-compose exec hoteldruid-web apache2ctl graceful
```

## 🔐 Security Features

### Built-in Security Enhancements

- ✅ **CSRF Protection**: Prevents cross-site request forgery
- ✅ **Rate Limiting**: Prevents brute force attacks
- ✅ **Input Validation**: Enhanced form validation
- ✅ **Secure Sessions**: Improved session management
- ✅ **SQL Injection Protection**: Prepared statements

### Production Security Checklist

- [ ] Change default passwords in `.env`
- [ ] Enable HTTPS/SSL certificates
- [ ] Configure firewall rules
- [ ] Set up regular backups
- [ ] Update containers regularly
- [ ] Monitor logs for security events

## 🎨 Modern UI Features

### Responsive Design

- ✅ **Mobile-First**: Responsive hamburger menu
- ✅ **Modern CSS**: CSS Grid and Flexbox layouts
- ✅ **Accessibility**: ARIA labels and keyboard navigation
- ✅ **Design System**: Consistent colors and typography

### Template Separation

- ✅ **Clean Architecture**: PHP logic separated from HTML
- ✅ **Reusable Components**: Template engine for consistency
- ✅ **Maintainable Code**: Modular menu generation

## 🐛 Troubleshooting

### Common Issues

#### 🔴 Port Already in Use

```bash
# Find what's using the port
netstat -tulpn | grep :8080

# Solution: Change ports in docker-compose.yml
ports:
  - "8090:80"  # Change 8080 to 8090
```

#### 🔴 Database Connection Failed

```bash
# Check database container
docker-compose logs hoteldruid-db

# Verify database is ready
docker-compose exec hoteldruid-db mysql -u root -p -e "SHOW DATABASES;"

# Reset database if needed
docker-compose down -v && docker-compose up -d
```

#### 🔴 Permission Errors

```bash
# Fix file permissions
docker-compose exec hoteldruid-web chown -R www-data:www-data /var/www/html
docker-compose exec hoteldruid-web chmod -R 755 /var/www/html
docker-compose exec hoteldruid-web chmod -R 777 /var/www/html/dati
```

#### 🔴 Application Not Loading

```bash
# Check container status
docker-compose ps

# View web server logs
docker-compose logs hoteldruid-web

# Restart web container
docker-compose restart hoteldruid-web
```

### Health Checks

```bash
# Test database connection
docker-compose exec hoteldruid-db mysql -u hoteldruid_user -p hoteldruid -e "SELECT 'Connection OK' as status;"

# Test web server
curl -I http://localhost:8080

# Run validation script
.\validate-docker-setup.ps1
```

## 📊 Monitoring and Logs

### Log Locations

```bash
# Apache logs (volume mounted)
./logs/apache/access.log
./logs/apache/error.log

# Container logs
docker-compose logs hoteldruid-web    # Web server
docker-compose logs hoteldruid-db     # Database
docker-compose logs phpmyadmin        # phpMyAdmin
```

### Performance Monitoring

```bash
# Container resource usage
docker stats

# Database performance
# Access phpMyAdmin -> Status -> Performance

# Web server status
curl http://localhost:8080/server-status
```

## 🔄 Updates and Maintenance

### Updating HotelDruid

1. **Backup Database**: `docker-compose exec hoteldruid-db mysqldump...`
2. **Stop Containers**: `docker-compose down`
3. **Update Files**: Replace files in `hoteldruid/` directory
4. **Rebuild**: `docker-compose build --no-cache`
5. **Start**: `docker-compose up -d`

### Updating Docker Images

```bash
# Pull latest base images
docker-compose pull

# Rebuild containers
docker-compose build --no-cache

# Restart with updates
docker-compose up -d
```

### Regular Maintenance

```bash
# Clean unused Docker resources
docker system prune -a

# Update application dependencies
# (Done automatically during container rebuild)

# Backup database weekly
# (Set up automated backup scripts)
```

## 📈 Performance Optimization

### Production Tuning

```env
# .env file optimizations
PHP_MEMORY_LIMIT=512M
PHP_MAX_EXECUTION_TIME=300
MYSQL_INNODB_BUFFER_POOL_SIZE=1G
```

### Resource Limits

```yaml
# docker-compose.yml resource limits
services:
  hoteldruid-web:
    deploy:
      resources:
        limits:
          memory: 512M
        reservations:
          memory: 256M
```

## 🚢 Deployment Environments

### Development

```bash
# Use default settings
docker-compose up -d
```

### Staging

```bash
# Use staging environment
cp .env.staging .env
docker-compose up -d
```

### Production

```bash
# Use production environment
cp .env.production .env
docker-compose -f docker-compose.prod.yml up -d
```

## 📞 Support and Resources

### Getting Help

- **HotelDruid Documentation**: [Official Docs](http://www.hoteldruid.com/)
- **Docker Issues**: Check container logs with `docker-compose logs`
- **Database Issues**: Use phpMyAdmin at <http://localhost:8081>

### Useful Links

- [Docker Compose Documentation](https://docs.docker.com/compose/)
- [PHP 8.1 Documentation](https://www.php.net/manual/en/)
- [MySQL 8.0 Documentation](https://dev.mysql.com/doc/refman/8.0/en/)
- [Apache HTTP Server Documentation](https://httpd.apache.org/docs/)

## ✨ Features Summary

### ✅ What This Setup Provides

- **Complete LAMP Stack**: Linux + Apache + MySQL + PHP
- **Containerized**: Isolated, reproducible environment
- **Modern Security**: CSRF protection, rate limiting, input validation
- **Responsive UI**: Mobile-friendly hamburger menu
- **Easy Management**: One-command deployment and management
- **Development Ready**: Hot-reload, logging, debugging tools
- **Production Ready**: Security hardening, performance optimization
- **Cross-Platform**: Works on Windows, macOS, and Linux

### 🎯 Perfect For

- Hotel management and booking systems
- Development and testing environments
- Production deployments with proper security
- Learning PHP and database development
- Demonstrating modern web application architecture

---

🎉 **Ready to manage your hotel with modern technology!** 🏨
