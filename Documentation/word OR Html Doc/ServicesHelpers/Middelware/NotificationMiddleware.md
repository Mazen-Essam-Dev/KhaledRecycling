# NotificationMiddleware Documentation

## File Structure
- **NotificationMiddleware**: NotificationMiddleware.cs

## Core Class

### NotificationMiddleware
**Purpose**: ASP.NET Core middleware that automatically handles toast notifications for CRUD operations by intercepting HTTP responses and setting appropriate session-based notification messages

## Constructor

### Dependencies
- `RequestDelegate next`: Next middleware in the pipeline

## Methods

### InvokeAsync
**Purpose**: Main middleware method that processes HTTP requests and sets notification messages based on action results

**Parameters**:
- `context`: HttpContext for current request
- `localizer`: IStringLocalizer for resource localization

## Notification Logic

### Operation Detection
- **POST Requests Only**: Processes only HTTP POST methods
- **Area Filtering**: Excludes Identity area requests
- **Status Code**: Triggers on 302 (redirect) responses
- **Action Analysis**: Examines route values for CRUD operations

### Action-Specific Notifications

#### AddEdit Action
- **Without ID**: "ToastAdd" (Create operation)
- **With ID**: "ToastEdit" (Update operation)

#### Create Action
- **Toast Type**: "ToastAdd" for creation success

#### Edit Action  
- **Toast Type**: "ToastEdit" for update success

#### Delete Action
- **Toast Type**: "ToastDelete" for deletion success
- **Style**: "error" type (typically red for delete operations)

#### Other Actions
- **Default**: "ToastDone" for general success

### Error Handling
- **500 Status**: "ToastFailed" for server errors
- **Error Style**: "error" type for failure notifications

### Session Storage
- **ToastMessage**: Notification content key
- **ToastType**: Styling type (success/error)
- **ToastKey**: Unique identifier for toast

## Technical Implementation

### Middleware Pipeline
- **Order**: Runs after controller action execution
- **Response Analysis**: Examines status codes and route data
- **Session Integration**: Stores notifications in user session

### Exclusion Rules
- **Login Actions**: Skips notification for authentication
- **Identity Area**: Excludes ASP.NET Core Identity pages
- **GET Requests**: Only processes POST operations

### Localization Support
- **Resource Integration**: Uses IStringLocalizer<Resource2>
- **Culture Awareness**: Respects current user culture
- **Key-based**: Uses resource keys for message lookup

## Usage Examples

### Middleware Registration
```csharp
// In Program.cs
app.UseMiddleware<NotificationMiddleware>();