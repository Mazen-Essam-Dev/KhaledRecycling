# Administrative Report Quarterly Annual System

## File Structure
- **Main View**: Index.cshtml
- **List Partial**: _ListPartial.cshtml
- **Activities Details**: ActivitiesDetails.cshtml
- **Model**: AdministrativeReportQuarterlyAnnualIndexVM, QuartersReportActivitiesDetailsVM
- **Enums**: QuartersYear

## Core Features

### Report Types
- Quarterly reports (Q1-Q4)
- Annual summary report (Q5)
- Activity-based reporting
- Administrative reporting
- Combined summary reports

### Security & Permissions
- Role-based access control
- Permission validation for All2Details
- Manager signature requirements
- OTP-based signature authentication

### Data Management

#### Report Navigation
- Year selection with validation
- Quarter-based report organization
- Dynamic title generation
- Empty state handling

#### Activity Reporting
- Monthly activity breakdown
- Participant counting
- Duration calculation
- Reason statement tracking

### User Interface Components

#### Navigation & Layout
- Hierarchical breadcrumb navigation
- Dynamic page titles based on quarter/year
- Card-based layout organization
- Responsive table design

#### Filtering System
- Year selection dropdown
- Required field validation
- Persistent filter state
- AJAX partial updates

### Report Actions

#### Quarterly Reports (Q1-Q4)
- Activity details reporting
- Administrative details reporting
- Combined summary reporting
- Tooltip-enabled action buttons

#### Annual Report (Q5)
- Combined summary reporting
- Print functionality with signature requirement
- Manager signature validation

### Activities Details Page

#### Data Presentation
- Monthly activity breakdown
- Participant statistics
- Duration calculations
- Chronological activity ordering

#### Visualization
- Doughnut chart for participation data
- Multi-level pie chart display
- Custom center text with totals
- Color-coded monthly data

#### Signature System
- Manager signature display
- OTP-based signature authentication
- Modal-based OTP entry
- Signature requirement validation

### Technical Implementation

#### JavaScript Functions
- Print window management
- Navigation functions for different report types
- OTP handling and validation
- Chart.js integration for data visualization
- AJAX partial view loading

#### State Management
- Session storage for filter persistence
- Dynamic title updates
- Real-time data loading
- Modal state management

#### Chart Features
- Multi-level doughnut chart
- Custom center text plugin
- Data labels integration
- Color-coded monthly segments
- Interactive tooltips

### Export Capabilities
- Print functionality for signed reports
- Window-based print delivery
- Signature requirement enforcement
- Year and quarter parameter passing

## Business Logic

### Quarter Validation
- Dynamic quarter availability
- Year-based quarter filtering
- Empty state for no selection
- Required year selection

### Signature Workflow
- Manager signature requirement for printing
- OTP-based authentication
- Role-based signature access
- Signature image display

### Data Calculation
- Activity duration summation
- Participant count aggregation
- Monthly subtotals
- Grand total calculations

## Integration Points
- Resource files for multilingual support
- Time zone handling (AppDubaiTime)
- Enum-based quarter management
- Culture-specific resource loading
- Bootstrap modal system
- Chart.js with datalabels plugin

## Page Components

### Main Index
- Year selection interface
- Quarterly report table
- Action buttons per quarter
- Dynamic title display

### List Partial
- Quarter-based report listing
- Conditional action rendering
- Empty state management
- Permission-based UI

### Activities Details
- Monthly activity breakdown
- Statistical summary
- Visualization charts
- Signature section
- Print functionality