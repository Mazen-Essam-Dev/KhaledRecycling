# Trainer Management System - File Documentation

## Index.cshtml

### Page Configuration
- **Purpose**: Main trainer management interface with search, filtering, and data display
- **View Model**: `TrainerIndexVM`
- **Title Configuration**: 
  - Main Title: "Trainers" from Resource2
  - Package Title: "Activities Management" from Resource1  
  - Single Title: "Trainers List" from Resource1

### Permission Management
- **Edit Permission**: Validates "Trainer/Edit" permission
- **Delete Permission**: Validates "Trainer/Delete" permission  
- **Add Permission**: Validates "Trainer/Add" permission
- **Language Support**: Arabic/English based on session language

### Layout Structure
- **Breadcrumb Navigation**: Activities Management → Trainers List
- **Record Counter**: Displays total record count with dynamic updates
- **Action Buttons**: 
  - Create New (conditional on add permission)
  - Print button with 4000 record limit validation
  - Excel export functionality

### Search & Filter System
- **Search Input**: Text search by trainer name with placeholder text
- **Form Handling**: GET method form submission
- **Clear Filters**: Button to reset search criteria
- **AJAX Integration**: Dynamic partial page updates without full reload

### JavaScript Functionality
- **Print Management**: Window popup with search term parameters
- **Excel Export**: Direct download with search filtering
- **Session Storage**: Persists filter state across page reloads
- **AJAX Pagination**: Dynamic page loading with preserved filters
- **Tooltip Management**: Reinitializes tooltips after AJAX calls

## _ListPartial.cshtml

### Partial View Configuration
- **Purpose**: Reusable trainer list component with pagination
- **View Model**: `TrainerIndexVM`
- **Hidden Field**: Stores record count for JavaScript access

### Page Size Selector
- **Options**: 50, 100, 150 records per page
- **Dynamic Selection**: Maintains current page size selection
- **AJAX Integration**: Triggers partial reload on size change

### Table Structure
- **Columns**: 
  - Trainer Name (clickable for edit navigation)
  - Phone Number
  - Email Address
  - Specialization/Department
  - Action buttons

### Data Display Features
- **Bilingual Support**: Shows Arabic/English names based on language
- **Empty State**: Custom no-data message with icon
- **Row Navigation**: Click trainer name to navigate to edit page

### Action Buttons
- **Edit Button**: Conditionally shown based on edit permission
- **Delete Button**: 
  - Enabled only if trainer has no courses
  - Shows confirmation dialog
  - Displays modal if deletion restricted
- **Tooltips**: Hover descriptions for all action buttons

### Pagination System
- **Previous/Next**: Conditional disabled states
- **Page Numbers**: Dynamic page number generation
- **Active State**: Highlights current page
- **AJAX Navigation**: Page changes without full reload

### Delete Restriction Modal
- **Trigger**: When trainer has associated courses
- **Message**: "Can't Delete Trainer" with explanation
- **User Experience**: Prevents invalid deletion attempts

## AddEdit.cshtml

### Form Configuration
- **Purpose**: Create and edit trainer profiles
- **View Model**: `TrainerVM`
- **Mode Detection**: Auto-detects create/edit mode by ID presence
- **Dynamic Titles**: "Create Trainer" or "Edit Trainer" based on mode

### Layout Structure
- **Header**: Icon-based title with breadcrumb navigation
- **Form Container**: Card-based layout with proper spacing
- **Action Buttons**: Save and Back to list

### Form Fields
- **User Selection**: Dropdown for user association with validation
- **Department Selection**: Specialization dropdown with validation
- **Bio/Profile**: Textarea for trainer biography
- **File Upload**: PDF attachment with size limit (5MB)

### File Management
- **PDF Restriction**: Accepts only PDF files
- **Existing File Display**: View link for existing attachments
- **Size Validation**: Clear 5MB limit indication
- **Upload Interface**: Custom styled file input

### Validation & UX
- **Client Validation**: ASP.NET validation spans
- **Visual Feedback**: Proper form labeling and error display
- **Navigation**: Consistent back button to main list

## Print.cshtml

### Print Layout
- **Purpose**: Printer-friendly trainer list report
- **Layout**: Null layout for clean print output
- **Language Support**: RTL/LTR CSS based on current language

### Header Structure
- **Bilingual Organization**: UAE/Fujairah Science Club in Arabic and English
- **Logo Integration**: Center-aligned organization logo
- **Report Title**: "Trainers" with timestamp

### Table Design
- **Print Optimized**: Clean, bordered table with proper alignment
- **Column Structure**: Name, Phone, Email, Specialization
- **Data Formatting**: Language-appropriate name display

### Loading Management
- **Loading Overlay**: Visual feedback during report generation
- **Timed Display**: Automatic overlay removal and print triggering
- **Auto-Close**: Window closure after printing completion

### Footer Information
- **Audit Trail**: Printed by user email and timestamp
- **Time Standard**: Uses Dubai time zone for consistency
- **Print Metadata**: Includes date/time of report generation

## Key Technical Features

### Security Implementation
- **Permission-based UI**: Buttons show/hide based on user permissions
- **Validation**: Server and client-side form validation
- **Safe Deletion**: Prevents deletion of trainers with courses

### Performance Optimizations
- **AJAX Partial Updates**: Reduces page reload overhead
- **Session Storage**: Maintains user filter preferences
- **Efficient Pagination**: Server-side pagination for large datasets

### User Experience
- **Responsive Design**: Works across desktop and mobile devices
- **Visual Feedback**: Loading states, tooltips, and clear messaging
- **Accessibility**: Proper labels and keyboard navigation support

### Internationalization
- **Resource Files**: Centralized string management
- **RTL Support**: Full Arabic language compatibility
- **Bilingual Data**: Displays appropriate language content