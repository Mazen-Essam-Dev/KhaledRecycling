# Participations In Event Report ViewModels

## File Structure
- **ParticipationsInEventReportDetailVM**: ParticipationsInEventReportDetailVM.cs
- **ParticipationsInEventReportVM**: ParticipationsInEventReportVM.cs

## Core ViewModels

### ParticipationsInEventReportDetailVM
**Purpose**: ViewModel for detailed activity tracking within event participation reports

**Properties**:

#### Identification
- `Id`: Primary key identifier (int)
- `ParticipationsInEventReportId`: Foreign key to parent report (int)

#### Activity Information
- `ActivityName`: Name/title of the activity (string, max 200)
- `ActivityAction`: Specific action or task performed (string, max 200)
- `ActivityReason`: Purpose or objective of the activity (string, max 500)

### ParticipationsInEventReportVM
**Purpose**: Main ViewModel for event participation reports with comprehensive documentation and approval workflow

**Properties**:

#### Basic Information
- `Id`: Primary identifier (int)
- `Date`: Report date period with validation (DateOnly?)
- `AdministrativeDepartment`: Responsible department with validation (string, max 200)
- `ReportTitle`: Title of the event participation report with validation (string, max 200)

#### Image Documentation
- `Image1`: First supporting image upload (IFormFile)
- `Image1Path`: Storage path for first image (string, max 300)
- `Image2`: Second supporting image upload (IFormFile)
- `Image2Path`: Storage path for second image (string, max 300)
- `Image3`: Third supporting image upload (IFormFile)
- `Image3Path`: Storage path for third image (string, max 300)
- `Image4`: Fourth supporting image upload (IFormFile)
- `Image4Path`: Storage path for fourth image (string, max 300)

#### Report Details & Approvals
- `Details`: List of detailed event participation activities (List<ParticipationsInEventReportDetailVM>)
- `TrainerSignitureId`: Trainer approval signature identifier (int?)
- `TrainerSignature`: Trainer signature entity reference
- `ManagerSignitureId`: Manager approval signature identifier (int?)
- `ManagerSignature`: Manager signature entity reference

## Validation Features

### Custom Validation Attributes
- `[LocalizedRequired]`: Localized required field validation for multi-language support
- `[MaxLength]`: String length constraints for database optimization
- `[DataType(DataType.Date)]`: Date type specification for UI controls

### UI Integration
- **File Upload**: IFormFile support for multiple image uploads
- **Collection Binding**: List structure for dynamic activity detail management
- **Form Optimization**: Pre-initialized collections to prevent null issues

## Key Features

### Event Participation Tracking
- **Activity Focus**: Detailed tracking of event activities and actions
- **Action Documentation**: Specific activity actions with reasons
- **Department Reporting**: Administrative department responsibility tracking

### Documentation & Evidence
- **Multi-image Support**: Four image upload slots for event documentation
- **Visual Evidence**: Image path storage for event photos and evidence
- **Comprehensive Reporting**: Combined text and visual documentation

### Approval Workflow
- **Dual Approval System**: Trainer and manager signature requirements
- **Signature Integration**: Digital signature tracking for approvals
- **Audit Trail**: Clear approval chain documentation

### Data Structure
- **Hierarchical Design**: Parent report with multiple activity details
- **Flexible Details**: Dynamic activity collection for various event types
- **Relationship Management**: Strong parent-child relationship enforcement

## Use Cases

### Event Reporting Interface
- Event participation report creation and management
- Activity and action documentation for events
- Visual evidence collection for event activities
- Department-level event reporting

### Approval Processes
- Trainer review and approval of event reports
- Manager validation and sign-off
- Multi-level event participation verification

### Event Documentation
- Comprehensive event activity tracking
- Action and reason documentation
- Visual evidence management
- Department accountability tracking