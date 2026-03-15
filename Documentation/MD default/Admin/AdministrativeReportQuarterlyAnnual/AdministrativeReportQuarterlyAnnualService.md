# AdministrativeReportQuarterlyAnnualService Documentation

## Overview
Service layer implementation for handling quarterly and annual administrative reports, including data aggregation, OTP verification, and signature management.

## Dependencies
- **IUnitOfWork**: Database operations and repository access
- **IWebHostEnvironment**: File system and environment access
- **IHttpContextAccessor**: HTTP context and user session access

## Core Methods

### Data Retrieval Methods

#### GetAllAsync()
- **Purpose**: Retrieves all monthly administrative reports
- **Returns**: `IEnumerable<MonthlyAdministrativeReport>`

#### GetByIdAsync(int id)
- **Purpose**: Gets specific report by ID with details
- **Includes**: Details navigation property
- **Returns**: `MonthlyAdministrativeReport?`

### Yearly Data Aggregation

#### GetAllMonthsofYearsAnnualy(int? year)
- **Purpose**: Gets monthly counts for activities and administrative reports
- **Filters**: By specified year
- **Returns**: DTO with activity/administrative counts per month

#### GetAllMonthsofYearsAnnualy_Data(int? year)
- **Purpose**: Retrieves detailed monthly data with full details
- **Returns**: DTO with complete report details including child collections

#### GetAllYearsInDb()
- **Purpose**: Extracts distinct years from all reports
- **Process**: Distinct year extraction with ordering
- **Returns**: Ordered list of years

### Quarterly Report Processing

#### GetAdministrativeAndActivitiesDetailsAsync(int selectedYear, int quarter, string lang)
- **Purpose**: Comprehensive quarterly data aggregation
- **Quarter Handling**:
  - Q1-Q4: Standard quarters (3 months each)
  - Quarter 5: Full year aggregation
- **Output**: Combined DTO with:
  - Administrative reports
  - Activity reports
  - Chart labels and data
  - Monthly counts for visualization

### Signature Management

#### CheckCollaborativeReportIsSiggned(int? year)
- **Purpose**: Verifies if collaborative annual report is signed
- **Checks**: Manager signature existence and image path
- **Returns**: Boolean indicating signed status

#### GetQuarterSignature(int? selectedYear, QuartersYear? quarter, QuarterlyReportType? ActionType)
- **Purpose**: Retrieves signature information for specific quarter reports
- **Includes**: Manager signature navigation property
- **Returns**: QuartersReportDTO with signature details

## OTP & Security System

### SendOtpAsync(int id, string role)
- **Purpose**: Initiates OTP process for report signing
- **Implementation**: Uses OTPHelper for OTP generation and storage
- **Returns**: Success status

### ValidateOtpAsync(int type, int quarter, int year, string code, string role, ClaimsPrincipal user)
- **Process**:
  1. Validates OTP code
  2. Retrieves user's latest signature
  3. Creates new quarters report record
  4. Links manager signature
- **Security**: User ID extraction from claims
- **Returns**: Tuple with success status and message

## Data Transfer Objects

### MonthsOfYearsAnnualyDTO
- Report metadata with counts
- Year/month extraction from dates
- Activity vs administrative categorization

### MonthsOfYearsAnnualyWithDetailsDTO
- Extended DTO with full detail collections
- Type categorization for filtering

### MonthsOfYearsAnnualyActivitiesAndAdministrativeDTO
- Comprehensive quarterly data container
- Chart-ready data structures
- Separated administrative and activity collections

## Business Logic

### Quarter Definition
```csharp
List<List<int>> quarters = new() {
    new List<int>{1,2,3},    // Q1: Jan-Mar
    new List<int>{4,5,6},    // Q2: Apr-Jun
    new List<int>{7,8,9},    // Q3: Jul-Sep
    new List<int>{10,11,12}  // Q4: Oct-Dec
};