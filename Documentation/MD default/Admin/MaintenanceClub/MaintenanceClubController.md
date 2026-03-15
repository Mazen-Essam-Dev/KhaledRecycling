# MaintenanceClubController Documentation

## Overview
ASP.NET Core Controller for managing club maintenance records with PDF file validation, comprehensive filtering, and reporting capabilities in a club management system.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - IMaintenanceClubService: Core maintenance business logic
  - ISupplierService: Supplier management
  - IUnitOfWork: Database operations
  - IMapper: AutoMapper for object mapping

## Core Maintenance Operations

### Index
- **Purpose**: Main maintenance record listing with advanced filtering
- **Filters**:
  - Search term (Title, Location, Details)
  - Date range (Date field with time normalization)
- **Pagination**: 50 items per page
- **Statistics**: Total count display in ViewBag
- **AJAX Support**: Partial view for AJAX requests
- **Date Handling**: Uses AtMidnight() extension for precise date filtering

### AddEdit (GET)
- **Purpose**: Display form for creating/editing maintenance records
- **Parameters**: `id` (optional for create vs edit mode)
- **Flow**: Creates new VM for add, maps entity for edit

### AddEdit (POST)
- **Purpose**: Handle form submission with PDF validation
- **PDF Validation**:
  - File type check (PDF only)
  - Size limit (5MB maximum)
  - Custom error messages via FileHelper
- **Redirect Logic**:
  - After Create: Redirect to Index
  - After Edit: Redirect back to edit form

### Delete
- **Purpose**: Remove maintenance records
- **Method**: HTTP POST for security
- **Flow**: Service deletion → Redirect to Index

## View-Only Operations

### Details
- **Purpose**: Read-only maintenance record details
- **Attributes**: `[IgnoreAction]` for route exclusion
- **Access**: Single record viewing

### PrintDetails
- **Purpose**: Printable version of maintenance details
- **Use Case**: Official documentation and record keeping
- **Format**: Print-optimized layout

## PDF File Validation System

### Comprehensive File Checks
```csharp
var PdfFile_Text = await FileHelper.CheckFileIsPdf_5Mg_Async(model.PdfFile);
if (PdfFile_Text != "OK" && PdfFile_Text != "null") 
    ModelState.AddModelError("PdfFilePath", PdfFile_Text);