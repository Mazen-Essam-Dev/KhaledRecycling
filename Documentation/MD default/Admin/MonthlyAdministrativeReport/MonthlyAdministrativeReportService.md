# MonthlyAdministrativeReportService Documentation

## Overview
C# service class implementing business logic for monthly administrative reports with comprehensive image management, dual-role OTP approval system, and month/year validation to prevent duplicate entries.

## Service Configuration
- **Namespace**: Application.Services.Admin
- **Interface**: IMonthlyAdministrativeReportService
- **Dependencies**:
  - IUnitOfWork: Database operations and repository management
  - IWebHostEnvironment: Web hosting environment for file operations
  - IHttpContextAccessor: HTTP context access for OTP and user management

## Core Report Operations

### Data Retrieval Methods
- **GetAllAsync()**: Retrieves all monthly administrative reports
- **GetByIdAsync(int id)**: Gets specific report by ID including details and dual signatures
- **GetAllYearsInDb()**: Returns distinct years from all report dates for UI filtering

### Report Management
- **AddAsync(MonthlyAdministrativeReport entity)**: Creates new report with automatic image processing
- **UpdateAsync(MonthlyAdministrativeReport entity)**: Updates existing report with image management and detail preservation
- **DeleteAsync(int id)**: Removes report and associated details with complete file cleanup

## Image Management System

### Add Operation Image Handling
- **Automatic Path Generation**: Saves up to 4 images using FileHelper
- **Null Safety**: Handles null images with empty string paths
- **Folder Organization**: Uses "MonthlyAdministrativeReports" directory
- **Batch Processing**: Processes all images in single operation

### Update Operation Image Management
- **Path Preservation**: Maintains existing image paths unless new images provided
- **Conditional Updates**: Only processes images that have been changed
- **Old File Cleanup**: Deletes previous image files when new ones uploaded
- **Detail Preservation**: Maintains existing report details during update

### Delete Operation File Cleanup
- **Complete Media Removal**: Deletes all associated image files from disk
- **Database Cascade**: Removes related report details records
- **Transaction Safety**: Ensures all operations complete successfully

## Dual-Role OTP Security System

### SendOtpAsync
- **Purpose**: Initiates OTP process for report approval
- **Parameters**: 
  - `id`: Report ID
  - `role`: "trainer" or "manager" for role-specific approval
- **Implementation**: Uses OTPHelper for secure code generation and storage

### ValidateOtpAsync
- **Purpose**: Verifies OTP and attaches appropriate signature based on role
- **Validation Flow**:
  1. Validates OTP code authenticity
  2. Verifies report existence
  3. Retrieves user's latest signature
  4. Assigns signature to appropriate role field (TrainerSignitureId or ManagerSignitureId)
  5. Commits changes to database

### Role-Based Signature Assignment
```csharp
if (role == "trainer")
    report.TrainerSignitureId = latestSignature.Id;
else if (role == "manager")
    report.ManagerSignitureId = latestSignature.Id;