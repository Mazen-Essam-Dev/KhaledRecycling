# AccountService Documentation

## Overview
C# service class implementing business logic for user account management with signature handling and username validation in a club management system.

## Service Configuration
- **Namespace**: Application.Services.Admin
- **Interface**: IAccountService
- **Dependencies**: IUnitOfWork for database operations
- **File Storage**: "Signatures" directory for signature images

## Signature Management System

### GetSignatureAsync
- **Purpose**: Retrieve a single signature for a specific user
- **Parameters**: `userId` - Target user identifier
- **Return**: Signature entity or new empty Signature if not found
- **Use Case**: Quick signature retrieval for user operations

### GetAllSignaturesAsync
- **Purpose**: Retrieve all signatures for a specific user
- **Parameters**: `userId` - Target user identifier  
- **Return**: Collection of Signature entities filtered by user
- **Use Case**: Signature history and management

### SaveSignatureAsync
- **Purpose**: Save new signature with file processing and timestamping
- **Parameters**: `model` - Signature entity with file data
- **File Processing**:
  - Automatic image path generation using FileHelper
  - "Signatures" directory organization
  - Async file save operations
- **Timestamp Management**:
  - Automatic CreatedAt date assignment if not provided
  - Uses current system date (DateOnly.FromDateTime(DateTime.Now))
- **Historical Preservation**: Always creates new records, never overwrites existing signatures
- **Error Handling**: Returns boolean success status with try-catch

```csharp
if (model.SignatureFile != null)
{
    model.ImagePath = await FileHelper.SaveImageAsync(model.SignatureFile, FileName);
}
if (model.CreatedAt == default)
    model.CreatedAt = DateOnly.FromDateTime(DateTime.Now);