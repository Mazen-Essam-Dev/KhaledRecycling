# Annual Schedule Management System

## File Structure
- **Main View**: Index.cshtml
- **List Partial**: _ListPartial.cshtml
- **Add/Edit Form**: AddEdit.cshtml
- **Details View**: Details.cshtml
- **Model**: AnnualScheduleVM, PaginatedList<AnnualScheduleVM>

## Core Features

### Inventory Management
- Annual inventory tracking and management
- Item-based inventory organization
- Multi-year inventory support
- Financial tracking (Previous, Current, Record amounts)
- Status monitoring and reporting

### Security & Permissions
- Role-based access control for inventory operations
- Permission validation for Add, Edit, Delete operations
- Secure action rendering based on user privileges
- Confirmation dialogs for delete operations

## Data Management

### Inventory Fields
- **Item**: Inventory item name/description
- **Statement**: Detailed description or notes
- **Previous**: Previous period amount
- **Current**: Current period amount
- **Record**: Recorded amount
- **Status**: Item status tracking
- **Day**: Date of inventory entry
- **Year**: Inventory year classification

### Financial Tracking
- Numeric amount tracking for financial items
- Decimal number support with placeholder formatting
- Three-tier amount system (Previous, Current, Record)
- Year-over-year inventory comparison capability

## User Interface Components

### Navigation & Layout
- Hierarchical breadcrumb navigation
- Dynamic page titles (Add/Edit mode)
- Card-based form layout
- Responsive table design
- Mobile-optimized data labels

### Form Management (AddEdit)
- Dynamic form titles based on create/edit mode
- Comprehensive form validation
- Date input with day name display
- Multi-language day name detection (Arabic/English)
- Disabled day name display field

### List Display (Index & _ListPartial)
- Paginated inventory listing
- Configurable page sizes (50, 100, 150)
- Search and filter capabilities
- Year-based filtering
- Action buttons with tooltips

## Filtering & Search System

### Search Functionality
- Text search across items and statements
- Year-based filtering dropdown
- Persistent filter state using session storage
- Clear filter functionality
- Real-time search results

### Year Management
- Dynamic year extraction from inventory data
- "Show All" option for year filtering
- Year-based inventory organization
- Multi-year inventory support

## Export Capabilities

### Print Functionality
- Print with search and year filters applied
- 4000 record limit validation
- New window print delivery
- Filter-aware printing

### Excel Export
- Excel report generation
- Direct download functionality
- Filter-aware data export
- Search term and year parameter inclusion

## Technical Implementation

### JavaScript Functions
- Dynamic day name display based on date selection
- Language-specific day name formatting (Arabic/English)
- Print and Excel export handlers
- AJAX partial view loading
- Session storage for filter persistence

### State Management
- Session storage for filter state
- Real-time record count updates
- Dynamic pagination handling
- Partial view reloading without page refresh

### Validation & UX
- Client-side form validation
- Confirmation dialogs for delete actions
- Tooltip integration for action buttons
- Loading states during AJAX operations
- Empty state management

## Business Logic

### Date Handling
- Date input with day name derivation
- Multi-language day name support
- Date formatting consistency
- Localized date display

### Inventory Operations
- Create new inventory items
- Edit existing inventory records
- Delete with confirmation
- Year-based inventory organization
- Financial amount tracking

## Integration Points
- Resource files for multilingual support
- Permission-based UI rendering
- Bootstrap modal system
- jQuery AJAX for dynamic content
- Session storage for state persistence
- Validation scripts partial

## Page Components

### Main Index
- Search and filter interface
- Action toolbar (Create, Print, Excel)
- Records count display
- Partial view container
- Delete confirmation modal

### AddEdit Form
- Comprehensive form layout
- Dynamic title (Add New/Edit)
- Date with day name display
- Financial amount inputs
- Save and navigation actions

### List Partial
- Paginated table display
- Action buttons per row
- Empty state handling
- Page size selector
- Pagination controls

### Details View
- Read-only inventory display
- Card-based layout
- Complete field overview
- Back navigation
- Formatted date display

## Responsive Design
- Mobile-optimized table layouts
- Responsive form design
- Adaptive action buttons
- Flexible filter layouts
- Bootstrap-based responsive grid