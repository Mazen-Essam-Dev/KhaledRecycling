# Activity Form - Razor View Documentation

## File Overview
- **Type**: ASP.NET Core Razor View (.cshtml)
- **Area**: Admin
- **ViewModel**: ActivityVM
- **Purpose**: Create/Edit activity form with bilingual support

## View Configuration
- **Dynamic Title**: Changes between Edit/Create based on Model.Id
- **Breadcrumb Navigation**: Shows activity management hierarchy
- **Form Action**: Posts to "AddEdit" method with file upload support

## Form Structure

### Header Section
- Book icon with dynamic title
- Responsive breadcrumb navigation

### Input Fields (Grid Layout)

#### Basic Information
- **TitleAr**: Arabic title input (required)
- **TitleEn**: English title input (required)
- **Location**: Activity location (required)
- **MinimumAge**: Age restriction input

#### Date Fields
- **StartDate**: Calendar input with icon
- **EndDate**: Calendar input with custom validation
- **Validation**: End date must be after start date

#### Content Areas
- **Description**: Details textarea (3 rows)
- **Achievement**: Achievement textarea (3 rows)

#### File Upload
- **Type**: PDF files only
- **Max Size**: 5MB
- **Features**: Upload icon, existing file preview in edit mode

### Action Buttons
- **Save**: Floppy disk icon with localized text
- **Back**: Success-styled button returning to Index

## Validation & Scripts

### Client-Side Validation
- ASP.NET Core validation partial
- Custom date validation JavaScript
- Real-time date comparison
- Error message display handling

### Date Validation Script
- Prevents end date before start date
- Dynamic error message display
- Input event listeners for real-time validation

## Styling & UI Features
- Bootstrap grid system for responsiveness
- Custom CSS classes for icons and borders
- Font Awesome icons throughout
- RTL/LTR support for file upload
- Error states with visual feedback

## Localization
Uses multiple resource files:
- Resource1: Activity-specific texts
- Resource2: Form labels and buttons  
- Resource3: File-related messages

## Security & Constraints
- Anti-forgery tokens
- File type restriction (.pdf only)
- File size limitation (5MB)
- Input validation on client and server