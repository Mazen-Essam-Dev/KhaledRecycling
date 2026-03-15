# Maintenance Club ViewModel

## MaintenanceClubVM

**Namespace**: `FougeraClub.Areas.Admin.ViewModels.MaintenanceClub`

**Purpose**: ViewModel for managing club maintenance activities with comprehensive scheduling and documentation support

### Properties

#### Basic Information
- `Id`: Primary identifier for maintenance record (int)

#### Maintenance Details
- `Title`: Maintenance title with validation and Arabic display (string, max 100)
  - **Validation**: Required field with localized messages
  - **Display**: "عنوان الصيانة" (Maintenance Title)

- `Date`: Maintenance date with validation and Arabic display (DateTime?)
  - **Validation**: Required field
  - **DataType**: Date picker compatible
  - **Display**: "تاريخ الصيانة" (Maintenance Date)

- `Time`: Maintenance time with validation and Arabic display (TimeSpan?)
  - **Validation**: Required field
  - **DataType**: Time picker compatible
  - **Display**: "وقت الصيانة" (Maintenance Time)

- `Location`: Maintenance location with validation and Arabic display (string, max 200)
  - **Validation**: Required field
  - **Display**: "مكان الصيانة" (Maintenance Location)

- `Details`: Detailed maintenance description with Arabic display (string, max 2000)
  - **Display**: "تفاصيل الصيانة" (Maintenance Details)

#### File Management
- `PdfFile`: PDF file upload for maintenance documentation (IFormFile)
  - **DataType**: Upload control compatible
  - **Display**: "ملف PDF" (PDF File)

- `PdfFilePath`: Stored PDF file path (string, max 200)

### Validation Features

#### Custom Validation
- `[LocalizedRequired]`: Localized required field validation for Arabic and English
- `[StringLength]`: Maximum length constraints for string fields

#### Data Annotations
- `[DataType]`: Specific data type handling (Date, Time, Upload)
- `[Display]`: Arabic display names for UI localization

### Key Features

#### Maintenance Scheduling
- **Complete Scheduling**: Date and time tracking for precise scheduling
- **Location Tracking**: Specific maintenance location information
- **Detailed Documentation**: Comprehensive details field for maintenance work

#### File Management
- **PDF Support**: Dedicated PDF file upload for maintenance documentation
- **Path Storage**: Secure file path storage for uploaded documents
- **Upload Integration**: IFormFile support for web file uploads

#### Localization
- **Arabic Interface**: All display names in Arabic for user-friendly interface
- **Localized Validation**: Culture-aware validation messages
- **Consistent Naming**: Standardized Arabic terminology across properties

### Use Cases
- Club facility maintenance scheduling and tracking
- Maintenance work documentation and record keeping
- PDF report attachment for maintenance activities
- Maintenance history and audit trails
- Multi-language support for Arabic-speaking users