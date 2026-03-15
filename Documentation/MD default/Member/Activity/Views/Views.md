# Activity Views Documentation

## File Structure
- **Index.cshtml**: Main activity listing page
- **_DetailsPartial.cshtml**: Activity details modal partial view
- **Create.cshtml**: Activity creation form (admin)
- **Subscribe.cshtml**: Activity subscription form

## Index.cshtml

### Overview
Main activity listing page displaying all available activities with subscription status, date information, and action controls

### Layout Features
- **Subscriber Badge**: Green badge indicating subscription status
- **Responsive Table**: Bootstrap-styled activity list
- **Empty State**: Graceful handling for no activities
- **Language Support**: Dynamic Arabic/English content

### Table Structure
- **Activity Date**: Start date in dd-MM-yyyy format
- **Duration**: Calculated days between start and end dates
- **Activity Title**: Language-specific title display
- **Age Requirement**: Minimum age with "Above X Year" format
- **Location**: Activity venue
- **Actions**: View details and subscription buttons

### Visual Design
- **Subscription Highlight**: Green background (#EAFDCB) for subscribed activities
- **Tooltips**: Bootstrap tooltips for action icons
- **Icons**: Font Awesome icons for actions
- **Badge System**: Visual subscription indicators

### JavaScript Functionality
- **Age Validation**: CheckAge() function with SweetAlert2 integration
- **Modal Loading**: Dynamic activity details modal
- **AJAX Integration**: Asynchronous detail loading
- **Error Handling**: Comprehensive error messages

## _DetailsPartial.cshtml

### Overview
Modal partial view for displaying comprehensive activity details in read-only format

### Form Structure
- **Title**: Language-specific activity title
- **Dates**: Start and end dates in separate fields
- **Age Requirement**: Formatted age restriction
- **Location**: Activity venue
- **Description**: Full activity description textarea
- **Attachment**: Downloadable file link with disabled state for missing files

### Technical Features
- **Read-only Form**: All inputs disabled for viewing only
- **File Handling**: Conditional attachment display
- **Modal Integration**: Designed for Bootstrap modal
- **Language Switching**: Dynamic field labels based on session language

## Create.cshtml

### Overview
Administrative form for creating new activities with comprehensive validation

### Form Fields
- **TitleAr**: Arabic title with regex validation
- **TitleEn**: English title with regex validation
- **StartDate**: Date picker for activity start
- **EndDate**: Date picker for activity end
- **MinimumAge**: Numeric age requirement
- **Location**: Activity venue location
- **Description**: Detailed activity description
- **AttachmentPath**: File attachment path

### Validation
- **Data Annotations**: Client-side validation
- **Model Validation**: Server-side validation summary
- **Required Fields**: All essential activity information

## Subscribe.cshtml

### Overview
Activity subscription confirmation page with terms agreement and read-only activity details

### Form Features
- **Read-only Display**: Activity details in disabled form
- **Terms Agreement**: Checkbox for subscription confirmation
- **Language-specific Title**: Dynamic title based on session
- **Date Display**: Formatted start and end dates

### User Experience
- **Conditional Submission**: Submit button disabled until terms accepted
- **Navigation**: Back to activities list option
- **Validation**: Client-side form validation
- **Clear Instructions**: Subscription terms and conditions

## Common Technical Features

### Multi-language Support
- **Session-based Language**: Uses SessionHelper.GetCurrentLanguage()
- **Dynamic Text**: Resource file integration (Resource1, Resource2, Resource3)
- **Culture-specific Formatting**: Date and number formatting

### Bootstrap Integration
- **Responsive Design**: Mobile-friendly table and forms
- **Modal System**: Bootstrap modal for details
- **Form Controls**: Consistent Bootstrap styling
- **Tooltip System**: Action explanations

### JavaScript Enhancements
- **SweetAlert2**: Modern alert dialogs for age validation
- **AJAX Loading**: Dynamic content loading for modals
- **Form Validation**: Client-side validation scripts
- **Conditional Logic**: Dynamic button states

## Security & Validation

### Access Control
- **Member Authorization**: MemberAuthorize attribute protection
- **Session Management**: Secure session data handling
- **Anti-forgery Tokens**: Form submission protection

### Business Logic
- **Age Validation**: Server-side age requirement checking
- **Date Validation**: End date after start date enforcement
- **Subscription Limits**: Prevents duplicate subscriptions
- **Activity Status**: Only active activities can be subscribed

## Integration Patterns

### Controller Integration
- **ViewModel Mapping**: Activity to ActivityVM conversion
- **Service Layer**: ActivityService for business logic
- **Session Management**: Member email retrieval from session

### Resource Management
- **Localized Strings**: Comprehensive resource file usage
- **Dynamic Content**: Language-specific field display
- **Consistent Terminology**: Standardized resource keys