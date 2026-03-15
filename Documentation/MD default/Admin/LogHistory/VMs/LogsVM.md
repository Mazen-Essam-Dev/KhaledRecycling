# Log History ViewModel

## LogsVM

**Namespace**: `FougeraClub.Areas.Admin.ViewModels.LogHistory`

**Purpose**: Tracks and displays system activity logs with comprehensive request and user information

### Properties

#### Request Identification
- `Id`: Primary log identifier (int)
- `UserId`: User identifier who performed the action (string, max 450)
- `RequestTime`: Timestamp of the request (DateTime)

#### Request Details
- `Path`: URL path of the request (string)
- `Method`: HTTP method used (GET, POST, PUT, DELETE, etc.) (string)
- `Controller`: MVC controller name (string)
- `Action`: MVC action method name (string)

#### Process Information
- `NameAr`: Arabic name of the process/operation (string)
- `NameEn`: English name of the process/operation (string)

#### Target Information
- `LogTarget`: Identifier of the targeted entity/record (string)

#### User Information
- `UserFullName`: Complete user name (string)
- `UserFullNameAr`: Arabic version of user's full name (string)
- `UserFullNameEn`: English version of user's full name (string)

### Key Features

#### Audit Trail
- Comprehensive request logging for security and debugging
- Tracks both technical (controller/action) and business (process name) information
- Multi-language support for process names

#### User Activity Monitoring
- Links actions to specific users
- Provides user identification in multiple languages
- Tracks targeted entities for change monitoring

#### Technical Insights
- Captures full request context (path, method, controller, action)
- Supports analysis of system usage patterns
- Enables debugging and performance monitoring

### Use Cases
- Security auditing and compliance
- User activity monitoring
- System debugging and troubleshooting
- Usage analytics and reporting
- Change tracking for specific entities