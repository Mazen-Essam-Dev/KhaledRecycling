# Monthly Administrative Report Entities

## File Structure
- **MonthlyAdministrativeReport**: MonthlyAdministrativeReport.cs
- **MonthlyAdministrativeReportDetail**: MonthlyAdministrativeReportDetail.cs

## Core Entities

### MonthlyAdministrativeReport
**Purpose**: Main entity for monthly administrative reports with comprehensive documentation and approval workflow

**Properties**:

#### Basic Information
- `Id`: Primary key identifier (int)
- `Date`: Report date period (DateOnly?)
- `Type`: Type of administrative report (MonthlyAdministrativeReportType enum)
- `AdministrativeDepartment`: Responsible department (string, max 200)
- `ReportTitle`: Title of the administrative report (string, max 200)

#### Image Documentation
- `Image1`: First supporting image upload (IFormFile - NotMapped)
- `Image1Path`: Storage path for first image (string, max 300)
- `Image2`: Second supporting image upload (IFormFile - NotMapped)
- `Image2Path`: Storage path for second image (string, max 300)
- `Image3`: Third supporting image upload (IFormFile - NotMapped)
- `Image3Path`: Storage path for third image (string, max 300)
- `Image4`: Fourth supporting image upload (IFormFile - NotMapped)
- `Image4Path`: Storage path for fourth image (string, max 300)

#### Report Details & Approvals
- `Details`: Collection of detailed report activities (ICollection<MonthlyAdministrativeReportDetail>)
- `ActivitySupervisorSignatureId` | int? | Signature ID of the ActivitySupervisor who approved
- `ActivitySupervisorSignature` | Signature | ActivitySupervisor's signature details 
- `ManagerSignitureId`: Manager approval signature identifier (int?)
- `ManagerSignature`: Navigation to manager signature entity

### MonthlyAdministrativeReportDetail
**Purpose**: Detailed activity tracking within monthly administrative reports

**Properties**:

#### Identification
- `Id`: Primary key identifier (int)
- `MonthlyAdministrativeReportId`: Foreign key to parent report (int)
- `MonthlyAdministrativeReport`: Navigation to parent report entity

#### Activity Information
- `ActivityStartDate`: Start date of the activity (DateOnly?)
- `ActivityEndDate`: End date of the activity (DateOnly?)
- `ActivityName`: Name/title of the activity (string, max 200)
- `NumberOfParticipants`: Count of participants in activity (int?)
- `Reason`: Purpose or objective of the activity (string, max 500)

## Key Features

### Report Management
- **Comprehensive Documentation**: Support for multiple images and detailed activities
- **Temporal Tracking**: Activity date ranges and report periods
- **Department Organization**: Administrative department categorization

### Approval Workflow
- **Dual Approval System**: Trainer and manager signature requirements
- **Signature Tracking**: Digital signature integration for approvals
- **Audit Trail**: Clear approval chain documentation

### File Management
- **Multiple Image Support**: Up to four supporting images per report
- **Database Separation**: File paths stored, files managed externally
- **Upload Interface**: IFormFile support for web uploads

### Data Integrity
- **Relationship Management**: Strong parent-child relationship between report and details
- **Collection Initialization**: Pre-initialized collections to avoid null issues
- **Data Annotations**: Comprehensive validation and database optimization

## Data Annotations

### Database Optimization
- `[Key]`: Primary key identification
- `[Column(TypeName = "date")]`: Specific date type for database storage
- `[MaxLength]`: String length constraints for database efficiency
- `[NotMapped]`: Excludes file uploads from database mapping
- `[ForeignKey]`: Explicit foreign key relationships

## Use Cases

### Administrative Reporting
- Monthly activity and performance reporting
- Department-level administrative tracking
- Activity participation and impact measurement

### Documentation & Compliance
- Visual documentation through image attachments
- Approval workflow compliance
- Audit and verification processes

### Data Analysis
- Activity frequency and participation trends
- Department performance metrics
- Resource allocation and planning