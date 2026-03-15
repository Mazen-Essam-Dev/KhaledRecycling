# Expense and Receipt Domain Entities

## File Structure
- **ExpenseAndReceiptAndOther**: ExpenseAndReceiptAndOther.cs
- **ExpenseAndReceiptReport**: ExpenseAndReceiptReport.cs
- **ExpensesAndReciptReportSign**: ExpensesAndReciptReportSign.cs
- **ExpensesGate**: ExpensesGate.cs
- **ExpensesReport**: ExpensesReport.cs
- **ExpensesSource**: ExpensesSource.cs
- **ItemTypeEntity**: ItemTypeEntity.cs
- **ReceivingReceipt**: ReceivingReceipt.cs
- **ReportSalaryType**: ReportSalaryType.cs
- **ReportType**: ReportType.cs
- **SalaryReportSign**: SalaryReportSign.cs

---

## Core Entities

### ExpenseAndReceiptAndOther
**Purpose**: Tracks financial expenditures or receipts with optional VAT and reporting links.

**Properties**:

#### Identification & Classification
- `Id`: Primary key (int)
- `ItemType`: Type identifier (int?)
- `ItemTypeEntity`: Navigation to `ItemTypeEntity`
- `ItemNumber`: Item identification number (string?)

#### Financial Categorization
- `BudgetItemId`: Budget item identifier (int?)
- `BudgetItem`: Navigation to `BudgetItem` entity
- `ExpensesGateId`: Expense gate identifier (int?)
- `ExpensesGate`: Navigation property
- `ExpensesSourceId`: Expense source identifier (int?)
- `ExpensesSource`: Navigation property

#### Supplier & Financial Details
- `SupplierId`: Supplier identifier (int?)
- `Supplier`: Navigation property
- `Amount`: Expense/Receipt amount (decimal?)
- `Vat`: VAT amount (decimal?)
- `Date`: Date of transaction (DateOnly)
- `Notes`: Additional notes (string?)

#### File Management
- `AttachmentPath`: Stored file path (string, max 300)
- `Attachment`: File upload interface (IFormFile - NotMapped)

#### Report Relationships
- `ExpensesReportId`: Link to ExpensesReport (int?)
- `ExpenseAndReceiptReportId`: Link to combined report (int?)

---

### ExpenseAndReceiptReport
**Purpose**: Combined report entity for expenses and receipts.

**Properties**:
- `Id`: Primary key (int)
- `BeginningBalance`: Starting balance (decimal)
- `Month`, `Year`: Reporting period
- `ExpensesOrReceipts`: Collection of related `ExpenseAndReceiptAndOther`
- `EndingBalance`: Ending balance (decimal)
- `AcountantSignatureId`, `ManagerSignitureId`: Signature references
- `AcountantSignature`, `ManagerSignature`: Navigation properties

---

### ExpensesAndReciptReportSign
**Purpose**: Tracks approval signatures for combined reports.

**Properties**:
- `Id`: Primary key
- `ReportTypeId`: Reference to `ReportType`
- `Month`, `Year`: Reporting period
- `AcountantSignatureId`, `ManagerSignitureId`: Signature references
- `AcountantSignature`, `ManagerSignature`: Navigation properties

---

### ExpensesGate
**Purpose**: Categorizes expenses by gate.

**Properties**:
- `Id`: Primary key (DatabaseGeneratedOption.None)
- `NameAr`, `NameEn`: Name translations
- `ExpenseAndReceiptAndOthers`: Navigation collection

---

### ExpensesReport
**Purpose**: Dedicated report entity for expenses.

**Properties**:
- `Id`: Primary key
- `BeginningBalance`: Starting balance
- `Month`, `Year`: Reporting period
- `ExpenseAndReceiptAndOthers`: Related expenses
- `EndingBalance`: Ending balance
- `AcountantSignatureId`, `ManagerSignitureId`: Signature references
- `AcountantSignature`, `ManagerSignature`: Navigation properties

---

### ExpensesSource
**Purpose**: Categorizes expenses by source.

**Properties**:
- `Id`: Primary key (DatabaseGeneratedOption.None)
- `NameAr`, `NameEn`: Name translations
- `ExpenseAndReceiptAndOthers`: Navigation collection

---

### ItemTypeEntity
**Purpose**: Defines type of item for transactions.

**Properties**:
- `Id`: Primary key
- `NameAr`, `NameEn`: Name translations
- `ExpenseAndReceiptAndOthers`: Navigation collection

---

### ReceivingReceipt
**Purpose**: Tracks incoming receipts with approval workflow.

**Properties**:

#### Receipt Information
- `Id`: Primary key
- `CodeSerial`: Serial code (string?)
- `itemType`: Item type id (int?)
- `ItemTypeEntity`: Navigation property
- `ItemNumber`: Item number
- `Date`: Receipt date
- `Amount`: Receipt amount

#### Receipt Details
- `Received`: Received from (max 500)
- `Payment`: Payment method (max 500)
- `SumOfAmount`: Amount summary
- `Being`: Purpose description (max 500)
- `FinalAmount`: Approved final amount
- `Notes`: Additional notes (max 500)

#### Approval & Reference
- `ExpenseId`: Related expense identifier
- `ExpenseAndReceiptAndOther`: Navigation property
- `ManagerSignitureId`: Manager signature reference
- `ManagerSignature`: Navigation property

---

### ReportSalaryType
**Purpose**: Defines types of salary reports.

**Properties**:
- `Id`: Primary key (DatabaseGeneratedOption.None)
- `NameAr`, `NameEn`: Name translations

---

### ReportType
**Purpose**: Defines types of financial reports.

**Properties**:
- `Id`: Primary key (DatabaseGeneratedOption.None)
- `NameAr`, `NameEn`: Name translations

---

### SalaryReportSign
**Purpose**: Tracks approval signatures for salary reports.

**Properties**:
- `Id`: Primary key
- `ReportSalaryTypeId`: Reference to `ReportSalaryType`
- `Month`, `Year`: Reporting period
- `AcountantSignatureId`, `ManagerSignitureId`: Signature references
- `AcountantSignature`, `ManagerSignature`: Navigation properties

---

## Key Features

### Data Annotations
- `[Key]`: Primary key
- `[MaxLength]`: String length constraint
- `[NotMapped]`: Exclude file uploads from DB
- `[ForeignKey]`: Explicit relationship mapping
- `[DatabaseGenerated(DatabaseGeneratedOption.None)]`: Prevent auto-increment

### Financial Management
- Tracks both expenses and receipts
- Supports VAT calculations
- Links to monthly and yearly reports
- Dual reporting: combined or dedicated reports

### Workflow Integration
- Manager and accountant approval signatures
- File attachment support
- Supplier and budget item relationship management

### Relationship Architecture
- One-to-Many: Reports → Expenses / Receipts
- Optional relationships for flexibility
- Navigation properties for EF Core
- Enum-based categorization
