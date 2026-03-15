# Account Index View Documentation

## Overview
Razor view displaying the main user management interface with advanced filtering, pagination, permission-based actions, and comprehensive user administration capabilities.

## View Configuration
- **Model**: PaginatedList<AdminVM>
- **Title**: "UsersList" from Resource1
- **Context**: Users Management Unite
- **Permissions**: Dynamic action visibility based on user permissions

## Layout Structure

### Header Section
- **Breadcrumb Navigation**: Users Management Unite → UsersList
- **Records Counter**: Real-time count display with dynamic updates
- **Visual Design**: Custom styling with proper spacing and alignment

### Action Header
- **Card Header**: Contains title and action buttons in flexible layout
- **Title**: "Users" with user icon and success coloring
- **Action Buttons**:
  - Create New Account (Add permission required)
  - Print functionality with record limit validation
  - Excel export with direct download

## Search & Filter System

### Search Interface
- **Search Box**: Icon-based input with placeholder text
- **Search Scope**: Name, Phone Number, Email fields
- **Visual Design**: Green label, search icon, proper padding

### Filter Management
- **Clear Filters**: Reset all search parameters
- **Search Action**: Submit filters with loading indication
- **Session Storage**: Persistent filter state across navigation

## Data Table Display

### Table Structure
- **Responsive Design**: Table-responsive for mobile compatibility
- **Column Layout**:
  - Full Name (localized based on session language)
  - Username
  - Email
  - Phone Number
  - Actions (permission-based)

### Empty State Handling
- **Visual Design**: Large alert icon with centered message
- **User Guidance**: "NoUsers" and "NoUsersMessage" from resources
- **Professional Layout**: Proper spacing and muted coloring

## Permission-Based Action System

### Action Buttons
- **Edit**: Pencil icon, requires "Account/Edit" permission
- **Delete**: Trash icon with multiple security checks

### Delete Security Logic
```csharp
@if (loggedInUserId != user.Id && DeletePermission && !(user.Role == Role.SuperAdmin.ToString()))
{
    // Additional trainer linkage check
    @if (user.IsTrainer)
    {
        // Show info modal instead of delete
    }
    else
    {
        // Actual delete form with confirmation
    }
}