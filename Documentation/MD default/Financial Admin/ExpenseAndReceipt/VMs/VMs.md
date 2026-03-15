# Expense and Receipts – ViewModels Explanation

This document explains the **ViewModel (VM)** classes used in the **Admin area** for managing **Expenses and Receipts**.  
All classes belong to the namespace:

`FougeraClub.Areas.Admin.ViewModels.ExpenseAndReceipts`

These ViewModels are responsible for:
- Handling user input and validation
- Supporting reports and balances
- Managing attachments and signatures
- Separating UI concerns from domain entities

---

## 1. ExpenseAndReceiptVM (Main Expense/Receipt Form)

This is the **core ViewModel** used to create and edit expense or receipt records.

### Purpose
- Represents a single expense or receipt transaction
- Handles validation logic
- Manages attachments and VAT calculations
- Supports conditional business rules

### Key Properties
- **Id**: Record identifier.
- **ItemTypeId / ItemType**: Type of transaction (enum-based).
- **ItemTypesList**: Dropdown list for item types.
- **ItemNumber**: Reference number (required).
- **BudgetItemId / BudgetItem**: Linked budget item.
- **BudgetItemsList**: Dropdown list of budget items.
- **ExpensesGateId / ExpensesGateList**: Expense gate selection.
- **ExpensesSourceId / ExpensesSourcesList**: Expense source selection.
- **SupplierId / Supplier**: Supplier information.
- **SuppliersList**: Dropdown list of suppliers.

### Financial Fields
- **Amount**: Base amount (required).
- **Vat**: VAT value.
- **TotalAmount**
- **TotalVAT**
- **TotalAmountWithVAT**

### Date & Notes
- **Date**: Defaults to current Dubai time.
- **Notes**: Description or remarks (required).

### Attachments
- **AttachmentPath**: Stored file path.
- **Attachment**: Uploaded file.
- **Attachment_TempFilePath**: Temporary upload location.
- **Attachment_OldPath**: Previously saved file path (for edit).

### State & Signature
- **isSigned**: Indicates if the record is signed.
- **idForReciptSign**: Identifier used for receipt signing logic.

### Custom Validation Logic
Implements `IValidatableObject`:
- Enforces a **minimum date (1 December 2025)** for all expense sources
  except **MiscellaneousExpenses**.

---

## 2. ExpensesAndReceiptsReportElementVM

Represents **a single row** in an expenses and receipts report.

### Purpose
- Displays transactional movement in reports

### Key Properties
- **Date**
- **Notes**
- **SupplierName**
- **Withdrawal**
- **Deposit**
- **Balance**

---

## 3. ExpensesAndReceiptsReportVM

Represents the **full Expenses & Receipts report**.

### Purpose
- Summarizes financial activity for a given period
- Tracks opening and closing balances
- Supports pagination and approvals

### Key Properties
- **year / month**: Reporting period.
- **BeginningBalance**
- **EndingBalance**
- **ExpensesAndReceipts**: Full list of report rows.
- **PaginatedExpensesAndReceipts**: Paginated report view.

### Signatures
- **AcountantSignatureId / AcountantSignature**
- **ManagerSignitureId / ManagerSignature**

### State
- **IsAbleToOpen**: Controls report accessibility.

---

## 4. ExpensesReportElementVM

Represents **a single expense row** in an expenses-only report.

### Purpose
- Used for detailed expense reporting

### Key Properties
- **ItemNumber**
- **Date**
- **Notes**
- **SupplierName**
- **BudgetItem**
- **Amount**
- **Vat**
- **AmountWithVat**
- **Balance**

---

## 5. ExpensesReportVM

Represents the **Expenses-only report**.

### Purpose
- Focuses solely on expenses (without receipts)
- Supports pagination and approval workflow

### Key Properties
- **year / month**
- **BeginningBalance**
- **EndingBalance**
- **Expenses**
- **PaginatedExpenses**

### Signatures
- **AcountantSignature**
- **ManagerSignature**

### State
- **IsAbleToOpen**

---

## 6. ReceivingReceiptVM

Represents a **Receiving Receipt** issued after collecting money.

### Purpose
- Confirms receipt of funds
- Supports receipt numbering and signatures

### Key Properties
- **Id**
- **itemType**
- **CodeSerial**
- **ItemNumber**
- **Date** (defaults to current Dubai time)
- **Amount**
- **Received**: Who received the money.
- **Payment**: Payment method.
- **SumOfAmount**: Amount in words.
- **Being**: Reason for receipt.
- **FinalAmount**
- **Notes**

### Relationships & State
- **ExpenseId**: Linked expense record.
- **ManagerSignatureId / ManagerSignature**
- **isSavedFull**: Indicates completion status.

---

## Validation & Localization Strategy

- **LocalizedRequired**: Ensures required fields with localized messages.
- **Range & custom validation**: Ensures financial correctness.
- **IValidatableObject**: Applies complex business rules beyond attributes.

---

## Architecture Summary

- ViewModels isolate UI logic from domain entities
- Support:
  - Validation
  - Localization
  - Reporting
  - Approval workflows
- Improve:
  - Maintainability
  - Security
  - User experience

These ViewModels together power the full **Expense & Receipt management and reporting system** in the Admin panel.
