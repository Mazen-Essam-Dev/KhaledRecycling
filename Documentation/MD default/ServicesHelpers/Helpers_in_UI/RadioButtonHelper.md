# RadioButtonHelper Documentation

## File Structure
- **RadioButtonHelper**: RadioButtonHelper.cs

## Core Class

### RadioButtonHelper
**Purpose**: Static utility class for generating radio button lists from enum values with localization support using Data Annotations Display attributes

## Methods

### GetDisplayName
**Purpose**: Retrieves the localized display name of a single enum value using DisplayAttribute

**Generic Constraint**: 
- `TEnum`: Must be an Enum type

**Parameters**:
- `value`: Enum value to get display name for

**Returns**: Localized display name or enum string representation

**Implementation**:
- Uses reflection to get enum member info
- Extracts DisplayAttribute if present
- Falls back to enum.ToString() if no attribute

### GetAllWithNames
**Purpose**: Returns all enum values with their localized names and integer IDs

**Generic Constraint**:
- `TEnum`: Must be an Enum type

**Returns**: List of tuples containing (Id, Name) for each enum value

**Implementation**:
- Iterates through all enum values
- Converts to integer ID
- Uses GetDisplayName for localized text
- Returns ordered list

### GetRadioList
**Purpose**: Generates SelectListItem collection for radio button binding in Razor views

**Generic Constraint**:
- `TEnum`: Must be an Enum type

**Parameters**:
- `selectedId`: Optional currently selected value ID

**Returns**: List of SelectListItem objects for radio button rendering

**Implementation**:
- Creates SelectListItem for each enum value
- Sets Value as integer string representation
- Sets Text as localized display name
- Applies selection based on selectedId parameter

## Technical Implementation

### Reflection Usage
- **MemberInfo Access**: Gets enum field information
- **Attribute Reading**: Extracts DisplayAttribute metadata
- **Type Safety**: Generic constraints ensure enum-only usage

### Localization Support
- **DisplayAttribute**: Uses Name property for localized text
- **Fallback Mechanism**: Defaults to enum string representation
- **Culture Awareness**: Respects current thread culture

### ASP.NET Integration
- **SelectListItem**: Compatible with ASP.NET Core tag helpers
- **Razor View Ready**: Direct usage in @Html.DropDownListFor
- **Model Binding**: Proper value/text formatting

## Usage Examples

### Basic Display Name
```csharp
public enum UserRole
{
    [Display(Name = "Administrator")]
    Admin,
    [Display(Name = "Regular User")]
    User
}

var displayName = UserRole.Admin.GetDisplayName();
// Returns: "Administrator"