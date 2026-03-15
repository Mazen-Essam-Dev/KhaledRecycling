# 📄 ExpenseService.cs

## 📦 Namespace
`Application.Services.Admin.ExpenseAndReceipt`

---

## 🧩 Class: ExpenseService
Provides all business logic related to expenses, receipts, financial reports, OTP approval workflows, signatures, balances calculation, and cascading monthly report updates.

Implements:
- `IExpenseService`

---

### 🔹 Dependencies
| Dependency | Purpose |
|----------|---------|
| `IUnitOfWork _unitOfWork` | Database access via repositories |
| `IHttpContextAccessor _httpContextAccessor` | Access current user context |
| `ISMSService _SMSService` | SMS sending service |
| `ISMSForSendingOTPService _SMSForSendingOTPService` | OTP delivery service |
| `IConfiguration configuration` | Reads seeded finance configuration |

---

### 🔹 Configuration Seeds
| Key | Purpose |
|----|--------|
| `Finance:BeginningBalanceExpensesReport` | Initial beginning balance for Expenses Report |
| `Finance:BeginningBalanceExpenseAndReceiptReport` | Initial beginning balance for Expenses & Receipts Report |

---

## 🔹 Enums

### `ItemType`
Defines the transaction type.
- `Expenses`
- `Receipts`

---

### `ReportTypeEnum`
Defines report classification.
- `ExpensesReport`
- `ExpensesAndReceiptsReport`

---

### `RoleNumber`
Defines system roles.
- `Accountant`
- `Manager`

---

## 🔹 Core Responsibilities

- Serial code generation
- OTP creation and validation
- Signature workflows (Accountant / Manager)
- Dynamic monthly report calculation
- Expense & receipt CRUD
- Automatic balance cascading across months
- Receiving Receipt management

---

## 🔹 Methods

### 1. `GetLastSerialCode() : Task<string>`
- Generates next 4-digit serial code.
- Based on latest `ReceivingReceipts` record.
- Filters by Expenses item type only.

---

### 2. `SendOtpAsync() : Task<bool>`
- Generates and saves OTP using `OTPHelper`.
- Sends OTP via SMS.
- Used for approval workflows.

---

## 🔹 OTP & Signatures

### 3. `ValidateOtp_OpenDetails_ExpensesReportAsync(...)`
- Validates OTP.
- Saves Accountant or Manager signature.
- Creates or updates sign record per (Year, Month).
- Restricts Manager signing unless Accountant already signed.

---

### 4. `ValidateOtp_OpenDetails_ExpensesAndReciptReportAsync(...)`
- Same workflow as Expenses Report.
- Applies to combined Expenses & Receipts report.

---

## 🔹 Expense & Receipt Retrieval

### 5. `GetAllAsync()`
- Returns all Expense records.
- Filters by `ItemType = Expenses`.
- Includes `BudgetItem` and `Supplier`.

---

### 6. `GetByIdAsync(int id)`
- Returns single Expense or Receipt entity by ID.

---

## 🔹 Reports (Dynamic – No Stored Rows Used)

### 7. `GetAllExpensesReportAsync(lang, year, month)`
- Computes beginning balance dynamically:
  - Seed value
  - Net prior months
  - Current month receipts (chapter-one)
- Lists **only expenses**.
- Calculates running balance.
- Includes signatures if exist.
- Returns `ExpensesReportDTO`.

---

### 8. `GetAllExpenseAndReceiptReportAsync(lang, year, month)`
- Computes beginning balance dynamically:
  - Seed value
  - Prior non–chapter-one transactions
- Combines expenses and receipts.
- Preserves database sequence order.
- Calculates running balance.
- Returns `ExpensesAndReceiptsReportDTO`.

---

## 🔹 Expense CRUD Logic

### 9. `AddAsync(ExpenseAndReceiptAndOther entity)`
- Assigns correct report based on:
  - ItemType
  - ExpensesSourceId
- Creates reports when missing.
- Updates ending balances.
- Cascades balances forward.
- Handles both Expenses Report and Combined Report.

---

### 10. `UpdateAsync(ExpenseAndReceiptAndOther entity)`
- Detects date/month changes.
- Reassigns report relationships.
- Refreshes affected months.
- Cascades recalculated balances.

---

### 11. `DeleteAsync(int id)`
- Deletes expense/receipt record.
- Recalculates or deletes related reports.
- Cascades balances forward if needed.
- Deletes attached file from disk.

---

## 🔹 Report Refresh Helpers

### 12. `RefreshExpensesReportForMonthAsync(year, month)`
- Rebuilds Expenses Report for a month.
- Applies only to Chapter-One expenses.
- Deletes report if empty.
- Cascades following months.

---

### 13. `RefreshExpenseAndReceiptReportForMonthAsync(year, month)`
- Rebuilds combined report.
- Excludes Chapter-One receipts.
- Deletes report if empty.
- Cascades following months.

---

## 🔹 Receiving Receipts

### 14. `GetReceivingReceiptByIdAsync(expenseId)`
- Retrieves receipt linked to expense.
- Includes Manager signature.

---

### 15. `AddAsync(ReceivingReceipt entity)`
- Adds new receiving receipt.
- Returns generated ID.

---

### 16. `UpdateAsync(ReceivingReceipt entity)`
- Updates receipt values.

---

### 17. `ValidateOtpAsync(receiptId, code)`
- Validates OTP.
- Assigns Manager signature to receipt.

---

## 🔹 Year Helpers

### 18. `GetAllYearsInDb()`
- Returns available years for Expenses Report.
- Derived dynamically from transactions (Chapter-One only).

---

### 19. `GetAllYearsOfExpenseAndReceiptReportInDb()`
- Returns years for combined report.
- Derived from transactions with `ExpensesSourceId = 1`.

---

## 🔹 Cascade Utilities

### 20. `CascadeUpdateExpensesReportsAsync(...)`
- Recalculates future Expenses Reports.
- Propagates beginning and ending balances.

---

### 21. `CascadeUpdateExpenseAndReceiptReportsAsync(...)`
- Recalculates future combined reports.
- Excludes Chapter-One receipts.

---


## 🔹 Notes
- `ExpensesReports` and `ExpenseAndReceiptReports` tables are **deprecated**.
- Reports are **calculated dynamically**,from transactions not stored as static snapshots.
- Enums are used consistently for type safety.
- OTP-based approval ensures controlled report access.
- Balance integrity is guaranteed via cascading recalculation.
- Chapter one **deprecated** as like any Chapter Now.
