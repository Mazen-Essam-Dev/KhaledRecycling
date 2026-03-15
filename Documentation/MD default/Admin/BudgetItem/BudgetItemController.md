# BudgetItemController Documentation

## Overview
ASP.NET Core Controller for managing budget items with automatic code generation, search functionality, and export capabilities.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - IBudgetItemService: Business logic service
  - IMapper: AutoMapper for object mapping

## Action Methods

### Index
- **Purpose**: Main budget items listing with search and pagination
- **Parameters**: `searchTerm`, pagination (default 50 items per page)
- **Search Features**:
  - Text search on ItemTitle (case-insensitive)
  - Numeric search on ItemNumber
  - Combined OR logic for text and number search
- **UI Features**:
  - Deletion failure feedback via TempData
  - AJAX partial view support
  - Paginated list implementation

### AddEdit (GET)
- **Purpose**: Display form for creating/editing budget items
- **Parameters**: `id` (optional for create vs edit)
- **Auto-Generation**: Automatically generates new item number for create operations
- **Flow**:
  - Create: Generates new code and returns empty VM
  - Edit: Fetches existing item and maps to VM

### AddEdit (POST)
- **Purpose**: Handle form submission for create/update operations
- **Validation**: ModelState validation
- **Redirect Logic**:
  - After Create: Redirect to Index
  - After Edit: Redirect back to edit form with ID
- **Security**: Anti-forgery token protection

### Delete
- **Purpose**: Remove budget item records
- **Feedback**: TempData for deletion failure notification
- **Process**: Service deletion with result checking

## Export & Print Features

### Print
- **Purpose**: Printer-friendly budget items list
- **Attributes**: `[IgnoreAction]`
- **Features**: Same search functionality as Index without pagination

### createExcelReport_Download
- **Purpose**: Generate and download Excel reports
- **Excel Columns**:
  - Item Number
  - Item Title
- **Process**:
  1. Applies same search filters as Index
  2. Maps data to ExcelDataDTO
  3. Generates bilingual Excel file
  4. Returns timestamped XLSX file
- **File Naming**: "BudgetItemsList_YYYYMMDD_HHmmss.xlsx"

## Search Implementation

### Dual Search Strategy
```csharp
// Text search OR numeric search
budgetItemVMs = budgetItemVMs.Where(s =>
    (!string.IsNullOrEmpty(s.ItemTitle) && 
     s.ItemTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
    (s.ItemNumber == searchNumberInt)
);