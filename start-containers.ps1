# HotelDruid Docker Startup Script for Windows
# PowerShell version of the startup script

Write-Host "🏨 Starting HotelDruid LAMP Stack..." -ForegroundColor Green

# Check if Docker is running
try {
    docker info | Out-Null
    Write-Host "✅ Docker is running" -ForegroundColor Green
} catch {
    Write-Host "❌ Docker is not running. Please start Docker Desktop first." -ForegroundColor Red
    exit 1
}

# Check if .env file exists, if not copy from example
if (-not (Test-Path ".env")) {
    Write-Host "📋 Creating .env file from example..." -ForegroundColor Yellow
    Copy-Item ".env.example" ".env"
    Write-Host "✅ .env file created. Please review and modify as needed." -ForegroundColor Green
}

# Create logs directory if it doesn't exist
if (-not (Test-Path "logs\apache")) {
    New-Item -ItemType Directory -Path "logs\apache" -Force | Out-Null
    Write-Host "📁 Created logs directory" -ForegroundColor Green
}

# Build and start the containers
Write-Host "🔨 Building HotelDruid container..." -ForegroundColor Cyan
docker-compose build

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Build failed!" -ForegroundColor Red
    exit 1
}

Write-Host "🚀 Starting services..." -ForegroundColor Cyan
docker-compose up -d

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to start services!" -ForegroundColor Red
    exit 1
}

# Wait a moment for services to start
Write-Host "⏳ Waiting for services to initialize..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

# Check if services are running
Write-Host "🔍 Checking service status..." -ForegroundColor Cyan
docker-compose ps

# Show connection information
Write-Host ""
Write-Host "🎉 HotelDruid LAMP Stack is running!" -ForegroundColor Green
Write-Host ""
Write-Host "📱 Application URLs:" -ForegroundColor Cyan
Write-Host "   HotelDruid:  http://localhost:8080" -ForegroundColor White
Write-Host "   phpMyAdmin:  http://localhost:8081" -ForegroundColor White
Write-Host ""
Write-Host "🗄️  Database Connection:" -ForegroundColor Cyan
Write-Host "   Host: localhost:3306" -ForegroundColor White
Write-Host "   Database: hoteldruid" -ForegroundColor White
Write-Host "   Username: hoteldruid_user" -ForegroundColor White
Write-Host "   Password: hoteldruid_pass_2024" -ForegroundColor White
Write-Host ""
Write-Host "📊 Container Status:" -ForegroundColor Cyan
docker-compose ps --format "table {{.Name}}\t{{.Status}}\t{{.Ports}}"
Write-Host ""
Write-Host "📝 To view logs: docker-compose logs -f" -ForegroundColor Yellow
Write-Host "🛑 To stop: docker-compose down" -ForegroundColor Yellow
Write-Host "🔄 To restart: docker-compose restart" -ForegroundColor Yellow
Write-Host ""
Write-Host "✨ Setup complete! You can now access HotelDruid at http://localhost:8080" -ForegroundColor Green