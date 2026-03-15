# CourseController Documentation

## Overview
ASP.NET Core Controller for comprehensive course management system with role-based access, member subscriptions, attendance tracking, and multi-format reporting. The controller serves as the central hub for all course-related administrative operations in the FougeraClub application.

**Recent Updates:** Enhanced role-based access control, improved Excel reporting with dynamic columns, streamlined file attachment handling, and optimized performance for large datasets.

---

## Controller Configuration
- **Area**: Admin - Restricted to administrative users only
- **Authorization**: AdminAuthorize attribute ensures proper authentication
- **Dependencies**:
  - ICourseService: Core course business logic and operations
  - ITrainerService: Trainer management and validation
  - IUnitOfWork: Database operations with repository pattern
  - IMapper: AutoMapper for object-object mapping between layers
  - IHttpContextAccessor: User context and session access
  - IAuthorizationService: Permission validation and role-based access control
  - IOCRService: Optical Character Recognition for document processing
  - IHubContext<NotificationHub>: Real-time SignalR notifications
  - INotificationService: System notification management

---

## Role-Based Access System

### User Type Detection
- **Trainer Access**: Restricted to own courses only with limited permissions
- **Admin Access**: Full system access with comprehensive control
- **Auto-Detection**: Based on logged-in user's trainer status using `GetThisTrainerId_IfTrainer_else_0()`
- **Recent Enhancement**: Improved trainer identification with email-based lookup

### Permission-Based Features
- Attendance tracking requires specific "Course/Attendance" permission
- Dynamic UI adaptation based on user role and permissions
- Selective data exposure for different user types
- **Recent Update**: Integrated `PermissionScanner` for granular permission checking

---

---
## ⚙️⚙️⚙️ Course Subscribe Order Logic

| No. | Description | Then |
|----|------------|------|
| 1 | Member goes to subscribe in a course but his ID Card is expired | When he goes to subscribe to the course, he must enter a new ID Card for the same person and it must not be expired. **Checked by OCR but #Now Commented#** |
| 2 | Member goes to subscribe in a course but has a valid (not expired) ID Card | When he subscribes to the course, a new ID Card is **not required** |
| 3 | After the member subscribes to this course | In **SuperAdmin → CoursesSubs**, the subscription will appear, but **SuperAdmin must accept or reject** it. Once chosen, it **cannot be changed**. After acceptance, it will appear in **Member Area → Courses** |
| 4 | Only if SuperAdmin accepted the subscription | It will appear in `/Admin/Course/MembersCourse` to allow the **Trainer to mark students as present or absent**, and the member will be allowed to **rate the course** |
| 5 | Only after the Trainer marks the student as present | It will appear in `/Member/Course`, and the **certificate will be available for download only after**: the course time is finished, the member rated the course, and the member attended the course |

-----------

## Core Course Management

### Index
- **Purpose**: Main course listing with advanced filtering and search capabilities
- **Filters**: Search term, department, trainer, date range with comprehensive validation
- **Role Adaptation**: Different data sets for trainers (own courses) vs admins (all courses)
- **Subscription Status**: Real-time subscription tracking with acceptance status
- **Pagination**: 50 items per page with AJAX support for dynamic updates
- **Performance**: Efficient query execution with AsQueryable() pattern
- **Recent Enhancement**: Added subscription count with acceptance filtering

### AddEdit (GET/POST)
- **Purpose**: Course creation and modification with full CRUD operations
- **Session Management**: Tracks return URL for proper redirects after operations
- **File Validation**: PDF attachment validation with 5MB size limit
- **Department Binding**: Dynamic department dropdowns with proper binding
- **Temp File Handling**: Temporary file storage during validation phase
- **Recent Update**: Improved session-based return URL tracking for better UX

#### File Attachment Workflow:
1. **Temporary Storage**: Files saved during validation
2. **PDF Validation**: Format and size checking
3. **Final Processing**: Move to permanent storage post-validation
4. **Cleanup**: Old file deletion on replacement

### Delete Operations
- **Course Deletion**: Removes course and related data with cascade consideration
- **Subscription Deletion**: Manages member subscriptions with proper cleanup
- **Recent Update**: Added async pattern for non-blocking operations

---

## Member Subscription System

### MembersCourse
- **Purpose**: Display members enrolled in specific course with detailed information
- **Features**: Attendance tracking, subscription management, member profiling
- **Pagination**: Member list with configurable pagination (default 50 per page)
- **Data Enrichment**: Includes nationality, contact info, and subscription dates
- **Recent Enhancement**: Added trainer information display with join queries

### SubscribedMembersInCourses
- **Purpose**: Comprehensive subscription management across all courses
- **Advanced Filtering**:
  - Trainer selection with dynamic loading
  - Nationality filtering with dropdown binding
  - Gender filtering with enum-based options
  - Department filtering with hierarchical selection
  - Date range filtering with proper validation
