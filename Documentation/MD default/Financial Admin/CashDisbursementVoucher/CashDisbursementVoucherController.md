# 📄 CashDisbursementVoucherController.cs

## 📦 Namespace
`FougeraClub.Areas.Admin.Controllers`

---

## 🧩 Class: CashDisbursementVoucherController
Handles **HTTP requests and MVC actions** for **Cash Disbursement Vouchers (CDV)**, including:

- CRUD operations for CDV
- Exchange Proof & Acknowledgment Receipt management
- OTP validation for multi-role approval
- File attachments management
- Printing & Excel export
- Real-time notifications via SignalR

### 🔹 Attributes
- `[AdminAuthorize]` — restricts access to admin users
- `[Area("Admin")]` — specifies the area for routing

---

## 🔹 Dependencies
| Dependency | Purpose |
|------------|---------|
| `ICashDisbursementVoucherService _service` | Business logic service for CDV |
| `IMapper _mapper` | AutoMapper for entity/ViewModel mapping |
| `IHubContext<Hub.NotificationHub> _hubContext` | SignalR hub for real-time notifications |
| `INotificationService _notificationService` | Push notifications to users/roles |
| `IHttpContextAccessor _httpContextAccessor` | Access session & HTTP context |
| `UserManager<ApplicationUser> _userManager` | Access user details & roles |

---

## 🔹 Core Responsibilities
- **CDV Management:** Add, Edit, Delete, List with pagination & filtering
- **Exchange Proof:** Add, Edit, Validate OTP, fetch signatures
- **Acknowledgment Receipt:** Add, Edit, Validate OTP, fetch signatures
- **Attachments:** Upload, fetch, and manage files
- **Printing & Export:** Generate printable list & Excel reports
- **Notifications:** Real-time via SignalR, role-based push notifications

---

## 🔹 Methods

### 0. OTP
* OTP Roles `1(activityMonitor)` --> `2(Manager)` Arranged
* when new Add send Notification to First `1(activityMonitor)`
* OTP First sign `1(activityMonitor)` then editing will be disabled for all users except the Manager, and a notification will be sent to the `2(Manager)`
and then Open Sign OTP for `2(Manager)` 
* if `2(Manager)` Submit Edit (Update) --> go back to `1(activityMonitor)` and remove his sign and send Update Notification To `1(activityMonitor)`
* if `2(Manager)` Validate otp then editing will be disabled for all users and Open ` Print`
* if First User Sign --> Delete Button will be disabled for all users.
---

### 1. CDV CRUD

- `Index(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)`  
  - Lists CDVs with optional search & date filters
  - Sets `ExchangeProofApprovalDone` & `AcknowledgmentReceiptApprovalDone`
  - Supports AJAX partial view

- `AddEdit(int? id)` — GET  
  - Returns Add/Edit view with **CDV ViewModel**
  - Populates type enum list

- `AddEdit(CashDisbursementVoucherVM model)` — POST  
  - Validates model
  - Maps to entity
  - Add or Update CDV
  - Sends notifications via SignalR & `_notificationService`

- `Delete(int id)` — POST  
  - Deletes CDV via service

---

### 2. Exchange Proof

- `ExchangeProof(int cashDisbursementVoucherId)` — GET  
  - Retrieves ExchangeProof for CDV
  - Maps to ViewModel
  - Sets Accountant & Manager full names
  - Checks if record is fully filled (`isSavedFull`)

- `ExchangeProof(ExchangeProofVM model)` — POST  
  - Add or Update ExchangeProof
  - Sends notifications
  - Maps DocumentNo if updating

- `PrintCashDisbursementRequest(int? id)` — GET  
  - Retrieves CDV + signatures
  - Prepares data for printing

- OTP Endpoints:  
  - `SendOtp(int id, string role)`  
  - `CashDisbursementRequestValidateOtp(OtpValidationRequest request)`  
  - `ExchangeProofRequestValidateOtp(OtpValidationRequest request)`  
  - `AcknowledgmentReceiptRequestValidateOtp(OtpValidationRequest request)`  
  - **Validates OTP**, updates latest user signature, sends notifications

---

### 3. Acknowledgment Receipt

- `AcknowledgmentReceipt(int cashDisbursementVoucherId)` — GET  
  - Retrieves acknowledgment receipt
  - Sets default text & header date if not present
  - Sets `isSavedFull` flag

- `AcknowledgmentReceipt(AcknowledgmentReceiptVM model)` — POST  
  - Add or update acknowledgment receipt
  - Uses `_service` for persistence

---

### 4. Printing & Export

- `Print(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo)` — GET  
  - Prepares list of CDVs for printing

- `createExcelReport_Download(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo)` — GET  
  - Generates Excel file
  - Uses `ExcelStaticReport.ExcelReportArEn_`
  - Supports Arabic/English based on session language

---

### 5. Attachments

- `GetCashDisbursementVoucherAttachments(int id)` — GET  
  - Fetches attachments for a CDV
  - Maps to ViewModel
  - Returns PartialView `_CashDisbursementVoucherAttachmentsModal`

- `UploadCashDisbursementVoucherFiles(CashDisbursementVoucherAttachmentsVM model)` — POST  
  - Maps to DTO
  - Calls `_service.UploadAttachmentsAsync`
  - Redirects to Index

---

## 🔹 Notes

- Uses **AutoMapper** extensively to convert between entities & ViewModels
- Real-time notifications use **SignalR Hub**
- Multi-role OTP validation integrated for:
  - `Manager`  
  - `Accountant`
- **isSavedFull** flag ensures all critical fields are filled
- Supports **Arabic & English** via session language
- Handles both **synchronous view rendering** and **AJAX partial updates**
- Excel export uses strongly typed `ExcelDataDTO`

---
