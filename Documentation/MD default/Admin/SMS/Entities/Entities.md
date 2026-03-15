# SMS and OTP Management Entities

## File Structure
- **OTP**: OTP.cs
- **SMS**: SMS.cs
- **SMSReceiver**: SMSReceiver.cs

## Core Entities

### OTP (One-Time Password)
**Purpose**: Secure OTP management for authentication and verification workflows

**Properties**:

#### Identification & Security
- `Id`: Primary key identifier (int)
- `Code`: Hashed OTP code (string, max 128) - SHA256 hash storage
- `CreateAt`: OTP creation timestamp (DateTime) - required
- `ExpireAt`: OTP expiration timestamp (DateTime) - required
- `IsUsed`: Usage status flag (bool) - defaults to false
- `UserId`: Optional user identifier (string, max 50)

### SMS
**Purpose**: Main SMS message entity for bulk messaging and tracking

**Properties**:

#### Message Information
- `Id`: Primary key identifier (int)
- `Text`: SMS message content (string)
- `SendingDate`: Message sending timestamp (DateTime) - defaults to current time
- `IsDeliveredSMS`: Overall delivery status (SMSStatus enum)

#### Relationships
- `Receivers`: Collection of SMS recipients (ICollection<SMSReceiver>)

### SMSReceiver
**Purpose**: Individual recipient tracking for SMS messages

**Properties**:

#### Identification
- `Id`: Primary key identifier (int)
- `ReceiverId`: Foreign key to member (int) - required
- `Receiver`: Navigation to MemberEntity
- `SMSId`: Foreign key to SMS message (int) - required
- `SMS`: Navigation to SMS entity

#### Delivery Information
- `PhoneNumber`: Recipient phone number (string, max 20)
- `ServMessage`: Service provider message (string)
- `ServResponse`: Service provider response (string)
- `IsDelivered`: Individual delivery status (SMSStatus enum)

## Key Features

### OTP Security
- **Hash Storage**: SHA256 hashed codes for security
- **Time-based Expiry**: Configurable expiration periods
- **Usage Tracking**: One-time use prevention
- **User Association**: Optional user linking for audit

### SMS Management
- **Bulk Messaging**: One-to-many message distribution
- **Delivery Tracking**: Individual recipient status monitoring
- **Service Integration**: Provider response logging
- **Timestamp Tracking**: Complete audit trail

### Data Integrity
- **Foreign Key Relationships**: Strong entity relationships
- **Collection Initialization**: Pre-initialized collections
- **Data Annotations**: Comprehensive validation
- **Enum Integration**: Status enumeration support

## Data Annotations

### Validation & Constraints
- `[Key]`: Primary key identification
- `[Required]`: Mandatory field validation
- `[MaxLength]`: String length constraints
- `[DataType]`: Specific data type handling
- `[ForeignKey]`: Explicit relationship mapping

### Database Optimization
- **Efficient Storage**: Appropriate data types and lengths
- **Relationship Management**: Proper foreign key configuration
- **Default Values**: Safe initialization for critical fields

## Use Cases

### Authentication & Security
- User verification workflows
- Two-factor authentication (2FA)
- Password reset procedures
- Transaction confirmations

### Communication Management
- Bulk SMS campaigns
- Member notifications
- Event reminders
- System alerts

### Audit & Compliance
- Message delivery tracking
- User activity verification
- Service provider logging
- Compliance reporting

## Integration Benefits

### Security
- **Hashed OTPs**: Protection against database breaches
- **Expiration Control**: Time-based security
- **Usage Limits**: Single-use code enforcement

### Scalability
- **Bulk Operations**: Efficient mass messaging
- **Status Tracking**: Individual delivery monitoring
- **Provider Integration**: External service compatibility

### Maintainability
- **Clear Relationships**: Well-defined entity connections
- **Extensible Design**: Easy addition of new features
- **Standard Patterns**: Consistent data management approach