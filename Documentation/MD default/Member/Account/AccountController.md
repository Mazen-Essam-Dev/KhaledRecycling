# Account Controller Documentation

## Controller Overview
- **Area**: Member
- **Route**: `/Member/Account/[action]`
- **Purpose**: Handles member authentication, registration, profile management, and document processing
- **Design Pattern**: Follows MVC architecture with service layer abstraction for business logic separation

---

## Dependencies & Services

### Injected Services
- `IAccountService`: Member account operations including registration and authentication
- `IUnitOfWork`: Database operations and repository access using Unit of Work pattern
- `IHttpContextAccessor`: HTTP context and session management for user state tracking
- `IMapper`: AutoMapper for object-object mapping between DTOs, ViewModels, and Entities
- `IMemberService`: Member-specific business logic and data operations
- `IOCRService`: Optical Character Recognition for ID document processing and data extraction
- `IWebHostEnvironment`: Web server environment configuration and file path resolution

### Configuration
- **Image Save Path**: `wwwroot/uploads/members` directory for member image storage
- **Recent Update**: Enhanced path management with web root environment awareness

---

## Core Methods

### IDClassification_Json
**Purpose**: AI-powered ID card classification and data extraction

#### Process Flow
1. **File Validation**: Checks for valid uploaded file with null and size validation
2. **AI Classification**: Uses ML model (`IDClassificationMLModel`) to identify ID card type
3. **Confidence Check**: Validates prediction confidence with 90% threshold requirement
4. **OCR Processing**: Extracts text data from ID card image using `IOCRService`
5. **Temporary File Cleanup**: Removes processed temporary image files to prevent storage bloat
6. **Response Mapping**: Returns extracted data with comprehensive success status indicators

#### Response Structure
- `doneAI_bool`: AI classification success flag (true/false)
- `doneOCR_bool`: Text extraction success flag (true/false)
- `lable`: Document type identification (e.g., "ID", "PASSPORT")
- `ProbabilityString`: Confidence percentage as formatted string
- `DoneTextExtracted_Error_Str`: Processing status message with localization support

#### Recent Enhancements
- **Error Handling**: Comprehensive try-catch with meaningful error responses
- **File Management**: Proper cleanup of temporary OCR-processed files
- **Resource Integration**: Localized error messages using `Resource1` for multi-language support

---

### Register
**Purpose**: Member registration with comprehensive validation and AI document verification

#### Registration Process
1. **Initial Setup**: Populates dropdowns (gender, profession, nationality, cities) using `SelectListHelper`
2. **Code Generation**: Creates unique member code via `_memberService.GenerateNewCode()`
3. **Image Management**: Handles profile, ID, and passport images with session-based temporary storage
4. **Data Validation**: Comprehensive form and image validation with model state checking
5. **AI Verification**: Optional ID data extraction and comparison with input data (currently commented)
6. **Account Creation**: Saves member to database with proper password hashing
7. **Auto-Login**: Automatically logs in successful registrations for seamless onboarding

#### Image Handling Features
- **Session Storage**: Temporary image storage during registration using HTTP session
- **File Validation**: 3MB size limit and image format checking via `FileHelper`
- **Path Management**: Moves images from session to permanent storage using `_accountService`
- **Duplicate Prevention**: Ensures ID and passport images are different files with content comparison
- **Recent Update**: Enhanced image reconstruction from stored paths using `IFormFile` recreation

#### Phone Number Processing
- **Format Standardization**: Converts to UAE format (+971) for consistency
- **Whitespace Removal**: Cleans input phone numbers before processing
- **International Prefix**: Ensures proper country code formatting using `PhoneHelper`
- **Validation Logic**: Handles both local and international phone number formats

#### Session-Based Image Management
- **Temporary Storage**: Images stored in session during form completion
- **Path Reconstruction**: Recreates `IFormFile` objects from stored paths for validation
- **Auto-Cleanup**: Session data removal post-registration to prevent memory leaks

---

### Login & Logout
**Purpose**: Member authentication and session management with security considerations

#### Login Process
- **Credential Validation**: Email and password verification via `_accountService.ValidateUserAsync()`
- **Account Status Check**: Validates non-suspended accounts before authentication
- **Session Management**: Creates authentication token and stores email in session
- **Security**: Anti-forgery token validation and secure session handling

#### Session Features
- **Auth Token**: GUID-based session authentication for security
- **Email Storage**: Session-based user identification for subsequent requests
- **Auto-Redirect**: Prevents logged-in users from accessing login page
- **Session Clear**: Complete session clearing on logout for security

#### Recent Security Enhancements
- **Token Generation**: GUID-based authentication tokens for session security
- **Suspended Account Check**: Prevents login for suspended accounts
- **Session Management**: Proper session clearing on logout to prevent session fixation

---

### Edit Profile
**Purpose**: Member profile updates with comprehensive image and data management

#### Edit Process
1. **Session Validation**: Retrieves member from session email to ensure authorization
2. **Data Binding**: Populates form with existing member data using AutoMapper
3. **Image Validation**: Checks new image uploads with size and format validation
4. **Phone Formatting**: Standardizes phone number format for consistency
5. **Profile Update**: Saves changes to database with proper entity tracking

