# Cash Disbursement Voucher Domain Models – Full Explanation

This document explains all C# entities related to the **Cash Disbursement Voucher (CDV)** module. These classes are part of the `Domain.Entities.CashDisbursementVoucher` namespace and are designed for use with **Entity Framework Core**, using data annotations for database mapping, relationships, and constraints.

Together, these entities model the full lifecycle of a cash disbursement: voucher creation, details, attachments, acknowledgments, and exchange proofs.

---

## 1. CashDisbursementVoucher (Main Entity)

The **CashDisbursementVoucher** is the core entity representing a financial disbursement record.

### Purpose
- Acts as the parent record for all related details, attachments, acknowledgments, and exchange proofs.
- Stores general voucher metadata and approval signatures.

### Key Properties
- **Id**: Primary key.
- **Type** (`CashDisbursementVoucherType?`): Enum defining the voucher type.
- **Date** (`DateOnly?`): Voucher date (stored as `date` only).
- **Month**: Month reference (text).
- **DocumentNo**: Voucher number.
- **Description**: Description of the disbursement.
- **TotalAmount** (`decimal?`): Total disbursed amount.

### Relationships
- **Details**: One-to-many with `CashDisbursementVoucherDetail`.
- **CashDisbursementVoucherAttachments**: One-to-many with attachments.
- **DisbursementRequestAccountantSignature**: Accountant approval signature.
- **DisbursementRequestSignature**: Manager approval signature.
- **AcknowledgmentReceipt_A_Sig**: Accountant signature used in acknowledgment receipt.

---

## 2. CashDisbursementVoucherDetail

Represents **line items** or breakdown entries within a cash disbursement voucher.

### Purpose
- Stores detailed financial entries related to a voucher.

### Key Properties
- **Id**: Primary key.
- **CashDisbursementVoucherId**: Foreign key to the main voucher.
- **DocumentNo**: Reference document number.
- **DocumentDate** (`DateOnly?`): Date of the supporting document.
- **Notes**: Description or explanation.
- **Amount** (`decimal?`): Amount for the line item.

### Relationships
- Many-to-one with `CashDisbursementVoucher`.

---

## 3. CashDisbursementVoucherAttachment

Represents files attached to a voucher (receipts, documents, proofs).

### Purpose
- Allows storing metadata for uploaded files linked to a voucher.

### Key Properties
- **Id**: Primary key.
- **Name**: File name (max 100 characters).
- **Path**: File storage path (max 300 characters).
- **File** (`IFormFile?`): Uploaded file (not mapped to the database).
- **CashDisbursementVoucherId**: Foreign key to voucher.

### Notes
- `[NotMapped]` ensures the file itself is not stored in the database.

---

## 4. AcknowledgmentReceipt

Represents a formal **acknowledgment of payment receipt** tied to a voucher.

### Purpose
- Confirms that payment has been received and acknowledged.

### Key Properties
- **Id**: Primary key.
- **CashDisbursementVoucherId**: Foreign key to voucher.
- **Title**: Receipt title.
- **ThisTo**: Addressed recipient.
- **Recived**: Recipient name.
- **PaymentVoucherNo**: Related payment voucher number.
- **Amount**: Amount received (string for formatted text).
- **Cheque**: Cheque reference.
- **Bank**: Bank name.
- **Being**: Reason for payment.
- **Date** (`DateOnly?`): Transaction date.
- **HeaderDate** (`DateOnly?`): Header date.
- **HeaderMonth**: Month shown in header.
- **AcknowledgmentText**: Custom acknowledgment message.

### Relationships
- **AccountantSigniture**: Signature of the accountant acknowledging receipt.

---

## 5. ExchangeProof

Represents proof of exchange or transaction evidence linked to a voucher.

### Purpose
- Documents the basis and authorization of financial exchanges.

### Key Properties
- **Id**: Primary key.
- **CashDisbursementVoucherId**: Foreign key to voucher.
- **DocumentNo**: Exchange document number.
- **BasedOn**: Basis of exchange.
- **ItWas**: Description of the transaction.
- **Amount**: Exchange amount.
- **By**: Performed by whom.
- **Bank**: Bank involved.
- **Date** (`DateOnly?`): Exchange date.
- **Being**: Purpose description.

### Relationships
- **AccountantSigniture**: Accountant approval.
- **ManagerSigniture**: Manager approval.
- **Details**: One-to-many with `ExchangeProofDetail`.

---

## 6. ExchangeProofDetail

Represents detailed financial rows within an exchange proof.

### Purpose
- Tracks balances and itemized transactions for an exchange.

### Key Properties
- **Id**: Primary key.
- **ExchangeProofId**: Foreign key to exchange proof.
- **AccountNo**: Account number.
- **CurrentBalance** (`decimal(18,2)?`): Balance before transaction.
- **Item**: Item or transaction name.
- **Amount** (`decimal(18,2)?`): Transaction amount.
- **PaymentType**: Payment method/type.
- **Balance** (`decimal(18,2)?`): Balance after transaction.

### Relationships
- Many-to-one with `ExchangeProof`.

---

## Overall Design Summary

- **CashDisbursementVoucher** is the root aggregate.
- All other entities depend on it either directly or indirectly.
- Entity Framework Core annotations control:
  - Primary keys
  - Foreign keys
  - Column types
  - Validation constraints
- Nullable properties provide flexibility for real-world accounting workflows.
- Signatures are modeled as reusable entities via foreign keys.

This structure supports a complete, auditable, and extensible **cash disbursement workflow**.
