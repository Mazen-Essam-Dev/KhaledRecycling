# TrainerController Documentation

## Overview
ASP.NET Core Controller for managing trainers with user integration, department assignment, and comprehensive reporting capabilities.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - ITrainerService: Trainer business logic
  - IUnitOfWork: Database operations
  - IMapper: AutoMapper for object mapping

## Main CRUD Operations

### Index
- **Purpose**: Main trainer listing with user integration
- **Data Structure**: Combines Trainer and User entities via join
- **Search Features**:
  - Trainer name search (Arabic/English)
  - User-based filtering across both name fields
- **Includes**: Related courses for each trainer
- **UI Features**:
  - Pagination (50 items per page)
  - AJAX partial view support
  - Department data for context

### AddEdit (GET)
- **Purpose**: Display form for creating/editing trainers
- **User Management**: Populates dropdown with users not currently trainers
- **Department Assignment**: Department dropdown population
- **Dynamic User List**: Updates available users based on current assignment
- **Parameters**: `id` (optional for create vs edit)

### AddEdit (POST)
- **Purpose**: Handle form submission with file validation
- **File Validation**: PDF attachment validation (5MB limit)
- **Dynamic Dropdown Refresh**: Maintains form state on validation failure
- **Redirect Logic**:
  - After Create: Redirect to Index
  - After Edit: Redirect back to edit form

### Delete
- **Purpose**: Remove trainer records
- **Method**: HTTP POST for security
- **Flow**: Service deletion → Redirect to Index

## Data Integration Features

### User-Trainer Relationship
- **User Selection**: Trainers are created from existing users
- **Exclusive Assignment**: Prevents duplicate trainer assignments
- **User Data Integration**: Leverages existing user profiles

### Department Management
- **Department Assignment**: Each trainer assigned to a department
- **Department Context**: Department data available throughout views
- **Specialization Tracking**: Department serves as trainer specialization

## Reporting & Export Features

### Print
- **Purpose**: Printer-friendly trainer list
- **Attributes**: `[IgnoreAction]`
- **Features**: Same search functionality as Index without pagination

### createExcelReport_Download
- **Purpose**: Generate and download Excel reports
- **Excel Columns**:
  - Trainer Name (localized)
  - Phone Number
  - Email/Username
  - Specialization/Department (localized)
- **Process**:
  1. Applies same search filters as Index
  2. Maps combined trainer-user data to ExcelDataDTO
  3. Generates bilingual Excel file
  4. Returns timestamped XLSX file
- **File Naming**: "TrainersList_YYYYMMDD_HHmmss.xlsx"

## Advanced Data Handling

### Entity Join Implementation
```csharp
var trainersWithUsers = await _unitOfWork.Trainers
    .Table
    .Include(t => t.Courses)
    .Join(
        _unitOfWork.Users.Table,
        trainer => trainer.UserId,
        user => user.Id,
        (trainer, user) => new TrainerWithUserVM
        {
            Trainer = trainer,
            User = user
        }
    )
    .ToListAsync();