# Course Management ViewModels

## File Structure
- **AcceptanceVM**: AcceptanceVM.cs
- **CertificateVM**: CertificateVM.cs
- **CourseVM**: CourseVM.cs
- **MembersCourseVM**: MembersCourseVM.cs
- **SubscribedMemberCourseVMTrainer**: SubscribedMemberCourseVMTrainer.cs
- **SubscribedMemberCourseVM**: SubscribedMemberCourseVM.cs

## Core ViewModels

### AcceptanceVM
**Purpose**: Handles course subscription acceptance/rejection operations in the view layer

**Properties**:
- `SubscriptionId`: Unique identifier for course subscription (int)
- `State`: Acceptance state - true for accepted, false for rejected (bool)
- `value`: Additional value parameter for extended operations (string)
- `Notes`: Administrative notes about the decision (string)

### CertificateVM
**Purpose**: Certificate data presentation with enhanced identity tracking

**Properties**:
- `Id`: Primary identifier for certificate tracking (int)
- `MemberNameAr`: Member's Arabic name (string)
- `MemberNameEn`: Member's English name (string)
- `GenderId`: Member's gender identifier (int?)
- `CourseTitleAr`: Arabic course title (string)
- `CourseTitleEn`: English course title (string)
- `StartDate`: Course start date (non-nullable DateOnly)
- `EndDate`: Course end date (non-nullable DateOnly)

### CourseVM
**Purpose**: Comprehensive course management with advanced validation and file attachments

**Properties**:
- **Basic Information**:
  - `Id`: Course identifier (int)
  - `DepartmentId`: Department foreign key with validation (int)
  - `Department`: Navigation to department entity
  - `DepartmentsList`: Dropdown options for departments
  - `TrainerId`: Trainer foreign key with validation (int)
  - `Trainer`: Navigation to trainer entity
  - `TrainersList`: Dropdown options for trainers

- **Course Details**:
  - `TitleAr`: Arabic title with regex validation (string)
  - `TitleEn`: English title with regex validation (string)
  - `StartDate`: Course start date with past-date prevention (DateOnly?)
  - `EndDate`: Course end date (DateOnly?)
  - `Location`: Course location with validation (string)
  - `Description`: Course description with validation (string)

- **File Management**:
  - `AttachmentPath`: Stored file path (string)
  - `Attachment`: File upload interface (IFormFile)

- **Status**:
  - `IsSubscribed`: Subscription status indicator (bool)

**Validation**: Implements `IValidatableObject` for custom date validation

### MembersCourseVM
**Purpose**: Aggregated view of course with members and trainer information

**Properties**:
- `Course`: Main course information (CourseVM)
- `Trainer`: Trainer details (TrainersNameVM)
- `Members`: Collection of enrolled members (IEnumerable<MemberVM>)
- `Members_Paginated`: Paginated member list (PaginatedList<MemberVM>)
- `Subscriptions`: Course subscriptions collection (IEnumerable<Subscription>)

### SubscribedMemberCourseVMTrainer
**Purpose**: Trainer-focused view of course subscriptions with filtering

**Properties**:
- `paginated`: Paginated list of subscribed members (PaginatedList<SubscribedMemberCourseVM>)
- `UserIsTrainer`: Role identification flag (bool)
- `trainerSelect`: Selected trainer filter (string)
- `Trainer`: Trainer entity reference
- `TrainersList`: Trainer dropdown options

### SubscribedMemberCourseVM
**Purpose**: Comprehensive member course subscription tracking with validation

**Properties**:
- **User Context**:
  - `UserIsTrainer`: Role-based access flag (bool)

- **Member Information**:
  - `MemberFullNameAr`: Arabic name with regex validation (string)
  - `MemberFullNameEn`: English name with regex validation (string)
  - `MemberIdNumber`: ID number with length validation (string)
  - `MemberId`: Member identifier (int?)
  - `MemberGenderId`: Gender identifier with validation (int?)
  - `NationalityId`: Nationality identifier with validation (int?)
  - `NationalityNameAr`: Arabic nationality with validation (string)
  - `NationalityNameEn`: English nationality with validation (string)

- **Department & Course**:
  - `DepartmentId`: Department identifier (int?)
  - `DepartmentNameAr`: Arabic department name with validation (string)
  - `DepartmentNameEn`: English department name with validation (string)
  - `CourseTitleAr`: Arabic course title with validation (string)
  - `CourseTitleEn`: English course title with validation (string)
  - `CourseID`: Course identifier (int?)

- **Subscription Details**:
  - `SubscriptionDate`: Subscription timestamp (DateTime?)
  - `CourseStartDate`: Course start date (DateOnly?)
  - `CourseEndDate`: Course end date (DateOnly?)
  - `SubscriptionId`: Unique subscription identifier (int)
  - `Acceptance`: Acceptance status (bool?)
  - `IsAttendance`: Attendance status (bool?)
  - `SelectedRate`: Member rating (int?)
  - `SubNotes`: Subscription notes (string)

- **Trainer Information**:
  - `TrainerFullNameAr`: Arabic trainer name (string)
  - `TrainerFullNameEn`: English trainer name (string)

- **Status Flags**:
  - `isCoursehasAcceptedOrRejectedMember`: Course member status (bool)

**Validation**: Implements `IValidatableObject` for course date validation

## Validation Framework

### Custom Validation Attributes
- `[LocalizedRequired]`: Localized required field validation
- `[LocalizedMaxLength]`: Localized maximum length validation
- `[NotInThePast]`: Custom attribute preventing past dates
- `[RegularExpression]`: Pattern validation for language-specific input

### Business Rule Validation
- **Date Validation**: End date must be after start date
- **Language-specific Patterns**: Arabic and English character validation
- **Cross-field Validation**: Custom validation contexts

## Integration Patterns

### Multi-language Support
- Consistent bilingual field naming (*Ar, *En)
- Culture-aware validation messages
- Language-specific regex patterns

### File Management
- IFormFile integration for uploads
- Path storage for attachments
- File handling in course management

### Pagination Support
- PaginatedList integration for large datasets
- Efficient data loading for member lists
- Scalable course management

## Usage Context

### Course Lifecycle Management
1. **Creation**: CourseVM with validation
2. **Subscription**: Member enrollment tracking
3. **Acceptance**: AcceptanceVM for admin decisions
4. **Completion**: CertificateVM for certification
5. **Reporting**: Comprehensive subscription analytics

### Role-based Access
- Trainer-specific views and filters
- Admin vs trainer functionality
- Conditional UI rendering based on roles