# Cash Disbursement Voucher – Views Documentation

This document describes the **Razor Views** used to manage **Cash Disbursement Vouchers** in the Admin area. It covers the **Index page**, **List Partial**, **Print view**, and **Attachments Modal**, explaining responsibilities, UI behavior, and client-side logic.

---

## 1️⃣ Index View (`Index.cshtml`)

### 📌 Purpose

The main entry page for listing, filtering, printing, exporting, and managing Cash Disbursement Vouchers.

### 🧩 Model

```csharp
PaginatedList<CashDisbursementVoucherVM>
```

Used to support pagination, filtering, and dynamic page size.

### 🔐 Permissions

Permissions are checked using `PermissionScanner`:

* **Add**: Show "Add New Voucher" button
* **Edit**: Enable edit actions
* **Delete**: Enable delete actions

### 🧭 UI Sections

#### ▪ Breadcrumb

Displays navigation hierarchy:

* Financial Management
* Cash Disbursement Voucher List

#### ▪ Header Actions

* ➕ Add new voucher (permission-based)
* 🖨 Print vouchers
* 📊 Export Excel report

#### ▪ Filter Panel

Allows filtering by:

* Bond / Document number
* Date range (From – To)

Filters are persisted using **`sessionStorage`** to restore state on page reload.

#### ▪ Partial List Container

```html
<div id="PartialNewListContainer">
    @await Html.PartialAsync("_ListPartial", Model)
</div>
```

Loads voucher list dynamically using AJAX.

---

## 2️⃣ List Partial View (`_ListPartial.cshtml`)

### 📌 Purpose

Renders the vouchers table, pagination, and row-level actions.

### 🧩 Model

```csharp
PaginatedList<CashDisbursementVoucherVM>
```

### 📊 Table Columns

* Voucher Number
* Total Amount
* Date
* Control Actions

### ⚙ Control Actions

Each row supports:

* 🧾 Print Disbursement Request
* 📄 Exchange Proof (conditional)
* 📑 Acknowledgment Receipt (conditional)
* ✏ Edit (permission-based)
* 📎 Upload attachments
* 🗑 Delete (permission & approval-based)

Disabled icons are shown if business rules are not met.

### 📄 Pagination

AJAX-based pagination using:

* Previous / Next buttons
* Page numbers

Page size selector supports:

* 50 / 100 / 150 / All

---

## 3️⃣ Print View (`Print.cshtml`)

### 📌 Purpose

A **layout-less** printable report for Cash Disbursement Vouchers.

### 🧩 Model

```csharp
IEnumerable<CashDisbursementVoucherVM>
```

### 🖨 Features

* RTL / LTR support based on language
* Official government header (Arabic & English)
* Auto print on load
* Auto close window after printing

### 📊 Printed Data

* Voucher Number
* Amount
* Date

### 🧾 Footer Information

* Print time
* Print date
* Printed by (current user)

---

## 4️⃣ Attachments Modal (`_CashDisbursementVoucherAttachmentsModal.cshtml`)

### 📌 Purpose

Manage uploading and viewing voucher attachments without leaving the list page.

### 🧩 Model

```csharp
CashDisbursementVoucherAttachmentsVM
```

### 📎 Supported Files

* Images: JPG, PNG, JPEG
* Documents: PDF

### 🧠 Modal Behavior

* Loaded dynamically via `fetch`
* Existing attachments are previewed
* New files are added client-side before submit

### 🗂 Attachment Card

Each attachment includes:

* Preview (image or icon)
* File name
* Remove button

Hidden inputs are generated to preserve correct model binding:

```text
Attachments[index].File
Attachments[index].Name
Attachments[index].Path
```

Dynamic re-indexing ensures correct submission after removals.

---

## 5️⃣ Client-Side Logic (JavaScript)

### 🔄 AJAX List Reload

* Filters
* Pagination
* Page size

All handled without full page reload.

### 💾 Session Storage

Stores:

* Search term
* Date range
* Page index
* Page size

Keyed by page URL to keep state per page.

### 🧾 Print & Export

* **Print**: Opens a popup window with limits (max 3999 records)
* **Excel**: Redirects to export endpoint

---

## ✅ Summary

This view set provides a **complete voucher management experience**:

* Secure (permission-based)
* User-friendly (AJAX + modals)
* Performant (partial loading)
* Printable & exportable

It follows clean separation of concerns:

* Index → Layout & filters
* Partial → Data rendering
* Modal → Attachments handling
* Print → Reporting

---

📌 **Used Technologies**

* ASP.NET Core MVC
* Razor Views & Partial Views
* jQuery + Fetch API
* Bootstrap 5
* Session Storage

---

✍️ *This document is intended for developers and maintainers of the Financial Management module.*
