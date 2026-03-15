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
**Purpose**: Handles course subscription acceptance/rejection operations

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
- `StartDate`: Course start date (DateOnly)
- `EndDate`: Course end date (DateOnly)

### CourseVM
**Purpose**: Comprehensive course management with advanced validation

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
**Purpose**: Comprehensive member course subscription tracking

**Properties**:
- **Member Information**:
  - `MemberFullNameAr`: Arabic name with regex validation (string)
  - `MemberFullNameEn`: English name with regex validation (string)
  - `MemberIdNumber`: ID number with length validation (string)
  - `MemberId`: Member identifier (int?)

- **Course Details**:
  - `CourseTitleAr`: Arabic course title with validation (string)
  - `CourseTitleEn`: English course title with validation (string)
  - `CourseStartDate`: Course start date (DateOnly?)
  - `CourseEndDate`: Course end date (DateOnly?)

- **Subscription Details**:
  - `SubscriptionId`: Unique subscription identifier (int)
  - `Acceptance`: Acceptance status (bool?)
  - `IsAttendance`: Attendance status (bool?)
  - `SelectedRate`: Member rating (int?)
  - `SubNotes`: Subscription notes (string)

## Validation Features

### Custom Attributes
- `[LocalizedRequired]`: Localized required field validation
- `[LocalizedMaxLength]`: Localized maximum length validation
- `[NotInThePast]`: Prevents past dates for start dates
- `[RegularExpression]`: Language-specific input validation

### Business Rules
- End date must be after start date
- Arabic text validation: `^[\u0621-\u064A0-9 ]+$`
- English text validation: `^[a-zA-Z0-9 ]+$`
- Multi-language error messages