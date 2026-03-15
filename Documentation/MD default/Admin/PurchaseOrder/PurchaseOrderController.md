# PurchaseOrderController Documentation

## Overview
ASP.NET Core Controller for managing purchase orders with supplier integration, auto-code generation, and OTP-based approval workflow.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - IPurchaseOrderService: Purchase order business logic
  - IMapper: AutoMapper for object mapping

## Core Purchase Order Operations

### 0. OTP
* OTP Roles `1(Manager)`.
* when new Add send Notification to First `1(Manager)`.
* if any user Submit Edit (Update) --> go back to `1(Manager)` send Updated .Notification To `1(Manager)`.
* OTP First sign `1(Manager)`.
* else if `1(Manager)` Validate otp then editing will be disabled for all users and Open ` Print`.
* if First User Sign --> Delete Button will be disabled for all users.
---

### Index
- **Purpose**: Main purchase order listing with advanced filtering
- **Filters**: 
  - Supplier selection
  - Date range filtering (DateFrom, DateTo)
- **UI Features**:
  - Supplier dropdown population
  - Date parameter preservation
  - Pagination (50 items per page)
  - AJAX partial view support

### AddEdit (GET)
- **Purpose**: Display form for creating/editing purchase orders
- **Auto-Code Generation**: Automatically generates new purchase order codes
- **Supplier Integration**: Populates supplier dropdown
- **Parameters**: `id` (optional for create vs edit)

### AddEdit (POST)
- **Purpose**: Handle form submission for create/update
- **Validation**: ModelState validation with supplier refresh
- **Code Preservation**: Maintains purchase order code on validation failure
- **Redirect Logic**:
  - After Create: Redirect to Index
  - After Edit: Redirect back to edit form

### Delete
- **Purpose**: Remove purchase order records
- **Method**: HTTP POST for security
- **Flow**: Service deletion → Redirect to Index

## View-Only Operations

### Details
- **Purpose**: Read-only purchase order details view
- **Supplier Context**: Shows supplier information
- **Attributes**: `[IgnoreAction]` for route exclusion

### PrintDetails
- **Purpose**: Printable version of purchase order details
- **Use Case**: Official documentation and supplier communication
- **Professional Formatting**: Print-optimized layout

## Reporting & Export Features

### Print
- **Purpose**: Printer-friendly purchase order list
- **Attributes**: `[IgnoreAction]`
- **Features**: Same filtering as Index without pagination
- **Supplier Integration**: Maintains supplier context

### createExcelReport_Download
- **Purpose**: Generate and download Excel reports
- **Excel Columns**:
  - Purchase Order Code
  - Supplier Name (localized)
  - Date (culture-aware formatting)
- **Process**:
  1. Applies same filters as Index
  2. Maps data to ExcelDataDTO with supplier lookups
  3. Generates bilingual Excel file
  4. Returns timestamped XLSX file
- **File Naming**: "PurchaseOrderList_YYYYMMDD_HHmmss.xlsx"

## OTP Security System

### SendOtp
- **Purpose**: Initiate OTP process for purchase order approval
- **Security**: `[NoLogging]` attribute for sensitive operations
- **Returns**: JSON success status

### ValidateOtp
- **Purpose**: Verify OTP codes for purchase order verification
- **Parameters**: OTP code from request body
- **Returns**: JSON with success status and message

## Data Processing

### Auto-Code Generation
```csharp
// Automatic code generation for new purchase orders
vm.PurchaseOrderCode = await _service.GetNewCodeAsync();