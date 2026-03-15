# Member Entity Documentation

## Entity Overview
- **Namespace**: `Domain.Entities`
- **Purpose**: Represents club member data with personal, contact, and authentication information
- **Database Table**: `MemberEntity` (auto-generated name)
- **Recent Change**: Entity name standardized from generic `Member` to `MemberEntity` for clarity and consistency with domain naming conventions

---

## Primary Key & Identification

### Identification Fields
- `Id`: Primary key (auto-increment)
- `Code`: Unique member code (nullable integer)
- **Recent Enhancement**: Used for internal member identification and reference across multiple systems

---

## Personal Information

### Name Fields
- `FullNameAr`: Arabic full name (max 100 characters)
- `FullNameEn`: English full name (max 100 characters)
- **Design Decision**: Supports bilingual naming for international applications and Middle Eastern audiences

### Demographic Data
- `NationalityId`: Foreign key to Nationality entity
- `GenderId`: Gender classification identifier
- `DateOfBirth`: Birth date (DateOnly type)
- `Age`: Calculated age (max 10 characters)
- `IdNumber`: National identification number (max 50 characters)
- `IdExpiryDate`: ID card expiration date
- **Schema Update**: Added `Nationality` navigation property for direct access to related data

---

## Contact Information

### Phone Numbers
- `PhoneNumber`: Primary contact number (max 20 characters)
- `FatherPhone`: Father's contact number (max 20 characters)
- `MotherPhone`: Mother's contact number (max 20 characters)
- **Business Requirement**: Supports family contact information for emergency notifications

### Email & Social Media
- `Email`: Primary email address (max 100 characters)
- `Facebook`: Facebook profile URL (max 100 characters)
- `Xplatform`: X (Twitter) platform handle (max 100 characters)
- `Instagram`: Instagram profile URL (max 100 characters)
- **Compatibility Update**: Added explicit `[Column("XPlatform")]` attribute for SQL Server database compatibility

---

## Education & Professional

### Academic Information
- `AcademicQualification`: Highest education level (max 100 characters)
- `EducationInstitution`: School/University name (max 100 characters)
- `Profession`: Current profession (max 50 characters)
- `GuardianProfession`: Parent/guardian profession (max 50 characters)

### Location Data
- `CityId`: Foreign key to City entity
- `Address`: Physical address (max 200 characters)
- **Future Enhancement**: Potential for adding navigation property to City entity

---

## Personal Preferences

### Interests & Languages
- `Hobby`: Member hobbies and interests (unlimited length)
- `Languages`: Known languages (unlimited length)
- `HeardBy`: How member heard about club (integer reference)

### Additional Information
- `License`: Boolean indicating license possession
- **Validation Strategy**: Free-form fields for additional member context with appropriate length limits

---

## Authentication & Security

### Login Credentials
- `Email`: Used as username for authentication
- `Password`: Hashed password (max 500 characters for hash storage)
- **Security Design**: Email serves dual purpose for communication and login, reducing duplicate data

---

## Image Management

### Profile Images
- `ProfileImagePath`: Server path to profile photo (max 300 characters)
- `ProfileImage`: Form file for upload (not mapped to database)
- `IdImagePath`: Server path to ID card image (max 300 characters)
- `IdImage`: Form file for ID card upload (not mapped)
- `PassportImagePath`: Server path to passport image (max 300 characters)
- `PassportImage`: Form file for passport upload (not mapped)

### File Handling Strategy
- **Architecture Decision**: `[NotMapped]` attributes exclude Form files from database persistence
- **Performance Consideration**: Only file paths stored in database, not binary data
- **Separation of Concerns**: Upload handling separated from entity storage for better scalability
- **Integration Point**: Works with `FileHelper` utilities and temporary file handling in controllers

---

## Relationships & Navigation

### Entity Relationships
- `Nationality`: Virtual navigation to Nationality entity
- `Subscriptions`: Collection of member subscription records
- **Relationship Design**: One-to-Many relationship with subscriptions enables activity tracking

### Collection Initialization
- `Subscriptions`: Auto-initialized as empty list
- **Preventive Measure**: Prevents null reference exceptions and supports LINQ operations

---

## System Management

### Registration & Status
- `RegistrationDate`: Auto-set to current Dubai time using `AppDubaiTime1.Now`
- `Suspended`: Account suspension flag (default: false)
- **Automation Enhancement**: Automatic timestamp ensures consistent registration tracking

---

## Data Annotations & Constraints

### Validation Attributes
- `[MaxLength]`: String field length restrictions prevent database bloat
- `[Key]`: Primary key identification for Entity Framework
- `[Column]`: Custom column naming and type specification for database compatibility
- `[NotMapped]`: Exclusion from database mapping for file uploads
- `[DataType]`: Semantic data type information for UI generation

