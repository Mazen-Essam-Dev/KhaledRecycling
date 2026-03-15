# 📊 Expenses & Receipts Report Module

This document explains the **ExpensesAndReceiptsReport** feature, including:

* Main report page
* AJAX list partial view
* Open Details & Approval (OTP + Signatures)

The module is part of the **Financial Management** area and provides a full lifecycle for viewing, approving, exporting, and printing financial movements.

---

## 1️⃣ ExpensesAndReceiptsReport (Main Page)

### 🎯 Purpose

Displays a summarized **Receipts & Payments Report** filtered by **Month** and **Year**, with:

* Pagination (AJAX)
* Excel export
* Approval entry point

---

### 🧭 Breadcrumb & Header

* Dynamic breadcrumb under **Financial Management**
* Displays total records count dynamically
* Action buttons:

  * **Approval** → opens detailed approval page
  * **Excel Export** → downloads report as Excel

---

### 🔎 Filters

* Month selector
* Year selector
* Validation prevents search without both values
* Filters state persisted using `sessionStorage`

---

### 📄 Report List (AJAX)

* Loaded via `_ExpensesAndReceiptsReportListPartial`
* Supports:

  * Pagination
  * Page size selection
  * Dynamic record count update

---

### 🧠 Client-Side Logic

* AJAX reload on:

  * Search
  * Page change
  * Page size change
* Prevents printing or approval without valid filters
* Toast notifications for invalid actions

---

## 2️⃣ _ExpensesAndReceiptsReportListPartial

### 🎯 Purpose

Renders the **tabular financial movements** inside the main report using AJAX.

---

### 📋 Table Structure

| Column             | Description       |
| ------------------ | ----------------- |
| Date               | Transaction date  |
| Description        | Notes             |
| Document Authority | Supplier / Source |
| Deposit            | Incoming amount   |
| Withdrawal         | Outgoing amount   |
| Balance            | Running balance   |

---

### 📊 Balance Rows

* **Beginning Balance** (before transactions)
* **Ending Balance** (after transactions)

---

### 📄 Empty State

If no data:

* Displays friendly no-data message
* Prompts user to select year & month

---

### 🔢 Pagination

* AJAX-based navigation
* Previous / Next buttons
* Active page highlighting

---

## 3️⃣ OpenDetails_ExpensesAndReciptReport (Approval Page)

### 🎯 Purpose

Displays a **printable, official financial report** with:

* OTP-based digital signatures
* Accountant & Manager approval
* Legal-ready print layout

---

### 🖨️ Print Design

* Government-style bilingual header (Arabic / English)
* Fixed-width printable area
* Print-only CSS
* Hidden UI controls during printing

---

### ✍️ Signature Logic

#### Roles

* **Accountant**
* **Manager**

#### Behavior

* If signature exists → display image
* If missing and user has permission → show *Click to Sign*

---

### 🔐 OTP Workflow

1. User clicks **Sign**
2. OTP sent via backend
3. OTP modal opens
4. User enters 4-digit code
5. Backend validates OTP
6. On success:

   * Signature saved
   * Page reloads

---

### 🧾 Security Rules

* Signatures locked after confirmation
* Printing allowed **only after Manager signature**
* OTP bound to:

  * Year
  * Month
  * Role

---

## 4️⃣ Excel & Print Actions

### 📥 Excel Export

* Requires valid month & year
* Generates downloadable Excel file

### 🖨️ Printing

* Restricted if records exceed limit
* Opens in isolated window
* Ensures page does not freeze

---

## 5️⃣ Key Design Decisions

* AJAX-first UI for performance
* Stateless filters stored client-side
* OTP-based digital approval
* Immutable signed reports
* Audit-ready printable output

---

## ✅ Summary

This module provides a **complete financial reporting lifecycle**:

✔ Filter & view data
✔ Paginate efficiently
✔ Export to Excel
✔ Secure approval (OTP)
✔ Official printable report