- **Role-Based Data**: Different views for trainers (restricted) vs admins (full access)
- **Performance**: Optimized data retrieval with DTO pattern
- **Recent Update**: Enhanced filtering with combined search capabilities

#### Filter Implementation:
- **Text Search**: Member names and ID numbers
- **Dropdown Filters**: Nationality, gender, department
- **Date Range**: Subscription date filtering
- **Trainer Filter**: Dynamic trainer selection

### Acceptance System
- **AJAX Endpoint**: Real-time subscription acceptance/rejection via `/Accept`
- **JSON API**: Clean RESTful API design for frontend integration
- **Validation**: Model state validation with proper error responses
- **Recent Update**: Added FromBody attribute for proper JSON binding

---

## Attendance Management

### Attendance Tracking
- **API Endpoint**: `/Attendance` for real-time attendance updates
- **Permission Control**: Requires "Course/Attendance" permission via `PermissionScanner`
- **Boolean Status**: Present/Absent tracking with simple API design
- **Validation**: Course and member existence validation
- **Recent Enhancement**: Integrated permission checking for secure access

---

## Reporting & Export System

### Print Functionality
- **MembersCourse**: Printable member lists per course with professional formatting
- **SubscribedMembersInCourses**: Comprehensive subscription reports with filters
- **Index**: Course catalog printing with search preservation
- **Certificates**: Individual course certificates with data integration
- **Recent Update**: Added print views for all major data sets

### Excel Export
- **Multiple Formats**: Role-appropriate column sets for different user types
- **Bilingual Support**: Arabic/English column headers with dynamic switching
- **Dynamic Columns**: Permission-based field inclusion/exclusion
- **File Naming**: Timestamped files with descriptive names for easy organization
- **Formatting**: Professional Excel formatting with proper data types
- **Recent Enhancement**: Added member code inclusion for trainer exports

#### Export Types:
1. **Course Lists**: Basic course information with trainer and department details
2. **Member Subscriptions**: Detailed subscription data with member information
3. **Attendance Reports**: Member participation tracking with attendance status
4. **Participant Lists**: Course-specific member lists with comprehensive data
5. **Recent Addition**: Trainer-specific exports with limited data scope

#### Excel Generation Features:
- **Localized Headers**: Resource-based column titles
- **Dynamic Column Count**: Varies based on user role and permissions
- **Error Handling**: Graceful fallback on generation failures
- **Memory Efficiency**: Stream-based file generation for large datasets

---

## Certificate System

### PrintCertificate
- **Purpose**: Generate course completion certificates with professional design
- **Data Integration**: Pulls course, member, and completion data with proper joins
- **Professional Formatting**: Print-ready certificate design with branding
- **Recent Update**: Improved data retrieval for certificate generation

---

## Advanced Features

### Dynamic Trainer Loading
- **Department-Based**: Load trainers by department for targeted assignment
- **AJAX Endpoint**: `GetTrainersInDept` for dynamic dropdown population
- **Bilingual Names**: Arabic/English trainer name support with proper mapping
- **Recent Enhancement**: Added trainer ID parameter for edit scenarios

### Session Management
- **Return URL Tracking**: Smart redirects after edit operations
- **State Preservation**: Maintains filter states across requests for better UX
- **Recent Update**: Improved session handling with extension methods

### Search Implementation
- **Multi-field Search**: Course titles, member names, ID numbers with combined logic
- **Case-Insensitive**: Comprehensive text matching across fields
- **Combined Filters**: Multiple filter types work together for precise results
- **Performance**: Optimized search with proper indexing considerations
- **Recent Enhancement**: Added member code search capability

---

## Security & Validation

### File Validation
- **PDF Only**: Restricts attachments to PDF format for security
- **Size Limit**: 5MB maximum file size to prevent server overload
- **Type Checking**: MIME type validation with proper error messages
- **Temp Storage**: Secure temporary file handling during validation
- **Recent Update**: Enhanced file validation with async pattern

### Model Validation
- **Comprehensive Checks**: Full model state validation across all operations
- **Anti-Forgery**: CSRF protection on all POST actions with tokens
- **Error Handling**: Graceful validation failure handling with user feedback
- **Data Integrity**: Ensures referential integrity across operations
- **Recent Enhancement**: Improved error messages with localization

### Permission Security
- **Role-Based Access**: Different functionality based on user role
- **Permission Scanning**: Dynamic permission checking via `PermissionScanner`
- **Feature Gating**: Selective feature exposure based on permissions
- **Recent Update**: Integrated permission system throughout controller

---

## API Design

### JSON Endpoints
- **Acceptance Updates**: RESTful subscription management with proper HTTP verbs
- **Attendance Tracking**: Simple boolean API for quick updates
- **Trainer Loading**: Dynamic dropdown data with AJAX support
- **Permission Checks**: Role-based feature access with proper responses
- **Recent Update**: Standardized JSON response format

### Response Format
```json
{
  "success": true/false,
  "message": "Descriptive message",
  "data": {} // Optional additional data
}