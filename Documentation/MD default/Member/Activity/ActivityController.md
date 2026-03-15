# ActivityController Documentation

## File Structure
- **ActivityController**: ActivityController.cs

## Core Controller

### ActivityController
**Purpose**: Member area controller for managing activity participation, subscriptions, and age validation with comprehensive activity browsing and subscription handling

## Attributes

### Controller-Level Attributes
- `[MemberAuthorize]`: Custom authorization for member access
- `[Area("Member")]`: Member area routing
- `[Route("Member/[controller]/[action]")]`: Consistent URL routing

## Dependencies

### Injected Services
- `IActivityService`: Activity business logic and data operations
- `IHttpContextAccessor`: HTTP context and session management
- `IMapper`: AutoMapper for object-object mapping

## Actions

### Index
**Purpose**: Displays all available activities with member subscription status

**Implementation**:
- Retrieves member email from session
- Fetches activities and existing subscriptions
- Maps to ActivityVM view models
- Sets subscription status for each activity

**Returns**: View with activity list and subscription indicators

### Details
**Purpose**: Shows detailed activity information in modal/partial view

**Parameters**:
- `id`: Activity ID to display

**Implementation**:
- Validates activity existence
- Processes attachment path for web access
- Checks file existence using FileHelper
- Returns partial view for modal display

**Returns**: Partial view with activity details

### Subscribe (GET)
**Purpose**: Displays activity subscription confirmation page

**Parameters**:
- `id`: Activity ID to subscribe to

**Implementation**:
- Validates activity existence
- Returns subscription confirmation view

**Returns**: Subscription confirmation view

### Subscribe (POST)
**Purpose**: Handles activity subscription requests

**Parameters**:
- `id`: Activity ID to subscribe to

**Implementation**:
- Retrieves member email from session
- Calls service layer for subscription
- Redirects to activity index

**Returns**: Redirect to activity list

### CheckAge
**Purpose**: Validates member age against activity requirements via AJAX

**Attributes**:
- `[IgnoreAction]`: Excludes from request logging

**Parameters**:
- `activityId`: Activity ID to check age requirements

**Implementation**:
- Retrieves member email from session
- Calls service for age validation
- Returns JSON with validation result and message

**Returns**: JSON response with validation status

## Technical Implementation

### Session Management
- **Email Retrieval**: Gets member identity from session
- **Context Access**: Uses IHttpContextAccessor for session data
- **Member Isolation**: Ensures member-specific data access

### File Handling
- **Path Processing**: Converts virtual paths to web paths
- **Existence Validation**: Uses FileHelper for file checks
- **Null Safety**: Proper null handling for attachments

### AJAX Integration
- **JSON Responses**: Age validation via AJAX calls
- **Resource Messages**: Localized validation messages
- **Async Operations**: Non-blocking age checks

## Security Features

### Authorization
- Member-only access enforcement
- Session-based authentication
- Activity existence validation

### Validation
- Anti-forgery token protection
- Parameter validation
- Age requirement enforcement

## Usage Patterns

### Activity Browsing
```http
GET /Member/Activity/Index