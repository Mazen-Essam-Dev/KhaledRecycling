# Member Views Documentation

## Overview
Complete member management interface including registration, authentication, profile editing, and printable reports. All views support bilingual (Arabic/English) functionality with comprehensive form validation.

## Register.cshtml

### Layout & Structure
- **Layout**: `_AuthLayout` for authentication pages
- **Title**: "Create New Account" from Resource2
- **Language Support**: Dynamic RTL/LTR CSS based on session language
- **External Libraries**: Select2 for enhanced dropdowns, jQuery for interactions

### Form Sections

#### Personal Information
- **Basic Data**: Member code (auto-generated, hidden), bilingual names, gender selection, nationality dropdown, date of birth with age calculation
- **Contact Details**: Phone numbers with guardian fields that show/hide based on age
- **Location**: City selection and address

#### ID Information
- **UAE ID Format**: Four-segment input (XXX-XXXX-XXXXXXX-X) with auto-advance functionality
- **Validation**: 18-character exact format with proper segmentation
- **Expiry Date**: Future date validation

#### Qualifications & Social
- **Education**: Academic qualification and institution
- **Interests**: Hobbies and languages with larger text areas
- **Social Media**: Facebook, X (Twitter), Instagram profiles
- **Marketing**: How member heard about club and media license agreement

#### Authentication Data
- **Email**: Unique validation with proper email format
- **Password**: Strong password policy with confirmation
- **Security**: Password visibility toggle with eye icons

#### File Attachments
- **Profile Image**: Selfie photograph
- **ID Image**: National identification document
- **Passport Image**: Passport document
- **Validation**: File type and size restrictions

### JavaScript Functionality

#### Form Management
- **Terms Agreement**: Submit button enabled only after accepting terms
- **Temporary File Cleanup**: Automatic cleanup of unsaved images using navigator.sendBeacon
- **Duplicate Prevention**: Prevents multiple submissions

#### Dynamic Field Management
- **Age Calculation**: Automatic age calculation from date of birth
- **Guardian Fields**: Father/mother phone numbers show only for members under 18
- **Profession Logic**: Different profession fields based on employment status

#### Input Validation
- **ID Number Segmentation**: Four-part input with auto-advance and backspace navigation
- **Numeric Only**: Phone number fields restrict to digits only
- **Password Toggle**: Show/hide password functionality

## Edit.cshtml

### Profile Editing Features
- **Read-only Fields**: Code, email, names, nationality, gender, date of birth cannot be changed
- **Editable Fields**: Phone numbers, address, qualifications, social media, password
- **Conditional Logic**: Same dynamic field management as registration

### Image Management
- **Current Image Display**: Shows existing images with modal previews
- **Update Capability**: Allows replacing existing images
- **File Validation**: Maintains same validation as registration

### Print Integration
- **Print Button**: Direct access to printable member details
- **Styled Interface**: Consistent with application theme

## Login.cshtml

### Authentication Interface
- **Standalone Layout**: No master layout for clean login experience
- **Minimal Design**: Focused on essential login fields
- **Loading States**: Animated submit button with loading indicator

### Features
- **Session Clearance**: Clears storage on login attempt
- **Password Toggle**: Visibility control for password field
- **Responsive Validation**: Error handling with proper user feedback
- **Registration Link**: Easy access to account creation

## PrintDetails.cshtml

### Printable Report
- **Bilingual Header**: UAE/Fujairah Science Club in Arabic and English
- **Comprehensive Layout**: Two-column responsive design for all member data
- **Image Integration**: Profile, ID, and passport images included
- **Auto-Print**: Automatically triggers print dialog on load

### Data Sections
- **Personal Details**: Code, names, nationality, gender, birth details
- **Contact Information**: Phone, city, address, email
- **Identification**: ID number and expiry date
- **Education & Interests**: Qualifications, hobbies, languages
- **Social & Marketing**: Social media profiles and referral source

### Print Optimization
- **Clean Layout**: Print-friendly styling with proper margins
- **Auto-Close**: Window closes after printing completion
- **Date Stamping**: Current date display on report

## Password.cshtml

### Password Management
- **Change Interface**: Old password, new password, confirmation fields
- **Security Standards**: Same password policy as registration
- **Update Process**: Dedicated password change functionality

## Common Features Across Views

### Validation System
- **Client-Side**: jQuery Validation with localized messages
- **Server-Side**: Model state validation with proper error display
- **Business Rules**: Age restrictions, ID expiry, format requirements

### Internationalization
- **Resource Files**: All text from Resource1, Resource2, Resource3
- **RTL Support**: Full Arabic language compatibility
- **Culture Awareness**: Date formats, text direction, validation messages

### Security Measures
- **Anti-Forgery Tokens**: CSRF protection on all forms
- **File Type Validation**: Image format and size restrictions
- **Input Sanitization**: Proper encoding and format validation

### User Experience
- **Progressive Disclosure**: Fields show/hide based on user input
- **Immediate Feedback**: Real-time validation and calculations
- **Accessibility**: Proper labels, keyboard navigation, visual feedback

## Technical Implementation

### CSS & Styling
- **Bootstrap 5**: Responsive grid system and components
- **Custom Themes**: Consistent color scheme and branding
- **Print Optimization**: Separate styles for printable views

### JavaScript Libraries
- **jQuery**: DOM manipulation and event handling
- **Select2**: Enhanced dropdown functionality
- **Font Awesome**: Consistent iconography

### File Handling
- **Upload Interface**: Custom styled file inputs with preview
- **Path Management**: Server-side path storage and retrieval
- **Validation**: Client and server-side file validation

### Form Management
- **Dynamic Validation**: Conditional required fields
- **State Persistence**: Maintains form state during validation
- **Error Handling**: Clear, localized error messages