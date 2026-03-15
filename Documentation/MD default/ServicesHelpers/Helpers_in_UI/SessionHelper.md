# SessionHelper Documentation

## File Structure
- **SessionHelper**: SessionHelper.cs

## Core Class

### SessionHelper
**Purpose**: Static utility class for managing session data with focus on language/culture settings and HTTP context access

## Methods

### Configure
**Purpose**: Initializes the SessionHelper with HTTP context accessor for session management

**Parameters**:
- `httpContextAccessor`: IHttpContextAccessor instance for accessing HTTP context

**Usage**: Must be called once at application startup to enable session functionality

### GetCurrentLanguage
**Purpose**: Retrieves the current language/culture setting from session with fallback default

**Returns**: String representing current language code ("ar" for Arabic, "en" for English, etc.)

**Session Key**: "CurrentCulture" - stores the current application language

**Fallback Behavior**: Returns "ar" (Arabic) if no language is set in session

## Technical Implementation

### Static Configuration
- **Singleton Pattern**: Single IHttpContextAccessor instance
- **Lifetime Management**: Configured once at application startup
- **Thread Safety**: Proper null checking for web environment

### Session Management
- **String-based Storage**: Uses GetString() for language code storage
- **Null Safety**: Comprehensive null checking for session and context
- **Default Fallback**: Arabic as default language for Middle East applications

### HTTP Context Integration
- **Dependency Injection**: Requires IHttpContextAccessor configuration
- **Context Access**: Safe access to HttpContext and Session
- **Web Request Scope**: Operates within current HTTP request context

## Usage Examples

### Application Startup
```csharp
// In Program.cs or Startup.cs
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure SessionHelper
var httpContextAccessor = app.Services.GetRequiredService<IHttpContextAccessor>();
SessionHelper.Configure(httpContextAccessor);