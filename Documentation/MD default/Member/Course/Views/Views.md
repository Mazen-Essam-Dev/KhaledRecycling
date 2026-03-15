# Course Views Documentation

## File Structure
- **Index.cshtml**: Main course listing page
- **PrintCertificate.cshtml**: Certificate generation and printing
- **Rate.cshtml**: Course rating interface
- **Subscribe.cshtml**: Course subscription form

## Index.cshtml

### Overview
Main course management interface displaying all available courses with subscription status, acceptance tracking, and comprehensive action controls

### Visual Design
- **Subscription Highlight**: Green background (#EAFDCB) for accepted courses
- **Badge System**: Visual subscription indicators
- **Tooltips**: Bootstrap tooltips for all action icons
- **Responsive Table**: Mobile-friendly course listing

### Table Structure
- **Course Date**: Start date in dd-MM-yyyy format
- **Department**: Language-specific department name
- **Location**: Course venue
- **Course Title**: Language-specific course title
- **Control Tools**: Comprehensive action buttons

### Action Icons & States
- **View Details**: Eye icon for course information modal
- **Subscribe**: File signature icon for new subscriptions
- **Rate**: Star icon for completed course rating
- **Rejected**: Close icon with rejection reason modal
- **Certificate**: Certificate icon for completion certification

### Modal Integration
- **Course Details Modal**: Comprehensive course information
- **Acceptance Modal**: Rejection status and notes display
- **Rating Modal**: 5-star rating system with visual feedback

## PrintCertificate.cshtml

### Overview
Professional certificate generation with dual-language support, gold-border design, and print optimization

### Certificate Design
- **Gold Border**: Premium certificate framing
- **Dual Logos**: Organization branding integration
- **Arabic Typography**: Traditional certificate styling
- **Responsive Layout**: Mobile and print compatibility

### Content Structure
- **Header**: Certificate title with gold styling
- **Participant Information**: Name with gender prefix
- **Course Details**: Title, dates, and participation statement
- **Official Seals**: Red and blue organizational stamps
- **Contact Footer**: Comprehensive organization contact information

### Print Optimization
- **A4 Landscape**: Professional certificate sizing
- **Auto-Print**: Automatic print dialog triggering
- **Loading States**: User feedback during generation
- **Cross-browser Support**: Consistent printing experience

## Rate.cshtml

### Overview
Visual course rating interface with emoji-based feedback system and immediate submission

### Rating System
- **4 Stars**: Excellent (Smiling face)
- **3 Stars**: Good (Grinning stars face)
- **2 Stars**: Fair (Neutral face)
- **1 Star**: Unhappy (Sad tear face)

### User Experience
- **Visual Feedback**: Solid/outline icon states
- **Immediate Submission**: Auto-submit on selection
- **Current Selection**: Preselected rating display
- **Responsive Layout**: Centered rating controls

## Subscribe.cshtml

### Overview
Course subscription form with conditional document uploads and terms agreement

### Form Features
- **Read-only Course Info**: Display-only course details
- **Conditional Uploads**: Dynamic document requirements
- **Terms Agreement**: Mandatory consent checkbox
- **Document Validation**: ID and passport upload controls

### Conditional Logic
- **ID Card Upload**: Required if missing or expired
- **Passport Upload**: Required if missing
- **Dynamic Fields**: Show/hide based on user document status
- **Validation States**: Clear error messaging

## Common Technical Features

### Multi-language Support
- **Session-based Language**: Dynamic content switching
- **Resource Files**: Comprehensive localization (Resource1, Resource2, Resource3)
- **Culture-specific Formatting**: Date and text formatting

### JavaScript Enhancements
- **Modal Management**: Dynamic content loading
- **Form Validation**: Client-side validation scripts
- **AJAX Integration**: Asynchronous operations
- **SweetAlert2**: Modern alert dialogs

### Bootstrap Integration
- **Responsive Grid**: Mobile-first design
- **Modal System**: Consistent modal styling
- **Form Controls**: Standardized input styling
- **Tooltip System**: Action explanations

## Security & Validation

### Access Control
- **Member Authorization**: Protected member area
- **Anti-forgery Tokens**: Form submission protection
- **Session Security**: Secure session data handling

### Business Logic Enforcement
- **Subscription Rules**: Prevent duplicate subscriptions
- **Rating Eligibility**: Only completed courses can be rated
- **Certificate Access**: Only successful completions
- **Document Requirements**: Conditional upload enforcement

## Integration Patterns

### Controller Integration
- **ViewModel Mapping**: Course to CourseVM conversion
- **Service Layer**: CourseService business logic
- **Session Management**: Member context handling

### Resource Management
- **Localized Strings**: Comprehensive resource usage
- **Dynamic Content**: Language-specific displays
- **Consistent Terminology**: Standardized resource keys

### Print Integration
- **Certificate Generation**: Dynamic certificate creation
- **Browser Print API**: Native printing functionality
- **PDF-ready Design**: Print-optimized styling