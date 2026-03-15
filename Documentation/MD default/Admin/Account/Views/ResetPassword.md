# ResetPassword View Documentation

## Overview
Razor view for secure password reset functionality in the admin area, providing a clean and user-friendly interface for administrators to reset user passwords.

## View Configuration
- **Model**: ResetPasswordVM
- **Title**: "ResetPassword" from Resource2
- **Context**: Users Management Unite
- **Layout**: Responsive Bootstrap design with proper form validation

## User Interface Structure

### Breadcrumb Navigation
- **Hierarchy**: Users Management Unite → ResetPassword
- **Visual Design**: Includes custom separator with distinct color (#931c11)
- **Navigation**: Clear path indication for user orientation within the admin area

### Form Layout
- **Form ID**: Implicit form with POST action to ResetPassword
- **Grid System**: Responsive layout with proper column spacing
- **Validation**: Comprehensive client and server-side validation

## Core Form Fields

### User Identification
- **Email Field**:
  - Read-only input displaying user's email
  - Serves as visual confirmation of target user
  - Pre-filled from model data
  - Proper localization with Resource2.Email

### Password Management
- **New Password Field**:
  - Label: "NewPassword" from Resource2
  - Password type input with eye toggle functionality
  - Custom CSS styling with right-aligned eye icon
  - Proper validation messaging

- **Confirm Password Field**:
  - Label: "ConfirmNewPassword" from Resource2  
  - Mirrors new password field design
  - Ensures password confirmation matches
  - Client-side matching validation

### Hidden Fields
- **User ID**: Hidden input storing Model.Id for backend processing
- **Security**: Prevents tampering with user identification

## Technical Implementation

### Password Visibility Toggle
```javascript
function togglePassword(inputId, iconElement) {
    const input = document.getElementById(inputId);
    if (input.type === "password") {
        input.type = "text";
        iconElement.classList.remove("fa-eye");
        iconElement.classList.add("fa-eye-slash");
    } else {
        input.type = "password";
        iconElement.classList.remove("fa-eye-slash");
        iconElement.classList.add("fa-eye");
    }
}