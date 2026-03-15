# Cash Exchange Bond – Add/Edit & Receipt

This document describes the **Cash Exchange Bond** feature, covering both:

* **Add/Edit Form** (data entry & validation)
* **Receipt / Print View** (final approved voucher with signatures & OTP)

---

## 1️⃣ Add/Edit – CashExchangeBondReceipt

### 📌 Purpose

Used to **create or edit** a Cash Exchange Bond with full validation, document details, totals calculation, and role-based restrictions.

---

### 🧩 Key Features

* Create / Edit Cash Exchange Bond
* Automatic total calculation
* Amount conversion to **Arabic & English text**
* Add / Remove multiple document details dynamically
* Prevent editing after **Accountant + Manager** signatures
* Date validation via AJAX

---

### 🧠 View Logic

* `isEdit` determines Create vs Edit mode
* Titles & breadcrumbs change dynamically
* Save button is disabled once both signatures exist

```csharp
var isEdit = Model?.Id > 0;
```

---

### 📝 Main Fields

| Field      | Description        |
| ---------- | ------------------ |
| Date       | Bond date          |
| Code       | Auto / manual code |
| Money      | Total cash amount  |
| PersonName | Disbursed to       |
| AboutText  | Reason for payment |

---

### 💰 Money Text Conversion

* On change of **Money** field
* AJAX call to `MoneyText` action
* Displays amount in:

  * Arabic words
  * English words

---

### 📄 Document Details Section

Allows adding multiple documents:

| Field         | Description          |
| ------------- | -------------------- |
| Document No   | Reference number     |
| Document Date | Date                 |
| Account No    | Accounting reference |
| Particular    | Description          |
| Money         | Amount               |

✔ Rows are dynamically added
✔ Validation before adding
✔ Total auto-calculated

---

### 📊 Total Calculation

* Sums all detail rows
* Updates:

  * Numeric total
  * Textual total (Arabic)

---

### ❌ Delete Confirmation

* Delete row opens confirmation modal
* Re-indexes rows after deletion

---

### 🔒 Restrictions

* If **Accountant & Manager** signatures exist:

  * Save button is disabled

---

## 2️⃣ Cash Exchange Bond Receipt (Print View)

### 📌 Purpose

Displays the **final approved voucher** for review, signature, OTP confirmation, and printing.

---

### 🖨 Print Rules

* Print button visible **only after both signatures**
* Optimized A4 layout
* RTL & LTR bilingual layout

---

### 🧾 Receipt Header

* UAE / Fujairah Government
* Fujairah Science Club
* Arabic & English titles

---

### 📅 Voucher Info

| Field | Value        |
| ----- | ------------ |
| Date  | `Model.Date` |
| Code  | `Model.Code` |

---

### 💵 Amount Section

* Numeric amount
* Arabic textual amount
* “Only” confirmation label

---

### 👤 Payee Information

* Disbursed To: `PersonName`
* Reason: `AboutText`

---

### ✍ Signatures & OTP

| Role       | Action                             |
| ---------- | ---------------------------------- |
| Accountant | OTP → Signature                    |
| Manager    | OTP → Signature (after accountant) |

* OTP sent via AJAX
* 4-digit code validation
* Modal-based confirmation

---

### 📑 Details Table

Displays all document rows:

| Money | Account No | Particular |
| ----- | ---------- | ---------- |

✔ Shows total
✔ Shows Arabic text total

---

### 🔐 Security Logic

* Manager signature allowed **only after accountant signs**
* OTP required per signature
* Auto reload after successful signing

---

### ✅ Final States

* Fully signed → Printable
* Partially signed → Pending

---

## ✔ Summary

The **Cash Exchange Bond module** ensures:

* Accurate financial entry
* Multi-document support
* Secure approval workflow
* OTP-based digital signatures
* Professional printable receipt

---
