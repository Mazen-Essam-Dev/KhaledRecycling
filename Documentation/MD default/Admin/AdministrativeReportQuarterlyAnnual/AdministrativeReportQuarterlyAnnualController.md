# AdministrativeReportQuarterlyAnnualController Documentation

## Overview
ASP.NET Core Controller for managing quarterly and annual administrative reports with activity tracking and signature verification.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**: 
  - IAdministrativeReportQuarterlyAnnualService
  - IMapper (AutoMapper)

## Action Methods

### Index
- **Purpose**: Main dashboard for quarterly/annual reports
- **Parameters**: `selectedYear` (optional)
- **Features**:
  - Year dropdown population
  - Quarterly data aggregation
  - Activity and administrative counts calculation
  - Collaborative report signature checking
  - AJAX partial view support

### All2Details
- **Purpose**: Combined administrative and activities details view
- **Parameters**: `selectedYear`, `quarter`, pagination
- **Logic**:
  - Delegates heavy processing to service layer
  - Handles signature verification for collaborative reports
  - Returns appropriate view models with mapped data

### AdminstrativeDetails
- **Purpose**: Administrative-only reports view
- **Parameters**: `selectedYear`, `quarter`, pagination
- **Features**:
  - Quarterly data filtering (Q1-Q4)
  - Annual summary (quarter = 5)
  - Administrative report type filtering
  - Signature verification integration

### ActivitiesDetails
- **Purpose**: Activities-focused reports with chart data
- **Parameters**: `selectedYear`, `quarter`, pagination
- **Special Features**:
  - Pie chart data generation
  - Monthly participation statistics
  - Activity duration calculations
  - Quarter-based month grouping

## Print Methods
- **PrintActivitiesDetails**: Printable version of activities report
- **PrintAdminstrativeDetails**: Printable administrative reports
- **PrintAll2Details**: Printable combined reports
- **Attributes**: `[IgnoreAction]` for route exclusion

## Signature & OTP System

### OTP Operations
- **SendOtp**: Generates and sends OTP for specific roles (trainer/manager)
- **ValidateOtp**: Verifies OTP codes for report signing
- **Security**: `[NoLogging]` attribute on sensitive operations

### Signature Integration
- Signature checking across all detail views
- Role-based authorization for signing
- Quarterly report type differentiation:
  - Collaborative
  - Administrative  
  - Activities

## Data Processing

### Quarter Handling
```csharp
List<List<int>> quarters = new() {
    new List<int>{1,2,3},    // Q1
    new List<int>{4,5,6},    // Q2  
    new List<int>{7,8,9},    // Q3
    new List<int>{10,11,12}  // Q4
};