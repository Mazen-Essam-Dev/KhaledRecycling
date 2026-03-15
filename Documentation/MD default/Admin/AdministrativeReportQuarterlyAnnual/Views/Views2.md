# Administrative Report Quarterly Annual System

## File Structure
- **Main View**: Index.cshtml
- **List Partial**: _ListPartial.cshtml
- **Activities Details**: ActivitiesDetails.cshtml
- **Administrative Details**: AdminstrativeDetails.cshtml
- **Combined Details**: All2Details.cshtml
- **Models**: AdministrativeReportQuarterlyAnnualIndexVM, QuartersReportActivitiesDetailsVM, QuartersReportAdminstrativeDetailsVM, QuartersReportCollaborativeDetailsVM
- **Enums**: QuartersYear

## Core Features

### Report Types
- **Quarterly Reports (Q1-Q4)**: Activity, Administrative, and Combined reports
- **Annual Summary Report (Q5)**: Yearly consolidated reporting
- **Activity-Based Reporting**: Event and participation tracking
- **Administrative Reporting**: Internal administrative activities
- **Combined Summary Reports**: Integrated activity and administrative data

### Security & Permissions
- Role-based access control system
- Permission validation for All2Details and PrintAll2Details
- Manager signature requirements for printing
- OTP-based signature authentication
- Role-based signature access (Manager role)

### Data Management

#### Report Navigation
- Dynamic year selection with validation
- Quarter-based report organization (Q1-Q5)
- Dynamic title generation based on quarter/year
- Empty state handling with user guidance
- Required field validation for year selection

#### Activity Reporting
- Monthly activity breakdown and organization
- Participant counting and statistics
- Activity duration calculations
- Reason statement tracking and display
- Chronological activity ordering

#### Administrative Reporting
- Administrative activity tracking
- Start and end date management
- Activity reason documentation
- Monthly administrative breakdown

### User Interface Components

#### Navigation & Layout
- Hierarchical breadcrumb navigation system
- Dynamic page titles based on quarter/year selection
- Card-based layout organization
- Responsive table designs with mobile optimization
- Color-coded monthly section headers

#### Filtering System
- Year selection dropdown with validation
- Required field validation with custom messages
- Persistent filter state using session storage
- AJAX partial updates for dynamic content
- Clear filter functionality

### Report Actions & Features

#### Quarterly Reports (Q1-Q4)
- Activity details reporting with participant statistics
- Administrative details reporting with date tracking
- Combined summary reporting integrating both data types
- Tooltip-enabled action buttons for better UX
- Conditional action rendering based on data availability

#### Annual Report (Q5)
- Combined summary reporting for full year
- Print functionality with signature requirement
- Manager signature validation system
- Consolidated annual data presentation

### Detailed Report Pages

#### Activities Details Page
- Monthly activity breakdown with participant counts
- Statistical summary with totals and calculations
- Advanced data visualization using Chart.js
- Doughnut charts for participation data
- Multi-level pie chart display with custom center text
- Color-coded monthly data segments

#### Administrative Details Page
- Administrative activity timeline
- Start and end date tracking
- Reason statement documentation
- Monthly administrative breakdown
- Signature requirement system

#### Combined Details Page (All2Details)
- Integrated activity and administrative data
- Monthly combined reporting
- Bar chart visualization for quarterly reports
- Dual dataset comparison (Activities vs Administrative)
- Annual summary exclusion for charting

### Visualization System

#### Chart Features
- **Doughnut Charts**: Multi-level participation data
- **Bar Charts**: Quarterly activity vs administrative comparison
- **Custom Center Text**: Total duration and participant counts
- **Color Coding**: Monthly segmentation with consistent colors
- **Interactive Tooltips**: Detailed data on hover
- **Responsive Design**: Adaptive chart sizing

#### Chart Configuration
- Chart.js integration with datalabels plugin
- Custom plugins for center text display
- Multi-dataset management for combined reports
- Font size customization for better readability
- Step-based Y-axis for whole number display

### Signature & Authentication System

#### OTP-Based Workflow
- Modal-based OTP entry interface
- 4-digit code validation system
- RTL-compatible input handling
- Real-time OTP validation
- Secure OTP transmission via AJAX

#### Signature Management
- Manager signature requirement for printing
- Signature image display and management
- Role-based signature access control
- Signature status validation
- OTP-based signature confirmation

### Technical Implementation

#### JavaScript Functions
- Print window management with parameter passing
- Navigation functions for different report types
- OTP handling and validation system
- Chart.js integration and configuration
- AJAX partial view loading
- Session storage management

#### State Management
- Session storage for filter persistence
- Dynamic title updates based on selections
- Real-time data loading via AJAX
- Modal state management for OTP
- Chart data serialization and rendering

#### Data Processing
- Monthly data aggregation and organization
- Participant count summation
- Duration calculations
- Data serialization for chart rendering
- Quarter-based data filtering

### Export Capabilities
- Print functionality for signed reports only
- Window-based print delivery system
- Signature requirement enforcement
- Year and quarter parameter passing
- Print restriction validation

## Business Logic

### Quarter Management
- Dynamic quarter availability based on year
- Quarter-based data filtering and organization
- Empty state handling for no selection
- Required year selection validation

### Signature Workflow
- Manager signature requirement for report printing
- OTP-based authentication for signature confirmation
- Role-based signature access permissions
- Signature image storage and display
- Signature status validation

### Data Validation
- Year selection requirement enforcement
- Quarter availability checking
- Data existence validation
- Empty state management
- Permission-based access control

## Integration Points
- Resource files for multilingual support
- Time zone handling (AppDubaiTime)
- Enum-based quarter management
- Culture-specific resource loading
- Bootstrap modal system integration
- Chart.js with datalabels plugin
- jQuery AJAX for dynamic content
- Session storage for state persistence

## Page Components

### Main Index
- Year selection interface with validation
- Quarterly report table with actions
- Dynamic title display
- Empty state management

### List Partial
- Quarter-based report listing
- Conditional action rendering
- Permission-based UI elements
- Empty state handling

### Detailed Report Pages
- Monthly data breakdown tables
- Statistical summaries and totals
- Data visualization charts
- Signature sections with OTP
- Print functionality with validation