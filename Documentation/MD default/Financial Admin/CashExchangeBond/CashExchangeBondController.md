# 📄 CashExchangeBondController.cs

## 📦 Namespace
`FougeraClub.Areas.Admin.Controllers`

---

## 🧩 Class: CashExchangeBondController
Handles **HTTP requests** and **user interactions** for **Cash Exchange Bonds (CEB)** in the Admin area, including:

- Listing and filtering bonds
- Add/Edit bonds
- Delete bonds
- Details and receipt views
- OTP validation and signatures
- File attachments management
- Excel report generation
- Real-time notifications via SignalR

### 🔹 Attributes
- `[AdminAuthorize]` — Restricts access to admin users
- `[Area("Admin")]` — Defines the controller area for routing

---

## 🔹 Dependencies
| Dependency | Purpose |
|------------|---------|
| `ICashExchangeBondService _service` | Business logic for CEB |
| `IMapper _mapper` | Maps entities ↔ view models |
| `IHubContext<Hub.NotificationHub> _hubContext` | SignalR for real-time notifications |
| `INotificationService _notificationService` | Push notifications to roles |
| `IHttpContextAccessor _httpContextAccessor` | Access to session & HTTP context |
| `UserManager<ApplicationUser> _userManager` | Fetch user information for signatures |

---

## 🔹 Core Responsibilities
- Handles all **UI-related logic** for CEB management
- Integrates **Service layer**, **AutoMapper**, and **SignalR**
- Supports filtering, pagination, and search
- Manages attachments upload and retrieval
- Sends **real-time notifications** to Manager/Accountant roles
- Handles OTP validation for role-based approval
- Generates **Excel reports** with multi-language support

---

## 🔹 Methods

### 0. OTP
#### 0.1 OTP (PrintCashDisbursementRequest `1`) FIRST DOCUMENT -`CashDisbursementRequestValidateOtp`-
* OTP Roles `1(Accountant)` --> `2(Manager)` Arranged
* when new Add send Notification to First `1(Accountant)`
* OTP First sign `1(Accountant)` then editing will be disabled for all users except the Manager, and a notification will be sent to the `2(Manager)`
and then Open Sign OTP for `2(Manager)` 
* if `2(Manager)` Submit Edit (Update) --> go back to `1(Accountant)` and remove his sign and send Update Notification To `1(Accountant)`
* else if `2(Manager)` Validate otp then editing will be disabled for all users and Open ` Print` (PrintCashDisbursementRequest `1`) AND ENABLE (ExchangeProof 2) SECOND DOCUMENT
* if First User Sign --> Delete Button will be disabled for all users.
---
#### 0.2 OTP (ExchangeProof `2`) SECOND DOCUMENT -`ExchangeProofRequestValidateOtp`-
* OTP Roles `1(Accountant)` --> `2(Manager)` Arranged
* when new Open send Notification to First `1(Accountant)`
* OTP First sign `1(Accountant)` then editing will be disabled for all users except the Manager, and a notification will be sent to the `2(Manager)`
and then Open Sign OTP for `2(Manager)` 
* if `2(Manager)` Submit Edit (Update) --> go back to `1(Accountant)` and remove his sign and send Update Notification To `1(Accountant)`
* else if `2(Manager)` Validate otp then editing will be disabled for all users and Open ` Print` (ExchangeProof `2`) AND ENABLE (AcknowledgmentReceipt 3) THIRD DOCUMENT
---
#### 0.3 OTP (AcknowledgmentReceipt `3`) THIRD DOCUMENT -`AcknowledgmentReceiptRequestValidateOtp`-
* OTP Roles `1(Accountant)`
* when new Open send Notification to First `1(Accountant)`
* OTP First sign `1(Accountant)` Validate otp then editing will be disabled for all users and Open ` Print` (AcknowledgmentReceipt `3`).
---

### 1. `Index(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)`
- Returns a paginated and optionally filtered list of bonds
- Supports **search by code, person name, or description**
- Supports **date filtering**
- Detects **Ajax requests** to return partial view

---

### 2. `AddEdit(int? id) : Task<IActionResult>`
- GET: Returns **empty form** for Add or **populated VM** for Edit
- POST: Handles both Add and Edit
  - Checks **month uniqueness** via `_service.CheckIsMonthRegistedBefore`
  - Sends **SignalR notifications** to Accountant role
  - Sends notifications via `_notificationService`
  - Redirects to Index after add, or back to form after edit

---

### 3. `Details(int? id)`
- Returns detailed view of a bond
- Converts **money to text** (Arabic/English) using `TafqeetHelper`

---

### 4. `CashExchangeBondReceipt(int? id)`
- Displays **receipt view**
- Fetches **full names** of accountant and manager for signatures
- Converts money to text (Arabic/English)

---

### 5. `Delete(int id)`
- Deletes bond via service
- Redirects to Index

---

### 6. `Print(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)`
- Returns **printable view** with all bonds filtered
- Supports search and date filtering

---

### 7. `CheckDate(int id, DateOnly date)`
- Ajax POST: Checks if the **month/year is already registered**
- Returns JSON boolean

---

### 8. `MoneyText(decimal? money)`
- Ajax POST: Converts decimal money to **Arabic/English text**
- Returns JSON with `arMoney`, `enMoney`, and `status`

---

### 9. `createExcelReport_Download(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)`
- Generates **Excel report** of filtered bonds
- Supports **multi-language headers** (AR/EN)
- Returns **FileContentResult** for download

---

### 10. OTP & Signature Actions

#### `SendOtp(int id, string role)`
- Sends OTP for manager/accountant approval
- Returns JSON with success status

#### `ValidateOtp(OtpValidationRequest request)`
- Validates OTP code for given role
- Updates bond signature if successful
- Sends **SignalR notifications** and role-based alerts
- Returns JSON `{ success, message }`

---

### 11. Attachments Management

#### `GetCashExchangeBondAttachments(int id)`
- Returns **partial view** with bond attachments
- Maps DTO to VM using AutoMapper

#### `UploadCashExchangeBondFiles(CashExchangeBondAttachmentsVM model)`
- Handles **file upload** for a bond
- Saves files via `_service.UploadAttachmentsAsync`
- Redirects to Index

---

## 🔹 Notes
- **ViewModels**: `CashExchangeBondVM` and `CashExchangeBondAttachmentsVM` used for UI mapping
- **Real-time notifications**: Implemented via **SignalR groups**
- **Money conversion**: `TafqeetHelper` used for Arabic/English text
- **Role handling**: Session-based role number and OTP validation
- **Excel reports**: Multi-language support with proper formatting
- **Ajax support**: Partial views for listing and attachments modal

---

