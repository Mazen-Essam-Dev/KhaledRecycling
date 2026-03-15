# SMS Management ViewModels

## File Structure
- **MessageLogVM**: MessageLogVM.cs
- **OtpValidationRequest**: OtpValidationRequest.cs
- **SendingVM**: SendingVM.cs
- **SMSVM**: SMSVM.cs

## Core ViewModels

### MessageLogVM
**Purpose**: ViewModel for displaying SMS message logs with comprehensive tracking information

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

### OtpValidationRequest
**Purpose**: ViewModel for OTP validation requests in signature workflows

**Properties**:
- `Id`: Entity identifier for validation (long)
- `Code`: OTP code entered by user (string) - defaults to empty string
- `Role`: User role for validation context (string) - defaults to empty string

### SendingVM
**Purpose**: Simplified ViewModel for SMS message sending operations

**Properties**:
- `Message`: SMS message content to send (string)
- `ReceiverIds`: List of recipient member IDs (List<int>)

### SMSVM
**Purpose**: Comprehensive ViewModel for SMS management with validation

**Properties**:

#### Basic Information
- `Id`: Primary identifier (int)
- `Receivers`: Collection of SMS recipients (List<SMSReceiver>)
- `Text`: SMS message content with validation (string) - required
- `SendingDate`: Message sending timestamp (DateTime)
- `Status`: Delivery status display (string)

#### Recipient Management
- `SendTo`: List of recipient member IDs with validation (List<int>) - required

## Validation Features

### Custom Validation Attributes
- `[LocalizedRequired]`: Localized required field validation for multi-language support
- `[Required]`: Standard required field validation
- `[DataType(DataType.DateTime)]`: DateTime type specification for UI controls

## Key Features

### Message Logging & Display
- **Comprehensive Tracking**: Full audit trail for SMS messages
- **Multi-language Support**: Arabic and English member names
- **Delivery Status**: Real-time status monitoring
- **Member Context**: Complete recipient information

### OTP Workflow Integration
- **Secure Validation**: OTP code verification for sensitive operations
- **Role-based Context**: Different validation rules per user role
- **Entity Linking**: Specific entity identification for targeted operations

### SMS Operations
- **Bulk Messaging**: Support for multiple recipients
- **Validation Support**: Comprehensive input validation
- **Status Tracking**: Delivery status management
- **Recipient Management**: Flexible recipient selection

### User Interface Integration
- **Form Validation**: Client and server-side validation support
- **Data Binding**: Optimized for MVC form submission
- **Localization**: Culture-aware validation messages
- **UI Controls**: Compatible with date/time pickers and dropdowns

## Use Cases

### SMS Management Interface
- Message composition and sending
- Recipient selection and management
- Delivery status monitoring
- Message history and logging

### Security Workflows
- OTP-based signature approval
- Role-based operation validation
- Secure transaction confirmation
- Multi-factor authentication

### Reporting & Monitoring
- Message delivery analytics
- Recipient communication history
- Service performance tracking
- Audit and compliance reporting

## Technical Implementation

### Data Integration
- **Entity Mapping**: Clear relationship with domain entities
- **Validation Pipeline**: Comprehensive input validation
- **Status Enumeration**: SMS status tracking integration
- **Collection Support**: List-based recipient management

### Security Features
- **OTP Protection**: Secure validation workflows
- **Role-based Access**: Context-aware operation validation
- **Input Validation**: Protection against invalid data
- **Audit Trail**: Comprehensive logging capabilities

### User Experience
- **Responsive Design**: Mobile-friendly data structures
- **Localization Support**: Multi-language compatibility
- **Validation Feedback**: Clear error messaging
- **Form Optimization**: Efficient data binding