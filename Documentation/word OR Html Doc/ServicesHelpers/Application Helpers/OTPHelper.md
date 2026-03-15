# OTPHelper Documentation

## File Structure
- **OTPHelper**: OTPHelper.cs

## Core Helper

### OTPHelper
**Purpose**: Extension methods for One-Time Password (OTP) generation, storage, and validation with secure hashing

## Methods

### SaveOtpAsync
**Purpose**: Generates and stores a hashed OTP code for user verification

**Parameters**:
- `_httpContextAccessor`: HTTP context accessor for user identification
- `_unitOfWork`: Database unit of work for data persistence

**Returns**: Boolean indicating success/failure of OTP generation

**Implementation Details**:
- Retrieves current user ID from claims
- Generates static OTP code "1111" (for testing/demo)
- Hashes OTP using SHA-256 via HashHelper
- Creates OTP entity with 5-minute expiration
- Saves to database via Unit of Work pattern

**OTP Entity Properties**:
- `Code`: Hashed OTP value
- `CreateAt`: Current Dubai timestamp
- `ExpireAt`: Creation time + 5 minutes
- `UserId`: Current authenticated user ID
- `IsUsed`: Default false (not yet validated)

### ValidateOtpAsync
**Purpose**: Validates user-provided OTP code against stored hashed value

**Parameters**:
- `_httpContextAccessor`: HTTP context accessor for user identification
- `_unitOfWork`: Database unit of work for data access
- `code`: User-provided OTP code to validate

**Returns**: Boolean indicating valid/invalid OTP

**Validation Logic**:
1. Hashes input code using SHA-256
2. Retrieves unused OTP records from database
3. Validates multiple conditions:
   - OTP code matches hashed value
   - Current user matches OTP user
   - OTP is not expired (current time < expiration)
   - OTP is not already used
4. Marks OTP as used upon successful validation
5. Updates database via Unit of Work

## Technical Implementation

### Security Features
- SHA-256 hashing for OTP storage
- Time-based expiration (5 minutes)
- User-specific OTP binding
- Single-use OTP prevention
- Secure database operations

### Integration Dependencies
- **HashHelper**: For secure OTP hashing
- **AppDubaiTime**: For timezone-aware timestamps
- **ClaimsPrincipalExtensions**: For user ID retrieval
- **IUnitOfWork**: For database operations

### Error Handling
- Comprehensive try-catch blocks
- Returns false on any exception
- Graceful failure handling

## Usage Context

### OTP Generation
```csharp
var otpSaved = await _httpContextAccessor.SaveOtpAsync(_unitOfWork);
if (otpSaved)
{
    // OTP successfully generated and stored
}
```
