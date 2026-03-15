# Maintenance Club Entity

## MaintenanceClub

**Namespace**: `Domain.Entities`

**Purpose**: Entity for managing club maintenance activities with scheduling and documentation support

### Properties

#### Basic Information
- `Id`: Primary key identifier for maintenance record (int)

#### Maintenance Details
- `Title`: Maintenance title with Arabic display (string, max 100)
  - **Display**: "عنوان الصيانة" (Maintenance Title)

- `Date`: Maintenance date with Arabic display (DateTime?)
  - **DataType**: Date storage optimized
  - **Display**: "تاريخ الصيانة" (Maintenance Date)

- `Time`: Maintenance time with Arabic display (TimeSpan?)
  - **DataType**: Time storage optimized
  - **Display**: "وقت الصيانة" (Maintenance Time)

- `Location`: Maintenance location with Arabic display (string, max 200)
  - **Display**: "مكان الصيانة" (Maintenance Location)

- `Details`: Detailed maintenance description with Arabic display (string, max 2000)
  - **Display**: "تفاصيل الصيانة" (Maintenance Details)

#### File Management
- `PdfFilePath`: Stored PDF file path for maintenance documentation (string)
  - **DataType**: Upload control compatible
  - **Display**: "ملف PDF" (PDF File)

- `PdfFile`: PDF file upload interface (IFormFile - NotMapped)

### Data Annotations

#### Database Optimization
- `[StringLength]`: Maximum length constraints for database efficiency
- `[DataType]`: Specific data type handling (Date, Time, Upload)
- `[NotMapped]`: Excludes file upload from database mapping

#### Localization
- `[Display]`: Arabic display names for UI localization

### Key Features

#### Maintenance Tracking
- **Comprehensive Scheduling**: Separate date and time tracking
- **Location Management**: Specific maintenance location storage
- **Detailed Documentation**: Extensive details field for maintenance descriptions

#### File Management Architecture
- **Database Separation**: File path storage in database, file content in storage system
- **PDF Documentation**: Dedicated PDF file support for maintenance reports
- **Upload Handling**: IFormFile interface for web file uploads

#### Localization Support
- **Arabic Interface**: All display names in Arabic
- **User-Friendly**: Arabic terminology for end users
- **Consistent Naming**: Standardized across the application

### Database Design
- **Efficient Storage**: Optimized string lengths and data types
- **File Management**: Separate file storage from database records
- **Scalable Structure**: Supports multiple maintenance records

### Use Cases
- Club facility maintenance scheduling and history
- Maintenance work documentation and reporting
- PDF attachment storage for maintenance records
- Maintenance audit trails and compliance
- Arabic-language maintenance management system