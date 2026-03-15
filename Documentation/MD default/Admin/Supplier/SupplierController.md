# SupplierController Documentation

## Overview
ASP.NET Core Controller for managing suppliers with bilingual support, comprehensive search capabilities, and export functionality.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - ISupplierService: Supplier business logic service
  - IMapper: AutoMapper for object mapping

## Main CRUD Operations

### Index
- **Purpose**: Main supplier listing with advanced search
- **Search Features**:
  - Supplier name search (Arabic/English)
  - Email address search
  - Phone number search
  - Case-insensitive string comparison
- **UI Features**:
  - Total count display
  - Pagination (50 items per page)
  - AJAX partial view support

### AddEdit (GET)
- **Purpose**: Display form for creating/editing suppliers
- **Parameters**: `id` (optional for create vs edit)
- **Flow**: Creates new VM or maps existing entity to VM

### AddEdit (POST)
- **Purpose**: Handle form submission for create/update
- **Validation**: ModelState validation
- **Redirect Logic**:
  - After Create: Redirect to Index
  - After Edit: Redirect back to edit form

### Delete
- **Purpose**: Remove supplier records
- **Method**: HTTP POST for security
- **Flow**: Service deletion → Redirect to Index

## Reporting & Export Features

### Print
- **Purpose**: Printer-friendly supplier list
- **Attributes**: `[IgnoreAction]`
- **Features**: Same search functionality as Index without pagination

### createExcelReport_Download
- **Purpose**: Generate and download Excel reports
- **Excel Columns**:
  - Supplier Name (localized)
  - Email Address
  - Mobile Number
  - VAT Number
- **Process**:
  1. Applies same search filters as Index
  2. Maps data to ExcelDataDTO with proper localization
  3. Generates bilingual Excel file
  4. Returns timestamped XLSX file
- **File Naming**: "SuppliersList_YYYYMMDD_HHmmss.xlsx"

## Data Processing

### Search Implementation
```csharp
supplierVMs = supplierVMs.Where(s =>
    (!string.IsNullOrEmpty(s.SupplierNameAr) && s.SupplierNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
    (!string.IsNullOrEmpty(s.SupplierNameEn) && s.SupplierNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
    (!string.IsNullOrEmpty(s.Email) && s.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
    (!string.IsNullOrEmpty(s.Phone) && s.Phone.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
);