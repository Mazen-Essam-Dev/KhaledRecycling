# 📄 CashDisbursementVoucherService.cs

## 📦 Namespace
`Application.Services.Admin`

---

## 🧩 Class: CashDisbursementVoucherService
Handles **business logic** for **Cash Disbursement Vouchers (CDV)**, including:

- CRUD operations for CDVs
- Handling **Exchange Proofs** and **Acknowledgment Receipts**
- OTP validation for different roles
- File attachments management
- Integration with SMS services for OTP

### 🔹 Attributes
- Implements `ICashDisbursementVoucherService`

---

## 🔹 Dependencies
| Dependency | Purpose |
|------------|---------|
| `IUnitOfWork _unitOfWork` | Access repositories and manage transactions |
| `IHttpContextAccessor _httpContextAccessor` | Access to session & HTTP context |
| `ISMSService _SMSService` | Sends SMS messages |
| `ISMSForSendingOTPService _SMSForSendingOTPService` | Sends OTP SMS |
| `FileName` | Base folder name for attachments |

---

## 🔹 Core Responsibilities
- Handles **CDV entity operations** (Add, Edit, Delete, Get)
- Handles **child entities**:
  - `ExchangeProof` (with details)
  - `AcknowledgmentReceipt`
- Supports **OTP validation** for manager/accountant roles
- Manages **attachments upload and retrieval**
- Integrates with `_unitOfWork` for database transactions
- Ensures **child entities are properly linked** to parent CDV

---

## 🔹 Methods

### 1. CDV CRUD

- `GetAllAsync()` — Returns all CDVs
- `GetByIdAsync(int id)` — Returns CDV with its **details and signatures**
- `AddAsync(CashDisbursementVoucher entity)` — Adds CDV and returns Id
- `UpdateAsync(CashDisbursementVoucher entity)` — Updates CDV with new details
- `DeleteAsync(int id)` — Deletes CDV along with its details

---

### 2. Exchange Proof Operations

- `GetExchangeProofByIdAsyncSingle(int id)` — Returns single exchange proof
- `GetExchangeProofByIdAsync(int cashDisbursementVoucherId)` — Returns exchange proof with **details and signatures**
- `GetExchangeProofSignaturesAsync(int cashDisbursementVoucherId)` — Returns tuple `(AccountantId, ManagerId)`
- `AddAsync(ExchangeProof entity)` — Adds exchange proof along with child details
- `UpdateAsync(ExchangeProof entity)` — Updates exchange proof and replaces child details

---

### 3. Acknowledgment Receipt Operations

- `GetAcknowledgmentReceiptByIdAsync(int cashDisbursementVoucherId)` — Returns receipt with accountant signature
- `GetAcknowledgmentReceiptSignaturesAsync(int cashDisbursementVoucherId)` — Checks if receipt is signed
- `AddAsync(AcknowledgmentReceipt entity)` — Adds new receipt
- `UpdateAsync(AcknowledgmentReceipt entity)` — Updates receipt

---

### 4. OTP Validation

- `SendOtpAsync(int id, string role)` — Sends OTP via SMS
- `DisbursementRequestValidateOtpAsync(int id, string code, string role, ClaimsPrincipal user)` — Validates OTP for CDV request
- `ExchangeProofValidateOtpAsync(int id, string code, string role, ClaimsPrincipal user)` — Validates OTP for exchange proof
- `AcknowledgmentReceiptValidateOtpAsync(int id, string code, string role, ClaimsPrincipal user)` — Validates OTP for acknowledgment receipt

**Notes:**
- Retrieves latest signature of user
- Updates correct signature Id in entity based on role
- Returns `(success, message)` tuple

---

### 5. Attachments Management

- `UploadAttachmentsAsync(CashDisbursementVoucherAttachmentsDTO model)`  
  - Deletes old files if removed
  - Saves new files
  - Uses `FileHelper.SaveImageAsync` for storage
- `GetAttachmentsAsync(int cashDisbursementVoucherId)` — Returns all attachments for a CDV

---

## 🔹 Notes
- **Child entities** are handled carefully with proper FK linking (`ExchangeProofDetail`)
- **OTPHelper** is used for multi-role OTP validation
- **FileHelper** is used to manage attachment files safely
- **IUnitOfWork** ensures all database changes are **transactional**
- Supports **async/await** for all I/O and DB operations

---
