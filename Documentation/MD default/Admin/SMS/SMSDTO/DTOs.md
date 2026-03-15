# SMS Management DTOs

## File Structure
- **MessageLogDTO**: MessageLogDTO.cs
- **SendingDTO**: SendingDTO.cs
- **SMSReceiverDTO**: SMSReceiverDTO.cs
- **SmsSettings**: SmsSettings.cs

## Data Transfer Objects

### MessageLogDTO
**Purpose**: Data transfer object for SMS message logging and reporting

**Properties**:

#### Recipient Information
- `MemberNameAr`: Arabic member name (string)
- `MemberNameEn`: English member name (string)
- `IdNumber`: Member identification number (string)
- `PhoneNumber`: Recipient phone number (string)

#### Message Details
- `Message`: SMS message content (string)
- `DateAndTime`: Message sending timestamp (DateTime?)
- `IsDelivered`: Delivery status (SMSStatus enum)

### SendingDTO
**Purpose**: DTO for SMS message sending operations

**Properties**:
- `Message`: SMS message content to send (string)
- `ReceiverIds`: List of recipient member IDs (List<int>)

### SMSReceiverDTO
**Purpose**: Data transfer object for SMS recipient information

**Properties**:

#### Identification
- `Id`: Primary identifier (int)
- `ReceiverId`: Foreign key to member (int)
- `Receiver`: Navigation to MemberEntity
- `SMSId`: Foreign key to SMS message (int)
- `SMS`: Navigation to SMS entity

#### Contact Information
- `PhoneNumber`: Recipient phone number (string, max 20)

### SmsSettings
**Purpose**: Configuration DTO for SMS service provider settings

**Properties**:
- `BaseUrl`: SMS gateway API endpoint (string)
- `UserId`: Service provider username/ID (string)
- `Password`: Service provider password (string)
- `Sender`: Sender ID/name (string)
- `MsgType`: Message type classification (string)

## Key Features

### Message Logging & Reporting
- **Comprehensive Tracking**: Full message delivery audit trail
- **Multi-language Support**: Arabic and English member names
- **Delivery Status**: Real-time delivery status monitoring
- **Member Context**: ID number and contact information

### Message Sending
- **Bulk Operations**: Support for multiple recipients
- **Simple Interface**: Clean message and recipient list structure
- **Flexible Targeting**: Dynamic recipient selection

### Service Integration
- **Provider Configuration**: Complete SMS gateway settings
- **API Compatibility**: Standard SMS service parameters
- **Security**: Credential management for external services

### Data Optimization
- **Lightweight Transfer**: Efficient API payloads
- **Serialization Ready**: Clean structure for JSON/XML
- **Integration Friendly**: Easy consumption by frontend applications

## Use Cases

### SMS Operations
- Bulk message sending to multiple members
- Individual message tracking and logging
- Delivery status monitoring and reporting
- Service provider configuration management

### Reporting & Analytics
- Message delivery success rates
- Member communication history
- Service provider performance metrics
- Audit trail for compliance

### Integration Scenarios
- **API Communication**: Clean data structures for REST APIs
- **Frontend Consumption**: Simplified data for UI components
- **External Services**: Provider configuration management
- **Batch Processing**: Efficient bulk operations

## Technical Benefits

### Performance
- **Minimal Payload**: Optimized data transfer
- **Efficient Queries**: Targeted data retrieval
- **Scalable Design**: Support for large recipient lists

### Maintainability
- **Clear Separation**: Distinct concerns for different operations
- **Extensible Structure**: Easy addition of new properties
- **Consistent Patterns**: Standard DTO approach across application

### Security
- **Configuration Isolation**: Separate settings management
- **Data Minimization**: Only necessary information exposed
- **Audit Trail**: Comprehensive logging capabilities