# Check if Docker Desktop is running
Write-Host "Checking Docker Desktop status..." -ForegroundColor Yellow

# First check if Docker process is running
$dockerProcess = Get-Process -Name "Docker Desktop" -ErrorAction SilentlyContinue
$dockerDaemonRunning = $false

if ($dockerProcess) {
    Write-Host "Docker Desktop process found. Testing daemon..." -ForegroundColor Cyan
    
    # Test if Docker daemon is responding with a timeout
    try {
        $dockerTest = Start-Job -ScriptBlock { docker version --format json 2>$null }
        $completed = Wait-Job $dockerTest -Timeout 10
        
        if ($completed) {
            $result = Receive-Job $dockerTest
            if ($result) {
                $dockerDaemonRunning = $true
                Write-Host "Docker Desktop is running and responsive." -ForegroundColor Green
            }
        }
        Remove-Job $dockerTest -Force
    } catch {
        Write-Host "Docker daemon test failed." -ForegroundColor Yellow
    }
}

if (-not $dockerDaemonRunning) {
    Write-Host "Docker Desktop is not running or not responsive. Starting Docker Desktop..." -ForegroundColor Red
    
    # Try to start Docker Desktop
    $dockerDesktopPath = Get-Command "Docker Desktop" -ErrorAction SilentlyContinue
    if (-not $dockerDesktopPath) {
        # Common installation paths for Docker Desktop
        $possiblePaths = @(
            "$env:ProgramFiles\Docker\Docker\Docker Desktop.exe",
            "$env:LOCALAPPDATA\Programs\Docker\Docker\Docker Desktop.exe",
            "${env:ProgramFiles(x86)}\Docker\Docker\Docker Desktop.exe"
        )
        
        foreach ($path in $possiblePaths) {
            if (Test-Path $path) {
                $dockerDesktopPath = $path
                break
            }
        }
    } else {
        $dockerDesktopPath = $dockerDesktopPath.Source
    }
    
    if ($dockerDesktopPath) {
        Write-Host "Starting Docker Desktop from: $dockerDesktopPath" -ForegroundColor Cyan
        Start-Process -FilePath $dockerDesktopPath -WindowStyle Minimized
        
        # Wait for Docker Desktop to start (up to 120 seconds)
        Write-Host "Waiting for Docker Desktop to start..." -ForegroundColor Yellow
        $timeout = 120
        $elapsed = 0
        
        while ($elapsed -lt $timeout) {
            # Check both process and daemon response
            $dockerProcess = Get-Process -Name "Docker Desktop" -ErrorAction SilentlyContinue
            if ($dockerProcess) {
                try {
                    $dockerTest = Start-Job -ScriptBlock { docker version --format json 2>$null }
                    $completed = Wait-Job $dockerTest -Timeout 5
                    
                    if ($completed) {
                        $result = Receive-Job $dockerTest
                        if ($result) {
                            Write-Host "`nDocker Desktop is now running and responsive!" -ForegroundColor Green
                            Remove-Job $dockerTest -Force
                            break
                        }
                    }
                    Remove-Job $dockerTest -Force
                } catch {
                    # Continue waiting
                }
            }
            
            Start-Sleep -Seconds 3
            $elapsed += 3
            Write-Host "." -NoNewline -ForegroundColor Yellow
        }
        
        if ($elapsed -ge $timeout) {
            Write-Host "`nTimeout waiting for Docker Desktop to start. Please start it manually and run this script again." -ForegroundColor Red
            exit 1
        }
        Write-Host ""
    } else {
        Write-Host "Docker Desktop not found. Please install Docker Desktop and try again." -ForegroundColor Red
        Write-Host "Download from: https://www.docker.com/products/docker-desktop" -ForegroundColor Cyan
        exit 1
    }
}

Write-Host "Starting HotelDroid Windows container..." -ForegroundColor Green

# Start Docker Compose in background job
$dockerJob = Start-Job -ScriptBlock {
    param($composePath)
    Set-Location (Split-Path $composePath)
    docker-compose --file $composePath -p windows up --build
} -ArgumentList (Resolve-Path ".\Docker.Windows.Container.compose.yml")

Write-Host "Container is starting in background..." -ForegroundColor Cyan
Write-Host "Waiting for web interface to become available..." -ForegroundColor Yellow

# Wait for the web interface to be ready (check port 8006)
$webReady = $false
$maxWaitTime = 300 # 5 minutes maximum wait
$elapsed = 0

while (-not $webReady -and $elapsed -lt $maxWaitTime) {
    try {
        $response = Invoke-WebRequest -Uri "http://localhost:8006" -TimeoutSec 5 -ErrorAction Stop
        $webReady = $true
        Write-Host "`nWeb interface is ready!" -ForegroundColor Green
    } catch {
        Start-Sleep -Seconds 5
        $elapsed += 5
        Write-Host "." -NoNewline -ForegroundColor Yellow
    }
}

if ($webReady) {
    Write-Host "Opening web browser..." -ForegroundColor Cyan
    # Start browser in background thread
    Start-Process "http://localhost:8006"
    
    Write-Host "Windows is ready to use!" -ForegroundColor Green
    Write-Host "Web interface opened in your default browser: http://localhost:8006" -ForegroundColor Cyan
    Write-Host "You can also connect via RDP to localhost:3389" -ForegroundColor Cyan
    Write-Host "Container will automatically shut down in 8 hours..." -ForegroundColor Yellow
    
    # Wait for the Docker job to complete or timeout
    Wait-Job $dockerJob -Timeout 28800 | Out-Null # 8 hours
} else {
    Write-Host "`nTimeout waiting for web interface. Container may still be starting..." -ForegroundColor Red
    Write-Host "Check manually at: http://localhost:8006" -ForegroundColor Cyan
    
    # Still wait for the job
    Wait-Job $dockerJob -Timeout 28800 | Out-Null # 8 hours
}

Start-Sleep -Seconds 28800 # 8 hours

Write-Host "Shutting down HotelDroid container..." -ForegroundColor Yellow

# Stop the Docker job if it's still running
if ($dockerJob.State -eq "Running") {
    Stop-Job $dockerJob
}
Remove-Job $dockerJob

docker-compose --file .\Docker.Windows.Container.compose.yml -p windows down