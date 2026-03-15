# Cash Disbursement Voucher – ViewModels Explanation

This document explains all **ViewModel (VM)** classes used in the **Admin area** for the Cash Disbursement Voucher (CDV) module.  
These ViewModels live under the namespace:

`FougeraClub.Areas.Admin.ViewModels.CashDisbursementVoucher`

They are **presentation-layer models**, designed to:
- Transfer data between Controllers and Views
- Apply UI-level validation
- Provide display-friendly fields (names, flags, enums)
- Avoid exposing full domain entities directly to the UI

---

## Key Difference: Entity vs ViewModel

- **Entities** → Represent database tables and business rules  
- **ViewModels** → Represent UI needs, validation, and display logic  

ViewModels often:
- Combine multiple entities
- Add display-only fields
- Use validation attributes like `LocalizedRequired`
- Exclude database-only concerns

---

## 1. CashDisbursementVoucherVM (Main Voucher ViewModel)

This is the **primary ViewModel** used when creating, editing, and displaying a cash disbursement voucher in the Admin UI.

### Purpose
- Represents the voucher form and details on screens
- Handles approval status and signature display
- Supports enum selection and validation

### Key Properties
- **Id**: Voucher identifier.
- **Type / TypeText**: Voucher type (enum value + display text).
- **TypeEnumList**: Dropdown list for voucher types.
- **Date** (`DateOnly?`): Voucher date.
- **Month**: Display month.
- **DocumentNo**: Voucher number.
- **Description**: Voucher description.
- **TotalAmount**: Calculated total amount.
- **Details**: Collection of voucher line items.

### Approval & Signature Info
- **DisbursementRequestSignatureAccountantId** :Accountant .
- **DisbursementRequestAccountantSignature** :Accountant .
- **DisbursementRequestSignatureId** :manager .
- **DisbursementRequestSignature** :manager .
- **ExchangeProofApprovalDone**
- **AcknowledgmentReceiptApprovalDone**

### Display-Only Fields
- **ManagerFullName**
- **AccountantFullName**

---

## 2. CashDisbursementVoucherDetailVM

Represents **individual line items** inside a voucher.

### Purpose
- Used for adding/editing voucher details in the UI

### Key Properties
- **Id**
- **CashDisbursementVoucherId**
- **DocumentNo**
- **DocumentDate** (`DateOnly?`)
- **Notes** (max 500 chars)
- **Amount**

### Notes
- May include a reference to `CashDisbursementVoucherVM` for UI context.

---

## 3. CashDisbursementVoucherAttachmentVM

Represents **a single attachment** uploaded for a voucher.

### Purpose
- Handles file uploads in forms

### Key Properties
- **Id**
- **Name**: File name
- **Path**: Stored file path
- **File** (`IFormFile?`): Uploaded file (UI-only)

---

## 4. CashDisbursementVoucherAttachmentsVM

Represents **a group of attachments** for one voucher.

### Purpose
- Used when uploading or displaying multiple attachments at once

### Key Properties
- **CashDisbursementVoucherId**
- **Attachments**: List of `CashDisbursementVoucherAttachmentVM`

---

## 5. AcknowledgmentReceiptVM

Represents the **Acknowledgment Receipt form** shown in the Admin UI.

### Purpose
- Confirms receipt of payment
- Enforces required fields via UI validation
- Displays accountant signature info

### Key Properties
- **Id**
- **CashDisbursementVoucherId**
- **Title**
- **ThisTo**
- **Recived**
- **PaymentVoucherNo**
- **Amount**
- **Cheque**
- **Bank**
- **Being**
- **Date** (`DateOnly?`)
- **HeaderDate**
- **HeaderMonth**
- **AcknowledgmentText**

### Signature & Display
- **AccountantSignitureId**
- **AccountantSigniture**
- **AccountantFullName**

### State Flag
- **isSavedFull**: Indicates whether the form is fully completed and saved

---

## 6. ExchangeProofVM

Represents the **Exchange Proof** approval document in the UI.

### Purpose
- Captures proof of exchange transactions
- Supports multi-level approval (Accountant & Manager)
- Displays detailed transaction rows

### Key Properties
- **Id**
- **CashDisbursementVoucherId**
- **DocumentNo**
- **BasedOn**
- **ItWas**
- **Amount**
- **By**
- **Bank**
- **Date** (`DateOnly?`)
- **Being**

### Signatures
- **AccountantSignitureId**
- **AccountantSigniture**
- **ManagerSignitureId**
- **ManagerSigniture**

### UI State & Display
- **AccountantFullName**
- **ManagerFullName**
- **IsUserLogedInISManager**
- **isSavedFull**

### Details
- **Details**: List of `ExchangeProofDetailVM`

---

## 7. ExchangeProofDetailVM

Represents **individual rows** inside an Exchange Proof.

### Purpose
- Displays financial breakdown for exchange transactions

### Key Properties
- **Id**
- **ExchangeProofId**
- **AccountNo**
- **CurrentBalance**
- **Item**
- **Amount**
- **PaymentType**
- **Balance**

---

## Validation Strategy

- **LocalizedRequired("Required")**
  - Used instead of standard `[Required]`
  - Supports localization and consistent UI messages

- ViewModels enforce **UI-level validation**, not database constraints

---

## Overall Architecture Summary

- **Entities** → Persist data
- **ViewModels** → Shape data for Admin UI
- ViewModels:
  - Add display-only fields (full names, flags)
  - Combine related data for screens
  - Support validation, dropdowns, and approval workflows
- This separation improves:
  - Maintainability
  - Security
  - UI flexibility
  - Clean architecture

These ViewModels collectively power the full **Cash Disbursement Voucher Admin workflow**.
