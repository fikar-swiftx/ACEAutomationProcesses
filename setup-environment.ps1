# ACE Automation Processes Environment Setup Script
# This script sets up the required environment variables and directories

# Function to check if running as administrator
function Test-Administrator {
    $user = [Security.Principal.WindowsIdentity]::GetCurrent()
    $principal = New-Object Security.Principal.WindowsPrincipal $user
    $principal.IsInRole([Security.Principal.WindowsBuiltinRole]::Administrator)
}

# Check if running as administrator
if (-not (Test-Administrator)) {
    Write-Host "Please run this script as Administrator" -ForegroundColor Red
    exit
}

# Create required directories
$directories = @(
    "logs",
    "logs\archive",
    "logs\error"
)

foreach ($dir in $directories) {
    if (-not (Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir | Out-Null
        Write-Host "Created directory: $dir" -ForegroundColor Green
    }
}

# Function to set environment variable
function Set-EnvironmentVariable {
    param(
        [string]$Name,
        [string]$Value,
        [string]$Description
    )
    
    [System.Environment]::SetEnvironmentVariable($Name, $Value, [System.EnvironmentVariableTarget]::Machine)
    Write-Host "Set $Name`nDescription: $Description`nValue: $Value" -ForegroundColor Yellow
}

# Set environment variables
$envVars = @{
    "ACE_ENVIRONMENT" = @{
        Value = "Development"
        Description = "Environment (Development/Production)"
    }
    "ACE_DB_CONNECTION_STRING" = @{
        Value = "Data Source=dev.vital.dsg.internal;Initial Catalog=devsotcsdb;uid=sa;password=!2EZ4u&me;Min Pool Size=5; Max Pool Size=20"
        Description = "Database connection string"
    }
    "ACE_OTCS_URL" = @{
        Value = "http://dev.vital.dsg.internal/OTCS/cs.exe/api/v1/"
        Description = "OTCS API URL"
    }
    "ACE_OTCS_USERNAME" = @{
        Value = "otadmin@otds.admin"
        Description = "OTCS username"
    }
    "ACE_OTCS_PASSWORD" = @{
        Value = "Livelink#CS16"
        Description = "OTCS password"
    }
    "ACE_SMTP_SERVER" = @{
        Value = "smtp.office365.com"
        Description = "SMTP server address"
    }
    "ACE_SMTP_PORT" = @{
        Value = "587"
        Description = "SMTP server port"
    }
    "ACE_SMTP_USERNAME" = @{
        Value = "your-email@domain.com"
        Description = "SMTP username"
    }
    "ACE_SMTP_PASSWORD" = @{
        Value = "your-password"
        Description = "SMTP password"
    }
    "ACE_NOTIFICATION_EMAIL" = @{
        Value = "admin@domain.com"
        Description = "Notification email address"
    }
    "ACE_ENCRYPTION_KEY" = @{
        Value = "Your-Secure-Key-123!@#"
        Description = "Encryption key for sensitive data"
    }
    "ACE_CERTIFICATE_THUMBPRINT" = @{
        Value = "Your-Certificate-Thumbprint"
        Description = "SSL certificate thumbprint for validation"
    }
}

# Set each environment variable
foreach ($var in $envVars.GetEnumerator()) {
    Set-EnvironmentVariable -Name $var.Key -Value $var.Value.Value -Description $var.Value.Description
}

Write-Host "`nEnvironment setup completed!" -ForegroundColor Green
Write-Host "Please review the settings and update any values as needed." -ForegroundColor Yellow
Write-Host "You may need to restart your application for the changes to take effect." -ForegroundColor Yellow 