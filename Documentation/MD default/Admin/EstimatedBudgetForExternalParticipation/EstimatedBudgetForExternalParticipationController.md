# EstimatedBudgetForExternalParticipationController Documentation

## Overview
ASP.NET Core Controller for managing estimated budgets for external participation events with OTP verification, detailed budgeting, and comprehensive reporting.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - IEstimatedBudgetForExternalParticipationService: Business logic service
  - IMapper: AutoMapper for object mapping

## Main CRUD Operations

### Index
- **Purpose**: Main listing of external participation budgets
- **Parameters**: `searchTerm`, `dateFrom`, `dateTo`, pagination
- **Search Features**:
  - Text search on ParticipatingTitle (case-insensitive)
  - Date range filtering on CreationDate
- **UI Features**:
  - Pagination (50 items per page)
  - Date parameter preservation
  - AJAX partial view support
  - TempData for navigation state

### AddEdit (GET)
- **Purpose**: Display form for creating/editing budget entries
- **Parameters**: `id` (optional for create vs edit)
- **Flow**: Creates new VM or maps existing entity to VM

### AddEdit (POST)
- **Purpose**: Handle form submission with detail management
- **Validation**: ModelState validation
- **Detail Processing**: Updates participation type details
- **Redirect Logic**:
  - After Create: Redirect to Index
  - After Edit: Redirect back to edit form

### Delete
- **Purpose**: Remove budget records
- **Method**: HTTP POST for security
- **Flow**: Service deletion → Redirect to Index

## OTP Security System

### SendOtp
- **Purpose**: Initiates OTP process for budget approval
- **Security**: `[NoLogging]` attribute for sensitive operation
- **Returns**: JSON success status

### ValidateOtp
- **Purpose**: Validates OTP codes for budget verification
- **Parameters**: OTP code from request body
- **Returns**: JSON with success status and message

## View-Only Operations

### Details
- **Purpose**: Read-only view of budget details
- **Attributes**: `[IgnoreAction]` for route exclusion
- **Use Case**: Budget review without editing

### PrintDetails
- **Purpose**: Printable version of budget details
- **Attributes**: `[IgnoreAction]`
- **Use Case**: Official documentation and reporting

## Reporting & Export Features

### Print
- **Purpose**: Printer-friendly budget list
- **Attributes**: `[IgnoreAction]`
- **Features**: Same filtering as Index without pagination
- **State Management**: TempData for navigation context

### createExcelReport_Download
- **Purpose**: Generate and download Excel reports
- **Excel Columns**:
  - Participating Title
  - Regulator
  - Budget Date (dd-MM-yyyy format)
  - Participating Country
- **Process**:
  1. Applies same filters as Index
  2. Maps data to ExcelDataDTO
  3. Generates bilingual Excel file
  4. Returns timestamped XLSX file
- **File Naming**: "EstimatedBudgetForExternalParticipationList_YYYYMMDD_HHmmss.xlsx"

## Data Processing

### Search Implementation
```csharp
EstimatedBudgetForExternalParticipationVMs = EstimatedBudgetForExternalParticipationVMs.Where(c =>
    (!string.IsNullOrEmpty(c.ParticipatingTitle) && 
     c.ParticipatingTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
);