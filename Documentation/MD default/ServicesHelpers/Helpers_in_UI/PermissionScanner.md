# PermissionScanner Documentation

## File Structure
- **PermissionScanner**: PermissionScanner.cs

## Core Class

### PermissionScanner
**Purpose**: Comprehensive permission management and role validation system for ASP.NET Core applications with controller action scanning and user role verification

## Dependencies

### Required Services
- `IHttpContextAccessor`: Access to HTTP context and user information
- `IAuthorizationService`: ASP.NET Core authorization service
- `IUnitOfWork`: Database access for user and role data

## Methods

### GetAllActionPermissions
**Purpose**: Scans all controllers to extract action-based permissions for role management

**Returns**: Ordered list of permission strings in "Controller.Action" format

**Scanning Logic**:
- Identifies controllers with `[AdminAuthorize]` attribute
- Filters out methods with `[NonAction]` and `[IgnoreAction]` attributes
- Special handling for "AddEdit" action (splits into Add and Edit permissions)
- Returns alphabetically sorted permission list

**Permission Format**:
- Standard actions: `"ControllerName.ActionName"`
- AddEdit action: `"ControllerName.Add"`, `"ControllerName.Edit"`

### ValidatePermission
**Purpose**: Validates if current user has permission for specific controller action

**Parameters**:
- `controller`: Controller name without "Controller" suffix
- `action`: Action method name

**Returns**: Boolean indicating permission grant status

**Validation Process**:
1. Checks user authentication status
2. Constructs policy name as "Controller.Action"
3. Uses ASP.NET Core authorization service
4. Returns authorization result

### ValidateRoleNumber
**Purpose**: Retrieves user's role number from session for role-based access control

**Returns**: RoleNumber enum value or NormalUser default

**Session Data**:
- Stores role number as integer in session
- Falls back to RoleNumber.NormalUser if not set

### CheckLoggedUserIfTrainer
**Purpose**: Verifies if currently logged-in user is associated with a trainer profile

**Returns**: Boolean indicating trainer status

**Validation Process**:
1. Extracts user email from identity
2. Queries user database record
3. Checks trainer association via UserId
4. Returns true if trainer profile exists

### CheckLoggedUserIfHasSignature
**Purpose**: Checks if current user has a digital signature in the system

**Returns**: Boolean indicating signature existence

**Validation Process**:
1. Gets user email from identity
2. Retrieves user entity from database
3. Queries signatures table for user association
4. Returns signature existence status

## Technical Implementation

### Permission Scanning
- **Reflection Usage**: Assembly scanning for controller discovery
- **Attribute Filtering**: AdminAuthorize attribute requirement
- **Method Exclusion**: Ignores NonAction and IgnoreAction methods
- **Special Case Handling**: AddEdit action splitting

### Authorization Integration
- **Policy-based**: Uses ASP.NET Core authorization policies
- **Async Support**: Proper async/await pattern
- **Session Management**: Role number storage in session

### Database Integration
- **User Lookup**: Email-based user identification
- **Relationship Checking**: Trainer and signature associations
- **Entity Framework**: Efficient database queries

## Usage Examples

### Permission Scanning
```csharp
var permissions = PermissionScanner.GetAllActionPermissions();
// Returns: ["User.Add", "User.Edit", "User.Delete", "Course.View", ...]