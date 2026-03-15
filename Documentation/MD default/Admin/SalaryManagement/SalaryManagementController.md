# SalaryManagementController Documentation

## Overview
ASP.NET Core Controller for comprehensive salary management system with payroll processing, absence tracking, and multi-format reporting capabilities.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - `ISalaryManagementService`: Salary business logic
  - `IUnitOfWork`: Database operations
  - `IMapper`: AutoMapper for object mapping
  - `IHubContext<NotificationHub>`: Real-time notifications
  - `INotificationService`: Notification management

## Authentication & Security

### SendOtp
- **Purpose**: Generate and send OTP for report authorization
- **Attributes**: `[IgnoreAction]`, `[NoLogging]`
- **Method**: HTTP POST
- **Response**: JSON with success status
- **Error Handling**: Returns false on exception

## Core Salary Management

### 0. OTP
* OTP Roles `1(Accountant)` --> `2(Manager)` Arranged
* when new Add Salary send Notification to First `1(Accountant)` 2 Reports New {`OpenDetails_payrollReport`,`OpenDetails_DiscountsAndBonusesReport`} by this --> `Month-Year`

* Note: old Prints has Been Depreciated.
* Note: that these two documents run in parallel and do not depend on each other.
* Note: Edit Not Disabled after Any Sign.
* Note: if Data updated No remove old Signs And Not Resend Notification.

#### 0.1 OTP (ValidateOtp_OpenDetails_payrollReportReport `1`) -`OpenDetails_payrollReport`-
* OTP Roles `1(Accountant)` --> `2(Manager)` Arranged
* OTP First sign `1(Accountant)` Then a notification will be sent to the `2(Manager)`
and then Open Sign OTP for `2(Manager)` 
* if `2(Manager)` Validate otp then editing will be disabled for all users and Open ` Print` (OpenDetails_payrollReport `1`).
---
#### 0.2 OTP (ValidateOtp_OpenDetails_DiscountsAndBonusesReport `2`) -`OpenDetails_DiscountsAndBonusesReport`-
* OTP Roles `1(Accountant)` --> `2(Manager)` Arranged
* OTP First sign `1(Accountant)` Then a notification will be sent to the `2(Manager)`
and then Open Sign OTP for `2(Manager)` 
* if `2(Manager)` Validate otp then editing will be disabled for all users and Open ` Print` (OpenDetails_DiscountsAndBonusesReport `2`).
---

### Index
- **Purpose**: Main salary records listing with advanced filtering
- **Parameters**:
  - `selectedEmployee`: Filter by employee ID
  - `selectedYear`: Filter by year
  - `selectedMonth`: Filter by month
  - `page`: Pagination page number (default: 1)
  - `pageSize`: Items per page (default: 50)
- **Features**:
  - Dynamic year dropdown populated from database
  - Employee name dropdown
  - Multi-parameter filtering
  - Pagination support
  - AJAX partial view rendering
- **Sorting**: Primary by EmployeeId, then Year, then Month (all descending)
- **UI Components**:
  - Partial view: `_ListPartial` for AJAX updates
  - ViewBag properties: `allYears`, `selectedYear`, `EmployeesNames`, `selectedEmployee`

### AddEdit (GET)
- **Purpose**: Display form for creating/editing salary records
- **Parameters**: `id` (nullable) - Record ID for editing
- **Features**:
  - Dynamic employee data loading
  - Auto-population of existing data for edits
  - Localization support via `SessionHelper.GetCurrentLanguage()`

### AddEdit (POST)
- **Purpose**: Process salary record creation/updates
- **Validation**: ModelState validation
- **Salary Calculations**:
  - **Total Salary**: Basic + Allowances
  - **Absence Deduction**: (Total Salary / 30) × Absent Days
  - **Annual Vacation Deduction**: (Allowances / 30) × 0.5 × Annual Leave Days
  - **Work Days**: Uses `ReducedWorkDays` or computes as: Work Days - Absent Days - Sick Leave Days - Annual Leave Days
  - **Total Deductions**: Sum of absence, annual vacation, and additional deductions
  - **Net Salary**: Total Salary - Total Deductions + Bonuses
- **Duplicate Prevention**: Checks for existing entries for same month/year
- **Notifications**: Sends real-time notifications to Accountant role for new entries
- **Redirect Logic**:
  - After Create: Redirect to Index
  - After Edit: Redirect back to edit form

### Delete
- **Purpose**: Remove salary records
- **Method**: HTTP POST
- **Security**: Direct service call without additional confirmation
- **Flow**: Service deletion → Redirect to Index

## Advanced Features

### GetAvailableMonths
- **Purpose**: AJAX endpoint for month availability checking
- **Attributes**: `[IgnoreAction]`
- **Logic**: Prevents duplicate salary entries for same employee/year/month
- **Returns**: JSON list of available months (1-12) with localized month names
- **Response Format**: Array of objects with `value` (month number) and `text` (month name)

### GetEmployeeJobAndSalary
- **Purpose**: AJAX endpoint for employee data retrieval
- **Attributes**: `[IgnoreAction]`
- **Parameters**: `employeeId` - Employee identifier
- **Data Returned**:
  - JobTitle from employee record
  - Basic salary (Salary field)
- **Use Case**: Auto-populate form fields during salary entry
- **Error Handling**: Returns BadRequest for invalid ID, NotFound for non-existent employee

## Reporting System

### Payroll Reports

