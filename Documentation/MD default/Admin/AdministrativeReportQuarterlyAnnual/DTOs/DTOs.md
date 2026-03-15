# Quarterly & Annual Administrative Reports (DTOs) — Explanation

This document explains the purpose of the Data Transfer Objects (DTOs) used for Quarterly and Annual Administrative Reports in the system. These DTOs are used to move structured data between layers (Controller → Service → UI / API) without exposing the database entities directly.

---

## 1. MonthlyAdministrativeReportDetailDTO

This object represents the details for a single administrative activity within a month.

### What it Represents
- Each record describes **one activity** during the month.
- Used in tables, forms, and summaries.

### Key Fields
- `Id`: Unique identifier.
- `ActivityStartDate` and `ActivityEndDate`: The duration of the activity.
- `ActivityName`: Name or title of the activity.
- `NumberOfParticipants`: How many people took part.
- `Reason`: Additional explanation or notes.

### Why It Matters
This DTO is the building block for reports. Multiple details form the report content.

---

## 2. MonthsOfYearsAnnualyActivitiesAndAdministrativeDTO

This object is used to compare **Activity Reports** and **Administrative Reports** within a selected quarter or year.

### What it Represents
- A combined model for **side-by-side comparison**.
- Used when displaying **quarterly analytics** or **annual summaries**.

### Key Fields
- `model_Activities`: List of monthly reports for Activities.
- `model_Administrative`: List of monthly reports for Administrative work.
- `listOfQuarter`: The months in the quarter (example: Q1 = 1,2,3).
- `labelsMonths`: Month names for charts.
- `adminstrative`: Count result per month for administrative reports.
- `activities`: Count result per month for activities.

### Why It Matters
Enables comparing **two report categories** in charts or dashboards.

---

## 3. MonthsOfYearsAnnualyDTO

This DTO provides **summary-level information** for monthly data without loading full details.

### What it Represents
- One record for one month.
- Useful for lists, overviews, and dashboards.

### Key Fields
- `year` / `month`: Identifies the month.
- `Date`: The month as a date value.
- `ActivitiesDetailsCount`: How many activities took place.
- `AdministrativeDetailsCount`: How many administrative items were recorded.

### Why It Matters
Great for displaying **"Month Summary Cards"** or **reports lists** efficiently.

---

## 4. MonthsOfYearsAnnualyWithDetailsDTO

This DTO includes the **full details** of the month's records.

### What it Represents
- A month plus its included related items.
- This is essentially a **full data object** for printing or review.

### Key Fields
- `year` / `month`: Identify the month.
- `Type`: Indicates whether the details relate to Activities or Administrative work.
- `Date`: The month.
- `Details`: A full list of MonthlyAdministrativeReportDetail items.

### Why It Matters
Used in:
- Report PDF generation
- Detailed review pages
- Pagination displays

---

# Summary

| DTO Name | Level | Contains Details? | Primary Use |
|---------|-------|------------------|-------------|
| MonthlyAdministrativeReportDetailDTO | Item level | Yes | Represents one recorded activity |
| MonthsOfYearsAnnualyDTO | Monthly summary | No | Dashboard counts and summaries |
| MonthsOfYearsAnnualyWithDetailsDTO | Monthly detailed | Yes | Full reporting and printing |
| MonthsOfYearsAnnualyActivitiesAndAdministrativeDTO | Quarterly/Annual comparison | Yes | Charts, side-by-side analysis |

---


