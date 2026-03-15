# CourseService Documentation

## Overview
Service layer implementation for comprehensive course management system with member subscriptions, file handling, and role-based data access. The service encapsulates all business logic for course operations, providing a clean separation between data access and business rules.

**Recent Updates:** Enhanced subscription management with certificate serial generation, improved role-based data filtering, optimized performance with efficient LINQ queries, and added comprehensive validation for course deletion operations.

---

## Dependencies
- **IUnitOfWork**: Database repository access and transaction management using Unit of Work pattern
- **IWebHostEnvironment**: File system access for course attachments with proper web root resolution
- **Recent Enhancement**: Added proper web environment awareness for cross-platform file operations

---

## Core Course Operations

### GetAllAsync()
- **Purpose**: Retrieves all courses with related entities for complete data representation
- **Includes**: Trainer, Department navigation properties via eager loading
- **Returns**: `IEnumerable<Course>` with complete relationship data
- **Performance**: Uses eager loading to prevent N+1 query problems
- **Recent Update**: Maintained simple retrieval pattern for comprehensive course lists

### GetAllCoursesOfMemberAsync(int memberId)
- **Purpose**: Gets courses subscribed by specific member with subscription status
- **Logic**: Cross-references courses with member's subscriptions using LINQ joins
- **Filter**: Only returns courses where member has active subscriptions
- **Returns**: Filtered course list based on subscription status
- **Recent Enhancement**: Added department inclusion for better member context

### GetByIdAsync(int id)
- **Purpose**: Fetches specific course by identifier with precise lookup
- **Parameters**: `int id` - Unique course identifier
- **Returns**: `Course?` entity with null safety for non-existent records
- **Error Handling**: Returns null rather than throwing exceptions
- **Recent Update**: Uses expression-based lookup for type safety

---

## Course Creation and Modification

### AddAsync(Course entity, IFormFile? file)
- **Purpose**: Creates new course with optional file attachment support
- **File Handling**: Saves attachment if provided using dedicated file management methods
- **Process**: Entity creation → File save → Transaction commit with atomic operations
- **Returns**: Integer ID of created course for immediate client reference
- **Validation**: Implicit validation through entity framework and file checking
- **Recent Update**: Maintains simple file save pattern without temporary storage

#### Add Workflow:
1. **File Check**: If file provided, save using `SaveImageAsync()`
2. **Entity Creation**: Add course to repository
3. **Transaction Commit**: Save changes with `CompleteAsync()`
4. **ID Return**: Return new course ID for reference

### UpdateAsync(Course entity, IFormFile? file)
- **Purpose**: Modifies existing course with comprehensive file management
- **File Logic**:
  - **Previous Approach**: Delete old file, save new file when provided
  - **Current Implementation**: File replacement logic commented out for controller-level management
  - **Rationale**: Allows more flexible file handling in controller layer
- **Process**: Entity fetch → File handling → Value update → Transaction commit
- **Recent Refactoring**: Separated file handling from entity update for better controller flexibility

#### UpdateValues Pattern:
- Uses `_unitOfWork.Courses.UpdateValues(existing, entity)` for efficient updates
- Tracks only changed properties rather than full entity replacement
- Reduces database overhead for partial updates
- Maintains data consistency through proper change tracking

---

## Advanced Deletion Logic

### DeleteAsync(int id)
- **Purpose**: Safely removes course after comprehensive subscription validation
- **Business Rules**:
  - Only allows deletion if no accepted subscriptions exist (`Acceptance == true`)
  - Removes all related subscriptions before course deletion
  - Cleans up associated file attachments with proper file system operations
- **Safety**: Prevents deletion of courses with active participants to maintain data integrity
- **Validation**: Checks subscription acceptance status before proceeding
- **Recent Enhancement**: Added acceptance-based validation for safer deletion

#### Deletion Workflow:
1. **Subscription Check**: Validate no accepted subscriptions exist
2. **Subscription Cleanup**: Remove all related subscriptions if validation passes
3. **File Cleanup**: Delete associated attachment files
4. **Entity Removal**: Remove course entity from repository
5. **Transaction Commit**: Save all changes atomically

### DeleteSubscriptionAsync(int id)
- **Purpose**: Removes individual member subscriptions for course withdrawal
- **Use Case**: Member withdrawal from courses, admin subscription management
- **Validation**: Checks subscription existence before deletion
- **Recent Update**: Added proper null checking and entity validation

---

## File Management System

### SaveImageAsync(IFormFile file)
- **Purpose**: Stores course attachment files with organized structure
- **Directory**: "uploads/Courses" in web root with proper path resolution
- **Naming**: GUID-based unique filenames to prevent collisions
- **Directory Creation**: Ensures target directory exists before file operations
- **Returns**: Relative path for database storage with consistent format
- **Recent Update**: Uses `IWebHostEnvironment` for proper cross-platform path handling

#### File Save Process:
1. **Path Resolution**: Combine web root path with uploads directory
2. **Directory Creation**: Ensure target directory exists
3. **Unique Naming**: Generate GUID-based filename with original extension
4. **File Copy**: Stream-based file copying for memory efficiency
5. **Path Return**: Return relative path for database storage

### DeleteImageFile(string? relativePath)
- **Purpose**: Removes physical attachment files with comprehensive safety checks
- **Safety**: Null checks and file existence verification before deletion
- **Path Handling**: Cross-platform path conversion with proper separator handling
- **Error Prevention**: Silent failure on non-existent files to prevent exceptions
- **Recent Enhancement**: Added cross-platform path separator handling

---

## Member Subscription Management

### GetAllMembersOfCourseAsync(int courseId)
- **Purpose**: Retrieves all members subscribed to specific course with filtering
- **Includes**: Member nationality data via eager loading for complete profiles
- **Filter**: Only accepted subscriptions (`Acceptance == true`) for active participants
- **Returns**: Tuple of (Members, Subscriptions) for flexible data usage
- **Performance**: Efficient LINQ join with in-memory filtering
- **Recent Update**: Added acceptance filtering for active participants only

#### Member Retrieval Logic:
```csharp
// Get all members with nationality data
var allMembers = await _unitOfWork.Members.GetAllAsync(x => x.Nationality);

// Get subscriptions for specific course with acceptance filter
var membersSubscriptionsCourse = await _unitOfWork.Subscriptions
    .GetAllAsync(s => s.SubscribedInId == courseId && 
                     s.SubscribedInType == SubscriptionType.Course && 
                     s.Acceptance == true);

// Filter members by subscription using LINQ join
var courseMembers = allMembers
    .Where(member => membersSubscriptionsCourse.Any(s => s.MemberId == member.Id));