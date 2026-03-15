# MemberController Documentation

## Overview
ASP.NET Core Controller for comprehensive member management with course enrollment tracking, multi-criteria filtering, and extensive reporting capabilities in a club management system.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - IMemberService: Core member business logic
  - IUnitOfWork: Database operations
  - IMapper: AutoMapper for object mapping
  - ICourseService: Course enrollment management

## Core Member Operations

### Index
- **Purpose**: Main member listing with advanced multi-criteria filtering
- **Filters**:
  - Search term (Name, ID, Phone, Code)
  - Nationality dropdown
  - Gender dropdown (enum-based)
  - Date range (RegistrationDate)
- **Pagination**: 50 items per page
- **Course Tracking**: Displays member course enrollment status
- **AJAX Support**: Partial view for AJAX requests

### AddEdit (GET)
- **Purpose**: Display form for creating/editing members
- **Navigation Tracking**: Session-based return URL management
- **Auto-Code Generation**: Automatic member code assignment for new members
- **Dropdown Population**: Nationalities and cities from database

### AddEdit (POST)
- **Purpose**: Handle member creation and updates
- **Phone Validation**: Automatic UAE phone number formatting (+971)
- **Navigation Logic**: Session-based redirect after operations
- **Form Validation**: ModelState validation with dropdown repopulation

### Delete & Suspend
- **Delete**: Permanent member removal
- **Suspend**: Toggle member suspension status via AJAX

## Course Management Integration

### MemberCourses
- **Purpose**: Display all courses enrolled by specific member
- **Pagination**: 50 courses per page
- **Member Context**: Shows member name in UI
- **AJAX Support**: Partial view rendering

### GetCourse & MemberCourseDetails
- **Purpose**: AJAX endpoints for course information
- **Data**: Course details with department information
- **File Handling**: Attachment path validation and formatting

## Comprehensive Reporting System

### Print Operations
- **Print**: Printer-friendly member list with same filters as Index
- **PrintDetails**: Individual member details for printing
- **PrintMemberCourses**: Member's course enrollment report

### Excel Export Operations
- **createExcelReport_Download**: Member list export with comprehensive data
- **createExcelReport_Download_MemberCourse**: Member-specific course export
- **Excel Columns**:
  - Member Export: Code, Name, ID, Phone, Nationality, Age, Registration Date
  - Course Export: Start Date, Department, Location, Course Title
- **Bilingual Support**: Arabic/English based on session language

## View-Only Operations

### ViewData
- **Purpose**: Read-only member details view
- **Features**: All member information without edit capabilities
- **Dropdowns**: Pre-populated for display purposes

## Advanced Filtering System

### Multi-Criteria Search
- **Text Search**: Name (AR/EN), ID Number, Phone, Member Code
- **Dropdown Filters**: Nationality, Gender (enum-based)
- **Date Range**: Registration date filtering
- **Persistence**: Filter parameters maintained in ViewBag

### Dropdown Management
- **Nationalities**: Database-driven from Nationalities table
- **Cities**: Database-driven from Cities table
- **Enums**: Gender, Profession, HeardBySources via SelectListHelper

## Phone Number Processing

### UAE Phone Formatting
```csharp
entity.PhoneNumber = entity.PhoneNumber?.Replace(" ", "");
if (entity.PhoneNumber != null && !entity.PhoneNumber.StartsWith("971"))
{
    entity.PhoneNumber = "971" + entity.PhoneNumber;
}