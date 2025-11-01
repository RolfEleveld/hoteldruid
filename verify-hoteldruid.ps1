#!/usr/bin/env pwsh

Write-Host "=== HotelDruid Docker LAMP Stack Verification ===" -ForegroundColor Green
Write-Host ""

# Check Docker containers
Write-Host "1. Checking Docker containers..." -ForegroundColor Yellow
$containers = docker ps --format "table {{.Names}}\t{{.Status}}\t{{.Ports}}"
Write-Host $containers

Write-Host ""

# Check web application
Write-Host "2. Testing web application..." -ForegroundColor Yellow
try {
    $response = Invoke-WebRequest -Uri "http://localhost:8080" -Method GET -UseBasicParsing -TimeoutSec 10
    if ($response.StatusCode -eq 200) {
        Write-Host "✅ Web application is responding (HTTP $($response.StatusCode))" -ForegroundColor Green
        
        # Check for HotelDruid content
        if ($response.Content -match "Hoteldruid") {
            Write-Host "✅ HotelDruid application loaded successfully" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Response received but HotelDruid content not detected" -ForegroundColor Yellow
        }
    } else {
        Write-Host "❌ Unexpected HTTP status: $($response.StatusCode)" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Web application test failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Check PHP syntax
Write-Host "3. Verifying PHP syntax..." -ForegroundColor Yellow
try {
    $syntaxCheck = docker exec hoteldruid-apache php -l /var/www/html/includes/menu_generator.php 2>&1
    if ($syntaxCheck -match "No syntax errors") {
        Write-Host "✅ PHP syntax is valid" -ForegroundColor Green
    } else {
        Write-Host "❌ PHP syntax issues detected: $syntaxCheck" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ PHP syntax check failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Check database
Write-Host "4. Testing database connection..." -ForegroundColor Yellow
try {
    $dbTest = docker exec hoteldruid-mysql mysql -u hoteldruid_user -photeldruid_password -e "SELECT 1 as test;" hoteldruid 2>&1
    if ($dbTest -match "test") {
        Write-Host "✅ Database connection successful" -ForegroundColor Green
    } else {
        Write-Host "❌ Database connection failed: $dbTest" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ Database test failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""

# Check phpMyAdmin
Write-Host "5. Testing phpMyAdmin..." -ForegroundColor Yellow
try {
    $phpMyAdminResponse = Invoke-WebRequest -Uri "http://localhost:8081" -Method GET -UseBasicParsing -TimeoutSec 10
    if ($phpMyAdminResponse.StatusCode -eq 200 -and $phpMyAdminResponse.Content -match "phpMyAdmin") {
        Write-Host "✅ phpMyAdmin is accessible" -ForegroundColor Green
    } else {
        Write-Host "❌ phpMyAdmin test failed" -ForegroundColor Red
    }
} catch {
    Write-Host "❌ phpMyAdmin test failed: $($_.Exception.Message)" -ForegroundColor Red
}

Write-Host ""
Write-Host "=== Verification Complete ===" -ForegroundColor Green
Write-Host ""
Write-Host "Access URLs:" -ForegroundColor Cyan
Write-Host "- HotelDruid Application: http://localhost:8080" -ForegroundColor White
Write-Host "- phpMyAdmin: http://localhost:8081" -ForegroundColor White
Write-Host ""
Write-Host "Docker Management:" -ForegroundColor Cyan
Write-Host "- View logs: docker logs hoteldruid-apache" -ForegroundColor White
Write-Host "- Stop containers: docker-compose down" -ForegroundColor White
Write-Host "- Start containers: docker-compose up -d" -ForegroundColor White