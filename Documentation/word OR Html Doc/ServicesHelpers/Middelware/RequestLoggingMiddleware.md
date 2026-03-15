# RequestLoggingMiddleware Documentation

## File Structure
- **RequestLoggingMiddleware**: RequestLoggingMiddleware.cs
- **NoLoggingAttribute**: Attribute for excluding logging
- **YesGetAttribute**: Attribute for including GET method logging

## Core Classes

### RequestLoggingMiddleware
**Purpose**: Comprehensive HTTP request logging middleware that automatically tracks user actions, controller operations, and system events with multi-language support and intelligent filtering

## Constructor

### Dependencies
- `RequestDelegate next`: Next middleware in pipeline
- `ILogger<RequestLoggingMiddleware> logger`: Application logger
- `IHttpContextAccessor httpContextAccessor`: HTTP context access

## Attributes

### NoLoggingAttribute
**Purpose**: Excludes specific methods or controllers from request logging

**Targets**: Methods and Classes

### YesGetAttribute  
**Purpose**: Explicitly includes GET methods in logging (normally excluded)

**Targets**: Methods

## Methods

### InvokeAsync
**Purpose**: Main middleware method that processes requests and creates detailed audit logs

**Parameters**:
- `context`: HttpContext for current request
- `db`: ApplicationDbContext for database logging

## Logging Logic

### User Identification
- **Member Area**: Uses session "Email" for member identification
- **Admin Area**: Uses Identity NameIdentifier claim
- **Anonymous Users**: Skips logging for unauthenticated requests

### Request Filtering
- **Success Only**: Logs only 200/302 status codes
- **GET Method Exclusion**: Skips GET requests unless [YesGet] attribute present
- **NoLogging Attribute**: Respects [NoLogging] exclusion
- **Area Support**: Differentiates between Member and Admin areas

### Route Data Extraction
- **Standard Routing**: Uses RouteValues for controller/action names
- **Fallback Logic**: Path analysis for unconventional routes
- **ID Detection**: Extracts route IDs for edit operations
- **Action Normalization**: Converts "AddEdit" to "Edit" when ID present

### Multi-language Logging
- **Resource Files**: Uses Resource1 and Resource2 for localization
- **Culture Support**: Arabic and English text generation
- **Dynamic Titles**: Combines controller and action names
- **Fallback Lookup**: Tries multiple resource files

## Technical Implementation

### Log Entry Structure
- **Path**: Request URL path
- **Method**: HTTP method (GET, POST, etc.)
- **Controller**: Controller name
- **Action**: Normalized action name
- **UserId**: Authenticated user identifier
- **LogTarget**: Custom target from HttpContext.Items
- **RequestTime**: UTC timestamp
- **NameAr/NameEn**: Localized operation descriptions

### Database Integration
- **Entity Framework**: Direct db context usage
- **Async Operations**: Non-blocking database calls
- **Error Handling**: Comprehensive exception management

### Performance Optimization
- **Early Exits**: Quick returns for excluded requests
- **Conditional Processing**: Minimal overhead for filtered requests
- **Resource Caching**: Efficient resource manager usage

## Usage Examples

### Middleware Registration
```csharp
// In Program.cs
app.UseMiddleware<RequestLoggingMiddleware>();