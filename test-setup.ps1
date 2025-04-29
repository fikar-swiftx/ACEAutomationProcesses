# ACE Automation Processes Setup Test Script
# This script verifies the environment setup and configuration

# Function to check if environment variable exists
function Test-EnvironmentVariable {
    param(
        [string]$Name,
        [string]$Description
    )
    
    $value = [System.Environment]::GetEnvironmentVariable($Name, [System.EnvironmentVariableTarget]::Machine)
    if ([string]::IsNullOrEmpty($value)) {
        Write-Host "❌ $Name is not set`nDescription: $Description" -ForegroundColor Red
        return $false
    }
    else {
        Write-Host "✅ $Name is set`nDescription: $Description" -ForegroundColor Green
        return $true
    }
}

# Function to check if directory exists
function Test-Directory {
    param(
        [string]$Path,
        [string]$Description
    )
    
    if (Test-Path $Path) {
        Write-Host "✅ Directory exists: $Path`nDescription: $Description" -ForegroundColor Green
        return $true
    }
    else {
        Write-Host "❌ Directory does not exist: $Path`nDescription: $Description" -ForegroundColor Red
        return $false
    }
}

# Check if running as administrator
$user = [Security.Principal.WindowsIdentity]::GetCurrent()
$principal = New-Object Security.Principal.WindowsPrincipal $user
if (-not $principal.IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)) {
    Write-Host "Please run this script as Administrator" -ForegroundColor Red
    exit
}

Write-Host "`nTesting Environment Setup...`n" -ForegroundColor Cyan

# Test environment variables
$envVars = @{
    "ACE_ENVIRONMENT" = "Environment (Development/Production)"
    "ACE_DB_CONNECTION_STRING" = "Database connection string"
    "ACE_OTCS_URL" = "OTCS API URL"
    "ACE_OTCS_USERNAME" = "OTCS username"
    "ACE_OTCS_PASSWORD" = "OTCS password"
    "ACE_SMTP_SERVER" = "SMTP server address"
    "ACE_SMTP_PORT" = "SMTP server port"
    "ACE_SMTP_USERNAME" = "SMTP username"
    "ACE_SMTP_PASSWORD" = "SMTP password"
    "ACE_NOTIFICATION_EMAIL" = "Notification email address"
    "ACE_ENCRYPTION_KEY" = "Encryption key for sensitive data"
    "ACE_CERTIFICATE_THUMBPRINT" = "SSL certificate thumbprint"
}

$envVarResults = @()
foreach ($var in $envVars.GetEnumerator()) {
    $envVarResults += Test-EnvironmentVariable -Name $var.Key -Description $var.Value
}

# Test directories
$directories = @{
    "logs" = "Main logs directory"
    "logs\archive" = "Archive logs directory"
    "logs\error" = "Error logs directory"
}

$dirResults = @()
foreach ($dir in $directories.GetEnumerator()) {
    $dirResults += Test-Directory -Path $dir.Key -Description $dir.Value
}

# Summary
Write-Host "`nTest Summary:" -ForegroundColor Cyan
Write-Host "Environment Variables: $($envVarResults.Where({$_}).Count)/$($envVarResults.Count) passed"
Write-Host "Directories: $($dirResults.Where({$_}).Count)/$($dirResults.Count) passed"

if ($envVarResults.Contains($false) -or $dirResults.Contains($false)) {
    Write-Host "`n❌ Some tests failed. Please fix the issues above and run the test again." -ForegroundColor Red
}
else {
    Write-Host "`n✅ All tests passed! The environment is properly configured." -ForegroundColor Green
} 