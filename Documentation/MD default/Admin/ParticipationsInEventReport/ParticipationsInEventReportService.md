# ParticipationsInEventReportService Documentation

## Overview
C# service class implementing business logic for managing event participation reports with image handling, dual-signature approval workflow, and comprehensive data management.

## Service Configuration
- **Namespace**: Application.Services.Admin
- **Interface**: IParticipationsInEventReportService
- **Dependencies**:
  - IUnitOfWork: Database operations and repository management
  - IWebHostEnvironment: Web hosting environment for file operations
  - IHttpContextAccessor: HTTP context access for OTP and user management

## Core Report Operations

### Data Retrieval Methods
- **GetAllAsync()**: Retrieves all participation reports
- **GetByIdAsync(int id)**: Gets specific report by ID including details and signatures
- **GetAllYearsInDb()**: Returns distinct years from all report dates for filtering

### Report Management
- **AddAsync(ParticipationsInEventReport entity)**: Creates new report with automatic image processing
- **UpdateAsync(ParticipationsInEventReport entity)**: Updates existing report with image management
- **DeleteAsync(int id)**: Removes report and associated details with file cleanup

## Image Management System

### Add Operation Image Handling
- **Automatic Path Generation**: Saves up to 4 images using FileHelper
- **Null Safety**: Handles null images with empty string paths
- **Folder Organization**: Uses "ParticipationsInEventReports" directory

```csharp
entity.Image1Path = entity.Image1 == null ? "" : await FileHelper.SaveImageAsync(entity.Image1, FileName);