# Cash Exchange Bond – ViewModels Explanation

This document explains the **ViewModel (VM)** classes used in the **Admin area** for the **Cash Exchange Bond** module.  
These ViewModels belong to the namespace:

`FougeraClub.Areas.Admin.ViewModels.CashExchangeBond`

They are designed specifically for the **presentation layer**, handling:
- UI validation (localized)
- User input
- Display-friendly data
- File uploads
- Approval and signature visualization

---

## Entity vs ViewModel Reminder

- **Domain Entities** → Database persistence and business rules  
- **ViewModels** → UI interaction, validation, and display  

ViewModels:
- Use localized validation attributes
- Add UI-only fields (full names, lists, flags)
- Avoid exposing domain entities directly to the UI

---

## 1. CashExchangeBondVM (Main ViewModel)

This is the **primary ViewModel** used to create, edit, and display a Cash Exchange Bond in the Admin UI.

### Purpose
- Represents the full cash exchange bond form
- Handles validation and approval signatures
- Displays accountant and manager information

### Key Properties
- **Id**: Identifier of the bond.
- **Code**: Bond reference code (required, max 100).
- **Date** (`DateOnly?`): Bond date.
- **Money** (`decimal(18,2)?`): Main cash amount (supports negative values).
- **PersonName**: Name of the person involved.
- **AboutText**: Description of the transaction.
- **Total** (`decimal(18,2)?`): Total calculated amount.

### Details
- **Details**: Collection of `CashExchangeBondDetail` records.
- Initialized to avoid null-reference issues.

### Signatures
- **AccountantSignitureId / AccountantSigniture**
- **ManagerSignitureId / ManagerSignature**

### Display-Only Fields
- **AccountantFullName**
- **ManagerFullName**

---

## 2. CashExchangeBondDetailVM

Represents **individual transaction rows** within a Cash Exchange Bond.

### Purpose
- Captures detailed accounting entries
- Enforces strict validation rules for financial data

### Key Properties
- **Id**
- **CashExchangeBondId**
- **DocumentNo** (required, max 200)
- **DocumentDate** (`DateOnly?`)
- **AccountNo** (required, max 100)
- **Particular** (required, max 200)
- **Money** (`decimal(18,2)?`)

### Validation
- Uses `LocalizedRequired` and `LocalizedMaxLength`
- Numeric validation via `Range` with localized error messages

---

## 3. CashExchangeBondAttachmentVM

Represents **a single attachment** for a cash exchange bond.

### Purpose
- Handles file upload and display in the UI

### Key Properties
- **Id**
- **Name**: File name
- **Path**: File storage path (max 300 characters)
- **File** (`IFormFile?`): Uploaded file (UI-only)

---

## 4. CashExchangeBondAttachmentsVM

Represents **a collection of attachments** linked to a bond.

### Purpose
- Simplifies batch attachment handling in views

### Key Properties
- **CashExchangeBondId**
- **Attachments**: List of `CashExchangeBondAttachmentVM`

---

## Validation & Localization Strategy

- **LocalizedRequired / LocalizedMaxLength**
  - Provide culture-aware validation messages
- **Range**
  - Ensures numeric correctness with localized errors
- Validation occurs at the **UI level**, not the database level

---

## Overall Architecture Summary

- **CashExchangeBondVM** is the central ViewModel
- Detail and attachment VMs support modular UI components
- ViewModels:
  - Enhance UX with localization
  - Protect domain entities
  - Support approval workflows
  - Cleanly separate concerns

These ViewModels together enable a robust, user-friendly **Cash Exchange Bond management experience** in the Admin panel.
