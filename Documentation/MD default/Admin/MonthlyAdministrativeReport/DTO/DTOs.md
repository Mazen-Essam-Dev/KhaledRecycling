# Administrative Report Quarterly/Annual DTOs

## File Structure
- **MonthlyAdministrativeReportDetailDTO**: MonthlyAdministrativeReportDetailDTO.cs
- **MonthsOfYearsAnnualyActivitiesAndAdministrativeDTO**: MonthsOfYearsAnnualyActivitiesAndAdministrativeDTO.cs
- **MonthsOfYearsAnnualyDTO**: MonthsOfYearsAnnualyDTO.cs
- **MonthsOfYearsAnnualyWithDetailsDTO**: MonthsOfYearsAnnualyWithDetailsDTO.cs

## Data Transfer Objects

### MonthlyAdministrativeReportDetailDTO
**Purpose**: Data transfer object for individual monthly administrative report details

**Properties**:
- `Id`: Primary identifier (int)
- `ActivityStartDate`: Start date of the activity (DateOnly?)
- `ActivityEndDate`: End date of the activity (DateOnly?)
- `ActivityName`: Name of the activity (string, max 200)
- `NumberOfParticipants`: Count of participants in the activity (int?)
- `Reason`: Purpose or reason for the activity (string, max 500)

### MonthsOfYearsAnnualyActivitiesAndAdministrativeDTO
**Purpose**: Container DTO for annual activities and administrative data with chart support

**Properties**:
- `model_Activities`: List of activity data with details
- `model_Administrative`: List of administrative data with details
- `listOfQuarter`: Quarter identifiers for reporting (List<int>)
- `labelsMonths`: Month labels for chart display (List<string>)
- `adminstrative`: Administrative data points for charts (List<int>)
- `activities`: Activity data points for charts (List<int>)

### MonthsOfYearsAnnualyDTO
**Purpose**: Summary DTO for monthly administrative and activities counts

**Properties**:
- `Id`: Primary identifier (int)
- `year`: Reporting year (int?)
- `month`: Reporting month (int?)
- `Date`: Reference date for the period (DateOnly?)
- `ActivitiesDetailsCount`: Number of activity details for the period (int)
- `AdministrativeDetailsCount`: Number of administrative details for the period (int)

### MonthsOfYearsAnnualyWithDetailsDTO
**Purpose**: Detailed DTO with child entities for comprehensive reporting

**Properties**:
- `Id`: Primary identifier (int)
- `year`: Reporting year (int?)
- `month`: Reporting month (int?)
- `Type`: Type classification for the data (int?)
- `Date`: Reference date for the period (DateOnly?)
- `Details`: Collection of monthly administrative report details (IEnumerable<MonthlyAdministrativeReportDetail>)

## Key Features

### Reporting Structure
- **Monthly Breakdown**: Detailed activity and administrative tracking by month
- **Annual Aggregation**: Yearly summary of monthly data
- **Quarterly Support**: Quarter-based reporting and analysis

### Data Analysis
- **Activity Metrics**: Participant counts and activity durations
- **Administrative Tracking**: Administrative workload measurement
- **Comparative Analysis**: Activities vs Administrative data comparison

### Chart Integration
- **Data Visualization**: Prepared data structures for chart libraries
- **Label Support**: Month labels for user-friendly chart displays
- **Data Points**: Numeric arrays for chart data series

### Temporal Organization
- **Date Range Tracking**: Activity start and end dates
- **Monthly Periods**: Organized by year and month
- **Quarter Grouping**: Support for quarterly reporting cycles

## Use Cases

### Administrative Reporting
- Monthly activity and administrative performance reports
- Quarterly and annual summary reporting
- Comparative analysis between activities and administrative work

### Data Visualization
- Chart and graph data preparation
- Dashboard reporting interfaces
- Performance metric tracking

### Analytics Integration
- Trend analysis across months and years
- Participant engagement metrics
- Resource allocation planning