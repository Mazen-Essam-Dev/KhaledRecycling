# ParticipationsInEventReportController Documentation

## Overview
ASP.NET Core Controller for managing event participation reports with OTP-based approval workflow, Excel export capabilities, and comprehensive reporting features.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - IParticipationsInEventReportService: Business logic for participation reports
  - IMapper: AutoMapper for object mapping between entities and view models

## Core Report Operations

### Index
- **Purpose**: Main report listing with advanced filtering and pagination
- **Filters**:
  - Search term (ReportTitle)
  - Date range (DateFrom, DateTo)
- **Pagination**: 50 items per page
- **AJAX Support**: Returns partial view for AJAX requests
- **View Data**: Preserves filter parameters in ViewBag

### AddEdit (GET)
- **Purpose**: Display form for creating/editing participation reports
- **Parameters**: `id` (optional for create vs edit mode)
- **Flow**: Retrieves existing report for edit, creates new VM for add

### AddEdit (POST)
- **Purpose**: Handle form submission for create/update operations
- **Validation**: ModelState validation
- **Redirect Logic**:
  - After Create: Redirect to Index
  - After Edit: Redirect back to edit form with same ID

### Delete
- **Purpose**: Remove participation report records
- **Method**: HTTP POST for security
- **Flow**: Service deletion → Redirect to Index

## View-Only Operations

### Details
- **Purpose**: Read-only report details view
- **Attributes**: `[IgnoreAction]` for route exclusion
- **Access**: Single report viewing

### PrintDetails
- **Purpose**: Printable version of report details
- **Use Case**: Official documentation and record keeping
- **Format**: Print-optimized layout

## OTP Security System

### SendOtp
- **Purpose**: Initiate OTP process for report approval
- **Parameters**: 
  - `id`: Report ID
  - `role`: "trainer" or "manager"
- **Security**: `[NoLogging]` attribute for sensitive operations
- **Returns**: JSON success status

### ValidateOtp
- **Purpose**: Verify OTP codes for report verification
- **Parameters**: OTP code, report ID, and role from request body
- **Returns**: JSON with success status and message
- **Integration**: Uses service layer for OTP validation and user context

## Reporting & Export Features

### Print
- **Purpose**: Printer-friendly report list
- **Attributes**: `[IgnoreAction]`
- **Features**: Same filtering as Index without pagination
- **Data**: Full filtered dataset for printing

### createExcelReport_Download
- **Purpose**: Generate and download Excel reports of participation data
- **Excel Columns**:
  - Report Title (ParticipatingTitle)
  - Administrative Department (DepartManage)
  - Date (culture-aware formatting)
- **Localization**: Bilingual support (Arabic/English)
- **Process Flow**:
  1. Applies same filters as Index
  2. Maps data to ExcelDataDTO
  3. Generates Excel file using ExcelStaticReport
  4. Returns timestamped XLSX file
- **File Naming**: "ParticipationsInEventList_YYYYMMDD_HHmmss.xlsx"
- **Error Handling**: Redirects to Index on exception

## Data Processing & Filtering

### Search Functionality
- **Field**: ReportTitle
- **Type**: Case-insensitive contains search
- **Implementation**: StringComparison.OrdinalIgnoreCase

### Date Filtering
- **Fields**: DateFrom, DateTo
- **Type**: DateOnly range filtering
- **Persistence**: Parameters stored in ViewBag for form repopulation

### Pagination
- **Implementation**: PaginatedList<T> helper class
- **Page Size**: 50 records per page
- **AJAX Integration**: Partial view rendering for smooth user experience

## Technical Implementation

### View Model Mapping
- **Entity**: ParticipationsInEventReport
- **View Model**: ParticipationsInEventReportVM
- **Mapper**: AutoMapper for clean object transformation

### TempData Usage
- **Purpose**: Track navigation state between actions
- **Key**: "FromParticipationsInEventReportIndex"

### Security Features
- **Anti-Forgery Tokens**: All POST actions
- **Admin Authorization**: Controller-level access control
- **NoLogging**: Sensitive OTP operations excluded from logs