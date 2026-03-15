# Application Helpers Documentation

## EnumHelper

### Overview
A static utility class that provides functionality for working with enumerations, specifically designed to extract display names from enum values using Data Annotations. This helper simplifies the process of retrieving user-friendly display names for enum values in UI applications.

### GetDisplayName Method

#### Purpose
Retrieves the display name of an enum value by reading the `DisplayAttribute` annotation. If no display attribute is found, it falls back to the enum value's string representation.

#### Signature
```csharp
public static string GetDisplayName(Enum enumValue)