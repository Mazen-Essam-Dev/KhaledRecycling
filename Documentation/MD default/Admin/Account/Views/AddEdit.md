# Account AddEdit View Documentation

## Overview
Razor view for creating and editing user accounts in the admin area, featuring comprehensive form handling, signature management with drawing capabilities, and responsive design.

## View Configuration
- **Model**: AdminVM
- **Title**: Dynamic based on create/edit mode
- **Localization**: Uses Resource files for multilingual support
- **Layout**: Responsive Bootstrap design

## User Interface Structure

### Breadcrumb Navigation
- **Hierarchy**: Users Management Unite → Create/Edit Account
- **Navigation**: Clear path indication for user orientation

### Form Layout
- **Grid System**: 2-column responsive layout (col-md-6)
- **Form ID**: `mainForm` with multipart form-data support
- **Validation**: Client-side validation with error display

## Core Form Fields

### Personal Information
- **FullNameAr**: Arabic full name with validation
- **FullNameEn**: English full name with validation
- **Username**: Unique identifier with remote validation
- **Email**: Email address with format validation
- **PhoneNumber**: Numeric input with UAE formatting

### Security Fields
- **PasswordHash**: 
  - Required for new users
  - Eye toggle for visibility
  - Password strength guidance
- **ConfirmPassword**: 
  - Password confirmation
  - Matching validation
  - Visibility toggle

### Role Management
- **RoleId**: Dropdown selection from RolesList
- **Options**: Pre-populated with available roles
- **Validation**: Required field with error messaging

## Advanced Signature Management

### Dual Input Methods
- **Upload Method**: File upload with image preview
- **Drawing Method**: Canvas-based signature pad
- **Modal Interface**: Tabbed modal for method selection

### Signature Modal Features
- **Upload Tab**: 
  - File input with image acceptance (.jpg, .jpeg, .png)
  - 3MB size limit indication
  - Automatic preview generation
- **Draw Tab**:
  - HTML5 Canvas drawing interface
  - Touch and mouse support
  - Clear and save functionality
  - Real-time drawing with stroke customization

### Signature Preview & Management
- **Live Preview**: Immediate image preview after upload/drawing
- **Existing Signature**: Display of previously saved signatures
- **Save Mechanism**: AJAX-based signature saving
- **Visual Feedback**: Success/error alerts with localization

## Technical Implementation

### JavaScript Functionality

#### Password Visibility Toggle
```javascript
function togglePassword(inputId, button) {
    // Toggles between password and text input
    // Updates eye icon accordingly
}