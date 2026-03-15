# Activity Management System

## File Structure
- **Main View**: Index.cshtml
- **List Partial**: _ListPartial.cshtml
- **Members Activity**: MembersActivity.cshtml
- **Members List Partial**: _ListPartialMemberSubsInEvents.cshtml
- **Model**: ActivityVM, SubscribedMemberInActivitiesVM
- **Pagination**: PaginatedList<T>

## Core Features

### Security & Permissions
- Role-based access control for Activity operations
- Permission validation for Add, Edit, Delete, MembersActivity
- Secure action rendering based on user privileges

### Activity Management

#### Data Display
- Bilingual support (Arabic/English) for activity titles
- Table columns: Title, Start Date, Location, Duration, Ages, Subscription Count
- Date formatting (dd-MM-yyyy)
- Age restriction display with minimum age
- Real-time subscription count tracking

#### Action Controls
- Members Activity access for subscribed activities
- Details modal with AJAX loading
- Edit restriction based on end date
- Delete restriction for activities with subscribers
- Tooltip-enabled action buttons

#### Business Logic
- Edit disabled for past activities
- Delete prevented for activities with subscribers
- Members activity access only for subscribed activities
- Current date validation using AppDubaiTime

### Members Activity Management

#### Participant Display
- Member information: Name, Nationality, Subscription Date, Age, Contact
- Bilingual member name display
- Nationality display in current language
- Subscription history with participation dates

#### Activity Summary
- Activity title in current language
- Start and end date display
- Participant count tracking
- Card-based summary layout

### Data Management
- Server-side pagination with configurable sizes (50, 100, 150)
- Real-time record counting
- AJAX partial updates without page reload
- Persistent filter state using session storage

### User Interface Components

#### Navigation & Layout
- Hierarchical breadcrumb navigation
- Responsive card-based layout
- Bootstrap table design with striping
- Mobile-optimized data labels

#### Filtering System
- Text search across activity titles
- Date range filtering (From/To)
- Visual search box with icon
- Persistent filter state
- One-click filter clearing

### Export Capabilities
- Print functionality with 4000 record limit
- Excel report generation and download
- Filter-aware exports
- Members activity specific exports
- Print restriction validation

### Technical Implementation

#### JavaScript Functions
- Print page opener with record limit validation
- Excel export handlers
- Filter management with session storage
- Dynamic partial view loading
- Modal content loading via AJAX
- Tooltip initialization

#### State Management
- Session storage for filter persistence
- Real-time record count updates
- Partial view reloading
- Pagination state maintenance

#### Integration Points
- Resource files for localization
- Current language detection
- Time zone handling (AppDubaiTime)
- Anti-forgery token support
- Bootstrap modal system

## Page Components

### Activity List Partial (_ListPartial)
- Paginated activity table
- Conditional action buttons
- Empty state management
- Page size selector

### Main Activity Index
- Header with breadcrumb and record count
- Action toolbar (Create, Print, Excel)
- Advanced filtering form
- Details modal container
- Partial view container

### Members Activity Page
- Activity summary cards
- Participants list table
- Export controls with validation
- Paginated member display

### Members List Partial
- Participant information table
- Bilingual data display
- Subscription date formatting
- Pagination controls

## Browser Compatibility
- Modern browser support
- Responsive design
- JavaScript dependency
- Session storage usage
- Bootstrap 5 compatibility
- Font Awesome icons