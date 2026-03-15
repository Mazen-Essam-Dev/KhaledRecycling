# Salary Management DTOs

## File Structure
- **AbsenceDTO**: AbsenceDTO.cs
- **EmployeesNameDTO**: EmployeesNameDTO.cs

## Data Transfer Objects

### AbsenceDTO
**Purpose**: Data transfer object for employee absence tracking and reporting

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

### EmployeesNameDTO
**Purpose**: Data transfer object for employee basic information with salary details

**Properties**:

#### Employee Identification
- `Id`: Primary employee identifier (int)
- `FullNameAr`: Arabic full name of employee (string)
- `FullNameEn`: English full name of employee (string)
- `JobTitle`: Employee's job title (string)

#### Salary Information
- `BasicSalary`: Employee's base salary amount (double?)

## Key Features

### Absence Management
- **Temporal Tracking**: Month and year based absence recording
- **Duration Measurement**: Number of days absence tracking
- **Employee Context**: Complete employee information with names and job titles

### Employee Data
- **Multi-language Support**: Arabic and English name fields
- **Job Information**: Job title for employee context
- **Salary Integration**: Basic salary reference for calculations

### Data Optimization
- **Lightweight Structure**: Minimal data for efficient transfer
- **API Friendly**: Simple structure for REST API responses
- **Serialization Ready**: Clean properties for JSON/XML serialization

## Use Cases

### Absence Reporting
- Employee absence tracking and reporting
- Monthly absence summaries
- Attendance management systems
- Payroll deduction calculations

### Employee Directory
- Employee basic information lookup
- Salary management interfaces
- Job title and role management
- Multi-language employee data display

### Integration Scenarios
- **API Responses**: Clean data structures for frontend applications
- **Report Generation**: Formatted data for absence and salary reports
- **Data Export**: Simplified employee data for external systems
- **Quick Lookups**: Essential employee information for search functions