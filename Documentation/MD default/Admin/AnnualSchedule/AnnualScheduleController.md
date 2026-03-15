# AnnualScheduleController Documentation

## Overview
ASP.NET Core Controller for managing annual schedules with multi-language support, Excel export, and comprehensive CRUD operations.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - IAnnualScheduleService: Business logic service
  - IUnitOfWork: Database operations
  - IMapper: AutoMapper for object mapping

## Action Methods

### Index
- **Purpose**: Main listing page with search and filtering
- **Parameters**: `searchTerm`, `selectedYear`, pagination
- **Features**:
  - Year-based filtering
  - Text search across Item and Statement fields
  - Pagination with 50 items per page
  - AJAX partial view support for dynamic updates
  - Distinct year extraction for dropdown

### AddEdit (GET)
- **Purpose**: Display form for creating/editing schedules
- **Parameters**: `id` (optional for create vs edit)
- **Localization**: Day name display in Arabic/English
- **Flow**: Creates new VM or maps existing entity to VM

### AddEdit (POST)
- **Purpose**: Handle form submission for create/update
- **Validation**: ModelState validation
- **Year Extraction**: Auto-generates year from date
- **Redirect Logic**:
  - After Create: Redirect to Index
  - After Edit: Redirect back to edit form

### Details
- **Purpose**: Read-only view of schedule details
- **Attributes**: `[IgnoreAction]` for route exclusion
- **Features**: Localized day name display

### Delete
- **Purpose**: Remove schedule records
- **Method**: HTTP POST for security
- **Flow**: Service deletion → Redirect to Index

## Export & Print Features

### Print
- **Purpose**: Printer-friendly version of schedule list
- **Attributes**: `[IgnoreAction]`
- **Features**: Same filtering as Index without pagination

### createExcelReport_Download
- **Purpose**: Generate and download Excel reports
- **Process**:
  1. Applies same filters as Index
  2. Maps data to ExcelDataDTO
  3. Generates bilingual Excel file
  4. Returns FileContentResult with timestamped filename
- **Localization**: Arabic/English column headers
- **Error Handling**: Graceful redirect on failure

## Data Processing & Localization

### Search & Filtering
- Case-insensitive text search
- Year-based filtering
- Combined search conditions

### Localization Features
- Dynamic day name display using CultureInfo
- Bilingual Excel report generation
- Resource-based column headers
- Language-specific date formatting

### Excel Generation
- **Columns**: Item, Statement, Previous, Current, Record, Status, Year
- **Title**: Combined from resources
- **Formats**: XLSX with proper MIME type
- **Naming**: "InventoryList_YYYYMMDD_HHmmss.xlsx"

## View Models & Mapping

### AnnualScheduleVM
- Form data container
- Two-way mapping with entity
- Validation attribute support

### ExcelDataDTO
- Flat structure for Excel export
- Property mapping (t1-t7) for flexible column assignment
- Resource-based header configuration

## Pagination & UI
- **PaginatedList<T>**: Custom pagination implementation
- **ViewBag Usage**: Year lists, selected filters, total counts
- **Partial Views**: AJAX-compatible list rendering

## Error Handling
- NotFound returns for invalid IDs
- ModelState validation feedback
- Try-catch for Excel generation errors
- Graceful redirects on exceptions