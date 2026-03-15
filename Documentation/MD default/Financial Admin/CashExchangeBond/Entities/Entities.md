# Cash Exchange Bond Domain Models – Explanation

This document explains the **Cash Exchange Bond** domain entities found in the  
`Domain.Entities.CashExchangeBond` namespace. These classes model a financial **cash exchange bond** process and are designed for use with **Entity Framework Core**, using data annotations for schema definition, validation, and relationships.

The module supports:
- A master bond record
- Multiple detail rows
- File attachments
- Accountant and manager approvals

---

## 1. CashExchangeBond (Main Entity)

The **CashExchangeBond** is the root entity representing a single cash exchange transaction.

### Purpose
- Records a cash exchange operation
- Acts as the parent entity for details, attachments, and signatures
- Stores summary and approval information

### Key Properties
- **Id**: Primary key.
- **Code** (`string?`): Unique or reference code for the bond.
- **Date** (`DateOnly?`): Date of the cash exchange (stored as `date` only).
- **Money** (`decimal(18,2)?`): Base amount of money involved.
- **PersonName** (`string?`): Name of the person related to the exchange.
- **AboutText** (`string?`): Description or purpose of the exchange.
- **Total** (`decimal(18,2)?`): Total calculated amount.

### Relationships
- **Details**:  
  One-to-many relationship with `CashExchangeBondDetail`.  
  Initialized to an empty list to prevent null reference issues.
- **CashExchangeBondAttachments**:  
  One-to-many relationship with attachment records.
- **AccountantSigniture**:  
  Approval signature from the accountant.
- **ManagerSignature**:  
  Approval signature from the manager.

---

## 2. CashExchangeBondDetail

Represents **individual line items** within a cash exchange bond.

### Purpose
- Stores detailed transactional data related to the bond
- Allows itemized accounting of exchanged cash

### Key Properties
- **Id**: Primary key.
- **CashExchangeBondId**: Foreign key linking to `CashExchangeBond`.
- **DocumentNo** (`string?`): Reference document number.
- **DocumentDate** (`DateOnly?`): Date of the related document.
- **AccountNo** (`string?`): Account number involved in the transaction.
- **Particular** (`string?`): Description of the transaction line.
- **Money** (`decimal(18,2)?`): Amount for this detail line.

### Relationships
- Many-to-one relationship with `CashExchangeBond`.

---

## 3. CashExchangeBondAttachment

Represents **file attachments** associated with a cash exchange bond.

### Purpose
- Stores metadata for uploaded documents (receipts, proofs, etc.)
- Supports file upload without storing binary data in the database

### Key Properties
- **Id**: Primary key.
- **Name**: File name (max 100 characters).
- **Path**: File storage path (max 300 characters).
- **File** (`IFormFile?`): Uploaded file (not mapped to the database).
- **CashExchangeBondId**: Foreign key linking to the parent bond.

### Notes
- `[NotMapped]` ensures the uploaded file is handled only at the application layer.

---

## Design & Architecture Summary

- **CashExchangeBond** is the aggregate root.
- **CashExchangeBondDetail** and **CashExchangeBondAttachment** depend on it.
- Entity Framework Core annotations define:
  - Primary keys
  - Foreign keys
  - Column types and precision
  - Length constraints
- Nullable fields provide flexibility for real-world accounting workflows.
- Signature entities enable approval tracking and auditability.

This structure supports a clean, extensible, and auditable **cash exchange bond workflow** within the financial system.
