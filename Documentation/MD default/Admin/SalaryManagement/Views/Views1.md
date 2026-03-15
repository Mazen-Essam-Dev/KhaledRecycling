# Salary Management Views Documentation

## File Structure
- **AddEdit.cshtml**: Salary management creation/editing interface
- **Index.cshtml**: Main salary management listing and reporting interface
- **_ListPartial.cshtml**: Partial view for salary data table rendering

## AddEdit View

### Purpose
Comprehensive salary management form for creating and editing employee salary records with real-time calculations.

### Key Features

#### Dual Mode Operation
- **Add Mode**: Full form with employee selection, year/month selection
- **Edit Mode**: Read-only display of employee, year, and month information

#### Form Structure
- **Employee Selection**: Dropdown with Arabic/English name support
- **Temporal Selection**: Year dropdown (current year - 100 years) and dynamic month loading
- **Salary Components**: Basic salary, allowances, total salary calculations
- **Attendance Tracking**: Work days, absent days, sick leave, annual leave
- **Financial Adjustments**: Deductions, bonuses, net salary calculations

#### Real-time Calculations
- **Total Salary**: Basic + Allowances (auto-calculated)
- **Total Deductions**: (Daily salary × Absent days) + Deductions addition
- **Net Salary**: Total salary - Total deductions + Bonuses

#### Validation & Security
- Required field validation with localized messages
- Integer/decimal input restrictions
- Anti-forgery token protection
- Permission-based access control

## Index View

### Purpose
Main dashboard for salary management with filtering, reporting, and data export capabilities.

### Key Features

#### Navigation & Controls
- **Breadcrumb Navigation**: Clear hierarchy display
- **Record Count**: Real-time record counting
- **Action Buttons**: Create, print reports, export to Excel

#### Filtering System
- **Year Filter**: Dropdown with available years
- **Employee Filter**: Multi-language employee selection
- **Month Filter**: Month selection for targeted reporting
- **Session Storage**: Filter persistence across page reloads

#### Reporting & Export
- **Payroll Report**: Printable payroll summary
- **Discounts & Bonuses Report**: Detailed adjustments report
- **Excel Export**: Both report types exportable to Excel
- **Print Limitations**: 3999 record limit with user feedback

#### Interactive Features
- **AJAX Pagination**: Smooth page transitions without reload
- **Filter Persistence**: Session storage for user preferences
- **Toast Notifications**: User feedback for actions

## _ListPartial View

### Purpose
Reusable partial view for displaying salary data in tabular format with pagination.

### Key Features

#### Table Structure
- **Comprehensive Columns**: 13 data columns covering all salary aspects
- **Multi-language Support**: Arabic/English employee names
- **Clickable Employee Names**: Direct navigation to employee details

#### Data Display
- **Formatted Dates**: Month names in current culture
- **Currency Formatting**: Proper decimal display for financial data
- **Empty State Handling**: User-friendly no-data message

#### Action Controls
- **Edit Access**: Permission-based edit buttons
- **Delete Functionality**: Secure deletion with confirmation
- **Tooltip Integration**: Action button descriptions

#### Pagination System
- **Page Size Selection**: 50, 100, 150 record options
- **AJAX Navigation**: Smooth page transitions
- **Active Page Highlighting**: Clear current page indication

## Technical Implementation

### JavaScript Functionality
- **Real-time Calculations**: Automatic salary computations
- **Dynamic Month Loading**: AJAX-based month availability
- **Filter Management**: Session storage integration
- **Print Handling**: Popup window management

### Security Features
- **Permission Checking**: Role-based access control
- **Anti-Forgery Protection**: CSRF prevention
- **Input Validation**: Client and server-side validation

### User Experience
- **Responsive Design**: Mobile-friendly interface
- **Loading States**: Visual feedback during operations
- **Error Handling**: Graceful error messages
- **Accessibility**: Proper labeling and ARIA attributes

## Integration Points

### Data Sources
- Employee information from `EmployeesNameDTO`
- Salary data from `SalaryManagementVM`
- Pagination through `PaginatedList<SalaryManagementVM>`

### External Dependencies
- Bootstrap for UI components
- Font Awesome for icons
- Toastr for notifications
- jQuery for AJAX operations

### Localization
- Resource files for multi-language support
- Culture-specific date formatting
- Arabic/English text switching