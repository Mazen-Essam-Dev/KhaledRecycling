# Application Helpers Documentation

## AppDubaiTime Helper

### Overview
A static utility class providing Dubai timezone-aware date and time operations for applications requiring consistent time handling in the UAE timezone.

### Core Properties

#### Now
- **Type**: `DateTime`
- **Description**: Current date and time converted to Dubai timezone
- **Source**: Converts current UTC time to Asia/Dubai timezone
- **Usage**: For real-time operations requiring current Dubai time

#### Today
- **Type**: `DateOnly`
- **Description**: Current date in Dubai timezone (time portion excluded)
- **Conversion**: Extracts date portion from `Now` property
- **Usage**: Date-specific operations without time component

#### NowOffset
- **Type**: `DateTimeOffset`
- **Description**: Current date and time with Dubai timezone offset
- **Advantage**: Preserves timezone information for precise time handling
- **Usage**: Operations requiring timezone-aware datetime objects

### Conversion Methods

#### ConvertToDubaiDateTime
**Purpose**: Converts UTC DateTime to Dubai timezone

**Parameters**
- `dateTimeUtc`: UTC DateTime to convert

**Implementation Details**
1. **Kind Specification**: Ensures input is treated as UTC using `DateTime.SpecifyKind`
2. **Timezone Conversion**: Uses `TimeZoneInfo.ConvertTimeFromUtc` for accurate conversion
3. **Return**: Dubai-localized DateTime object

**Usage Example**
```csharp
var utcTime = new DateTime(2024, 1, 1, 10, 0, 0, DateTimeKind.Utc);
var dubaiTime = AppDubaiTime.ConvertToDubaiDateTime(utcTime);