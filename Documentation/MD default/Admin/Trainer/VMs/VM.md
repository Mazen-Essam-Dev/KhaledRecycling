# Trainer Management ViewModels

## File Structure
- **IndexCoursesVM**: IndexCoursesVM.cs
- **TrainersNameVM**: TrainersNameVM.cs
- **TrainerIndexVM**: TrainerIndexVM.cs
- **TrainerWithUserVM**: TrainerWithUserVM.cs
- **TrainerVM**: TrainerVM.cs

## Core ViewModels

### IndexCoursesVM
**Purpose**: Comprehensive ViewModel for trainer course management with filtering and pagination

**Properties**:
- `UserIsTrainer`: Role identification flag (bool)
- `CoursesVM_Paginated`: Paginated list of courses (PaginatedList<CourseVM>)
- `CoursesVM`: Queryable courses collection (IQueryable<CourseVM>)
- `TrainersNameVM_List`: List of available trainers (List<TrainersNameVM>)
- `trainerSelect`: Selected trainer filter with validation (int)
- `Trainer`: Trainer entity reference
- `TrainersList`: Trainer dropdown options (List<SelectListItem>)

### TrainersNameVM
**Purpose**: Lightweight ViewModel for trainer selection and display

**Properties**:
- `Id`: Trainer identifier (int)
- `UserId`: Associated user account (string)
- `FullNameAr`: Arabic full name (string)
- `FullNameEn`: English full name (string)
- `Email`: Contact email (string)
- `DepartmentId`: Department association (int?)
- `PhoneNumber`: Contact phone (string)

### TrainerIndexVM
**Purpose**: Main ViewModel for trainer listing with department context

**Properties**:
- `TrainerWithUserVM_Paginated`: Paginated trainer-user combinations (PaginatedList<TrainerWithUserVM>)
- `Departments`: Available departments for filtering (List<Department>)

### TrainerWithUserVM
**Purpose**: Combined ViewModel linking trainers with user account information

**Properties**:
- `Trainer`: Trainer entity with professional details (Trainer)
- `User`: User entity with personal information (ApplicationUser)

### TrainerVM
**Purpose**: Comprehensive ViewModel for trainer creation and editing

**Properties**:

#### Department & User Association
- `Id`: Primary identifier (int)
- `DepartmentId`: Department assignment with validation (int)
- `Department`: Navigation to department entity
- `DepartmentsList`: Department dropdown options (List<SelectListItem>)
- `User`: Associated user account (ApplicationUser)
- `UserId`: User identifier with validation (string)
- `UsersList`: Available users dropdown (List<SelectListItem>)

#### Professional Information
- `Bio`: Trainer biography and qualifications (string)
- `AttachmentPath`: Document storage path (string, max 255)
- `Attachment`: File upload interface (IFormFile)

## Validation Features

### Custom Validation Attributes
- `[LocalizedRequired]`: Localized required field validation
- `[MaxLength]`: String length constraints for database optimization

### Business Rule Validation
- **Required Department**: Trainer must be assigned to a department
- **Required User Association**: Trainer must be linked to a user account
- **File Upload Support**: Attachment handling with size constraints

## Key Features

### Role-based Access
- **Trainer vs Admin Views**: Different functionality based on user role
- **Filtered Course Access**: Trainers see only their assigned courses
- **Administrative Controls**: Full access for admin users

### Comprehensive Course Management
- **Paginated Lists**: Efficient handling of large course datasets
- **Trainer Filtering**: Course filtering by assigned trainer
- **Multi-trainer Support**: Support for multiple trainers per department

### User Integration
- **Account Linking**: Seamless integration with identity system
- **User Selection**: Dropdown for assigning users as trainers
- **Combined Data**: Trainer and user information in single views

### Department Organization
- **Structured Management**: Department-based trainer organization
- **Department Filtering**: Filter trainers by department
- **Organizational Hierarchy**: Clear department-trainer relationships

## Use Cases

### Course Administration
- Trainer course assignment and management
- Course filtering by trainer
- Trainer workload monitoring
- Department course coordination

### Trainer Management
- Trainer registration and profile creation
- Department assignment and transfers
- Professional qualification tracking
- User account synchronization

### Administrative Functions
- Department trainer listings
- Trainer performance reporting
- Course assignment workflows
- Organizational structure management

## Technical Implementation

### Pagination Support
- **Efficient Data Loading**: Paginated lists for large datasets
- **User Experience**: Smooth navigation through trainer and course lists
- **Performance Optimization**: Reduced data transfer and processing

### Dropdown Integration
- **Department Selection**: Organized department assignment
- **User Assignment**: Secure user-to-trainer linking
- **Trainer Filtering**: Dynamic course filtering by trainer

### File Management
- **Document Upload**: Certificate and qualification storage
- **Path Management**: Secure file path handling
- **Upload Interface**: IFormFile support for web uploads

## Business Logic

### Workflow Support
1. **Trainer Onboarding**: User selection → Department assignment → Profile creation
2. **Course Assignment**: Trainer selection → Course assignment → Schedule management
3. **Department Management**: Department creation → Trainer allocation → Course offering

### Data Integrity
- **User Validation**: Ensure valid user accounts for trainers
- **Department Validation**: Prevent orphaned trainer records
- **Course Assignment**: Validate trainer-course compatibility
- **File Management**: Secure document storage and retrieval