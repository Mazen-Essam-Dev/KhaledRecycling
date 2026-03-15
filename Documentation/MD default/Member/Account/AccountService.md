# Account Service Documentation

## Service Overview
- **Namespace**: `Application.Services.Member`
- **Implements**: `IAccountService`
- **Purpose**: Handles member registration, authentication, and image management operations with session-based workflow
- **Design Pattern**: Service layer with repository pattern and session state management

---

## Dependencies

### Injected Services
- `IGenericRepository<MemberEntity>`: Data access layer for member entities using generic repository pattern
- `IHttpContextAccessor`: HTTP context and session management for multi-step registration workflow
- `Interfaces.Admin.IMemberService`: Member code generation and admin operations integration

### Configuration
- **Images Full Path**: `wwwroot/uploads/Members` (physical directory for file storage)
- **Images Path**: `uploads/Members/` (web-relative path for database storage)
- **Recent Enhancement**: Consistent path management with both physical and web-relative path formats

---

## Core Methods

### RegisterAsync
**Purpose**: Handles new member registration with comprehensive validation and duplicate checking

#### Registration Process
1. **Duplicate Validation**:
   - Checks for existing email using repository pattern
   - Checks for existing ID number with separate validation
   - Returns specific error codes for each duplicate scenario
   - **Recent Improvement**: Added combined error for when both email and ID are duplicates

2. **Code Generation**:
   - Validates member code uniqueness before registration
   - Generates new code if duplicate exists using admin service
   - Uses `_memberService.GenerateNewCode()` for consistent code generation
   - **Recent Enhancement**: Automatic code regeneration on conflicts prevents registration failures

3. **Data Persistence**:
   - Adds new member to repository using async pattern
   - Saves changes to database with transactional safety
   - Returns success status with descriptive message
   - **Recent Update**: Returns tuple `(bool, string)` for better error handling

#### Return Values
- `(true, "done")`: Successful registration with confirmation
- `(false, "EmailUsedBefore")`: Duplicate email with specific error code
- `(false, "IdNationalNumber_UsedBefore")`: Duplicate ID number with clear identification
- `(false, "EmailUsedBefore_&&_IdNationalNumber_UsedBefore")`: Both duplicates with combined error
- **Recent Enhancement**: Structured return values enable precise error handling in controller

---

### ValidateUserAsync
**Purpose**: Authenticates member credentials with secure password comparison

#### Authentication Process
1. **Email Verification**: Checks if email exists in system using repository lookup
2. **Password Validation**: Compares SHA256 hashed passwords for security
3. **Status Return**: Returns member entity and validation status as tuple
4. **Recent Security Enhancement**: Uses `HashHelper.ComputeSha256Hash()` for consistent hashing

#### Return Scenarios
- `(user, "Valid")`: Successful authentication with member entity
- `(null, "NotExisting")`: Email not found with specific status
- `(null, "NotMatchingBassword")`: Incorrect password with clear identification
- **Recent Improvement**: Returns nullable member entity for safe handling in controller

---

### GetMemberByEmailAsync
**Purpose**: Retrieves member by email address for profile management
- **Parameter**: Email string to search for with null safety
- **Returns**: MemberEntity or null if not found with async pattern
- **Usage**: Profile editing and session management operations
- **Recent Enhancement**: Uses `GetByColumnAsync()` for efficient single-record retrieval

---

### UpdateMemberAsync
**Purpose**: Updates existing member information with existence validation
- **Validation**: Checks if member exists before update using ID lookup
- **Operation**: Uses repository update method with proper entity tracking
- **Persistence**: Saves changes and returns boolean success status
- **Recent Update**: Added existence check to prevent updating non-existent members

---

## Image Management System

### SaveImagesInSession
**Purpose**: Handles temporary image storage during multi-step registration process

#### Image Processing Flow
1. **Directory Creation**: Ensures upload directory exists using `Directory.CreateDirectory()`
2. **Old File Cleanup**: Deletes previous temporary images to prevent storage bloat
3. **File Naming**: Uses member code for consistent naming with descriptive prefixes
   - Format: `{MemberCode}_{ImageType}{Extension}`
4. **File Storage**: Saves image to server directory with proper stream handling
5. **Session Storage**: Stores image path in session for later retrieval in registration flow
6. **Recent Enhancement**: Changed from GUID-based naming to member code-based naming for better traceability

#### Supported Image Types
- Profile Image: `{MemberCode}_ProfileImage.{ext}` - For member profile pictures
- ID Image: `{MemberCode}_IdImage.{ext}` - For identification documents
- Passport Image: `{MemberCode}_PassportImage.{ext}` - For passport documents
- **Recent Standardization**: Consistent naming convention across all image types

#### Session Management
- **Safe Context Access**: Uses null-conditional operators for HTTP context safety
- **Path Storage**: Stores web-relative paths in session for easy retrieval
- **Old Image Cleanup**: Deletes previous session images to prevent orphaned files
- **Recent Improvement**: Added old file cleanup before saving new images

---

### MoveImagesFromSessionToModel
**Purpose**: Transfers images from session storage to model for database persistence

#### Transfer Logic
- **Condition**: Only transfers if image file is null in model (preserves new uploads)
- **Session Retrieval**: Gets image paths from session storage with null checking
- **Session Cleanup**: Removes session values after successful transfer
- **Model Update**: Updates model with session image paths for persistence
- **Recent Enhancement**: Added conditional transfer to prioritize new uploads over session data

#### Session Key Management
- `ProfileImagePath`: Transfers profile image from session to model
- `IdImagePath`: Transfers ID image from session to model
- `PassportImagePath`: Transfers passport image from session to model
- **Recent Update**: Proper session removal after transfer prevents data leakage

---

### DeleteImagesFromSession
**Purpose**: Cleans up temporary images from session with comprehensive error handling

#### Cleanup Process
1. **File Deletion**: Physically removes image files from server using `FileHelper.DeleteImageFile()`
2. **Session Clearance**: Removes image paths from session storage
3. **Error Handling**: Returns false if cleanup fails with try-catch protection
4. **Recent Enhancement**: Comprehensive try-catch block ensures cleanup failures don't crash registration

#### Error Resilience
- **Try-Catch Protection**: Wraps all cleanup operations in try-catch
- **Individual Operations**: Each image type cleaned up independently
- **Boolean Return**: Clear success/failure status for controller handling
- **Recent Improvement**: Returns boolean status instead of throwing exceptions

---

## File Path Management

### Directory Structure
