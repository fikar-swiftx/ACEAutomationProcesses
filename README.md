# ACE Automation Processes

## Overview
ACE Automation Processes is a C# application that automates the processing of various CSV files for employee data management. The application handles appointments, exits, metadata changes, ID changes, and secondments.

## Features
- Secure credential management
- Batch processing for large files
- Comprehensive error handling
- Email notifications for critical errors
- Environment-specific configurations
- Proper logging and monitoring

## Prerequisites
- .NET Framework 4.8
- SQL Server
- SMTP Server (for email notifications)
- OTCS API access

## Installation

### 1. Environment Setup
Run the environment setup script as Administrator:
```powershell
.\setup-environment.ps1
```

This will:
- Create required directories
- Set up environment variables
- Configure logging

### 2. Configuration
Update the following environment variables with your specific values:
- `ACE_ENVIRONMENT`: Set to "Development" or "Production"
- `ACE_DB_CONNECTION_STRING`: Your database connection string
- `ACE_OTCS_URL`: OTCS API URL
- `ACE_OTCS_USERNAME`: OTCS username
- `ACE_OTCS_PASSWORD`: OTCS password
- `ACE_SMTP_SERVER`: SMTP server address
- `ACE_SMTP_PORT`: SMTP server port
- `ACE_SMTP_USERNAME`: SMTP username
- `ACE_SMTP_PASSWORD`: SMTP password
- `ACE_NOTIFICATION_EMAIL`: Notification email address
- `ACE_ENCRYPTION_KEY`: Encryption key for sensitive data
- `ACE_CERTIFICATE_THUMBPRINT`: SSL certificate thumbprint

### 3. Build and Run
1. Open the solution in Visual Studio
2. Restore NuGet packages
3. Build the solution
4. Run the application

## Project Structure
```
ACEAutomationProcesses/
├── Configuration/
│   ├── EnvironmentConfig.cs
│   └── log4net.config
├── Services/
│   ├── BatchProcessingService.cs
│   ├── CsvProcessingService.cs
│   └── NotificationService.cs
├── Exceptions/
│   └── ExceptionHandler.cs
├── Model/
│   └── CSV/
├── Program.cs
└── App.config
```

## Error Handling
The application includes comprehensive error handling:
- All exceptions are logged
- Critical errors trigger email notifications
- Errors are categorized and handled appropriately
- Detailed error logs are maintained

## Logging
Logs are stored in the following locations:
- Main logs: `logs/ACEAutomationProcesses.log`
- Error logs: `logs/Error.log`
- Archive logs: `logs/archive/`

## Security
- All sensitive data is encrypted
- Credentials are stored in environment variables
- SSL certificate validation is implemented
- Environment-specific security settings

## Performance
- Batch processing for large files
- Async operations where appropriate
- Optimized buffer sizes
- Connection pooling

## Troubleshooting
1. Check the logs in the `logs` directory
2. Verify environment variables are set correctly
3. Ensure all prerequisites are met
4. Check network connectivity to required services

## Support
For support, contact:
- Email: [Your Support Email]
- Phone: [Your Support Phone]

## License
[Your License Information] 