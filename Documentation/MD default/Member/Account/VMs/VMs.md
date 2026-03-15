# Member View Models Documentation

## Overview
Three main View Models for member management:
- **MemberRegisterVM**: New member registration with comprehensive validation
- **MemberEditVM**: Member profile editing with business rule validation  
- **MemberVM**: General member data representation with display capabilities
- **MemberLoginVM**: Simple authentication model

## Common Features

### Shared Properties
All main VMs inherit similar structure with:
- Personal identification (ID, Code, Names)
- Contact information (Phone, Email, Address)
- Demographic data (Nationality, Gender, Date of Birth)
- Professional information (Education, Profession)
- Image management (Profile, ID, Passport images)
- Custom validation via `IValidatableObject`

### Validation Framework
- **Localized Validation**: Multi-language error messages from Resource files
- **Custom Attributes**: `LocalizedRequired`, `LocalizedMaxLength`, `InThePast`, `NotInThePast`
- **Regular Expressions**: Arabic/English name patterns, ID number format
- **Business Rules**: Age restrictions, ID expiry validation

## MemberRegisterVM

### Purpose
New member registration with strict validation and duplicate prevention

### Unique Registration Features

#### Identity Validation
- **ID Number Format**: `\b\d{3}-\d{4}-\d{7}-\d\b` (18-character UAE format)
- **Unique Constraints**: 
  - `IdNumber`: Prevents duplicate national IDs
  - `PhoneNumber`: Ensures unique phone registration
  - `Email`: Prevents duplicate email accounts

#### Password Security
- **Strong Password Policy**:
  - Minimum 8 characters
  - At least one uppercase letter
  - At least one lowercase letter  
  - At least one digit
  - At least one special character (@$!#%*?&)
- **Confirmation**: Password match validation

#### ID Expiry Validation
- **Future Date Requirement**: `NotInThePast` attribute for ID expiry
- **Business Logic**: Ensures valid identification documents

### Component Fields
- `g1, g2, g3, g4`: ID number segment storage
- `ConfirmPassword`: Password verification field
- `Unique` attributes for data integrity

## MemberEditVM

### Purpose
Member profile editing with maintained data integrity

### Edit-Specific Features

#### Selective Validation
- **Maintained Data**: No unique constraints on existing data
- **Business Rules**: Age and expiry validation preserved
- **Image Management**: Optional image updates

#### Streamlined Properties
- No password confirmation requirements
- Maintains existing member data relationships
- Focuses on updatable fields only

## MemberVM  

### Purpose
General member data representation for display and management

### Display Features

#### Navigation Properties
- `Nationality`: Full nationality entity for display
- `City`: Complete city information
- `NationalitiesList`, `CitiesList`: Dropdown data sources

#### Comprehensive Data
- Registration timeline tracking
- Account status monitoring
- Full relationship support for UI display

### Validation Inheritance
- Maintains all business rule validations
- Supports both create and edit scenarios
- Comprehensive error messaging

## MemberLoginVM

### Purpose
Simple authentication model
- `Email`: Login identifier
- `Password`: Authentication credential
- Minimal validation for login efficiency

## Validation System

### Custom Validation Attributes

#### Localized Validation
- `LocalizedRequired`: Culture-aware required field validation
- `LocalizedMaxLength`: Language-specific length restrictions
- `LocalizedMinLength`: Minimum length with localization

#### Business Rule Attributes
- `InThePast`: Ensures dates are before current date
- `NotInThePast`: Ensures dates are in the future
- `Unique`: Database-level uniqueness validation

### Custom Business Logic Validation

#### IValidatableObject Implementation
All main VMs implement custom validation:

**ID Expiry Validation**
- Checks if ID expiration date has passed
- Compares against current Dubai time
- Bilingual error messages (Arabic/English)

**Age Restriction Validation**  
- Ensures members are at least 9 years old
- Calculates age from date of birth
- Prevents underage registrations

#### Date Handling
- Uses `AppDubaiTime.Now` for consistent timezone
- `AtMidnight()` for date comparison accuracy
- `DateOnly` to `DateTime` conversion for validation

## Data Annotations & Constraints

### String Length Limits
- **Names**: 100 characters max
- **ID Numbers**: 18 characters exact
- **Phone Numbers**: 20 characters max  
- **Address**: 200 characters max
- **Hobby/Languages**: 500 characters max
- **Social Media**: 100 characters max
- **Password**: 500 characters max (hash storage)

### Format Validation
- **Arabic Names**: Only Arabic letters, numbers, spaces
- **English Names**: Only English letters, numbers, spaces  
- **Phone Numbers**: International phone format validation
- **Email**: Standard email format with uniqueness
- **ID Numbers**: Specific UAE ID card format

## Internationalization Support

### Multi-language Resources
- **Error Messages**: From Resource1 and Resource2
- **Field Labels**: Localized display names
- **Validation Messages**: Culture-specific feedback

### Bilingual Content
- Arabic and English name support
- Language-specific regular expressions
- Session-based language detection for validation messages

## Image Management

### File Upload Support
- **ProfileImage**: Member photograph
- **IdImage**: National ID card document
- **PassportImage**: Passport document
- **Path Storage**: Server path strings for persistence

### Validation
- File type checking (via separate service)
- Size limitations
- Optional updates in edit scenarios

## Security Features

### Password Policy
- Strong complexity requirements
- Secure hashing in service layer
- Confirmation validation for registration

### Data Protection
- Unique constraints on sensitive data
- Format validation for all inputs
- Business rule enforcement

## Usage Context

### MemberRegisterVM
- **Registration forms**
- **New account creation**
- **Strict initial validation**

### MemberEditVM  
- **Profile management**
- **Account updates**
- **Maintained data integrity**

### MemberVM
- **Display purposes**
- **Administrative views**
- **Comprehensive data representation**

### MemberLoginVM
- **Authentication flows**
- **Simple credential validation**