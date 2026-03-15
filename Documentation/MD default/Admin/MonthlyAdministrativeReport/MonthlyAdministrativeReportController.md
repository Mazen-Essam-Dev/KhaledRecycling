# MonthlyAdministrativeReportController Documentation

## Overview
ASP.NET Core Controller for managing monthly administrative reports with month/year validation, dual-role OTP approval, and comprehensive reporting features.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - IMonthlyAdministrativeReportService: Business logic for administrative reports
  - IMapper: AutoMapper for object mapping between entities and view models

## Core Report Operations

### OTP
* OTP Roles `1(activityMonitor)` --> `2(Manager)` Arranged
* when new Add send Notification to First `1(activityMonitor)`
* if this new Add in new Quarter will send Notification to Manager `New Quarter year ..`.
* And if first Quarter in year send Notification Annually Also to Manager.
* OTP First sign `1(activityMonitor)` then editing will be disabled for all users except the Manager, and a notification will be sent to the `2(Manager)`
and then Open Sign OTP for `2(Manager)` 
* if `2(Manager)` Submit Edit (Update) --> go back to `1(activityMonitor)` and remove his sign and send Update Notification To `1(activityMonitor)`
* else if `2(Manager)` Validate otp then editing will be disabled for all users and Open ` Print`
* if First User Sign --> Delete Button will be disabled for all users.
---

### Index
- **Purpose**: Main report listing with month/year filtering and pagination
- **Filters**:
  - Selected Year (dropdown from database years)
  - Selected Month (1-12 with localized month names)
- **Pagination**: 50 items per page
- **UI Features**:
  - Year dropdown populated from database
  - Month dropdown with localized names
  - Enum type to text conversion for display
  - AJAX partial view support

### AddEdit (GET)
- **Purpose**: Display form for creating/editing administrative reports
- **Parameters**: `id` (optional for create vs edit mode)
- **Enum Support**: Populates report type dropdown from MonthlyAdministrativeReportType enum

### AddEdit (POST)
- **Purpose**: Handle form submission with month/year validation
- **Validation**:
  - ModelState validation
  - Month/Year/Type uniqueness check via CheckIsMonthRegistedBefore
- **Business Rule**: Prevents duplicate reports for same month/year/type combination
- **Redirect Logic**:
  - After Create: Redirect to Index
  - After Edit: Redirect back to edit form

### Delete
- **Purpose**: Remove administrative report records
- **Method**: HTTP POST for security
- **Flow**: Service deletion → Redirect to Index

## View-Only Operations

### Details
- **Purpose**: Read-only report details view
- **Features**: Enum type conversion to display text
- **Data Enhancement**: Converts type integer to readable text

### PrintDetails
- **Purpose**: Printable version of report details
- **Enum Handling**: Uses Enum.GetName for type display
- **Use Case**: Official documentation and record keeping

## OTP Security System

### SendOtp
- **Purpose**: Initiate OTP process for report approval
- **Parameters**:
  - `id`: Report ID
  - `role`: "trainer" or "manager"
- **Security**: `[NoLogging]` and `[IgnoreAction]` attributes
- **Returns**: JSON success status

### ValidateOtp
- **Purpose**: Verify OTP codes for report verification
- **Parameters**: OTP code, report ID, and role from request body
- **Returns**: JSON with success status and message
- **Integration**: Uses service layer for OTP validation

## Date Validation System

### CheckDate
- **Purpose**: AJAX endpoint for real-time date validation
- **Parameters**: Report ID, Date, and Report Type
- **Validation**: Checks for existing reports in same month/year/type
- **Returns**: JSON boolean indicating if date is already registered
- **Use Case**: Client-side validation during form filling

## Reporting & Export Features

### Print
- **Purpose**: Printer-friendly report list
- **Attributes**: `[IgnoreAction]`
- **Features**: Same month/year filtering as Index
- **Data Enhancement**: Enum type conversion for display

### createExcelReport_Download
- **Purpose**: Generate and download Excel reports of administrative data
- **Excel Columns**:
  - Year (extracted from date)
  - Month (localized month name)
  - Report Type (converted from enum)
- **Localization**: Bilingual support (Arabic/English)
- **Process Flow**:
  1. Applies same month/year filters as Index
  2. Converts enum types to display text
  3. Maps data to ExcelDataDTO
  4. Generates Excel file using ExcelStaticReport
- **File Naming**: "MonthlyAdministrativeReportList_YYYYMMDD_HHmmss.xlsx"
- **Error Handling**: Redirects to Index on exception

## Data Processing & UI Enhancement

### Enum Management
- **Type Conversion**: Integer enum values to display text
- **Dropdown Population**: SelectListHelper for enum-based dropdowns
- **Display Names**: EnumHelper.GetDisplayName for user-friendly text

### Filter Persistence
- **ViewBag Usage**: Preserves selected year/month for form repopulation
- **Dropdown Data**: Years from database, months from enum range

### AJAX Integration
- **Partial Views**: _ListPartial for AJAX pagination
- **Real-time Validation**: CheckDate endpoint for immediate feedback
- **Request Detection**: XMLHttpRequest header checking

## Business Rules & Validation

### Month/Year Uniqueness
- **Rule**: Only one report per month/year/type combination
- **Validation**: Server-side in AddEdit, client-side via CheckDate
- **Error Message**: Resource2.ReportIsExist for localized error display

### Enum Type Handling
- **Storage**: Integer values in database
- **Display**: Localized text representation
- **Forms**: Dropdown selection with enum values