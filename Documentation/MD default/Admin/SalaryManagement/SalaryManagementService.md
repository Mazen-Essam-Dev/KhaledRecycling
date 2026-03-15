# SalaryManagementService Documentation

## Overview
Service layer implementation for comprehensive salary management system with employee integration, absence tracking, and file attachment handling.

## Dependencies
- **IUnitOfWork**: Database repository access and transaction management
- **IWebHostEnvironment**: File system access for salary-related attachments

## Core Salary Operations

### ValidateOtp_OpenDetails_payrollReportAsync()
- **Purpose**: Validate OTP for Salary Report to Accountant And Manager
- **Data Structure**: if ok update this Salary Record with new Sign

### ValidateOtp_OpenDetails_DiscountsAndBonusesReportAsync()
- **Purpose**: Validate OTP for DiscountsAndBonuses Report to Accountant And Manager
- **Data Structure**: if ok update this Salary Record with new Sign

### GetAllAsync()
- **Purpose**: Retrieves all salary management records
- **Returns**: `IEnumerable<SalaryManagement>`
- **Implementation**: Direct delegation to repository

### GetByIdAsync(int id)
- **Purpose**: Fetches specific salary record by identifier
- **Returns**: `SalaryManagement?` entity

### AddAsync(SalaryManagement entity)
- **Purpose**: Creates new salary record
- **Process**: Entity addition with transaction commit
- **Returns**: Integer ID of created salary record

### UpdateAsync(SalaryManagement entity)
- **Purpose**: Modifies existing salary record
- **Process**: Entity fetch → Value update → Transaction commit
- **Safety**: Null-check prevents errors on missing entities

### DeleteAsync(int id)
- **Purpose**: Removes salary record
- **Process**: Entity fetch → Database removal → Transaction commit
- **Safety**: Null-check prevents deletion attempts on non-existent entities

## Employee Data Management

### GetAllEmplyeeNames()
- **Purpose**: Retrieves comprehensive employee data for salary management
- **Data Structure**: `EmployeesNameDTO` with complete employee profile
- **Includes**:
  - Bilingual names (Arabic/English)
  - Job title information
  - Basic salary data
- **Use Case**: Dropdown population and salary form auto-population

## Absence Tracking System

### GetAllAbsencesAsync()
- **Purpose**: Generates comprehensive absence reporting data
- **Data Sources**: Salary management records with employee relationships
- **Data Structure**: `AbsenceDTO` with absence analytics
- **Includes**:
  - Employee identification and names
  - Job title information
  - Monthly absence tracking
  - Year-based organization

### Absence Data Model
```csharp
var absences = allSalaries.Select(x => new AbsenceDTO
{
    Id = x.Id,
    EmpId = x.EmployeeId,
    FullNameAr = x.Employee?.FullNameAr,
    FullNameEn = x.Employee?.FullNameEn,
    JobTitle = x.Employee?.JobTitle,
    Month = x.Month,
    Year = x.Year,
    NoOfDays = x.AbsentDays
});