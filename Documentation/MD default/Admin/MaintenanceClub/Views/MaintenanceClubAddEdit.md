# Maintenance Club Views Documentation

## Overview
This is a Razor view file for a maintenance club management system, used for adding or editing maintenance club records.

## File Structure
File Type: ASP.NET Core Razor View (.cshtml)

## Model: MaintenanceClubVM

- **Purpose** : Form for creating/editing maintenance club entries

## Key Components

### Layout Sections
### Header Section
```
Page title with icon
Breadcrumb navigation
Form Section
Hidden ID field
Various input fields for maintenance data
File upload for PDF documents with 5 Mb maximum
Action buttons (Save/Back)
```

### Form Fields 
```
Field	Type	Description
Title	Text	Maintenance title
Date	Date	Maintenance date with calendar icon
Time	Time	Maintenance time
Location	Text	Maintenance location
Details	Textarea	Maintenance details
PdfFile	File	PDF file upload (max 5MB)
```
### Features
- **Dynamic Titles**: Changes based on Add/Edit mode

- **File Preview**: View existing PDF files

- **Validation**: Client-side validation with error messages

- **Responsive Design**: Uses Bootstrap grid system

- **Localization Support**: Uses resource files for multilingual support

### Actions
- **Save**: Submit form data

- **Back**: Return to index page

### Technical Details
This view integrates with a backend controller action AddEdit and follows ASP.NET Core MVC patterns for form handling and validation. The form includes:

- **Model Binding**: Uses asp-for tag helpers

- **File Upload**: Supports PDF files with size validation

- **Client Validation**: Uses ASP.NET Core validation attributes

- **Responsive Layout**: Bootstrap-based responsive design

- **Icon Integration**: Font Awesome icons for UI elements
