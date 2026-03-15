# Participations In Event Report Entities

## File Structure
- **ParticipationsInEventReport**: ParticipationsInEventReport.cs
- **ParticipationsInEventReportDetail**: ParticipationsInEventReportDetail.cs

## Core Entities

### ParticipationsInEventReport
**Purpose**: Main entity for event participation reports with comprehensive documentation and dual-approval workflow

**Properties**:

#### Basic Information
- `Id`: Primary key identifier (int)
- `Date`: Report date period (DateOnly?)
- `AdministrativeDepartment`: Responsible department for the event (string, max 200)
- `ReportTitle`: Title of the event participation report (string, max 200)

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
- `Details`: Collection of detailed event participation activities (ICollection<ParticipationsInEventReportDetail>)
- `TrainerSignitureId`: Trainer approval signature identifier (int?)
- `TrainerSignature`: Navigation to trainer signature entity
- `ManagerSignitureId`: Manager approval signature identifier (int?)
- `ManagerSignature`: Navigation to manager signature entity

### ParticipationsInEventReportDetail
**Purpose**: Detailed activity tracking within event participation reports

**Properties**:

#### Identification
- `Id`: Primary key identifier (int)
- `ParticipationsInEventReportId`: Foreign key to parent report (int)
- `ParticipationsInEventReport`: Navigation to parent report entity

#### Activity Information
- `ActivityName`: Name/title of the activity (string, max 200)
- `ActivityAction`: Specific action or task performed during the activity (string, max 200)
- `ActivityReason`: Purpose or objective of the activity (string, max 500)

## Key Features

### Event Participation Tracking
- **Activity Documentation**: Comprehensive tracking of event activities and actions
- **Action-Reason Mapping**: Clear connection between activities and their purposes
- **Department Accountability**: Administrative department responsibility tracking

### Documentation & Evidence
- **Multi-image Support**: Four image upload slots for visual event documentation
- **Database Separation**: File paths stored in database, files managed externally
- **Visual Evidence**: Support for event photos, screenshots, and documentation

### Approval Workflow
- **Dual Approval System**: Requires both trainer and manager signatures
- **Signature Integration**: Digital signature tracking for compliance
- **Audit Trail**: Clear documentation of approval chain

### Data Integrity
- **Strong Relationships**: Enforced parent-child relationship between report and details
- **Collection Initialization**: Pre-initialized collections to prevent null reference issues
- **Data Annotations**: Comprehensive validation and database optimization

## Data Annotations

### Database Optimization
- `[Key]`: Primary key identification
- `[Column(TypeName = "date")]`: Specific date type for database storage
- `[MaxLength]`: String length constraints for database efficiency
- `[NotMapped]`: Excludes file uploads from database mapping
- `[ForeignKey]`: Explicit foreign key relationships

## Use Cases

### Event Management
- Event participation tracking and reporting
- Activity documentation for events and programs
- Department-level event accountability

### Compliance & Verification
- Visual evidence collection for event participation
- Approval workflow compliance for event reports
- Audit trail maintenance for event activities

### Reporting & Analysis
- Event activity frequency and type analysis
- Department participation metrics
- Event impact and outcome measurement