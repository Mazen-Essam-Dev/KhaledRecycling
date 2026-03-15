# RemoteAttributes Documentation

## File Structure
- **RemoteAttributes**: RemoteAttributes.cs

## Core Class

### RemoteAttributes
**Purpose**: Provides server-side validation for unique field constraints using Entity Framework and expression trees, supporting both create and edit scenarios

## Constructor

### Dependencies
- `IUnitOfWork`: Database access interface for repository pattern

## Methods

### IsUnique
**Purpose**: Validates if a property value is unique across database records, excluding current entity during updates

**Generic Constraint**:
- `T`: Must be a class entity type

**Parameters**:
- `propertyName`: Name of the property to validate uniqueness
- `value`: Property value to check for uniqueness
- `id`: Optional entity ID to exclude during update scenarios

**Returns**: Boolean indicating if the value is unique (true) or exists (false)

## Technical Implementation

### Expression Tree Construction
- **Parameter Expression**: Creates "x" parameter for entity type
- **Property Access**: Dynamically accesses specified property
- **Value Comparison**: Builds equality expression (x.Property == value)
- **ID Exclusion**: Adds ID inequality check for update scenarios

### Database Query
- **Repository Pattern**: Uses IUnitOfWork for data access
- **Any() Method**: Efficient existence checking
- **Lambda Execution**: Converts expression tree to database query

### Update Scenario Handling
- **ID Check**: Excludes current entity when ID provided
- **Null Safety**: Proper null handling for ID parameter
- **Property Reflection**: Dynamically finds "Id" property

## Usage Examples

### Basic Unique Validation
```csharp
var remoteAttr = new RemoteAttributes(unitOfWork);
var isUnique = remoteAttr.IsUnique<User>("Email", "user@example.com");
// Returns true if no user has this email