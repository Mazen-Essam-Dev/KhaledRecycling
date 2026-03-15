# SelectListHelper Documentation

## File Structure
- **SelectListHelper**: SelectListHelper.cs

## Core Class

### SelectListHelper
**Purpose**: Comprehensive utility for generating SelectListItem collections from enums and entity lists with multi-language support and dynamic property mapping

## Methods

### GetEnumSelectList (Two Overloads)

**Purpose**: Creates SelectListItem collections from enum values with DisplayAttribute support

**Generic Constraint**: `T` must be an Enum type

**Overload 1**: Basic enum to SelectList conversion
- Returns unselected SelectListItems with enum values and display names

**Overload 2**: Enum to SelectList with selection
- `selectedId`: Optional ID for pre-selected item
- Returns SelectListItems with selection state

### GetDisplayName
**Purpose**: Private helper to extract DisplayAttribute names from enum values

**Implementation**:
- Uses reflection to get DisplayAttribute
- Falls back to enum name if no attribute found

### BindSelectList
**Purpose**: Generates SelectList from entity lists with language-aware text properties

**Parameters**:
- `list`: Source entity list
- `selected`: Optional selected ID (int)
- `valueProperty`: Property name for option value (default: "Id")
- `nameAr`: Arabic text property name (default: "NameAr")
- `nameEn`: English text property name (default: "NameEn")

**Features**:
- Auto-detects current language from session
- Dynamic property reflection
- Selection state handling

### BindSelectListIdString
**Purpose**: String-based ID version of BindSelectList for string primary keys

**Parameters**:
- `selected`: Optional selected value as string
- Same property parameters as BindSelectList

### BindSelectListWithDataFromUsers
**Purpose**: Advanced SelectList generation with user data joining

**Parameters**:
- `items`: Main entity list
- `users`: User data list for text resolution
- `itemValueProperty`: Item ID property (default: "Id")
- `userKeyProperty`: User ID property (default: "Id")
- `userMatchProperty`: Foreign key in items (default: "UserId")
- `userNameAr/En`: User name properties

**Use Case**: For displaying user-related data in dropdowns

## Technical Implementation

### Reflection-Based Property Access
- **Dynamic Property Resolution**: Uses GetProperty() for flexible mapping
- **Type Safety**: Runtime property validation
- **Null Handling**: Comprehensive null checks

### Multi-Language Support
- **Session Integration**: Uses SessionHelper.GetCurrentLanguage()
- **Dynamic Text Selection**: Switches between Ar/En properties
- **Fallback Text**: "Unknown" for missing user data

### Selection Logic
- **Type Conversion**: Handles int and string selection values
- **Comparison Safety**: Proper null and type handling
- **Selection State**: Accurate selected flag setting

## Usage Examples

### Enum Dropdown
```csharp
public enum Status
{
    [Display(Name = "Active")]
    Active = 1,
    [Display(Name = "Inactive")]
    Inactive = 2
}

var statusList = SelectListHelper.GetEnumSelectList<Status>();
var selectedStatus = SelectListHelper.GetEnumSelectList<Status>(selectedId: 1);