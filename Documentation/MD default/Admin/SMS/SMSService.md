# SMSService Documentation

## Overview
Service layer implementation for SMS messaging system with gateway integration, member targeting, message logging, and comprehensive delivery tracking.

## Dependencies
- **SmsSettings**: SMS gateway configuration via IOptions pattern
- **HttpClient**: HTTP client for SMS API communication
- **IUnitOfWork**: Database repository access and transaction management

## Core SMS Operations

### GetAllAsync()
- **Purpose**: Retrieves all SMS records with receiver information
- **Includes**: Receivers navigation property for complete message history
- **Returns**: `IEnumerable<SMS>` with receiver relationships

### SendMessageAsync(SendingDTO dto)
- **Purpose**: Main SMS sending functionality with flexible recipient targeting
- **Recipient Scenarios**:
  - **Broadcast Mode**: Sends to all members when no specific receivers selected
  - **Targeted Mode**: Sends only to specified member IDs
- **Process Flow**:
  1. Message validation and SMS entity creation
  2. Recipient list generation based on input
  3. Individual SMS delivery to each recipient
  4. Delivery status tracking and logging
  5. Database persistence

## Message Logging & Tracking

### GetAllMessagesLogsAsync()
- **Purpose**: Retrieves comprehensive SMS message history
- **Data Structure**: `MessageLogDTO` with complete message audit trail
- **Includes**: Member information, message content, delivery status, timestamps
- **Use Case**: Reporting, analytics, and delivery verification

## SMS Gateway Integration

### SendSMSAsync(string to, string body)
- **Purpose**: Direct SMS gateway communication
- **URL Construction**: Builds parameterized API URL with proper encoding
- **Parameters**:
  - User ID and password for authentication
  - Recipient mobile number
  - Sender identification
  - Message content (URL encoded)
  - Message type configuration
- **Response Handling**: Structured success/failure responses with gateway feedback

### Gateway Communication
```csharp
var url = $"{_smsSettings.BaseUrl}" +
          $"?userid={Uri.EscapeDataString(_smsSettings.UserId)}" +
          $"&pwd={Uri.EscapeDataString(_smsSettings.Password)}" +
          $"&mobile={Uri.EscapeDataString(to)}" +
          $"&sender={Uri.EscapeDataString(_smsSettings.Sender)}" +
          $"&msg={Uri.EscapeDataString(body)}" +
          $"&msgtype={Uri.EscapeDataString(_smsSettings.MsgType)}";