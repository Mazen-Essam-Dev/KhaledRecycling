# SMSController Documentation

## Overview
ASP.NET Core Controller for managing SMS messaging system with member targeting, message logging, and comprehensive reporting capabilities.

## Controller Configuration
- **Area**: Admin
- **Authorization**: AdminAuthorize attribute
- **Dependencies**:
  - ISMSService: SMS business logic and gateway integration
  - IMemberService: Member data access
  - IMapper: AutoMapper for object mapping
  - IConfiguration: Application configuration access

## SMS Operations

### SendSMS (GET)
- **Purpose**: Test endpoint for SMS functionality
- **Test Implementation**: Hardcoded recipient and message for testing
- **Configuration Ready**: Prepared for dynamic configuration values
- **Response Handling**: Structured JSON responses with success status

### Send (POST)
- **Purpose**: Main SMS sending endpoint
- **Attributes**: `[IgnoreAction]` for API-style endpoint
- **Parameters**: `SendingVM` with message details
- **Process**: Maps to DTO → Service call → JSON response
- **Use Case**: AJAX-based message sending from UI

## Member Management

### Members
- **Purpose**: Member listing for SMS targeting
- **Search Features**:
  - Member name search (Arabic/English)
  - ID number search
  - Phone number search
  - Comprehensive multi-field search
- **UI Features**:
  - Pagination (50 items per page)
  - AJAX partial view support
  - Clean member selection interface

## Message Logging & Reporting

### MessagesLogs
- **Purpose**: Comprehensive SMS message history and tracking
- **Search Features**:
  - Member name search (Arabic/English)
  - ID number search
  - Message content search
  - Phone number search
  - Multi-field search across all message attributes
- **Audit Trail**: Complete history of all sent messages

### PrintMessagesLogs
- **Purpose**: Printer-friendly message log
- **Attributes**: `[IgnoreAction]`
- **Features**: Same search functionality without pagination
- **Use Case**: Official documentation and reporting

### createExcelReport_Download
- **Purpose**: Generate and download SMS logs in Excel format
- **Excel Columns**:
  - Subscriber Name (localized)
  - Message Text
  - Receiver Phone
  - Date/Time (formatted as "HH:mm:ss dd-MM-yyyy")
  - Delivery Status
- **Process**:
  1. Applies same search filters as MessagesLogs
  2. Maps data to ExcelDataDTO with proper formatting
  3. Generates bilingual Excel file
  4. Returns timestamped XLSX file
- **File Naming**: "SMSList_YYYYMMDD_HHmmss.xlsx"

## Data Processing

### Search Implementation
```csharp
messagesLogsVMs = messagesLogsVMs.Where(c =>
    (!string.IsNullOrEmpty(c.MemberNameAr) && c.MemberNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
    (!string.IsNullOrEmpty(c.MemberNameEn) && c.MemberNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
    (!string.IsNullOrEmpty(c.IdNumber) && c.IdNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
    (!string.IsNullOrEmpty(c.Message) && c.Message.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
);