# 📄 ExpenseAndReceiptController.cs

## 📦 Namespace
`FougeraClub.Areas.Admin.Controllers`

---

## 🧩 Class: ExpenseAndReceiptController
Manages **Expenses**, **Receipts**, and **Expense Reports**, including CRUD operations, validation, file handling,
Financial Reports, Excel exports, printing, OTP-based approvals, and receiving receipts in the Admin area.

---

## 🔹 Attributes
- `[AdminAuthorize]` — Restricts access to admin users.
- `[Area("Admin")]` — Defines routing under the Admin area.

---

## 🔹 Dependencies

| Dependency | Description |
|---------|------------|
| `IExpenseService _expenseService` | Handles expense-related business logic. |
| `IReceiptService _receiptService` | Handles receipt-related business logic. |
| `IUnitOfWork _unitOfWork` | Provides access to repositories and database transactions. |
| `IMapper _mapper` | Maps between entities and ViewModels using AutoMapper. |
| `IHubContext<NotificationHub> _hubContext` | Sends real-time notifications via SignalR. |
| `INotificationService _notificationService` | Sends role-based system notifications. |

---

## 🔹 Constructor

`ExpenseAndReceiptController(IExpenseService, IReceiptService, IMapper, IUnitOfWork, IHubContext, INotificationService)`

- Injects required services, mapper, unit of work, and notification handlers.

---

## 🔹 Actions

### 0. OTP
* OTP Roles `1(Manager)`.
* when new Add send Notification to First `1(Manager)`.
* OTP First sign `1(Manager)`.
* if any user Submit Edit (Update) --> go back to `1(Manager)` send Updated .Notification To `1(Manager)`.
* else if `1(Manager)` Validate otp then editing will be disabled for all users and Open ` Print`.
* if First User Sign --> Delete Button will be disabled for all users.
---


## 1. Add / Edit

### `AddEdit(int? id, ItemType itemType) : IActionResult` (GET)
- Displays Add or Edit form for Expenses or Receipts.
- Initializes dropdowns:
  - Item Types (Radio Buttons)
  - Suppliers (filtered for Receipts: category = 2)
  - Expense Sources
  - Budget Items & Expense Gates (Expenses only)
- Loads existing entity when `id` is provided.
- Detects signed Receiving Receipts and sets `isSigned`.

---

### `AddEdit(ExpenseAndReceiptVM model) : IActionResult` (POST)
- Handles create and update operations.
- Dynamic validation based on `ItemType`:
  - Expenses: Budget Item, Expense Gate, Source, Supplier required
  - Receipts: Source and Supplier required
- Validates Notes for all cases.
- Validates attachment:
  - PDF only
  - Max size 5 MB
- Supports temporary file caching on validation failure.
- Handles final file processing:
  - New upload
  - Temp file promotion
  - Preserve old file (edit)
- Sends notifications when creating the first monthly report.
- Persists entity via Expense or Receipt service.
- Redirects based on operation type.

---

## 2. Delete

### `Delete(int id, ItemType itemType) : IActionResult`
- Deletes Expense or Receipt based on `ItemType`.
- Redirects to corresponding list page.

---

## AJAX & Helpers

### 3. `GetSuppliersByCategory(int? categoryId) : IActionResult`
- Returns suppliers as JSON.
- Optional filtering by `SupplierCategoryId`.
- Localized response based on session language.

---

## 🔹 Receipts (List / Print / Excel)

### 4.1 `Receipts(...) : IActionResult`
- Displays paginated receipt list.
- Supports filtering:
  - Search term (Notes)
  - Supplier (category = 2)
  - Expense Source
  - Date range
- Calculates total amount.
- AJAX-supported partial rendering.

---

### 4.2 `Print_Receipts(...) : IActionResult`
- Generates printable receipt list.
- Applies same filters as `Receipts`.
- Calculates total amount.

---

### 4.3 `createExcelReport_Download_Receipts(...) : IActionResult`
- Generates Excel report for receipts.
- Supports Arabic and English.
- Appends total row.
- Uses `ExcelStaticReport.ExcelReportArEn_`.

---

## 🔹 Expenses (List / Print / Excel)

### 5.1 `Expenses(...) : IActionResult`
- Displays paginated expenses list.
- Supports filtering:
  - Budget Item
  - Expense Gate
  - Expense Source
  - Supplier
  - Date range
- Calculates:
  - Total Amount
  - Total VAT
  - Total Amount with VAT
- Marks expenses with signed Receiving Receipts.
- AJAX-supported partial rendering.

---

### 5.2 `Print_Expenses(...) : IActionResult`
- Generates printable expenses list.
- Includes totals (Amount, VAT, Amount with VAT).

---

