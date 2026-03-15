# Course Management DTOs

## File Structure
- **AcceptanceDTO**: AcceptanceDTO.cs
- **CertificateDTO**: CertificateDTO.cs
- **SubscribedMemberCourseDTO**: SubscribedMemberCourseDTO.cs

## Core Data Transfer Objects

### AcceptanceDTO
**Purpose**: Handles course subscription acceptance/rejection operations

**Properties**:
- `SubscriptionId`: Unique identifier for the course subscription (int)
- `State`: Acceptance state - true for accepted, false for rejected (bool)
- `value`: Additional value parameter for extended functionality (string)
- `Notes`: Administrative notes or comments about the decision (string)

### CertificateDTO
**Purpose**: Certificate generation and member course completion data

**Properties**:
- `MemberNameAr`: Member's name in Arabic (string)
- `MemberNameEn`: Member's name in English (string)
- `GenderId`: Member's gender identifier (int?)
- `CourseTitleAr`: Course title in Arabic (string)
- `CourseTitleEn`: Course title in English (string)
- `StartDate`: Course start date (DateOnly?)
- `EndDate`: Course end date (DateOnly?)

### SubscribedMemberCourseDTO
**Purpose**: Comprehensive member course subscription and attendance tracking

**Properties**:
- **Member Information**:
  - `MemberFullNameAr`: Arabic full name (string)
  - `MemberFullNameEn`: English full name (string)
  - `MemberId`: Unique member identifier (int?)
  - `MemberIdNumber`: Member identification number (string)
  - `MemberGenderId`: Gender identifier (int?)
  - `NationalityId`: Nationality identifier (int?)
  - `NationalityNameAr`: Arabic nationality name (string)
  - `NationalityNameEn`: English nationality name (string)

- **Department Information**:
  - `DepartmentId`: Department identifier (int?)
  - `DepartmentNameAr`: Arabic department name (string)
  - `DepartmentNameEn`: English department name (string)

- **Course Information**:
  - `CourseID`: Course identifier (int?)
  - `CourseTitleAr`: Arabic course title (string)
  - `CourseTitleEn`: English course title (string)
  - `CourseStartDate`: Course start date (DateOnly?)
  - `CourseEndDate`: Course end date (DateOnly?)

- **Subscription Details**:
  - `SubscriptionId`: Unique subscription identifier (int)
  - `SubscriptionDate`: Date of subscription (DateTime?)
  - `Acceptance`: Acceptance status (bool?)
  - `Attendance`: Attendance status (bool?)
  - `SelectedRate`: Rating given by member (int?)
  - `SubNotes`: Subscription notes (string)
  - `isCoursehasAcceptedOrRejectedMember`: Course status flag (bool)

- **Trainer Information**:
  - `TrainerFullNameAr`: Arabic trainer name (string)
  - `TrainerFullNameEn`: English trainer name (string)

## Business Domain Usage

### Course Management Workflow
1. **Subscription**: Member subscribes to course
2. **Acceptance**: Admin accepts/rejects with AcceptanceDTO
3. **Tracking**: Comprehensive tracking via SubscribedMemberCourseDTO
4. **Completion**: Certificate generation via CertificateDTO

### Multi-language Support
- Bilingual fields for Arabic and English
- Consistent naming convention (*Ar, *En)
- Culture-aware data handling

## Data Types & Patterns

### Temporal Data Handling
- `DateOnly`: For course dates (start/end)
- `DateTime`: For subscription timestamp
- Nullable types for optional temporal data

### Status Tracking
- Boolean flags for acceptance, attendance
- Integer ratings for member feedback
- Comprehensive status flags for workflow management

## Integration Context

### API Operations
- **AcceptanceDTO**: POST/PUT operations for subscription decisions
- **CertificateDTO**: GET operations for certificate data
- **SubscribedMemberCourseDTO**: Comprehensive reporting and listing

### Reporting & Analytics
- Member course participation tracking
- Attendance and acceptance statistics
- Multi-dimensional reporting capabilities
- Trainer performance monitoring

### Administrative Functions
- Subscription management
- Member progress tracking
- Certificate generation
- Course completion validation