# HashHelper Documentation

## File Structure
- **HashHelper**: HashHelper.cs

## Core Helper

### HashHelper
**Purpose**: Cryptographic utility for secure password hashing using SHA-256 algorithm

## Methods

### ComputeSha256Hash
**Purpose**: Computes SHA-256 hash of input string for secure password storage

**Parameters**:
- `rawData`: Input string to hash (typically password)

**Returns**: Hexadecimal string representation of SHA-256 hash

**Implementation Details**:
- Uses SHA256 cryptographic algorithm
- Encodes input as UTF-8 bytes
- Converts hash to hexadecimal string (.NET 5+)
- Proper resource disposal with `using` statement

## Technical Implementation

### Security Features
- Industry-standard SHA-256 algorithm
- One-way hashing (cannot reverse engineer)
- UTF-8 encoding for international character support
- Proper cryptographic resource management

### .NET Compatibility
- Requires .NET 5 or later
- Uses `Convert.ToHexString()` method
- No external dependencies

## Usage Context

### Password Security
```csharp
// Hash password for storage
var hashedPassword = HashHelper.ComputeSha256Hash(password);

// Verify password during login
var inputHash = HashHelper.ComputeSha256Hash(inputPassword);
bool isValid = inputHash == storedHash;