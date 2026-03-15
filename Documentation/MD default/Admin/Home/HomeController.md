# HomeController Documentation

## Overview
ASP.NET Core Controller serving as the central hub for admin area navigation, language management, and comprehensive error handling in a club management system.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Route**: Fixed route template "Admin/[controller]/[action]"
- **Purpose**: Central administration entry point and system utilities

## Core Navigation & Routing

### Index
- **Purpose**: Primary admin dashboard redirect
- **Attributes**: 
  - `[NoLogging]`: Excludes from activity logging
  - `[IgnoreAction]`: Excludes from route generation
- **Flow**: Redirects to Statistics controller Index action
- **Use Case**: Main admin entry point

## Language Management System

### ChangeLanguage
- **Purpose**: Dynamic language switching between Arabic and English
- **Parameters**: `lang` - target language code
- **Session Storage**: Sets "CurrentCulture" session variable
- **Navigation**: Returns to referring page via Referer header
- **Attributes**:
  - `[NoLogging]`: Language changes not logged
  - `[IgnoreAction]`: Hidden from direct routing
  - `[HttpGet]`: Safe GET operation for language switching

```csharp
HttpContext.Session.SetString("CurrentCulture", lang);
var referer = Request.Headers["Referer"].ToString();
return Redirect(referer ?? "/");