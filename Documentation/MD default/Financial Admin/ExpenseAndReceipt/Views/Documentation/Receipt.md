# Receipt Voucher (Acknowledgment Receipt)

## Overview

This document describes the **Receipt Voucher / Acknowledgment Receipt** view used in the Financial Management module. The page supports **data entry, digital signing with OTP, and official printing** for both Receipts and Expenses flows.

Model used:

* `ReceivingReceiptVM`

---

## 1️⃣ Page Purpose

The Receipt Voucher page is responsible for:

* Displaying an official bilingual receipt voucher
* Capturing acknowledgment details from the receiver
* Handling **manager digital signature** using OTP
* Locking the form after signature
* Printing an audit-ready receipt document

The same view is reused for:

* Receipts flow
* Expenses flow (based on `itemType`)

---

## 2️⃣ Header & Breadcrumb Logic

### Breadcrumb Resolution

* Breadcrumb list dynamically switches based on `itemType`:

  * Receipts → Receipts List
  * Expenses → Expenses List

This ensures correct navigation context for the user.

---

### Header Actions

| Action | Condition            | Description          |
| ------ | -------------------- | -------------------- |
| Save   | No manager signature | Saves receipt data   |
| Print  | Signature exists     | Prints the voucher   |
| Back   | Always               | Returns to list page |

Once the receipt is signed, **Save is disabled** and only printing is allowed.

---

## 3️⃣ Signature State Handling

### Signed Flag

```text
boolSigned = ManagerSignature != null && ImagePath exists
```

Behavior:

* When signed → all input fields become readonly
* When unsigned → fields are editable

This guarantees data immutability after approval.

---

## 4️⃣ Form Structure

### Hidden System Fields

* Receipt Id
* Expense Id
* Item Type (Receipt / Expense)
* Code Serial
* Amount (auto-filled before submit)

These fields ensure backend consistency without user interaction.

---

### Visible Fields

| Field         | Description                 |
| ------------- | --------------------------- |
| Date          | Receipt date (readonly)     |
| Code Serial   | Auto-generated receipt code |
| Received      | Person/entity who paid      |
| Payment       | Method of payment           |
| Sum Of Amount | Amount in words             |
| Being         | Purpose of payment          |
| Final Amount  | Numeric amount              |
| Notes         | Description                 |

Arabic and English labels are displayed side-by-side for official use.

---

## 5️⃣ Amount Presentation

### Money Table

* Numeric amount
* Description
* Amount converted to Arabic words using:

```text
TafqeetHelper.Tafqeet
```

Currency:

* AED (Dirham)

This ensures legal and accounting compliance.

---

## 6️⃣ Manager Signature Section

### Signature Rules

* Signature appears only after full save
* Signature image is rendered when available
* If user is a Manager with a registered signature:

  * A clickable **"Click to Sign"** section is shown

---

## 7️⃣ OTP-Based Digital Signing

### OTP Workflow

1. Manager clicks signature area
2. System sends OTP (`SendOtp` endpoint)
3. OTP modal opens
4. Manager enters 4-digit OTP
5. OTP validated via `ValidateOtp`
6. On success:

   * Signature is saved
   * Page reloads
   * Form becomes readonly

---

### OTP Modal Behavior

* Fixed-length (4 digits)
* RTL-friendly input navigation
* Client-side validation
* Server-side verification
* Prevents modal closing until valid

---

## 8️⃣ Printing Behavior

### Print Mode

* Optimized print CSS
* Layout width fixed to 800px
* Navbar & buttons hidden
* Background colors preserved

### Print Trigger

* Enabled only after signature
* Uses browser print dialog

Printed document includes:

* Official government header
* Receipt details
* Signature image

---

## 9️⃣ Client-Side Enhancements

### Data Integrity

* `Amount` is auto-filled from `FinalAmount` before submit

### UX Improvements

* Auto focus in OTP fields
* Inline validation messages
* Prevents accidental edits after signing

---

## 🔐 Security & Audit

* OTP-based approval
* Role-based signing (Manager only)
* Immutable data after signature
* Printed receipt contains official branding

---

## Design Goals

* Legal compliance
* Audit traceability
* Multilingual official output
* Safe approval workflow

---

*Documented for ASP.NET Core MVC – Financial Management / Receipt Voucher*
