# AccountController Documentation

## Overview
ASP.NET Core Controller for comprehensive user account management in the admin area, featuring user CRUD operations, role management, password reset, signature handling, and reporting capabilities.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - UserManager<ApplicationUser>: ASP.NET Core Identity user management
  - RoleManager<ApplicationRole>: Role-based authorization
  - IAccountService: Custom account business logic
  - ITrainerService: Trainer-specific functionality
  - IMapper: AutoMapper for object mapping

## Core User Management

### Index
- **Purpose**: User listing with advanced filtering and role resolution
- **Pagination**: 50 users per page
- **Search Functionality**: Name (AR/EN), Phone, Username
- **Trainer Integration**: Identifies users who are also trainers
- **Role Resolution**: Displays user roles from Identity system
- **AJAX Support**: Partial view for seamless pagination

### AddEdit (GET)
- **Purpose**: User creation and editing form
- **Role Management**: Populates role dropdown from RoleManager
- **Signature Integration**: Retrieves latest user signature
- **User Resolution**: Maps existing user data for editing
- **Dynamic Form**: Adapts for create vs edit modes

### AddEdit (POST)
- **Purpose**: Handle user creation and updates with comprehensive validation
- **Password Management**:
  - Create: Password required with confirmation
  - Update: Optional password change with secure reset token
- **Phone Formatting**: Automatic UAE phone number formatting (+971)
- **Role Assignment**: Single role selection and assignment
- **Error Handling**: Identity result validation with user feedback

## Security & Authentication

### Password Reset
- **Dedicated View**: Secure password reset form
- **Token Generation**: Uses Identity password reset tokens
- **Validation**: ModelState and Identity result validation
- **Flow**: Separate from main user editing for security

### Phone Number Processing
```csharp
model.PhoneNumber = model.PhoneNumber?.Replace(" ", "");
if (model.PhoneNumber != null && !model.PhoneNumber.StartsWith("971"))
{
    model.PhoneNumber = "971" + model.PhoneNumber;
}