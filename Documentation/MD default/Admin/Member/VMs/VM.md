# Member Management ViewModels

## File Structure
- **MemberCoursesVM**: MemberCoursesVM.cs
- **MemberVM**: MemberVM.cs

## Core ViewModels

### MemberCoursesVM
**Purpose**: Container for member course enrollment with pagination support

**Properties**:
- `Paginated`: Paginated list of courses available to member (PaginatedList<CourseVM>)
- `memberId`: Member identifier for course filtering (int?)

### MemberVM
**Purpose**: Comprehensive member registration and management with advanced validation

**Properties**:

#### Basic Information
- `HasCourses`: Flag indicating if member has enrolled courses (bool)
- `Id`: Primary member identifier (int)
- `Code`: Member code/number (int?)

#### Personal Details
- `FullNameAr`: Arabic full name with regex validation (string, max 100)
- `FullNameEn`: English full name with regex validation (string, max 100)
- `NationalityId`: Nationality identifier with validation (int?)
- `Nationality`: Navigation to Nationality entity
- `NationalitiesList`: Nationality dropdown options

#### Identity & Demographics
- `GenderId`: Gender identifier with validation (int?)
- `Genders`: Gender options select list
- `SelectedGenderEnum`: Selected gender enumeration
- `GenderEnumList`: Gender enumeration dropdown
- `IdNumber`: National ID number with unique validation (string, length 18)
- `IdExpiryDate`: ID expiration date (DateOnly?)
- `DateOfBirth`: Birth date with past-date validation (DateOnly?)
- `Age`: Calculated age with validation (int?)

#### Contact Information
- `PhoneNumber`: Primary phone with unique validation (string, max 20)
- `FatherPhone`: Father's contact number (string, max 20)
- `MotherPhone`: Mother's contact number (string, max 20)

#### Education & Profession
- `AcademicQualification`: Educational qualification (string, max 100)
- `EducationInstitution`: Educational institution (string, max 100)
- `ProfessionId`: Profession identifier (int?)
- `Professions`: Profession options select list
- `SelectedProfessionEnum`: Selected profession enumeration
- `ProfessionEnumList`: Profession enumeration dropdown
- `Profession`: Custom profession field (string, max 50)
- `GuardianProfession`: Guardian's profession (string, max 50)

#### Location & Interests
- `CityId`: City identifier with validation (int?)
- `City`: Navigation to City entity
- `CitiesList`: City dropdown options
- `Address`: Residential address (string, max 200)
- `Hobby`: Member hobbies (string, max 500)
- `Languages`: Known languages (string, max 500)

#### Registration Details
- `HeardBy`: Information source identifier (int?)
- `HeardByOptions`: Source options select list
- `SelectedHeardByEnum`: Selected source enumeration
- `HeardByEnumList`: Source enumeration dropdown
- `License`: License agreement acceptance (bool?)

#### Social Media
- `Facebook`: Facebook profile (string, max 100)
- `Xplatform`: X/Twitter profile (string, max 100)
- `Instagram`: Instagram profile (string, max 100)

#### Account Security
- `Email`: Email address with unique validation (string, max 100)
- `Password`: Password with strong regex validation (string)
- `ConfirmPassword`: Password confirmation with compare validation (string)

#### Document Management
- `ProfileImagePath`: Profile photo storage path (string, max 300)
- `ProfileImage`: Profile photo upload (IFormFile)
- `IdImagePath`: ID document storage path (string, max 300)
- `IdImage`: ID document upload (IFormFile)
- `PassportImagePath`: Passport storage path (string, max 300)
- `PassportImage`: Passport upload (IFormFile)

#### Status & Dates
- `RegistrationDate`: Member registration date (DateOnly)
- `Suspended`: Account suspension status (bool)

## Validation Features

### Custom Validation Attributes
- `[LocalizedRequired]`: Localized required field validation
- `[LocalizedMaxLength]`: Localized maximum length validation
- `[LocalizedMinLength]`: Localized minimum length validation
- `[Unique]`: Database uniqueness validation
- `[InThePast]`: Past date validation
- `[RegularExpression]`: Pattern validation for names and passwords

### Business Rule Validation
- **ID Expiry Check**: ID must not be expired
- **Age Restriction**: Minimum age of 9 years required
- **Password Strength**: Complex password requirements
- **Email Uniqueness**: Unique email across system
- **Phone Uniqueness**: Unique phone number validation

### Multi-language Support
- Arabic and English error messages
- Culture-aware validation based on session language
- Localized resource files for validation messages

## Security Features
- Strong password policy enforcement
- Email uniqueness prevention
- Phone number uniqueness
- Document upload security
- Age verification system