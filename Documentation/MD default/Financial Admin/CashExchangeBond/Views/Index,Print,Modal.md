# Cash Exchange Bond – Views Documentation

This document describes the **Razor Views** used to manage **Cash Exchange Bonds** in the Admin area. It explains the structure, responsibilities, permissions, and client-side behavior for:

* `Index.cshtml`
* `_ListPartial.cshtml`
* `Print.cshtml`
* `_CashExchangeBondAttachmentsModal.cshtml`

---

## 1️⃣ Index View (`Index.cshtml`)

### 📌 Purpose

The main listing page for **Cash Exchange Bonds**. It provides searching, filtering, printing, Excel exporting, pagination, and attachment management.

---

### 🧩 Model

```csharp
PaginatedList<CashExchangeBondVM>
```

Used to support:

* Paging
* Page size selection
* Search and date filtering

---

### 🔐 Permissions

Permissions are validated using `PermissionScanner`:

* **Add** → Show "Add New Voucher" button
* **Edit** → Enable edit action
* **Delete** → Enable delete action

UI actions are enabled or disabled based on these permissions.

---

### 🧭 Page Layout

#### ▪ Breadcrumb

Displays navigation hierarchy:

* Financial Management
* Cash Exchange Bond List

#### ▪ Records Counter

Displays total records count dynamically:

```html
<span id="countRecords">@Model.Count</span>
```

Updated after AJAX reloads.

---

### 🧰 Header Actions

* ➕ **Add New Voucher** (permission-based)
* 🖨 **Print** (opens printable report)
* 📊 **Excel Export** (downloads Excel report)

---

### 🔍 Search & Filter Panel

Allows filtering by:

* Document Number (search term)
* Date From
* Date To

Filters are persisted using **`sessionStorage`** so the page restores its state after refresh.

---

### 📄 Partial List Container

```html
<div id="PartialNewListContainer">
    @await Html.PartialAsync("_ListPartial", Model)
</div>
```

The list is reloaded dynamically using AJAX without full page refresh.

---

### 📎 Attachments Modal (Loader)

A Bootstrap modal is prepared and dynamically filled using `fetch`:

* Loads `_CashExchangeBondAttachmentsModal`
* Opens without leaving the index page

---

### ⚙ Client-Side Logic (Index)

#### AJAX Reloading

* Search
* Pagination
* Page size change

Handled via `$.ajax()` calls to `Index` action.

#### Session Storage

Stores:

* Search term
* Date range
* Page index
* Page size

Keyed by page URL to isolate state per page.

#### Print Logic

* Blocks printing if records exceed **3999**
* Opens a standalone print window

#### Excel Export

* Redirects to Excel generation endpoint with filters applied

---

## 2️⃣ List Partial View (`_ListPartial.cshtml`)

### 📌 Purpose

Renders the **Cash Exchange Bonds table**, pagination controls, and row-level actions.

---

### 🧩 Model

```csharp
PaginatedList<CashExchangeBondVM>
```

---

### 📊 Table Columns

| Column        | Description    |
| ------------- | -------------- |
| No            | Bond Code      |
| Money         | Amount         |
| Date          | Bond Date      |
| Control Tools | Action buttons |

---

### ⚙ Row-Level Business Rules

Editing and deleting depend on:

* Accountant signature
* Manager signature
* Logged-in user role

Once both signatures exist, editing and deletion are disabled.

---

### 🧰 Control Actions

Each row may include:

* 📄 **Cash Exchange Bond Receipt**
* ✏ **Edit** (conditional)
* 📎 **Attachments modal**
* 🗑 **Delete** (conditional)

Disabled actions are rendered visually but blocked logically.

---

### 📄 Page Size Selector

Supports:

* 50
* 100
* 150
* All

Triggers AJAX reload on change.

---

### 🔢 Pagination

* Previous / Next
* Page numbers

All pagination links use AJAX (`.ajax-page`).

---

## 3️⃣ Print View (`Print.cshtml`)

### 📌 Purpose

Generates a **printable report** for Cash Exchange Bonds.

---

### 🧩 Model

```csharp
IEnumerable<CashExchangeBondVM>
```

---

### 🖨 Features

* Printable layout (no main site layout)
* RTL / LTR based on language
* Auto-print on load
* Auto-close after printing

---

### 📊 Printed Data

* Bond Code
* Amount
* Date

---

## 4️⃣ Attachments Modal (`_CashExchangeBondAttachmentsModal.cshtml`)

### 📌 Purpose

Manages uploading, previewing, and removing **Cash Exchange Bond attachments** without leaving the index page.

---

### 🧩 Model

```csharp
CashExchangeBondAttachmentsVM
```

---

### 📎 Supported File Types

* Images: JPG, JPEG, PNG
* Documents: PDF

---

### 🧠 Modal Behavior

* Loaded dynamically via `fetch`
* Existing attachments are displayed
* New files are added client-side before submission

---

### 📁 Attachment Preview

Each attachment card includes:

* File preview (image / PDF icon)
* File name
* Remove button

Hidden inputs ensure correct MVC model binding:

```text
Attachments[index].File
Attachments[index].Name
Attachments[index].Path
```

Dynamic re-indexing maintains consistency after deletions.

---

### 🗂 Styling

Custom CSS is applied for:

* Attachment cards
* Hover effects
* Remove button styling

---

## ✅ Summary

The **Cash Exchange Bond views** provide a complete workflow:

* Secure access (permission-based)
* High performance (AJAX partial loading)
* User-friendly filtering and pagination
* Integrated printing and Excel export
* Seamless attachment management

The architecture cleanly separates:

* Index (filters & layout)
* Partial (data rendering)
* Modal (attachments)
* Print (reporting)

---

📌 **Technologies Used**

* ASP.NET Core MVC
* Razor Views & Partial Views
* jQuery & Fetch API
* Bootstrap 5
* Session Storage

---

✍️ *This document is intended for developers and maintainers of the Financial Management module.*
