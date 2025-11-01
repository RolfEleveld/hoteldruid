#!/usr/bin/env pwsh

Write-Host "=== HotelDruid Database Setup ===" -ForegroundColor Green
Write-Host ""

Write-Host "Setting up HotelDruid database tables..." -ForegroundColor Yellow

# Database configuration for Docker environment
$dbConfig = @{
    database_phprdb = "hoteldruid"
    host_phprdb = "hoteldruid-db"
    port_phprdb = "3306"
    user_phprdb = "hoteldruid_user"
    password_phprdb = "hoteldruid_password"
    tipo_db = "mysql"
    database_esistente = "SI"
    tempdatabase = ""
    prefisso_tab = ""
    nomeappartamenti = "Camera"
    numappartamenti = "5"
    numletti = "10"
    lingua = "ita"
    creabase = "SI"
    insappartamenti = "SI"
}

Write-Host "Database Configuration:" -ForegroundColor Cyan
Write-Host "- Host: $($dbConfig.host_phprdb):$($dbConfig.port_phprdb)" -ForegroundColor White
Write-Host "- Database: $($dbConfig.database_phprdb)" -ForegroundColor White
Write-Host "- User: $($dbConfig.user_phprdb)" -ForegroundColor White

try {
    Write-Host ""
    Write-Host "1. Testing web application connectivity..." -ForegroundColor Yellow
    
    $testResponse = Invoke-WebRequest -Uri "http://localhost:8080/creadb.php" -Method GET -UseBasicParsing -TimeoutSec 10
    if ($testResponse.StatusCode -eq 200) {
        Write-Host "✅ Web application is accessible" -ForegroundColor Green
    } else {
        throw "Web application returned status: $($testResponse.StatusCode)"
    }

    Write-Host ""
    Write-Host "2. Creating HotelDruid database tables..." -ForegroundColor Yellow
    
    # Prepare form data for database creation
    $formData = @{}
    foreach ($key in $dbConfig.Keys) {
        $formData[$key] = $dbConfig[$key]
    }
    
    # Submit database creation form
    $setupResponse = Invoke-WebRequest -Uri "http://localhost:8080/creadb.php" -Method POST -Body $formData -UseBasicParsing -TimeoutSec 30
    
    if ($setupResponse.StatusCode -eq 200) {
        Write-Host "✅ Database setup request submitted successfully" -ForegroundColor Green
        
        # Check if setup was successful by looking for success indicators
        if ($setupResponse.Content -match "success|creato|completat" -or $setupResponse.Content -notmatch "error|errore|fatal") {
            Write-Host "✅ Database tables appear to be created successfully" -ForegroundColor Green
        } else {
            Write-Host "⚠️  Database setup may have encountered issues" -ForegroundColor Yellow
            Write-Host "Response content preview:" -ForegroundColor Gray
            Write-Host ($setupResponse.Content.Substring(0, [Math]::Min(500, $setupResponse.Content.Length))) -ForegroundColor Gray
        }
    } else {
        throw "Database setup failed with status: $($setupResponse.StatusCode)"
    }

    Write-Host ""
    Write-Host "3. Testing HotelDruid application..." -ForegroundColor Yellow
    
    $appTestResponse = Invoke-WebRequest -Uri "http://localhost:8080/inizio.php" -Method GET -UseBasicParsing -TimeoutSec 10
    if ($appTestResponse.StatusCode -eq 200) {
        if ($appTestResponse.Content -match "fatal|error|Table.*doesn't exist") {
            Write-Host "⚠️  Application is still showing database errors" -ForegroundColor Yellow
            Write-Host "You may need to complete the setup manually at: http://localhost:8080/creadb.php" -ForegroundColor Cyan
        } else {
            Write-Host "✅ HotelDruid application is working!" -ForegroundColor Green
        }
    } else {
        Write-Host "❌ Application test failed with status: $($appTestResponse.StatusCode)" -ForegroundColor Red
    }

} catch {
    Write-Host "❌ Setup failed: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Manual Setup Instructions:" -ForegroundColor Cyan
    Write-Host "1. Open: http://localhost:8080/creadb.php" -ForegroundColor White
    Write-Host "2. Use these database settings:" -ForegroundColor White
    Write-Host "   - Database: hoteldruid" -ForegroundColor White
    Write-Host "   - Host: hoteldruid-db" -ForegroundColor White
    Write-Host "   - Port: 3306" -ForegroundColor White
    Write-Host "   - User: hoteldruid_user" -ForegroundColor White
    Write-Host "   - Password: hoteldruid_password" -ForegroundColor White
    Write-Host "3. Set rooms to 5 and beds to 10" -ForegroundColor White
    Write-Host "4. Click 'Create Database'" -ForegroundColor White
}

Write-Host ""
Write-Host "=== Setup Complete ===" -ForegroundColor Green
Write-Host ""
Write-Host "Access URLs:" -ForegroundColor Cyan
Write-Host "- HotelDruid Application: http://localhost:8080" -ForegroundColor White
Write-Host "- Database Setup (if needed): http://localhost:8080/creadb.php" -ForegroundColor White
Write-Host "- phpMyAdmin: http://localhost:8081" -ForegroundColor White