#### OpenDetails_payrollReport
- **Purpose**: Comprehensive payroll report with signature validation
- **Attributes**: `[YesGet]`
- **Parameters**: Same filtering as Index (employee, year, month, pagination)
- **Features**:
  - Dynamic title generation with localized month names
  - Employee-specific report titles
  - Signature validation for Accountant and Manager roles
  - Redirects to Index if no data found
- **Data Structure**: Returns `SalaryReportVM` containing:
  - Filtered salary records
  - Signature information for the report period
  - Month/Year context

#### ValidateOtp_OpenDetails_payrollReport
- **Purpose**: OTP validation for payroll report authorization
- **Attributes**: `[IgnoreAction]`
- **Method**: HTTP POST with JSON body
- **Request Model**: `OtpValidationRequest` with Year, Month, Code, Role
- **Process**:
  1. Validates OTP via service method
  2. Sends notifications to Manager role when Accountant signs
  3. Returns JSON success/error response
- **Notification**: Triggers notifications for report approval workflow

### Discounts & Bonuses Reports

#### OpenDetails_DiscountsAndBonusesReport
- **Purpose**: Focused report on financial adjustments
- **Attributes**: `[YesGet]`
- **Scope**: Deductions, bonuses, and net salary analysis
- **Features**:
  - Same filtering as payroll reports
  - Signature validation support
  - Dynamic title generation
- **Use Case**: Financial analysis and auditing of salary adjustments

#### ValidateOtp_OpenDetails_DiscountsAndBonusesReport
- **Purpose**: OTP validation for discounts/bonuses report
- **Attributes**: `[IgnoreAction]`
- **Method**: HTTP POST
- **Process**: Similar to payroll report validation with specific report type check
- **Notification**: Sends approval notifications to Manager role

### Excel Export Functions

#### createExcelReport_Download_payrollReport
- **Purpose**: Excel export for payroll data
- **Attributes**: `[IgnoreAction]`, `[YesGet]`
- **Columns**: 12 comprehensive fields:
  1. Name (localized)
  2. Job Title
  3. Basic Salary
  4. Allowances
  5. Total Salary
  6. Work Days
  7. Absent Days
  8. Sick Leave Days
  9. Annual Leave Days
  10. Total Deductions
  11. Bonuses and Missions
  12. Net Salary
- **Localization**: Bilingual column headers and data
- **Custom Title**: Dynamic report title in Excel file
- **File Naming**: Includes timestamp for uniqueness

#### createExcelReport_Download_DiscountsAndBonusesReport
- **Purpose**: Excel export for financial adjustments
- **Attributes**: `[IgnoreAction]`, `[YesGet]`
- **Columns**: 7 key financial fields:
  1. Name (localized)
  2. Job Title
  3. Basic Salary
  4. Allowances
  5. Total Deductions
  6. Bonuses and Missions
  7. Net Salary
- **Focus**: Salary adjustments and net calculations
- **Features**: Same localization and file naming conventions as payroll export

## Absence Management System

### Absences
- **Purpose**: Comprehensive absence tracking and reporting
- **Attributes**: `[YesGet]`
- **Filters**:
  - `empId`: Specific employee
  - `fromYear`/`toYear`: Year range
  - `fromMonth`/`toMonth`: Month range (1-12)
- **Analytics**:
  - Total absence days calculation displayed in ViewBag
  - Multi-parameter filtering with range support
- **UI Features**:
  - Dynamic dropdowns for employees, years, months
  - AJAX partial view support (`_AbsencesListPartial`)
  - Pagination with default 50 items per page
- **Data Sources**:
  - Employees from database
  - Years from salary management service
  - Months as SelectListItem with localized names

### PrintAbsences
- **Purpose**: Printer-friendly absence report
- **Attributes**: `[IgnoreAction]`, `[YesGet]`
- **Features**: Same filtering as main absence view without pagination
- **Data**: Complete absence list with total days calculation

### DownloadAbsencesExcel
- **Purpose**: Excel export for absence data
- **Attributes**: `[IgnoreAction]`, `[YesGet]`
- **Columns**:
  1. Name (localized)
  2. Job Title
  3. Month (localized month name)
  4. Year
  5. Number of Absence Days
- **Summary**: Includes total absence days as last row
- **Localization**: Culture-aware month names and column headers
- **File Naming**: Uses "AbsenceList" with timestamp

## Data Processing & Business Logic

### Salary Calculation Logic
#### Basic Calculations:
Total Salary = Basic Salary + Allowances
Absence Deduction = (Total Salary / 30) × Absent Days
Annual Vacation Deduction = (Allowances / 30) × 0.5 × Annual Leave Days

#### Work Days Computation:
If ReducedWorkDays provided: Use directly
Otherwise: WorkDays - AbsentDays - SickLeaveDays - AnnualLeaveDays
Minimum: 0 days

#### Final Calculations:
Total Deductions = Absence + Annual Vacation + Additional Deductions
Net Salary = Total Salary - Total Deductions + Bonuses
All values rounded to 2 decimal places

--------------------
--------------------

### Dynamic Filter Implementation
```csharp
if (selectedEmployee != null)
{
    salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
        (s.EmployeeId == selectedEmployee)
    );
}
if (selectedYear != null)
{
    salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
        (s.Year == selectedYear)
    );
}
if (selectedMonth != null)
{
    salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
        (s.Month == selectedMonth)
    );
}

## Notes
* Note All old Prints has Been Depreciated {`PrintpayrollReport`,`PrintDiscountsAndBonusesReport`}
