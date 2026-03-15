# EstimatedBudgetForExternalParticipationService Documentation

## Overview
Service layer implementation for managing estimated budgets for external participation events with OTP-based approval workflow and detailed budget line item management.

## Dependencies
- **IUnitOfWork**: Database repository access and transaction management
- **IHttpContextAccessor**: HTTP context access for OTP operations and user identification
- **FileName Constant**: "EstimatedBudgetForExternalParticipations" for organizational purposes

## OTP Security & Approval System

### SendOtpAsync()
- **Purpose**: Initiates OTP process for budget approval
- **Implementation**: Uses OTPHelper for OTP generation and storage
- **Process**: Creates and stores OTP in current HTTP context

### ValidateOtpAsync(int id, string code)
- **Purpose**: Validates OTP and attaches approval signature to budget
- **Process**:
  1. OTP validation using OTPHelper
  2. User signature retrieval (latest signature by current user)
  3. Budget entity update with signature ID
  4. Transaction commit
- **Returns**: Boolean indicating successful approval

### Signature Attachment Logic
```csharp
var latestSignature = allSignatures
    .Where(s => s.UserId == userId)
    .OrderByDescending(s => s.CreatedAt)
    .FirstOrDefault();