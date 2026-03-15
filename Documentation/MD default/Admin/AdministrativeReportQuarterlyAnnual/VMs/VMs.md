# Administrative Quarterly & Annual Reports Module Documentation

This file documents the ViewModels used for the **Administrative and Activities Quarterly/Annual Reports** in your ASP.NET Core 9 MVC application. It covers purpose, structure, and relationships between the ViewModels.

---

## 1. ActivitiesReportQuarterlyVM

**Namespace:** `FougeraClub.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual`

**Purpose:** Represents the quarterly activities report with detailed months and chart data.

**Properties:**
- `MonthsOfYearsAnnualyWithDetails`: List of `MonthsOfYearsAnnualyWithDetailsVM` for each month in the quarter.
- `PieChart`: `PieChartVM` representing participation statistics visually.

---

## 2. AdministrativeReportQuarterlyAnnualIndexVM

**Purpose:** ViewModel for the index page to display summary counts and available quarters.

**Properties:**
- `Quarters`: List of `SelectListItem` for dropdown selection of quarters.
- `AdministrativeCounts`: List of integers representing counts per administrative category.
- `ActivitiesCounts`: List of integers representing counts per activity category.
- `CollaborativeIsSiggned`: Boolean indicating if collaborative signature exists.

---

## 3. AdministrativeReportQuarterlyAnnualVM

**Purpose:** Detailed ViewModel for creating/editing a quarterly/annual administrative report.

**Properties:**
- `Id`: Report identifier.
- `Date`: Report date.
- `TypeText` / `Type` / `TypeEnumList`: Type description, enum value, and dropdown options.
- `AdministrativeDepartment`: Department name (required, max 200 chars).
- `ReportTitle`: Report title (required, max 200 chars).
- `Image1-4` / `Image1Path-4Path`: Optional images and their saved paths.
- `Details`: List of `MonthlyAdministrativeReportDetailVM` representing child records.
- `TrainerSignitureId` / `ManagerSignitureId` and `TrainerSignature` / `ManagerSignature`: Signatures for verification.

---

## 4. MonthsOfYearsAnnualyActivitiesAndAdministrative

**Purpose:** Combines administrative and activities data for quarterly reporting charts.

**Properties:**
- `model_Activities`: List of monthly activity details.
- `model_Administrative`: List of monthly administrative details.
- `listOfQuarter`: List of quarters.
- `labelsMonths`: List of month names for chart labels.
- `adminstrative`: List of administrative counts per month.
- `activities`: List of activities counts per month.

---

## 5. MonthsOfYearsAnnualyVM

**Purpose:** Represents monthly data within a year for administrative or activity reports.

**Properties:**
- `Id`, `year`, `month`
- `Date`
- `Details`: List of `MonthlyAdministrativeReportDetailVM`.

---

## 6. MonthsOfYearsAnnualyWithDetailsVM

**Purpose:** Monthly data with child details and paging info for reports.

**Properties:**
- `Id`, `year`, `month`, `Type`
- `Date`
- `Details`: IEnumerable of `MonthlyAdministrativeReportDetailVM`
- `CurrentPage`, `PageSize`, `TotalDetails`: Paging information.

---

## 7. OTPRequest

**Purpose:** Represents OTP validation request for signing reports.

**Properties:**
- `Type`: Report type.
- `Quarter`: Selected quarter.
- `Year`: Selected year.
- Inherits from `OtpValidationRequest`.

---

## 8. PieChartVM

**Purpose:** Stores chart data for quarterly participation visualization.

**Properties:**
- `MonthsLables`: Names of months.
- `Month1NoOfParticipations`, `Month2NoOfParticipations`, `Month3NoOfParticipations`: Participation counts for each month in the quarter.

---

## 9. QuartersReportActivitiesDetailsVM

**Purpose:** Stores detailed activities for a specific quarter report.

**Properties:**
- `Id`
- `Type`: QuarterlyReportType enum.
- `Quarter`: QuartersYear enum.
- `Year`
- `ManagerSignitureId` / `ManagerSignature`
- `ActivitiesDetailsVM`: `ActivitiesReportQuarterlyVM` containing monthly details and chart data.

---

## 10. QuartersReportAdminstrativeDetailsVM

**Purpose:** Stores detailed administrative report data for a specific quarter.

**Properties:**
- `Id`, `Type`, `Quarter`, `Year`
- `ManagerSignitureId` / `ManagerSignature`
- `AdminstrativeDetailsVM_List`: List of `MonthsOfYearsAnnualyWithDetailsVM`.

---

## 11. QuartersReportCollaborativeDetailsVM

**Purpose:** Represents collaborative reports combining activities and administrative data.

**Properties:**
- `Id`, `Type`, `Quarter`, `Year`
- `ManagerSignitureId` / `ManagerSignature`
- `MonthsOfYearsAnnualyActivitiesAndAdministrativeVM`: Combined data for chart and table display.

---

## 12. General Notes

- **Child Details:** Most monthly ViewModels include a list of `MonthlyAdministrativeReportDetailVM`.
- **Signatures:** Manager and Trainer signatures are stored using `Signature` entity and linked via `SignitureId`.
- **Charts:** `PieChartVM` is used to visualize monthly participation data per quarter.
- **Enums:** `QuarterlyReportType` and `QuartersYear` are used to classify reports.
- **Validation:** Required fields and max length attributes are applied where needed.
- **Localization:** `LocalizedRequired` is used for error messages.

---

### Summary

This module handles **quarterly and annual reporting** for administrative and activity data:

1. **Monthly Data:** `MonthsOfYearsAnnualyVM`, `MonthsOfYearsAnnualyWithDetailsVM`.
2. **Quarterly Data:** `ActivitiesReportQuarterlyVM`, `QuartersReportActivitiesDetailsVM`, `QuartersReportAdminstrativeDetailsVM`, `QuartersReportCollaborativeDetailsVM`.
3. **Index & Charts:** `AdministrativeReportQuarterlyAnnualIndexVM`, `PieChartVM`.
4. **Signatures & OTP:** `TrainerSignature`, `ManagerSignature`, and `OTPRequest`.

All ViewModels are structured to allow hierarchical display of monthly details, chart visualization, and signature verification for secure reporting.
