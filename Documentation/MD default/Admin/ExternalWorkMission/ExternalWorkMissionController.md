# ExternalWorkMissionController Documentation

## Overview
ASP.NET Core Controller for managing external work missions with employee assignment, mission type tracking, and OTP-based approval workflow.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - IExternalWorkMissionService: Business logic service
  - IMapper: AutoMapper for object mapping
  - IUnitOfWork: Database operations

## Main CRUD Operations

### Index
- **Purpose**: Main mission listing with advanced employee search
- **Search Features**:
  - Employee name search (Arabic/English)
  - Date range filtering on MissionDate
  - Employee ID-based filtering
- **Employee Integration**: Dynamic employee dropdown with job titles
- **UI Features**:
  - Pagination (50 items per page)
  - AJAX partial view support
  - Date parameter preservation

### AddEdit (GET)
- **Purpose**: Display form for creating/editing missions
- **Employee Data**: Populates employee dropdown with job titles
- **Dynamic Job Display**: Shows selected employee's job title
- **Parameters**: `id` (optional for create vs edit)

### AddEdit (POST)
- **Purpose**: Handle form submission with mission type management
- **Mission Types**: Supports multiple mission type selection
- **Employee Validation**: Dynamic job title display
- **Redirect Logic**:
  - After Create: Redirect to Index
  - After Edit: Redirect back to edit form

### Delete
- **Purpose**: Remove mission records
- **Method**: HTTP POST for security
- **Flow**: Service deletion → Redirect to Index

## View-Only Operations

### Details
- **Purpose**: Read-only mission details view
- **Employee Context**: Shows assigned employee and job information
- **Attributes**: `[IgnoreAction]` for route exclusion

### PrintDetails
- **Purpose**: Printable version of mission details
- **Use Case**: Official documentation and reporting
- **Professional Formatting**: Print-optimized layout

## OTP Security System

### SendOtp
- **Purpose**: Initiates OTP process for mission approval
- **Security**: `[NoLogging]` attribute for sensitive operations
- **Returns**: JSON success status

### ValidateOtp
- **Purpose**: Validates OTP codes for mission verification
- **Type Handling**: Proper ID conversion from long to int
- **Returns**: JSON with success status and message

## Reporting & Export Features

### Print
- **Purpose**: Printer-friendly mission list
- **Attributes**: `[IgnoreAction]`
- **Features**: Same filtering as Index without pagination
- **Employee Search**: Maintains employee-based filtering

### createExcelReport_Download
- **Purpose**: Generate and download Excel reports
- **Excel Columns**:
  - Employee Name (localized)
  - Employee Job Title
  - Mission Date (dd-MM-yyyy format)
  - Mission Location
  - Mission Country
  - Mission Types (concatenated)
- **Process**:
  1. Applies same filters as Index
  2. Maps data to ExcelDataDTO with employee lookups
  3. Generates bilingual Excel file
  4. Returns timestamped XLSX file
- **File Naming**: "ExternalWorkMissionList_YYYYMMDD_HHmmss.xlsx"

## Employee Integration

### Dynamic Employee Management
- **Employee Dropdown**: Real-time employee data loading
- **Job Title Display**: Automatic job title retrieval and display
- **Search Integration**: Employee name-based filtering

### GetJob API
- **Purpose**: AJAX endpoint for employee job title retrieval
- **Parameters**: `empId` for specific employee lookup
- **Returns**: JSON with job title and status
- **Security**: `[NoLogging]` attribute

## Data Processing

### Advanced Search Implementation
```csharp
// Employee name search with ID conversion
var EmployeesNamesHasTerm = allEmployeesNames.Where(c =>
    (!string.IsNullOrEmpty(c.FullNameAr) && c.FullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
    (!string.IsNullOrEmpty(c.FullNameEn) && c.FullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
);
var EmpIds = EmployeesNamesHasTerm.Select(x => x.Id).ToList();