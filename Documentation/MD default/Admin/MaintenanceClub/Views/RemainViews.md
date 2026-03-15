# Maintenance Club Management System

## File Structure
- **Main View**: Index.cshtml
- **Partial View**: _ListPartial.cshtml  
- **Details View**: Details.cshtml
- **Model**: MaintenanceClubVM
- **Pagination**: PaginatedList<MaintenanceClubVM>

## Core Features

### Security & Permissions
- Role-based access control for Add, Edit, Delete operations
- Permission validation using PermissionScanner
- Secure action rendering based on user privileges

### Data Management
- Server-side pagination with configurable page sizes (50, 100, 150)
- Real-time record counting and display
- AJAX partial updates without page reload
- Persistent filter state using session storage

### User Interface Components

#### Navigation & Layout
- Hierarchical breadcrumb navigation
- Responsive card-based layout
- Bootstrap table design with hover effects
- Mobile-optimized data labels

#### Action Controls
- View details with eye icon
- Edit records with pencil icon (permission-based)
- Delete with confirmation modal (permission-based)
- Create new records button
- Tooltip-enabled action buttons

#### Filtering System
- Date range filtering with calendar inputs
- Text search across maintenance titles
- Visual filter form with clear/submit actions
- Persistent filter state across sessions

### Export Capabilities
- Print functionality with 4000 record limit
- Excel report generation and download
- Filter-aware exports (applies current search criteria)
- Details page printing

### Data Display
- Table columns: Location, Title, Date, Time, Actions
- Formatted date display (dd-MM-yyyy)
- Time display in 24-hour format (hh:mm)
- Empty state handling with create guidance
- PDF file attachment viewing

### Technical Implementation

#### JavaScript Functions
- Print page opener with validation
- Excel export handler
- Filter management with clear/save operations
- Dynamic partial view loading
- AJAX pagination handling
- Tooltip initialization

#### State Management
- Session storage for filter persistence
- Real-time record count updates
- Partial view reloading
- Modal confirmation dialogs

#### Integration Points
- Resource files for localization
- Validation scripts partial
- Anti-forgery token support
- Bootstrap modal system
- Font Awesome icons

## Page Components

### List View (_ListPartial)
- Paginated table display
- Page size selector
- Action buttons per row
- Empty state management

### Main Index Page
- Header with breadcrumb and record count
- Action toolbar (Create, Print, Excel)
- Advanced filtering form
- Partial view container
- Delete confirmation modal

### Details Page
- Read-only form display
- PDF file viewer
- Print functionality
- Back navigation

## Browser Compatibility
- Modern browser support
- Responsive design
- JavaScript dependency
- Session storage usage
- Bootstrap 5 compatibility