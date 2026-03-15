# Salary Management Entities — Full Documentation (Names + Types, No Code)

This file explains the salary-related entities involved in salary calculation, reporting, and approval workflows.  
Each section lists the **object name, property names, property types, and their real meaning** — without any code.

---

## 1) SalaryReportSign (Salary Report Signing/Approval Record)

This entity tracks the **approval process** for salary reports by managers and accountants.

| Property Name | Type | Meaning |
|--------------|------|--------|
| Id | int | Unique record identifier |
| ReportSalaryTypeId | int? | Links to the type of salary report (e.g., monthly, special) |
| ReportSalaryType | ReportSalaryType | The specific salary report type object |
| Month | int? | The month the salary report covers (1-12) |
| Year | int? | The year the salary report covers |
| AccountantSignatureId | int? | Signature ID of the accountant who approved |
| AccountantSignature | Signature | Accountant's signature details |
| ManagerSignatureId | int? | Signature ID of the manager who approved |
| ManagerSignature | Signature | Manager's signature details |

**Purpose:** Provides an audit trail of who approved salary reports and when, ensuring proper financial controls.

---

## 2) SalaryManagement (Employee Salary Record)

This is the **main salary calculation entity** that stores monthly salary details for each employee.

| Property Name | Type | Meaning |
|--------------|------|--------|
| Id | int | Unique salary record identifier |
| EmployeeId | int | Links to the employee receiving this salary |
| Employee | Employee | Employee details and information |
| Year | int | Year of salary calculation |
| Month | int | Month of salary calculation (1-12) |
| JobTitle | string? | Employee's job title during this period |
| BasicSalary | decimal? | Base salary amount |
| Allowances | decimal? | Additional allowances (housing, transportation, etc.) |
| TotalSalary | decimal? | Basic + Allowances before deductions |
| WorkDays | int | Total working days in the month |
| AbsentDays | int? | Days the employee was absent |
| SickLeaveDays | int? | Days taken as sick leave |
| AnnualLeaveDays | int? | Days taken as annual leave |
| DeductionsAddition | decimal? | Additional deductions beyond standard |
| TotalDeductions | decimal? | Sum of all deductions |
| BonusesAndMissions | decimal? | Extra bonuses and mission allowances |
| NetSalary | decimal? | Final payable amount (Total - Deductions + Bonuses) |
| CreatedAt | DateTime | Timestamp when this record was created |

**Purpose:** Comprehensive monthly salary calculation and storage for each employee, including all components, deductions, and leave calculations.

---

## Workflow Summary

| Step | Entity | Purpose |
|-----|--------|---------|
| 1 | SalaryManagement | Calculate and store monthly salary details for each employee |
| 2 | SalaryReportSign | Obtain required approvals from accountant and manager |
| 3 | ReportSalaryType | Classify salary reports (monthly, bonus, adjustment, etc.) |
| 4 | Signature Entities | Record who approved the salary reports |

---

## Key Relationships

1. **SalaryManagement → Employee** - Each salary record belongs to one employee
2. **SalaryReportSign → ReportSalaryType** - Each signed report has a specific type
3. **SalaryReportSign → Signature** - Multiple approval signatures per report
4. **SalaryManagement → Year/Month** - Organized by period for reporting

---

## Business Rules

- Salary calculations are **monthly based**
- Multiple approval levels required (Accountant → Manager)
- Comprehensive tracking of leaves, bonuses, and deductions
- Audit trail through signature tracking
- Time-stamped creation for version control

---

### Final Notes

- This structure ensures **accurate payroll processing** with proper approvals
- It supports **financial auditing, compliance, and reporting**
- Every salary payment is **documented, calculated, and approved** before processing
- Flexible enough to handle different salary structures and bonus schemes

---