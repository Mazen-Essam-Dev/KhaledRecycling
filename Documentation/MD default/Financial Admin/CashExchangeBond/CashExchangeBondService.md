# 📄 CashExchangeBondService.cs

## 📦 Namespace
`Application.Services.Admin`

---

## 🧩 Class: CashExchangeBondService
Handles all business logic related to **Cash Exchange Bonds (CEB)**, including:

- CRUD operations
- Year/month filtering
- Signature and OTP validation
- Attachments handling
- Preventing duplicate month registrations

Implements:
- `ICashExchangeBondService`

---

## 🔹 Dependencies
| Dependency | Purpose |
|------------|---------|
| `IUnitOfWork _unitOfWork` | Repository access and transaction management |
| `IWebHostEnvironment _env` | Server environment for file storage |
| `IHttpContextAccessor _httpContextAccessor` | Access to HTTP context for OTP/session |
| `ISMSService _SMSService` | Sending SMS notifications |
| `ISMSForSendingOTPService _SMSForSendingOTPService` | Sending OTP codes via SMS |

---

## 🔹 Enums

### `UserRoles`
Defines the role for OTP/signature validation.
- `manager`
- `accountant`
- Other domain-defined roles as needed

---

### `CashExchangeBondStatus`
(Optional enum for future use)
- `Pending`
- `Approved`
- `Rejected`

---

## 🔹 Core Responsibilities
- CRUD operations for `CashExchangeBond`
- Ensures monthly uniqueness of bonds
- Signature/OTP verification for manager/accountant
- Uploading and managing attachments
- Maintaining data integrity across details and attachments

---

## 🔹 Methods

### 1. `GetAllAsync() : Task<IEnumerable<CashExchangeBond>>`
- Returns all bonds with related accountant and manager signatures included.

---

### 2. `GetByIdAsync(int id) : Task<CashExchangeBond?>`
- Retrieves a single bond with details and signatures.

---

### 3. `GetAllYearsInDb() : Task<IEnumerable<int>>`
- Returns all distinct years present in bonds for filtering or reporting.

---

### 4. `AddAsync(CashExchangeBond entity) : Task<int>`
- Adds a new bond.
- Returns the generated ID after save.

---

### 5. `UpdateAsync(CashExchangeBond entity)`
- Updates bond details.
- Preserves related details collection.
- Saves changes via UnitOfWork.

---

### 6. `DeleteAsync(int id)`
- Deletes bond and associated details.
- Ensures database integrity.

---

### 7. `CheckIsMonthRegistedBefore(int id, DateOnly? dateOnly) : Task<bool>`
- Checks if a bond for the same month/year already exists (excluding the current ID).

---

### 8. Signature & OTP

#### `SendOtpAsync(int id, string role) : Task<bool>`
- Generates OTP, saves it, and sends via SMS.
- Returns true if SMS was sent successfully.

#### `ValidateOtpAsync(int id, string code, string role, ClaimsPrincipal user) : Task<(bool success, string? message)>`
- Validates the OTP code.
- Saves the signature ID to bond based on role.
- Returns success/failure with optional message.

---

### 9. Attachments Handling

#### `UploadAttachmentsAsync(CashExchangeBondAttachmentsDTO model)`
- Deletes removed attachments from storage and database.
- Saves new attachments to storage and database.

#### `GetAttachmentsAsync(int cashExchangeBondId) : Task<CashExchangeBondAttachmentsDTO>`
- Retrieves all attachments for a bond.

---

## 🔹 Notes
- Enums are used for type safety in roles and bond sources.
- OTP and signature validation is **role-based**.
- Attachments are saved in `CashExchangeBondAttachments` folder.
- All database operations are transactional via `IUnitOfWork`.
- No external reports or summary tables are maintained; all calculations are real-time.
