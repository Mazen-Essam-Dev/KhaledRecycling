# Expense and Receipt DTOs

## File Structure
- **ExpensesAndReceiptsReportDTO**: ExpensesAndReceiptsReportDTO.cs
- **ExpensesAndReceiptsReportElementDTO**: ExpensesAndReceiptsReportElementDTO.cs
- **ExpensesReportDTO**: ExpensesReportDTO.cs
- **ExpensesReportElementDTO**: ExpensesReportElementDTO.cs
- **ReceiptDTO**: ReceiptDTO.cs

---

## Core DTOs

### ExpensesAndReceiptsReportDTO
**Purpose**: Data Transfer Object for combined financial reports of expenses and receipts.

**Properties**:
- `Id`: Optional report identifier
- `BeginningBalance`: Starting financial balance (decimal)
- `Month`, `Year`: Reporting period
- `ExpensesAndReceipts`: List of `ExpensesAndReceiptsReportElementDTO`
- `EndingBalance`: Final financial balance (decimal)
- `AcountantSignatureId`, `ManagerSignitureId`: Optional approval references
- `AcountantSignature`, `ManagerSignature`: Navigation properties

---

### ExpensesAndReceiptsReportElementDTO
**Purpose**: Represents a single line item in a combined expenses/receipts report.

**Properties**:
- `Date`: Transaction date
- `Notes`: Transaction notes
- `SupplierName`: Supplier name
- `Withdrawal`: Expense amount
- `Deposit`: Receipt amount
- `Balance`: Running balance after transaction

---

### ExpensesReportDTO
**Purpose**: DTO for dedicated expense reports.

**Properties**:
- `Id`: Optional report identifier
- `BeginningBalance`: Starting balance
- `Month`, `Year`: Reporting period
- `Expenses`: List of `ExpensesReportElementDTO`
- `EndingBalance`: Final balance
- `AcountantSignatureId`, `ManagerSignitureId`: Optional approval references
- `AcountantSignature`, `ManagerSignature`: Navigation properties

---

### ExpensesReportElementDTO
**Purpose**: Represents a single expense in an expense report.

**Properties**:
- `ItemNumber`: Item identifier
- `Date`: Expense date
- `Notes`: Notes for the expense
- `SupplierName`: Supplier name
- `BudgetItem`: Associated budget item
- `Amount`: Expense amount
- `Vat`: VAT amount
- `AmountWithVat`: Total amount including VAT
- `Balance`: Running balance after expense

---

### ReceiptDTO
**Purpose**: DTO for incoming receipts.

**Properties**:
- `ItemNumber`: Item identifier
- `Amount`: Receipt amount
- `Date`: Receipt date
- `FinalAmount`: Approved/Final amount
- `Notes`: Additional notes
