# 📄 ReceiptService.cs

## 📦 Namespace
`Application.Services.Admin.ExpenseAndReceipt`

---

## 🧩 Class: ReceiptService
Handles all business logic related to **Receipts**, including CRUD operations, serial code generation, financial consistency, and cascading balance updates across months.

Implements:
- `IReceiptService`

---

### 🔹 Dependencies
| Dependency | Purpose |
|------------|---------|
| `IUnitOfWork _unitOfWork` | Repository access and transaction management |
| `IConfiguration configuration` | Reads finance-related configuration values |

---

## 🔹 Enums

### `ItemType`
Defines transaction classification.
- `Expenses`
- `Receipts`

---

### `ExpensesSourceEnum`
Defines receipt / expense source classification.
- `MiscellaneousExpenses`
- Other domain-defined sources

---

## 🔹 Core Responsibilities
- Receipt CRUD operations
- Serial code generation
- Receipt classification by source
- Monthly balance recalculation
- Cascading balance propagation across future months
- Maintaining financial integrity after add, update, or delete

---

## 🔹 Methods

### 1. `GetLastSerialCode() : Task<string>`
- Generates the next receipt serial code.
- Uses incremental numeric logic.
- Returns a formatted 4-digit string.

---

### 2. `GetAllAsync()`
- Returns all receipt records.
- Filters by `ItemType = Receipts`.
- Includes related `Supplier` and `BudgetItem`.

---

### 3. `GetByIdAsync(int id)`
- Retrieves a single receipt by ID.
- Restricted to receipt-type records only.

---

### 4. `AddAsync(ExpenseAndReceiptAndOther entity)`
- Adds a new receipt record.
- Determines correct financial classification based on:
  - Receipt date (year / month)
  - Source enum
- Automatically:
  - Updates monthly balances
  - Cascades balance changes forward

---

### 5. `UpdateAsync(ExpenseAndReceiptAndOther entity)`
- Updates receipt values.
- Detects date or amount changes.
- Recalculates affected balances.
- Propagates changes to subsequent months.

---

### 6. `DeleteAsync(int id)`
- Deletes a receipt record.
- Removes related attachment file if exists.
- Recalculates balances for affected months.
- Ensures cascading balance consistency.

---

## 🔹 Cascading Utilities

### 7. `CascadeUpdateBalancesAsync(startingBalance, year, month)`
- Recalculates balances starting from a specific month.
- Applies changes sequentially to all future months.
- Ensures no balance gaps or inconsistencies.

---

## 🔹 Configuration Notes
- Initial balances are read from application configuration.
- Defaults are applied if configuration values are missing.

---

## 🔹 Notes
- `ExpenseAndReceiptReports` table has been **removed**.
- `ExpensesReports` table has been **removed**.
- Monthly balances are now calculated dynamically from transactional data.
- No report snapshot tables are used.
- Financial integrity is maintained via cascading recalculation logic.
- Enums are used for strict type safety and clearer business rules.