### 5.3 `createExcelReport_Download_Expenses(...) : IActionResult`
- Generates Excel report for expenses.
- Includes VAT and total calculations.
- Appends summary row.
- Language-aware output.

---

## 🔹 Expenses Report (Monthly)

### 6.1 `ExpensesReport(int? year, int? month) : IActionResult`
- Displays monthly aggregated expenses report.
- Supports pagination.
- Loads available years and months.
- AJAX-supported partial rendering.

---

### 6.2 `OpenDetails_ExpensesReport(int? year, int? month) : IActionResult`
- Opens detailed monthly report view.
- Redirects if report is not available.

---

### 6.3 `ValidateOtp_OpenDetails_ExpensesReport(OtpValidationRequest request) : IActionResult`
- Validates OTP before opening report details.
- Sends notification to Manager after Accountant approval.
- Returns JSON result.

---

### 6.4 `Print_ExpensesReport(int? year, int? month) : IActionResult`
- Generates printable monthly expenses report.

### 6.5 `createExcelReport_Download_ExpensesReport(...) : IActionResult`
- **Attributes**: `[IgnoreAction]`, `[YesGet]`
- Generates an Excel file for the Expenses Report.
- Filters by `selectedYear` and `selectedMonth`.
- Uses `ExpensesReportVM` as the data source.
- Inserts Beginning Balance and Ending Balance rows.
- Supports Arabic and English output.
- Returns a downloadable `.xlsx` file.

---

### 7.1 `ExpensesAndReceiptsReport(...) : IActionResult`
- **Attributes**: `[YesGet]`
- Displays the combined Expenses & Receipts report.
- Supports year/month filtering and pagination.
- Prepares selectable Years and Months.
- Uses `PaginatedList<ExpensesAndReceiptsReportElementVM>`.
- Supports AJAX partial rendering.

---

### 7.2 `OpenDetails_ExpensesAndReciptReport(...) : IActionResult`
- **Attributes**: `[YesGet]`
- Opens the detailed Expenses & Receipts report view.
- Validates data availability before opening.
- Redirects back if no data exists.

---

### 7.3 `ValidateOtp_OpenDetails_ExpensesAndReciptReport(...) : IActionResult`
- **Attributes**: `[IgnoreAction]`, `[HttpPost]`
- Validates OTP before opening report details.
- Sends notifications to Managers upon successful validation.
- Returns JSON response (`success`, `message`).

---

### 7.4 `Print_ExpensesAndReceiptsReport(...) : IActionResult`
- **Attributes**: `[IgnoreAction]`, `[YesGet]`
- Returns a print-friendly view of the Expenses & Receipts report.

---

### 7.5 `createExcelReport_Download_ExpensesAndReceiptsReport(...) : IActionResult`
- **Attributes**: `[IgnoreAction]`, `[YesGet]`
- Exports the Expenses & Receipts report to Excel.
- Includes Beginning and Ending Balance rows.
- Supports Arabic and English titles.
- Returns a downloadable `.xlsx` file.

---

## 🔹 Receipt Management

### 8.1 `Receipt(int? expenseId, int? receiptId) : IActionResult`
- Displays the Receiving Receipt form.
- Works with either Expense or Receipt context.
- Prefills data when no receipt exists.
- Auto-generates `ItemNumber` and `CodeSerial`.
- Calculates `FinalAmount`.
- Determines save completeness via `isSavedFull`.

---

### 8.2 `Receipt(ReceivingReceiptVM model) : IActionResult`
- **Attributes**: `[HttpPost]`, `[ValidateAntiForgeryToken]`
- Creates or updates a Receiving Receipt.
- Generates serial code for new records.
- Sends notifications for approval.
- Redirects back to the Receipt view.

---

## 🔹 OTP & Approval

### 9.1 `SendOtp() : IActionResult`
- **Attributes**: `[IgnoreAction]`, `[NoLogging]`, `[HttpPost]`
- Sends OTP for approval workflows.
- Returns JSON success status.

---

### 9.2
 `ValidateOtp(...) : IActionResult`
- **Attributes**: `[IgnoreAction]`, `[HttpPost]`
- Validates OTP codes.
- Returns JSON indicating success or failure.

---

## 🔹 Notes
- Uses `PaginatedList<T>` for server-side pagination.
- AJAX detection via:
  - `"X-Requested-With" == "XMLHttpRequest"`
- File uploads handled via `FileHelper` with temp-file strategy.
- Notifications are sent using SignalR and role-based notification service.
- Localization is driven by `SessionHelper.GetCurrentLanguage()`.
- Excel reports are generated using `ExcelStaticReport.ExcelReportArEn_`.
- Language selection depends on `SessionHelper.GetCurrentLanguage()`.
- Notifications are sent using SignalR and role-based notification services.
- Supports both Expenses and Receipts via `ItemType`.
