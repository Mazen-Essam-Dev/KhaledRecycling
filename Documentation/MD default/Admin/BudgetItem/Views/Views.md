# Budget Item Management System

## File Structure
- **Main View**: Index.cshtml
- **List Partial**: _ListPartial.cshtml
- **Add/Edit Form**: AddEdit.cshtml
- **Model**: BudgetItemVM, PaginatedList<BudgetItemVM>

## Core Features

### Budget Item Management
- Financial budget item tracking and organization
- Item number and title management
- Simple two-field data structure for financial categorization
- Read-only item numbers for data integrity

### Security & Permissions
- Role-based access control for budget operations
- Permission validation for Add, Edit, Delete operations
- Secure action rendering based on user privileges
- Confirmation dialogs for delete operations

## Data Management

### Budget Item Fields
- **ItemNumber**: Unique identifier (read-only for data integrity)
- **ItemTitle**: Descriptive title for budget item
- **Id**: Primary key for database operations

### Financial Organization
- Simple yet effective budget categorization
- Number-based organization system
- Title-based search and filtering
- Minimalist data structure for financial management

## User Interface Components

### Navigation & Layout
- Hierarchical breadcrumb navigation within Financial Management
- Dynamic page titles (Add/Edit mode)
- Card-based form and list layout
- Clean, minimalist table design
- Mobile-optimized data labels

### Form Management (AddEdit)
- Dynamic form titles based on create/edit mode
- Read-only ItemNumber field to maintain data integrity
- Simple two-field form structure
- Form validation and error display
- Save and navigation actions

### List Display (Index & _ListPartial)
- Paginated budget item listing
- Configurable page sizes (50, 100, 150)
- Search functionality across item numbers and titles
- Clean table design with striped rows
- Action buttons with tooltips

## Filtering & Search System

### Search Functionality
- Text search across item numbers and titles
- Simple search interface
- Persistent filter state using session storage
- Clear search functionality
- Real-time search results

### Search Features
- Single search input for both number and title
- Placeholder guidance for users
- Quick clear option
- Session-based search persistence

## Export Capabilities

### Print Functionality
- Print with search filters applied
- 4000 record limit validation
- New window print delivery
- Search term parameter inclusion

### Excel Export
- Excel report generation with search filters
- Direct download functionality
- Search term parameter inclusion
- Simple data structure export

## Technical Implementation

### JavaScript Functions
- Print and Excel export handlers
- AJAX partial view loading
- Session storage for search persistence
- Real-time record count updates
- Dynamic pagination handling

### State Management
- Session storage for search state
- Real-time UI updates
- Partial view reloading without page refresh
- Simple state management for minimalist system

### Form Handling
- Read-only field management
- Client-side validation
- Clean form submission
- Error state management

## Business Logic

### Budget Item Operations
- Create new budget items with auto-generated numbers
- Edit existing budget item titles
- Delete with confirmation
- Simple two-field data management

### Data Integrity
- Read-only item numbers prevent accidental changes
- Consistent numbering system
- Title-based organization
- Search optimization

## Integration Points
- Resource files for multilingual support
- Permission-based UI rendering
- Bootstrap modal system
- jQuery AJAX for dynamic content
- Session storage for state persistence
- Validation scripts partial

## Page Components

### Main Index
- Search interface
- Action toolbar (Create, Print, Excel)
- Records count display
- Partial view container
- Delete confirmation modal

### AddEdit Form
- Minimalist form layout
- Dynamic title (Add New/Edit)
- Read-only item number display
- Form validation
- Save and navigation actions

### List Partial
- Clean table display with essential columns
- Action buttons per row (Edit, Delete)
- Empty state handling
- Page size selector
- Pagination controls

## Responsive Design
- Mobile-optimized table layouts with data labels
- Responsive form design
- Adaptive action buttons
- Clean, uncluttered interface
- Bootstrap-based responsive grid

## Security Features
- Permission-based action visibility
- Confirmation for delete operations
- Input validation
- Secure data handling

# Supplier Details View

## Overview
- Read-only supplier information display
- Comprehensive supplier data presentation
- Card-based layout for clean organization
- Back navigation to main supplier list

## Supplier Data Fields
- **SupplierNameAr**: Arabic supplier name
- **SupplierNameEn**: English supplier name  
- **Email**: Contact email address
- **Phone**: Primary phone number
- **Mobile**: Mobile phone number
- **VATNumber**: Tax identification number
- **Address**: Physical address
- **Description**: Additional notes or description

## User Interface
- Definition list (dl) layout for clear data presentation
- Card-based container with shadow
- Responsive column layout
- Clean typography and spacing
- Back to list navigation

## Integration
- Part of Suppliers Management system
- Consistent breadcrumb navigation
- Multilingual resource support
- Bootstrap styling framework