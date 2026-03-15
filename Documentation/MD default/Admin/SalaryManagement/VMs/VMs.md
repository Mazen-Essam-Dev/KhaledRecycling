# Salary Management ViewModels

## File Structure
- **AbsenceVM**: AbsenceVM.cs
- **SalaryManagementVM**: SalaryManagementVM.cs

## Core ViewModels

### AbsenceVM
**Purpose**: ViewModel for employee absence tracking and reporting

**Properties**:

#### Employee Information
- `Id`: Primary identifier for absence record (int)
- `EmpId`: Employee identifier (int?)
- `FullNameAr`: Arabic full name of employee (string)
- `FullNameEn`: English full name of employee (string)
- `JobTitle`: Employee's job title (string)

#### Absence Details
- `Month`: Month of absence (int?)
- `Year`: Year of absence (int?)
- `NoOfDays`: Number of absence days (int?)

### SalaryManagementVM
**Purpose**: Comprehensive ViewModel for employee salary management with detailed calculations

**Properties**:

#### Basic Information
- `Id`: Primary identifier (int)
- `EmployeeId`: Employee reference with validation (int) - required
- `Employee`: Navigation to Employee entity
- `Year`: Salary year with validation (int) - required
- `Month`: Salary month with range validation (int) - defaults to 0

#### Sign Information
| `AccountantSignatureId` | int? | Signature ID of the accountant who approved |
| `AccountantSignature` | Signature | Accountant's signature details |
| `ManagerSignatureId` | int? | Signature ID of the manager who approved |
| `ManagerSignature` | Signature | Manager's signature details |

#### Sign Details
- `JobTitle`: Employee's job title (string, max 150)

#### Employee Details
- `JobTitle`: Employee's job title (string, max 150)

#### Salary Components
- `BasicSalary`: Base salary amount (decimal?)
- `Allowances`: Additional allowances amount (decimal?)
- `TotalSalary`: Gross salary total (decimal?)

#### Attendance & Leave Tracking
- `WorkDays`: Number of work days with validation (int?) - defaults to 30
- `AbsentDays`: Number of absent days with validation (int?)
- `SickLeaveDays`: Number of sick leave days with validation (int?)
- `AnnualLeaveDays`: Number of annual leave days with validation (int?)

#### Financial Adjustments
- `DeductionsAddition`: Additional deductions amount (decimal?)
- `TotalDeductions`: Total deductions amount (decimal?)
- `BonusesAndMissions`: Bonuses and mission allowances (decimal?)

#### Final Calculations
- `NetSalary`: Final net salary after all adjustments (decimal?)
- `CreatedAt`: Record creation timestamp (DateTime) - defaults to current Dubai time

## Validation Features

### Custom Validation Attributes
- `[LocalizedRequired]`: Localized required field validation
- `[LocalizedMaxLength]`: Localized maximum length validation
- `[Range]`: Numeric range validation with resource-based error messages
- `[ForeignKey]`: Explicit foreign key relationships

### Business Rule Validation
- **Month Validation**: 1-12 range for valid months
- **Days Validation**: 0-31 range for all day-based fields
- **Resource Integration**: Uses Resource1 for localized error messages

## Key Features

### Comprehensive Salary Management
- **Multi-component Salary**: Basic salary, allowances, and bonuses
- **Leave Tracking**: Sick leave, annual leave, and absence management
- **Deduction System**: Flexible deductions and additions

### Attendance Integration
- **Work Day Calculation**: Default 30-day work month
- **Leave Type Separation**: Distinct tracking for different leave types
- **Attendance Impact**: Direct impact on salary calculations

### Financial Calculations
- **Gross Salary**: Basic + Allowances = TotalSalary
- **Net Salary**: TotalSalary + Bonuses - Deductions = NetSalary
- **Decimal Precision**: 18,2 decimal precision for financial accuracy

### Audit & Compliance
- **Timestamp Tracking**: Automatic creation date recording
- **Range Validation**: Ensures data integrity for days and months
- **Localization Support**: Arabic-compatible validation messages

## Use Cases

### Salary Processing
- Monthly salary calculation and management
- Employee compensation processing
- Leave and attendance impact on salary
- Bonus and deduction application

### Reporting & Analytics
- Salary history and trends
- Attendance impact analysis
- Leave utilization reporting
- Financial compliance reporting

### Employee Management
- Job title and role tracking
- Salary component breakdown
- Leave balance management
- Performance-based compensation