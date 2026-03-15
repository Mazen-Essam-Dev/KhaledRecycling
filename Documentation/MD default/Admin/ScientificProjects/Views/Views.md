# Scientific Projects Management Views

## File Structure
- **Index.cshtml**: Main projects listing and management interface
- **_ListPartial.cshtml**: Partial view for projects table rendering
- **Create.cshtml**: Project creation form
- **Details.cshtml**: Project details and viewing interface
- **Edit.cshtml**: Project editing form

## Index View

### Purpose
Comprehensive scientific projects dashboard with advanced filtering, search, and reporting capabilities.

### Key Features

#### Navigation & Controls
- **Breadcrumb Navigation**: Clear page hierarchy
- **Record Counting**: Real-time record display
- **Action Buttons**: Create, print, Excel export with permission checks

#### Advanced Filtering
- **Search Functionality**: Project name and department search
- **Date Range Filtering**: Start and end date selection
- **Session Storage**: Filter persistence across page reloads
- **Responsive Design**: Multi-column filter layout

#### Reporting & Export
- **Print Reports**: Browser-based printing with record limits
- **Excel Export**: Direct Excel download
- **Record Limits**: 3999 record maximum with user feedback

#### Interactive Features
- **AJAX Pagination**: Smooth page transitions
- **Dynamic Content**: Partial view loading
- **Tooltip Integration**: Action button descriptions

## _ListPartial View

### Purpose
Reusable partial view for displaying scientific projects in tabular format.

### Key Features

#### Table Structure
- **Essential Columns**: Project number, name, idea owner, date, specialization, actions
- **Multi-language Support**: Arabic/English project names
- **Clickable Names**: Direct navigation to project details
- **Department Integration**: Scientific specialization display

#### Action Controls
- **Details Access**: Permission-based viewing
- **Edit Restrictions**: Disabled when both signatures exist
- **Project Cards**: Special project card functionality
- **Delete Protection**: Prevention when signed
- **Signature Workflow**: OTP-based approval system

#### Data Presentation
- **Serial Codes**: Project numbering system
- **Date Formatting**: Consistent date display
- **Empty State**: User-friendly no-data message

## Create View

### Purpose
Comprehensive project creation form with multi-section organization.

### Key Features

#### Form Structure
- **Basic Data**: Serial code, dates, departments, project names
- **Idea & Elements**: Project descriptions and element tracking
- **Delivery Notes**: Installation and delivery specifications
- **Participants & Supervisors**: Team member tracking
- **Financial Planning**: Expected cost estimation
- **Image Management**: Multiple file uploads with validation

#### Validation & UX
- **Required Fields**: Comprehensive form validation
- **File Uploads**: Image-specific accept attributes
- **Multi-language**: Arabic/English field support
- **Section Organization**: Clear visual separation

## Details View

### Purpose
* This `1(Trainer)` Who has OTP is selected by DropDown
* OTP Roles `1(Trainer)` --> `2(activityMonitor)` --> `3(Manager)` Arranged
* OTP First sign `1(Trainer)` then close edit for him and sending Notification to `2(activityMonitor)`
and then Open Sign OTP for `2(activityMonitor)` and if he Submit Edit (Update) --> Not remove his sign 
* if `2(activityMonitor)` Validate otp then close edit for him and sending Notification to `3(Manager)`
* if `3(Manager)` Validate otp then close edit for Any one and Open `ProjectCard Enabled And his Print `

Project viewing interface with read-only data display and signature workflow.

### Key Features

#### Data Display
- **Read-only Form**: All fields disabled for viewing
- **Conditional Display**: Empty field handling
- **Image Gallery**: Modal-based image viewing
- **Signature Status**: Supervisor and manager signature tracking

#### Signature Workflow
- **OTP Verification**: Secure signature approval
- **Role-based Access**: Trainer and manager permissions
- **Visual Indicators**: Signature image display
- **Print Restrictions**: Only available when fully signed

#### Navigation
- **Print Access**: Conditional print button
- **Back Navigation**: Consistent return flow
- **Permission Checks**: Role-based feature access

## Edit View

### Purpose
Project modification interface with file management and validation.

### Key Features

#### Form Management
- **Field Preservation**: Serial code and date protection
- **File Handling**: Current file display with modal previews
- **Conditional Editing**: Signature-based restrictions
- **Validation**: Comprehensive input validation

#### File Management
- **Image Previews**: Modal-based current image viewing
- **Upload Controls**: File replacement functionality
- **Validation Messages**: Size and format restrictions
- **Visual Feedback**: Upload button styling

#### User Experience
- **Consistent Layout**: Matching create view structure
- **Action Buttons**: Save and back navigation
- **Error Handling**: Clear validation messages

## Technical Implementation

### Security Features
- **Permission Checking**: Role-based access control
- **Signature Protection**: Edit/delete restrictions when signed
- **OTP Verification**: Secure approval workflow
- **Anti-forgery Protection**: Form security

### JavaScript Functionality
- **OTP Handling**: Modal-based code entry
- **Filter Management**: Session storage integration
- **Print Handling**: Window management
- **AJAX Operations**: Dynamic content loading

### Styling & UX
- **Responsive Design**: Mobile-friendly interfaces
- **Visual Hierarchy**: Clear section organization
- **Arabic Support**: RTL layout compatibility
- **Consistent Components**: Bootstrap-based UI

## Business Logic

### Project Lifecycle
1. **Creation**: Basic data entry and team assignment
2. **Editing**: Modifications before signatures
3. **Approval**: Supervisor and manager OTP signatures
4. **Completion**: Read-only details and reporting

### Signature Workflow
- **Sequential Approval**: Supervisor first, then manager
- **OTP Security**: Four-digit code verification
- **Visual Confirmation**: Signature image display
- **Edit Lock**: Prevention after full approval

### Data Management
- **Serial Numbering**: Automatic project coding
- **Department Categorization**: Scientific specialization
- **File Storage**: Multiple image support
- **Cost Tracking**: Financial planning integration