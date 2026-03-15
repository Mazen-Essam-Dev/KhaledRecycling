# ArchivingDocumentController Documentation

## Overview
ASP.NET Core Controller for managing document archiving system with comprehensive search, filtering, and export capabilities.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - ISupplierService: Supplier data access
  - IUnitOfWork: Database operations
  - IMapper: AutoMapper for object mapping
  - IArchivingDocumentService: Main business logic service

## Action Methods

### Index
- **Purpose**: Main document listing with advanced filtering
- **Parameters**: `searchTerm`, `type`, `dateFrom`, `dateTo`, pagination
- **Filtering**:
  - Document type enum filtering
  - Text search across Title, Authority, Reference Number
  - Date range filtering with midnight normalization
- **Features**:
  - Pagination (50 items per page)
  - Enum display name resolution
  - AJAX partial view support
  - Date parameter preservation

### AddEdit (GET)
- **Purpose**: Display form for creating/editing documents
- **Parameters**: `id` (optional for create vs edit)
- **Enum Handling**: Populates DocumentType dropdown
- **Flow**: Creates new VM or maps existing entity with enum text

### AddEdit (POST)
- **Purpose**: Handle form submission with file validation
- **Validation**:
  - ModelState validation
  - PDF file validation (type and 5MB size limit)
  - Anti-forgery token protection
- **File Check**: Uses FileHelper for PDF validation
- **Redirect Logic**:
  - After Create: Redirect to Index
  - After Edit: Redirect back to edit form

### Delete
- **Purpose**: Remove document records
- **Method**: HTTP POST for security
- **Flow**: Service deletion → Redirect to Index

## Export & Print Features

### Print
- **Purpose**: Printer-friendly document list
- **Attributes**: `[IgnoreAction]`
- **Features**: Same filtering as Index without pagination

### createExcelReport_Download
- **Purpose**: Generate and download Excel reports
- **Excel Columns**:
  - Document Reference Number
  - Document Title
  - Document Type
  - Date (dd-MM-yyyy format)
  - Document Authority
- **Process**:
  1. Applies same filters as Index
  2. Maps data to ExcelDataDTO
  3. Generates bilingual Excel file
  4. Returns timestamped XLSX file
- **Localization**: Arabic/English column headers

## Data Processing & Filtering

### Search Implementation
- Multi-field text search (Title, Authority, Reference Number)
- Case-insensitive string comparison
- Combined search conditions with OR logic

### Date Filtering
- Uses AtMidnight() extension for date normalization
- Range filtering with inclusive boundaries
- Date parameter formatting for UI persistence

### Enum Management
- DocumentType enum with display names
- SelectListHelper for dropdown population
- EnumHelper for display name resolution
- TypeText property for human-readable values

## View Models & Mapping

### ArchivingDocumentVM
- Form data container with validation
- Enum handling properties
- File upload support
- TypeText for display purposes

### ExcelDataDTO
- Flat structure for Excel export
- Property mapping (t1-t5) for column assignment
- Date formatting for consistency
- Resource-based localization

## UI Components

### Pagination
- Custom PaginatedList implementation
- Search term preservation
- Date parameter serialization
- Total count display

### Enum Dropdown
- Dynamic SelectList generation
- Pre-selected values in edit mode
- Resource-based display names

## Error Handling & Validation
- ModelState validation with error display
- PDF file validation with custom error messages
- Graceful exception handling in Excel generation
- Null-safe enum operations