# SMS Status Enumeration

## SMSStatus

**Namespace**: `Domain.Enums`

**Purpose**: Defines the delivery status options for SMS messages with localized display names

### Enum Values

#### Success
- **Value**: `0`
- **Display**: "Success" (Localized from Resources.Resource2)
- **Description**: Indicates the SMS message was successfully delivered to the recipient

#### Fail
- **Value**: `1`
- **Display**: "Fail" (Localized from Resources.Resource2)
- **Description**: Indicates the SMS message failed to deliver to the recipient

### Key Features

#### Localization Support
- **Resource-Based**: Uses `Resources.Resource2` for display names
- **Multi-language**: Supports internationalization through resource files
- **Display Attributes**: Proper UI representation with `[Display]` attributes

#### Status Tracking
- **Binary Outcomes**: Clear success/failure status for delivery tracking
- **Simple Classification**: Easy-to-understand status options
- **Integration Ready**: Compatible with most SMS gateway responses

#### Technical Implementation
- **Enum Values**: Sequential integer values (0, 1)
- **Database Storage**: Efficient integer storage in database
- **Type Safety**: Compile-time checking of status values

### Use Cases

#### SMS Delivery Monitoring
- Real-time delivery status tracking
- Failed message retry logic
- Delivery success rate analytics
- Service provider performance monitoring

#### Reporting & Analytics
- Delivery success rate calculations
- Failed message analysis and troubleshooting
- Service quality metrics
- Compliance and audit reporting

#### User Interface
- Status display in SMS management interfaces
- Filtering and sorting by delivery status
- Color-coded status indicators (green for success, red for failure)
- Localized status text for multi-language support

### Integration Benefits

#### Database Efficiency
- **Compact Storage**: Integer values for minimal storage footprint
- **Fast Queries**: Efficient filtering and indexing
- **Data Integrity**: Predefined status values prevent invalid data

#### Application Consistency
- **Standardized Status**: Uniform status handling across the application
- **Clear Semantics**: Well-defined meaning for each status
- **Easy Extension**: Simple to add new status values if needed

#### User Experience
- **Intuitive Display**: Clear success/failure indicators
- **Localized Text**: Culture-appropriate status messages
- **Consistent Terminology**: Standardized across all SMS-related features