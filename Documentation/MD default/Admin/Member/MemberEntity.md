# Member Entity

## MemberEntity

**Namespace**: `Domain.Entities`

**Purpose**: Core entity for managing member information with comprehensive personal, contact, and document details

### Properties

#### Basic Information
- `Id`: Primary key identifier (int)
- `Code`: Unique member code/number (int?)

#### Personal Details
- `FullNameAr`: Arabic full name (string, max 100)
- `FullNameEn`: English full name (string, max 100)
- `NationalityId`: Foreign key to nationality (int?)
- `Nationality`: Navigation to Nationality entity
- `GenderId`: Gender identifier (int?)

#### Identity & Demographics
- `IdNumber`: National identification number (string, max 50)
- `IdExpiryDate`: ID expiration date (DateOnly?)
- `DateOfBirth`: Birth date (DateOnly?)
- `Age`: Calculated age (int?, max 10)

#### Contact Information
- `PhoneNumber`: Primary contact number (string, max 20)
- `FatherPhone`: Father's contact number (string, max 20)
- `MotherPhone`: Mother's contact number (string, max 20)

#### Education & Profession
- `AcademicQualification`: Educational qualification (string, max 100)
- `EducationInstitution`: Educational institution (string, max 100)
- `ProfessionId`: Profession identifier (int?)
- `CityId`: City identifier (int?)
- `Profession`: Custom profession field (string, max 50)
- `GuardianProfession`: Guardian's profession (string, max 50)

#### Location & Interests
- `Address`: Residential address (string, max 200)
- `Hobby`: Member hobbies and interests (string)
- `Languages`: Known languages (string)

#### Registration Details
- `HeardBy`: Information source identifier (int?)
- `License`: License agreement acceptance (bool?)

#### Social Media
- `Facebook`: Facebook profile URL (string, max 100)
- `Xplatform`: X/Twitter profile URL (string, max 100)
- `Instagram`: Instagram profile URL (string, max 100)

#### Account Security
- `Email`: Email address (string, max 100)
- `Password`: Hashed password (string, max 500)

#### Document Management
- `ProfileImagePath`: Profile photo storage path (string, max 300)
- `ProfileImage`: Profile photo upload interface (IFormFile - NotMapped)
- `IdImagePath`: ID document storage path (string, max 300)
- `IdImage`: ID document upload interface (IFormFile - NotMapped)
- `PassportImagePath`: Passport storage path (string, max 300)
- `PassportImage`: Passport upload interface (IFormFile - NotMapped)

#### Relationships & Status
- `Subscriptions`: Collection of course subscriptions
- `RegistrationDate`: Member registration date (DateOnly) - defaults to current date
- `Suspended`: Account suspension status (bool) - defaults to false

### Data Annotations

#### Database Optimization
- `[Key]`: Primary key identification
- `[MaxLength]`: String length constraints for database efficiency
- `[Column(TypeName = "date")]`: Specific database date type
- `[Column("XPlatform")]`: Custom column name mapping
- `[NotMapped]`: Excludes file uploads from database mapping
- `[DataType(DataType.Date)]`: Date type specification

#### Default Values
- `RegistrationDate`: Automatically set to current Dubai time
- `Suspended`: Defaults to false for new members

### Key Features

#### Comprehensive Member Management
- **Multi-language Support**: Arabic and English name fields
- **Identity Verification**: ID number and expiration tracking
- **Age Management**: Birth date and calculated age
- **Contact Hierarchy**: Primary, father, and mother contacts

#### Document & Media Handling
- **File Upload Support**: Profile, ID, and passport images
- **Path Storage**: Secure file path storage in database
- **Database Separation**: File content stored externally

#### Relationship Management
- **Course Subscriptions**: Track member course enrollments
- **Geographic Data**: City and nationality relationships
- **Social Integration**: Social media profile tracking

#### Security & Compliance
- **Password Hashing**: Secure password storage
- **License Agreement**: Legal compliance tracking
- **Suspension System**: Member account management

### Use Cases
- Member registration and profile management
- Course enrollment and subscription tracking
- Document verification and management
- Member communication and contact management
- Account security and access control