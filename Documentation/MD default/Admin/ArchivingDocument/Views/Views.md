# Archiving Document Management System

## File Structure
- **Main View**: Index.cshtml
- **List Partial**: _ListPartial.cshtml
- **Add/Edit Form**: AddEdit.cshtml
- **Model**: ArchivingDocumentVM, PaginatedList<ArchivingDocumentVM>

## Core Features

### Document Management
- Comprehensive document archiving system
- Document type classification and organization
- PDF file attachment management
- Document reference number tracking
- Authority and date-based document organization

### Security & Permissions
- Role-based access control for document operations
- Permission validation for Add, Edit, Delete operations
- Secure action rendering based on user privileges
- Confirmation dialogs for delete operations

## Data Management

### Document Fields
- **Type**: Document classification (radio button selection)
- **Title**: Document title/description
- **Authority**: Issuing authority/organization
- **DocumentReferenceNumber**: Unique reference identifier
- **Date**: Document date
- **PdfFile**: PDF document attachment
- **PdfFilePath**: Stored file path

### File Management
- PDF file upload support
- 5MB file size limit
- File existence validation
- Secure file path handling
- File preview capability

## User Interface Components

### Navigation & Layout
- Hierarchical breadcrumb navigation
- Dynamic page titles (Add/Edit mode)
- Card-based form and list layout
- Responsive table design
- Mobile-optimized data labels

### Form Management (AddEdit)
- Dynamic form titles based on create/edit mode
- Radio button group for document type selection
- Comprehensive form validation
- File upload with visual feedback
- Existing file preview with view option

### List Display (Index & _ListPartial)
- Paginated document listing
- Configurable page sizes (50, 100, 150)
- Advanced search and filter capabilities
- Date range filtering
- Type-based filtering
- Action buttons with tooltips

## Filtering & Search System

### Advanced Search Functionality
- Text search across document titles and reference numbers
- Date range filtering (Start Date to End Date)
- Document type filtering (radio button selection)
- Persistent filter state using session storage
- Clear filter functionality
- Real-time search results

### Filter Components
- Date range pickers for precise filtering
- Type radio button group for category filtering
- Search input with icon for text-based filtering
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
- Comprehensive data export
- Search term, date range, and type parameter inclusion

## Technical Implementation

### JavaScript Functions
- Print and Excel export handlers with filter parameters
- AJAX partial view loading
- Session storage for filter persistence
- Real-time record count updates
- Dynamic pagination handling

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

### Document Type Management
- Enum-based document type classification
- Radio button selection interface
- Type-based filtering and organization
- Dynamic type text display

### Document Operations
- Create new document records with file attachments
- Edit existing document information
- Delete with confirmation and file cleanup
- Comprehensive document metadata tracking

## Integration Points
- Resource files for multilingual support
- Permission-based UI rendering
- Bootstrap modal system
- jQuery AJAX for dynamic content
- Session storage for state persistence
- Validation scripts partial
- File helper utilities

## Page Components

### Main Index
- Advanced search and filter interface
- Action toolbar (Create, Print, Excel)
- Records count display
- Partial view container
- Delete confirmation modal

### AddEdit Form
- Comprehensive form layout with type selection
- Dynamic title (Add New/Edit)
- File upload with preview capability
- Form validation and error display
- Save and navigation actions

### List Partial
- Paginated table display with document details
- File view action with existence validation
- Action buttons per row (View, Edit, Delete)
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
- Confirmation for destructive operations
- Input validation and sanitization