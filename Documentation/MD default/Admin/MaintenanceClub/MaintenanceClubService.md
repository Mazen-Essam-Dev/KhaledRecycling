# MaintenanceClubService Documentation

## Overview
C# service class implementing business logic for club maintenance record management with PDF file handling and basic CRUD operations in a club management system.

## Service Configuration
- **Namespace**: Application.Services.Admin
- **Interface**: IMaintenanceClubService
- **Dependencies**:
  - IUnitOfWork: Database operations and repository management
  - IWebHostEnvironment: Web hosting environment for file operations

## Core Maintenance Operations

### Data Retrieval Methods
- **GetAllAsync()**: Retrieves all maintenance club records
- **GetByIdAsync(int id)**: Gets specific maintenance record by ID

### Maintenance Management
- **AddAsync(MaintenanceClub entity)**: Creates new maintenance record with PDF file processing
- **UpdateAsync(MaintenanceClub entity)**: Updates existing record with conditional PDF file management
- **DeleteAsync(int id)**: Removes maintenance record and associated PDF file

## PDF File Management System

### Add Operation
- **File Processing**: Automatically saves uploaded PDF files
- **Path Generation**: Uses FileHelper for consistent file storage
- **Null Safety**: Handles null files with empty string paths
- **Folder Organization**: Uses "MaintenanceClubs" directory

```csharp
entity.PdfFilePath = entity.PdfFile == null ? "" : await FileHelper.SaveImageAsync(entity.PdfFile, FileName);