# 📄 ExpensesReport Module (Admin Area)

**Namespace:** `FougeraClub.Areas.Admin.ViewModels.ExpenseAndReceipts`
**ViewModel:** `ExpensesReportVM`
**Area:** `Admin`
**Controller:** `ExpenseAndReceiptController`

---

## 1️⃣ ExpensesReport.cshtml

### Purpose

* Displays the main report page for Expenses & Receipts.
* Allows filtering by **Year** and **Month**.
* Supports **pagination**, **Excel export**, **printing**, and **opening details page**.
* Checks user permissions for add, edit, delete.

### Key Features

* **Breadcrumb navigation** with dynamic titles:

  * `ViewData["Title pakage"]` → Financial Management
  * `ViewData["Title Single"]` → Petty Cash Report
* **Filter form**:

  * Month dropdown
  * Year dropdown
  * Clear button
  * Search button
* **Partial table rendering** using `_ExpensesReportListPartial`
* **Pagination & page size selection**
* **Excel export** (`openExcelPage`)
* **Print** (`openPrintPage`) with record limit warning
* **Open details** (`OpenDetails_ExpensesReportPage`) with validation for filters
* **SessionStorage** to save last selected filters

### JavaScript Highlights

* `saveFilters(page, pageSizeOverride)` → store filters in sessionStorage
* `loadNewPartialList(page, pageSizeOverride)` → AJAX load `_ExpensesReportListPartial`
* Toasters for invalid filter selection
* Print helper ensures print window doesn’t freeze main page

---

## 2️⃣ _ExpensesReportListPartial.cshtml

### Purpose

* Partial view that renders **table of expenses** dynamically.
* Supports **pagination** and **page size selection**.

### Table Columns

| No | Date | Description | Company Name | Expenses Type | Amount (without VAT) | VAT | Amount with VAT | Balance |

### Special Rows

* **Beginning Balance**
* **Ending Balance**

### Behavior

* Displays a friendly message if **no data available**
* Updates **record count** in parent view
* Pagination links call AJAX to reload table dynamically

---

## 3️⃣ OpenDetails_ExpensesReport.cshtml

### Purpose

* Shows **detailed report** with Accountant & Manager signatures
* Supports **OTP signing workflow** for approvals
* Printable **report area** (`printArea`) with corporate header
* Table structure same as `_ExpensesReportListPartial`

### Signature Workflow

* Displays **existing signatures** if available
* If no signature:

  * Show clickable **“Click here to sign”**
  * Opens **OTP modal**
  * Sends OTP via `/SendOtp` AJAX call
  * Validates OTP via `/ValidateOtp_OpenDetails_ExpensesReport`

### OTP Modal

* 4-digit code input
* RTL input support
* Auto-focus next input
* Shows invalid feedback if OTP is wrong

### Print & Excel

* Print only allowed if Manager has signed
* Uses `window.print()` for inline printing
* Excel export URL `/createExcelReport_Download_ExpensesReport`

### JavaScript Highlights

* `printDiv(divId)` → triggers browser print
* `OpenDetails_ExpensesReportPage()` → validates filters before opening details page
* OTP modal logic:

  * Sends OTP to server
  * Handles 4-digit input navigation
  * Submits OTP for validation
  * Refreshes page on success

---

## 4️⃣ Permissions & Roles

* Uses `PermissionScanner.ValidatePermission` for Add/Edit/Delete
* Accountant & Manager roles supported for OTP signing
* Signature section checks:

  * `CheckLoggedUserIfHasSignature()`
  * `ValidateRoleNumber()`

---

## 5️⃣ Styles

* `.overflow-table` → responsive table scrolling
* Print media queries:

  * Hide navigation & buttons
  * Adjust table width
  * Preserve colors & layout
* Table cells use **inset box-shadow** for print borders

---

## 6️⃣ Notes

* `ExpensesReportVM` contains:

  * `PaginatedExpenses` → for partial list
  * `Expenses` → full detail for `OpenDetails_ExpensesReport`
  * `BeginningBalance` / `EndingBalance`
  * `AcountantSignature` / `ManagerSignature`
  * `month` / `year`
* All date & amount formatting applied consistently (`d` format for date, `F2` for amounts)
* Multi-language support for **Arabic & English** via `Resource1` / `Resource2`
* AJAX errors are displayed inline in the partial container

---

✅ This `.md` consolidates all three views (`ExpensesReport`, `_ExpensesReportListPartial`, `OpenDetails_ExpensesReport`) with **filtering, pagination, print/export, and OTP signing** logic.