### Database Mapping Strategy
- **Type Selection**: `DateOnly` types for date-specific fields without time component
- **Naming Convention**: Custom column names for database compatibility and consistency
- **Internationalization**: Appropriate data types and lengths for global applications

---

## Business Logic Support

### Multi-language Ready
- **Design Feature**: Arabic/English name support with separate fields
- **Format Handling**: International phone number formatting support
- **Address System**: Global address handling with city reference

### Membership Management
- **Tracking Capability**: Subscription tracking through collection property
- **Status Monitoring**: Account suspension flag for administrative control
- **Timeline Tracking**: Registration date with automatic Dubai timezone

### Security Considerations
- **Storage Capacity**: 500 characters allocated for password hash storage
- **File Separation**: Image file handling separated from entity persistence
- **Data Protection**: Sensitive data length limitations for ID numbers and contact info

---

## Architecture & Design Patterns

### Entity Framework Integration
- **Lazy Loading**: Navigation properties marked as `virtual` for EF Core lazy loading
- **Indexing Strategy**: Primary key on `Id`, with potential indexes on `Email` and `Code`
- **Query Efficiency**: Proper relationships prevent N+1 query problems

### Nullable Reference Types
- **Modern Pattern**: Strategic use of nullable types for optional fields
- **Clarity**: Clear distinction between required and optional data fields
- **Compatibility**: Alignment with modern .NET nullable reference type patterns

### Migration History
1. **Initial Version**: Basic member fields only
2. **Version 1.1**: Added image path fields and subscription relationship
3. **Version 1.2**: Added social media fields and enhanced contact info
4. **Current Version**: Added registration date, suspension flag, and navigation properties

---

## Integration Points

### Authentication System
- **Username Strategy**: `Email` field serves as username for authentication
- **Password Storage**: `Password` stores hashed credentials using ASP.NET Core patterns
- **Security Integration**: Compatible with identity management systems

### Subscription Management
- **Activity Tracking**: One-to-Many relationship with `Subscription` entity
- **Multi-subscription**: Supports multiple course/event subscriptions per member
- **Data Relationship**: Enables comprehensive member activity analysis

### File Upload System
- **Utility Integration**: Works with `FileHelper` utilities for file operations
- **Temporary Handling**: Supports temporary file handling during upload processes
- **Path Resolution**: File path management handled in controller layer

---

## Validation Rules

### Required Fields
- `Email`: Required for authentication system
- `FullNameAr/En`: Required for member identification

### Format Validation
- **Email Format**: Validated through UI/backend validation
- **Phone Patterns**: Phone number validation handled in presentation layer
- **Date Validation**: Business rules for age and expiry date checking

### Business Rules
- **Age Validation**: Minimum/maximum age requirements
- **ID Expiry**: ID card expiry date monitoring
- **Uniqueness**: `Email` uniqueness enforced at database level

---

## Future Enhancement Opportunities

### Audit Trail
- **Tracking Fields**: Potential addition of `CreatedBy`, `ModifiedBy`, `ModifiedDate`
- **Change History**: Audit logging for compliance and debugging

### Soft Delete
- **Data Preservation**: `IsDeleted` flag instead of physical deletion
- **Recovery Option**: Enable data recovery and historical analysis

### Concurrency Control
- **Optimistic Concurrency**: Add row version/timestamp fields
- **Conflict Resolution**: Handle concurrent updates effectively

### Additional Relationships
- **Entity Links**: Potential navigation to `City`, `Profession` entities
- **Enhanced Data**: More detailed member profiling

### Localization Support
- **Language Expansion**: Additional language-specific fields as needed
- **Regional Adaptation**: Custom fields for different geographical regions

---

## Code Structure Reference

```csharp
public class MemberEntity
{
    // Identification
    [Key] public int Id { get; set; }
    public int? Code { get; set; }
    
    // Personal Information
    [MaxLength(100)] public string? FullNameAr { get; set; }
    [MaxLength(100)] public string? FullNameEn { get; set; }
    
    // Relationships
    public int? NationalityId { get; set; }
    public virtual Nationality? Nationality { get; set; }
    public virtual ICollection<Subscription> Subscriptions { get; set; } = new List<Subscription>();
    
    // Authentication
    [MaxLength(100)] public string? Email { get; set; }
    [MaxLength(500)] public string? Password { get; set; }
    
    // File Management (NotMapped)
    [NotMapped] public IFormFile? ProfileImage { get; set; }
    [MaxLength(300)] public string? ProfileImagePath { get; set; }
    
    // System Fields
    public DateOnly RegistrationDate { get; set; } = DateOnly.FromDateTime(AppDubaiTime1.Now);
    public bool Suspended { get; set; } = false;
}