#### Security Measures
- **Session-based Access**: Prevents editing other profiles by verifying session ownership
- **Image Size Limits**: 3MB maximum per image with format validation
- **Form Validation**: Comprehensive model state validation before processing
- **Password Handling**: Separate password and confirmation fields for security

#### Data Management Updates
- **Select List Population**: Dynamic dropdown population based on current data
- **Phone Format Consistency**: Maintains consistent phone number formatting
- **Image Path Preservation**: Retains existing image paths when no new uploads
- **Entity Mapping**: Clean mapping between ViewModel and Entity objects

---

### PrintDetails
**Purpose**: Generates printable member details view for administrative purposes

#### Features
- **Member Retrieval**: Fetches member by ID with proper null checking
- **Dropdown Population**: Loads all related data lists (nationalities, cities, professions)
- **View Preparation**: Formats data for print-friendly display with proper layout
- **Data Binding**: Comprehensive ViewModel population with related entity data

#### Recent Improvements
- **ID Parameter Handling**: Proper null checking and validation for member ID
- **Data Ordering**: City list ordering by ID for consistent presentation
- **Distinct Values**: Profession, gender, and heard-by sources with distinct values
- **ViewBag Management**: Clean separation of display data from main ViewModel

---

## Validation & Security

### Image Validation
- **Size Limit**: 3MB per image file with proper error messaging
- **Format Check**: Valid image format verification using MIME type detection
- **Duplicate Detection**: Prevents identical ID/passport images with content comparison
- **AI Verification**: Optional ID data cross-validation for document authenticity
- **Recent Enhancement**: Added `FileHelper.CheckFileIsImage_3Mg_Async()` for async validation

### Business Rules
- **Email Uniqueness**: Prevents duplicate email registration with database-level checking
- **ID Number Uniqueness**: Ensures unique national identification across members
- **Phone Formatting**: Standardizes UAE phone numbers with +971 prefix
- **Account Suspension**: Blocks suspended account access with proper user feedback
- **Session Management**: Temporary image storage during multi-step registration

### Session Management
- **Temporary Storage**: Image paths during registration process
- **Authentication Tokens**: GUID-based session security with token generation
- **Auto-Cleanup**: Removes temporary data post-registration to prevent data leakage
- **Email Tracking**: Session-based user identification for authenticated requests
- **Recent Update**: Added session data removal in registration success flow

---

## Error Handling

### Registration Errors
- `EmailUsedBefore`: Duplicate email address with localized error message
- `IdNationalNumber_UsedBefore`: Duplicate ID number with proper user guidance
- `EmailUsedBefore_&&_IdNationalNumber_UsedBefore`: Both email and ID duplicates with combined error

### Login Errors
- `NotExisting`: Email not found in system with security-conscious messaging
- `NotMatchingBassword`: Incorrect password with generic error for security
- `AccountSuspended`: Member account suspended with administrative guidance

### Image Processing Errors
- **File Size Exceeded**: Clear messaging for 3MB limit violations
- **Invalid Image Format**: Specific format requirements guidance
- **AI Classification Failures**: Detailed error messages for document recognition failures
- **OCR Extraction Errors**: Specific guidance for image quality improvements

### Recent Error Handling Improvements
- **Localized Messages**: All errors use `Resource1` for multi-language support
- **Specific Error Codes**: Clear error identification for different failure scenarios
- **User-Friendly Guidance**: Actionable error messages for user correction
- **Security Considerations**: Generic error messages for security-sensitive failures

---

## Internationalization
- **Resource Files**: Multi-language support via Resource1, Resource2 with proper fallbacks
- **Session Language**: Language-specific content display based on user preference
- **Arabic/English**: Full bilingual support with RTL/LTR layout considerations
- **Localized Validation**: Culture-specific validation messages and formatting
- **Recent Update**: Enhanced resource integration throughout validation and error handling

---

## File Management
- **Upload Directory**: Organized member file storage in `wwwroot/uploads/members`
- **Temporary Files**: Session-based temporary storage during form completion
- **Cleanup Operations**: Automatic temporary file deletion post-processing
- **Path Management**: Consistent file path handling with web root awareness
- **Image Reconstruction**: Recreation of `IFormFile` from stored paths for validation
- **Recent Enhancement**: Added `IWebHostEnvironment` integration for proper path resolution

---

## AI Integration
- **ID Classification**: Machine learning document recognition using `IDClassificationMLModel`
- **OCR Processing**: Automated text extraction from IDs via `IOCRService`
- **Data Validation**: Cross-reference between input and extracted data for verification
- **Confidence Scoring**: Quality assessment of AI processing with 90% threshold
- **Gray Scale Processing**: Image preprocessing for improved OCR accuracy
- **Recent Update**: Comprehensive error handling in AI pipeline with proper cleanup

---

## Code Architecture Patterns

### Dependency Injection
- **Constructor Injection**: All services injected via constructor for testability
- **Interface Abstraction**: Service interfaces for loose coupling
- **Configuration Injection**: Environment and path configuration via DI

