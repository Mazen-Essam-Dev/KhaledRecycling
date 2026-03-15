# Car Service Management System

## File Structure
- **Main View**: Index.cshtml
- **List Partial**: _ListPartial.cshtml
- **Add/Edit Form**: AddEdit.cshtml
- **Model**: CarServiceVM, PaginatedList<CarServiceVM>

## Core Features

### Vehicle Maintenance Management
- Comprehensive car service and maintenance tracking
- Integration with car fleet management system
- Service documentation and record keeping
- Maintenance history and scheduling

### Security & Permissions
- Role-based access control for service operations
- Permission validation for Add, Edit, Delete operations
- Integration with car management permissions
- Secure action rendering based on user privileges

## Data Management

### Service Information Fields
- **CarId**: Vehicle association (dropdown selection)
- **Date**: Service date with calendar input
- **Details**: Maintenance details and description
- **Attachment**: PDF service documentation
- **AttachmentPath**: Stored file path

### File Management
- PDF file upload support for service documentation
- 5MB file size limit
- File existence validation
- Secure file path handling
- File preview capability

## User Interface Components

### Navigation & Layout
- Hierarchical breadcrumb navigation within Cars Management
- Dynamic page titles (Create Service/Edit Service)
- Card-based form and list layout
- Responsive table design with striped rows
- Mobile-optimized data labels

### Form Management (AddEdit)
- Dynamic form titles based on create/edit mode
- Car selection dropdown with conditional editing
- Date input with calendar icon
- Service details text area
- File upload with visual feedback
- Existing file preview with view option
- Form validation and error display

### List Display (Index & _ListPartial)
- Paginated service listing
- Configurable page sizes (50, 100, 150)
- Advanced filtering capabilities
- Date range filtering for service dates
- Car type-based filtering
- Action buttons with tooltips

## Filtering & Search System

### Advanced Filtering Functionality
- Date range filtering for service dates
- Car type filtering dropdown
- Persistent filter state using session storage
- Clear filter functionality
- Real-time filter results

### Filter Components
- Date range pickers with calendar icons
- Car type dropdown for vehicle categorization
- Combined filter parameter handling
- Session-based filter persistence

## Export Capabilities

### Print Functionality
- Print with all applied filters (date range, car type)
- 4000 record limit validation
- New window print delivery
- Filter-aware printing

### Excel Export
- Excel report generation with applied filters
- Direct download functionality
- Comprehensive service data export
- Date range and car type parameter inclusion

## Technical Implementation

### JavaScript Functions
- Print and Excel export handlers with filter parameters
- AJAX partial view loading
- Session storage for filter persistence
- Real-time record count updates
- Dynamic pagination handling
- Tooltip initialization

### State Management
- Session storage for comprehensive filter state
- Real-time UI updates
- Partial view reloading without page refresh
- Multi-parameter filter persistence

### File Handling
- PDF file upload with size validation
- File existence checking
- Secure file path management
- File preview in new tab

## Business Logic

### Service Operations
- Create new service records with car association
- Edit existing service information
- Delete service records
- Comprehensive service documentation

### Car Integration
- Car selection with dropdown population
- Conditional car field editing (locked during edit mode)
- Car type-based filtering
- Vehicle information display in listings

### Service Management
- Service date tracking and filtering
- Maintenance details documentation
- Service attachment management
- Historical service tracking

## Integration Points
- Resource files for multilingual support
- Permission-based UI rendering
- Bootstrap modal system
- jQuery AJAX for dynamic content
- Session storage for state persistence
- Validation scripts partial
- File helper utilities
- Car management module integration

## Page Components

### Main Index
- Advanced filtering interface
- Action toolbar (Add Service, Print, Excel)
- Records count display
- Partial view container
- Delete information modal

### AddEdit Form
- Comprehensive service form layout
- Dynamic title (Create Service/Edit Service)
- Conditional car selection (editable during create, locked during edit)
- File upload with preview capability
- Calendar date inputs with icons
- Form validation and error display
- Save and navigation actions

### List Partial
- Paginated table display with service details
- Essential service information (Car Type, Details, Date)
- Action buttons per row (View PDF, Edit, Delete)
- Empty state handling
- Page size selector and pagination controls

## Responsive Design
- Mobile-optimized table layouts with data labels
- Responsive form design
- Adaptive action buttons
- Flexible filter layouts for different screen sizes
- Bootstrap-based responsive grid system

## Security Features
- File upload restrictions (PDF only, 5MB limit)
- Secure file path handling
- Permission-based action visibility
- Input validation and sanitization
- Anti-forgery token protection
- Conditional field editing based on context

## Service Documentation
- PDF attachment support for service records
- File preview functionality
- Document management and organization
- Service history documentation