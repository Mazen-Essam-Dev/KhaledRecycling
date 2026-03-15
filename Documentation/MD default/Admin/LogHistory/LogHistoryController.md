# LogHistoryController Documentation

## Overview
ASP.NET Core Controller for comprehensive system log management with advanced filtering, dual-view display (Timeline/List), member-user association, and bulk log operations in a club management system.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**: IUnitOfWork for database operations

## Dual View Modes

### Index (List View)
- **Purpose**: Traditional paginated list view of system logs
- **Layout**: Tabular format with detailed columns
- **User Resolution**: Application users only (no member fallback)

### Timeline (Alternative View)
- **Purpose**: Chronological timeline display of system activities
- **User Resolution**: Application users with member fallback
- **Layout**: Timeline/activity stream format

## Advanced Filtering System

### Multi-Source Search
- **Text Search**: Log names (AR/EN), user names, controller/action names
- **Member Integration**: Special handling for member-related searches
- **Date Range**: Request time filtering with inclusive date boundaries
- **Noise Filtering**: Automatic exclusion of "negotiate" system logs

### Smart Member Search
```csharp
if (searchTerm.Trim().Contains("عضو") || searchTerm.Trim().Contains("Member"))
{
    // Special logic for member-specific searches
    var Terms = searchTerm.Trim().Split(" - ");
    // Complex member email resolution
}