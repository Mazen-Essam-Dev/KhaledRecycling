# Monthly Administrative Report ViewModels

## File Structure
- **MonthlyAdministrativeReportDetailVM**: MonthlyAdministrativeReportDetailVM.cs
- **MonthlyAdministrativeReportVM**: MonthlyAdministrativeReportVM.cs

## Core ViewModels

### MonthlyAdministrativeReportDetailVM
**Purpose**: ViewModel for detailed activity tracking within monthly administrative reports

**Properties**:

#### Activity Information
- `Id`: Primary identifier (int)
- `ActivityStartDate`: Start date of the activity (DateOnly?)
- `ActivityEndDate`: End date of the activity (DateOnly?)
- `ActivityName`: Name/title of the activity (string, max 200)
- `NumberOfParticipants`: Count of participants in activity (int?)
- `Reason`: Purpose or objective of the activity (string, max 500)

### MonthlyAdministrativeReportVM
**Purpose**: Main ViewModel for monthly administrative reports with comprehensive validation and UI support

**Properties**:

#### Basic Information
- `Id`: Primary identifier (int)
- `Date`: Report date period with validation (DateOnly?)
- `TypeText`: Display text for report type (string)
- `Type`: Report type identifier with validation (int?)
- `TypeEnumList`: Dropdown options for report types (IEnumerable<SelectListItem>)
- `AdministrativeDepartment`: Responsible department with validation (string, max 200)
- `ReportTitle`: Title of the administrative report with validation (string, max 200)

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
- `Details`: List of detailed report activities (List<MonthlyAdministrativeReportDetailVM>)
- `ActivitySupervisorSignatureId` | int? | Signature ID of the ActivitySupervisor who approved 
- `ActivitySupervisorSignature` | Signature | ActivitySupervisor's signature details 
- `ManagerSignitureId`: Manager approval signature identifier (int?)
- `ManagerSignature`: Manager signature entity reference

## Validation Features

### Custom Validation Attributes
- `[LocalizedRequired]`: Localized required field validation for multi-language support
- `[MaxLength]`: String length constraints for database optimization
- `[DataType(DataType.Date)]`: Date type specification for UI controls

### UI Integration
- **Dropdown Support**: Enum list for report type selection
- **File Upload**: IFormFile support for image uploads
- **Display Text**: TypeText for user-friendly display
- **Collection Binding**: List structure for dynamic detail management

## Key Features

### Report Management
- **Comprehensive Validation**: Required field validation for critical data
- **Multi-image Support**: Four image upload slots with path storage
- **Dynamic Details**: Flexible activity detail collection

### User Interface
- **Enum Integration**: Dropdown lists for type selection
- **File Handling**: Image upload with path management
- **Form Binding**: Optimized for MVC form submission

### Data Structure
- **Hierarchical Design**: Parent report with child details
- **Pre-initialized Collections**: Default list initialization to prevent null issues
- **Signature Tracking**: Approval workflow integration

## Use Cases

### Administrative Reporting Interface
- Monthly report creation and editing
- Activity detail management
- Image documentation upload
- Report type selection

### Approval Workflow
- Trainer and manager signature tracking
- Report validation and submission
- Multi-level approval processes

### Data Presentation
- User-friendly type displays
- Structured activity reporting
- Comprehensive report documentation