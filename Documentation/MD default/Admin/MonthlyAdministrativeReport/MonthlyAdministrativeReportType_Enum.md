# Monthly Administrative Report Type Enumeration

## MonthlyAdministrativeReportType

**Namespace**: `Domain.Enums`

**Purpose**: Defines the types of monthly administrative reports with localized display names

### Enum Values

#### 1. Activities
- **Value**: `1`
- **Display**: "ActivitiesType" (Localized from Resources.Resource2)
- **Description**: Report type focused on tracking activities, events, and participant engagements

#### 2. Administrative
- **Value**: `2`
- **Display**: "AdministrativeType" (Localized from Resources.Resource2)
- **Description**: Report type focused on administrative tasks, departmental operations, and management activities

### Key Features

#### Localization Support
- **Resource-Based**: Uses `Resources.Resource2` for display names
- **Multi-language**: Supports internationalization through resource files
- **Display Attributes**: Proper UI representation with `[Display]` attributes

#### Report Categorization
- **Activities Type**: For tracking events, programs, and participant-focused activities
- **Administrative Type**: For departmental operations and management reporting
- **Clear Distinction**: Well-defined categories for proper report classification

#### Integration Benefits
- **Type Safety**: Compile-time checking of report types
- **Database Storage**: Integer values for efficient storage
- **UI Consistency**: Standardized display names across application
- **Extensibility**: Easy to add new report types as needed

### Use Cases
- Report type classification in administrative reporting systems
- Filtering and categorization of monthly reports
- UI dropdowns and selection interfaces for report creation
- Business logic branching based on report type
- Localized display in user interfaces for different report categories