# Car Management System

## File Structure
- **Main View**: Index.cshtml
- **List Partial**: _ListPartial.cshtml
- **Add/Edit Form**: AddEdit.cshtml
- **Model**: CarVM, PaginatedList<CarVM>

## Core Features

### Vehicle Management
- Comprehensive car fleet management system
- Vehicle registration and documentation
- Ownership and maintenance tracking
- Multi-dimensional vehicle data organization

### Security & Permissions
- Role-based access control for car operations
- Permission validation for Add, Edit, Delete operations
- Car service management permissions integration
- Secure action rendering based on user privileges

## Data Management

### Vehicle Information Fields
- **PlateNumber**: License plate identification
- **Type**: Vehicle type/category
- **DriverName**: Assigned driver information
- **OwnershipExpiryDate**: Vehicle registration expiry
- **Model**: Vehicle model information
- **Color**: Vehicle color
- **Notes**: Additional vehicle notes
- **Attachment**: PDF documentation file
- **AttachmentPath**: Stored file path

### File Management
- PDF file upload support for vehicle documentation
- 5MB file size limit
- File existence validation
- Secure file path handling
- File preview capability

## User Interface Components

### Navigation & Layout
- Hierarchical breadcrumb navigation
- Dynamic page titles (Create/Edit mode)
- Card-based form and list layout
- Responsive table design with striped rows
- Mobile-optimized data labels

### Form Management (AddEdit)
- Dynamic form titles based on create/edit mode
- Comprehensive vehicle information form
- Date input with calendar icon
- File upload with visual feedback
- Existing file preview with view option
- Form validation and error display

### List Display (Index & _ListPartial)
- Paginated vehicle listing
- Configurable page sizes (50, 100, 150)
- Advanced search and filter capabilities
- Date range filtering for ownership expiry
- Type-based filtering
- Action buttons with tooltips

## Filtering & Search System

### Advanced Search Functionality
- Text search across plate numbers and models
- Date range filtering for ownership expiry
- Vehicle type filtering dropdown
- Persistent filter state using session storage
- Clear filter functionality
- Real-time search results

### Filter Components
- Date range pickers with calendar icons
- Type dropdown for vehicle categorization
- Search input for text-based filtering
- Combined filter parameter handling

## Export Capabilities

### Print Functionality
- Print with all applied filters (search, date range, type)
- 4000 record limit validation
- New window print delivery
- Filter-aware printing

### Excel Export
- Excel report generation with applied filters
- Direct download functionality
- Comprehensive vehicle data export
- Search term, date range, and type parameter inclusion

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

### Vehicle Operations
- Create new vehicle records with documentation
- Edit existing vehicle information
- Delete with service dependency validation
- Comprehensive vehicle metadata tracking

### Service Integration
- Car service management integration
- Service dependency validation for deletion
- Add service action from car list
- Cross-module permission checking

### Ownership Management
- Expiry date tracking and filtering
- Calendar-based date selection
- Date validation and formatting
- Ownership documentation management

## Integration Points
- Resource files for multilingual support
- Permission-based UI rendering
- Bootstrap modal system
- jQuery AJAX for dynamic content
- Session storage for state persistence
- Validation scripts partial
- File helper utilities
- Car service module integration

## Page Components

### Main Index
- Advanced search and filter interface
- Action toolbar (Add Car, Print, Excel)
- Records count display with car-specific labeling
- Partial view container
- Delete confirmation modal with service validation

### AddEdit Form
- Comprehensive vehicle form layout
- Dynamic title (Create Car/Edit Car)
- File upload with preview capability
- Calendar date inputs with icons
- Form validation and error display
- Save and navigation actions

### List Partial
- Paginated table display with vehicle details
- Multi-action buttons per row (Add Service, View PDF, Edit, Delete)
- Service dependency validation for delete operations
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
- Service dependency validation for deletions
- Input validation and sanitization
- Anti-forgery token protection

## Vehicle Service Integration
- Direct navigation to car service creation
- Service existence checking
- Permission-based service action visibility
- Cross-module data consistency
- Service dependency management