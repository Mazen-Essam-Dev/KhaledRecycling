# Salary Management Views Documentation

## File Structure
- **Absences.cshtml**: Employee absence tracking and reporting interface
- **_AbsencesListPartial.cshtml**: Partial view for absence data table
- **PrintAbsences.cshtml**: Printable absence report format
- **PrintDiscountsAndBonusesReport.cshtml**: Printable discounts and bonuses report
- **PrintpayrollReport.cshtml**: Printable payroll summary report

## Absences View

### Purpose
Comprehensive employee absence tracking system with filtering, reporting, and export capabilities.

### Key Features

#### Filtering System
- **Employee Selection**: Required employee dropdown with validation
- **Date Range Filtering**: From/To year and month selection
- **Arabic Interface**: Full Arabic language support
- **Form Validation**: Required field highlighting and toast notifications

#### Reporting & Export
- **Print Functionality**: Browser-based printing with validation
- **Excel Export**: Direct Excel download capability
- **Record Limits**: 3999 record limit with user feedback
- **Multi-format Support**: Both print and Excel export options

#### User Interface
- **Breadcrumb Navigation**: Clear page hierarchy
- **Record Counting**: Real-time record count display
- **Responsive Design**: Mobile-friendly filter layout
- **Session Storage**: Filter persistence across page reloads

## _AbsencesListPartial View

### Purpose
Reusable partial view for displaying absence data in tabular format.

### Key Features

#### Table Structure
- **Essential Columns**: Employee name, job title, period, absence days
- **Multi-language**: Arabic/English employee name display
- **Total Calculation**: Summary row with total absence days
- **Empty State**: User-friendly no-data message

#### Data Presentation
- **Culture-aware Dates**: Localized month names
- **Number Formatting**: Proper absence day counting
- **Summary Row**: Bold total calculation at bottom

#### Pagination & Navigation
- **Page Size Options**: 50, 100, 150 records per page
- **AJAX Pagination**: Smooth page transitions
- **Active Page Highlighting**: Clear current page indication

## Print Views (Absences, Discounts & Bonuses, Payroll)

### Common Features

#### Professional Layout
- **Official Header**: UAE and Fujairah Science Club branding
- **Bilingual Support**: Arabic and English content
- **Logo Integration**: Club logo with fallback handling
- **Print Optimization**: A4 landscape orientation for reports

#### Technical Implementation
- **No Layout**: Standalone print-optimized pages
- **CSS Media Queries**: Print-specific styling
- **Loading Overlay**: Visual feedback during generation
- **Auto-close**: Automatic window closing after printing

#### Report Specifics

**PrintAbsences**:
- Simple 4-column layout for absence tracking
- Total absence days calculation
- Culture-aware month names

**OpenDetails_DiscountsAndBonusesReport**:
- Opened after Select month And year if has Data
- Financial adjustments summary
- Report Monthly has grid with Column grouping for name and job title
- Total calculations for all financial columns
- sign Firstly To show Print Button

**OpenDetails_payrollReport**:
- Opened after Select month And year if has Data
- Comprehensive payroll data (15 columns)
- Detailed salary breakdown
- Report Monthly has grid with Summation row for all numeric columns
- sign Firstly To show Print Button

### Security & Audit Features
- **User Tracking**: Printed by user email and timestamp
- **Current Time**: Dubai time display
- **Access Control**: User context in printed output

## Technical Implementation

### JavaScript Functionality
- **Print Handling**: Window management and auto-close
- **Validation**: Employee selection requirement
- **Filter Management**: Session storage integration
- **AJAX Operations**: Dynamic content loading

### Styling & UX
- **Print Optimization**: Color-adjust exact for backgrounds
- **Responsive Tables**: Horizontal scrolling for wide data
- **Loading States**: Professional loading indicators
- **Error Handling**: Graceful empty state messages

### Integration Points
- **User Management**: Identity integration for audit trails
- **Localization**: Resource files for multi-language support
- **Culture Settings**: Proper date and number formatting
- **Bootstrap**: Consistent UI components

## Business Logic

### Absence Tracking
- Monthly absence recording per employee
- Job title context for reporting
- Total absence calculations

### Financial Reporting
- Salary component breakdown
- Deductions and bonuses tracking
- Net salary calculations
- Department-wide summaries

### Data Validation
- Required field enforcement
- Record limit handling
- User feedback mechanisms
- Print preparation checks