### Service Layer Separation
- **Business Logic**: Delegated to service classes (`IAccountService`, `IMemberService`)
- **Data Access**: Abstracted through `IUnitOfWork` and repositories
- **Presentation Logic**: Limited to controller for MVC pattern adherence

### Session Management Strategy
- **Temporary Storage**: Session used for multi-step processes
- **Security Tokens**: GUID-based authentication tokens
- **State Management**: Email and authentication state in session

### Validation Strategy
- **Model State Validation**: Comprehensive client-server validation
- **Business Rule Validation**: Service layer validation for complex rules
- **File Validation**: Specialized validation for image uploads

---

## Performance Considerations

### Image Processing
- **Memory Stream Usage**: Efficient file handling without disk I/O for small files
- **Temporary Files**: Proper cleanup to prevent storage bloat
- **Async Operations**: All file operations use async/await for scalability

### Database Operations
- **Eager Loading**: Related data loaded efficiently for views
- **Transaction Management**: Unit of Work pattern for data consistency
- **Selective Updates**: Only modified fields updated in edit operations

### Session Management
- **Minimal Data**: Only essential data stored in session
- **Cleanup**: Proper session data removal post-process
- **Token Security**: Secure token generation for authentication

---

## Security Implementation

### Authentication Security
- **Session Tokens**: GUID-based authentication tokens
- **Password Handling**: Proper password field management in forms
- **Account Status**: Suspended account prevention

### Data Security
- **Input Validation**: Comprehensive validation at multiple levels
- **File Upload Security**: Size, format, and content validation
- **SQL Injection Prevention**: Parameterized queries via Entity Framework

### Session Security
- **Token Regeneration**: New tokens on authentication
- **Session Clearance**: Complete session clearing on logout
- **Authorization Checks**: Session-based ownership verification

---

## Testing Considerations

### Unit Test Points
- **Service Methods**: `IAccountService` methods for registration and validation
- **Validation Logic**: Image and form validation methods
- **Mapping Logic**: AutoMapper configurations and mappings

### Integration Test Points
- **Registration Flow**: End-to-end registration with image upload
- **Login Flow**: Authentication and session management
- **Profile Edit**: Data update with image management

### Mocking Requirements
- **File System**: Mock file operations for testing
- **Session**: Mock HTTP context and session
- **Services**: Mock service layer for isolation

---

## Maintenance and Extensibility

### Code Organization
- **Region-based**: Logical grouping of related methods
- **Service Delegation**: Business logic in service layer
- **Helper Classes**: Reusable utilities for common operations

### Extension Points
- **Additional Validation**: Easy addition of new validation rules
- **New Image Types**: Support for additional document types
- **Enhanced AI**: Integration of additional AI/ML capabilities

### Refactoring Opportunities
- **Extract Methods**: Further decomposition of large methods
- **ViewModel Consolidation**: Rationalization of ViewModel classes
- **Service Consolidation**: Potential service layer optimization

---

## Recent Architecture Decisions

### Session vs. Database for Temporary Data
- **Choice**: Session storage for temporary image paths during registration
- **Rationale**: Simpler than temporary database records, works well for short-lived data
- **Alternative Considered**: Temporary database table with cleanup job

### Auto-Login After Registration
- **Choice**: Automatic login after successful registration
- **Rationale**: Better user experience, reduces friction
- **Implementation**: Direct session setup post-registration

### Commented AI Validation
- **Current State**: AI validation code commented out in registration
- **Rationale**: Allows gradual rollout of AI features
- **Future Plan**: Uncomment and enhance based on user feedback and accuracy

### Phone Number Formatting
- **Approach**: Server-side formatting with +971 prefix
- **Rationale**: Consistent data storage, simplifies queries
- **User Experience**: Accepts multiple formats, stores consistently

---

## Future Enhancement Opportunities

### AI Feature Expansion
- **Real-time Validation**: Live ID validation during registration
- **Multiple Document Types**: Support for additional ID documents
- **Enhanced OCR**: Improved text extraction accuracy

### Security Enhancements
- **Two-Factor Authentication**: SMS or email verification
- **Password Policies**: Configurable password requirements
- **Login Attempt Tracking**: Brute force protection

### User Experience Improvements
- **Progress Tracking**: Multi-step registration with progress indicator
- **Image Preview**: Live preview of uploaded images
- **Auto-save**: Draft saving during form completion

### Performance Optimizations
- **Image Compression**: Server-side image optimization
- **Caching**: Frequently accessed data caching
- **Async Processing**: Background processing for AI operations

---

## Summary

The AccountController provides comprehensive member management functionality with:
- **Robust Registration**: Multi-step process with AI document verification
- **Secure Authentication**: Session-based login with proper security measures
- **Profile Management**: Full CRUD operations with image handling
- **AI Integration**: Document classification and OCR capabilities
- **International Support**: Full bilingual implementation
- **Scalable Architecture**: Service layer separation for maintainability

The controller demonstrates good separation of concerns, proper error handling, and consideration for both security and user experience. Recent updates have enhanced file management, session handling, and error messaging while maintaining backward compatibility and extensibility for future enhancements.