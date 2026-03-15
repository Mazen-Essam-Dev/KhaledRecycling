# Receipts Module – List, Partial View & Print Report

## Overview

This document describes the **Receipts listing workflow** in the Financial Management module. It covers:

* Main Receipts List page
* `_ReceiptsListPartial` (AJAX-loaded table)
* `Print_Receipts` printable report

The module is designed for **high-volume financial data**, supports filtering, permissions, printing, and exporting.

Models used:

* `PaginatedList<ExpenseAndReceiptVM>` (List & Partial)
* `IQueryable<ExpenseAndReceiptVM>` (Print)

---

## 1️⃣ Receipts List View (Main Page)

### Purpose

* Display all receipt records
* Allow filtering, pagination, printing, and Excel export
* Act as a container for the AJAX-loaded partial list

---

### Page Metadata & Permissions

* Dynamic titles via localization resources
* Permissions resolved using `PermissionScanner`:

  * `Add` → Create new receipt
  * `Edit` → Edit receipt
  * `Delete` → Delete receipt

UI actions are rendered **only if permission exists**.

---

### Header Actions

| Action     | Description                                |
| ---------- | ------------------------------------------ |
| Create New | Opens Add/Edit page in Receipt mode        |
| Print      | Opens printable report with active filters |
| Excel      | Downloads Excel report using same filters  |

Print action is **restricted** when record count exceeds `3999`.

---

### Filters Section

#### Available Filters

* Search (Notes / Description)
* Supplier
* Source
* Date From
* Date To

#### Behavior

* Submitted via **AJAX** (no full page reload)
* Filter values are stored in `sessionStorage`
* Filters are restored automatically on page revisit

---

### AJAX Rendering

* Partial view loaded inside:

```html
#PartialNewListContainer
```

* Endpoint:

```text
GET /Admin/ExpenseAndReceipt/Receipts
```

* Loading indicator displayed during request
* Record count updated dynamically

---

## 2️⃣ `_ReceiptsListPartial` View

### Purpose

* Render paginated receipts table
* Reusable & lightweight
* Optimized for AJAX updates

---

### Table Columns

| Column      | Description                       |
| ----------- | --------------------------------- |
| Item No     | Receipt serial number             |
| Amount      | Incoming amount                   |
| Description | Notes                             |
| Supplier    | Arabic / English based on session |
| Source      | Resolved from ExpensesSourcesList |
| Date        | Formatted receipt date            |
| Actions     | View / Receipt / Edit / Delete    |

---

### Actions Column Logic

| Action          | Rule                         |
| --------------- | ---------------------------- |
| View Attachment | Enabled only if PDF exists   |
| Receipt         | Opens receipt preview        |
| Edit            | Visible if Edit permission   |
| Delete          | Visible if Delete permission |

All actions are protected by:

* Permissions
* Anti-forgery tokens

---

### Totals Row

* Rendered only when records exist
* Displays total receipt amount
* Highlighted visually

---

### Pagination

* Page size selector: `50 / 100 / 150`
* AJAX pagination using `.ajax-page`
* Preserves filters and page size

---

## 3️⃣ Print_Receipts View

### Purpose

* Generate **official printable receipts report**
* Auto-print on load
* Used by finance & audit teams

---

### Layout Characteristics

* No layout file (`Layout = null`)
* RTL / LTR styles based on language
* Print-optimized CSS

---

### Report Header

* Government & organization branding
* Logo centered
* Bilingual titles

---

### Printed Table Columns

| Column      | Description        |
| ----------- | ------------------ |
| Item No     | Receipt number     |
| Amount      | Incoming amount    |
| Description | Notes              |
| Supplier    | Destination entity |
| Date        | Receipt date       |

---

### Totals Section

* Final row displays total amount
* Highlighted background

---

### Footer Information

* Print time & date (Dubai time)
* Printed by (current user)
* Official footer copyright

---

### Auto Print Behavior

```text
- Show loading overlay
- Auto-print after load
- Auto-close window after printing
```

This ensures fast, user-friendly reporting.

---

## Security & UX Considerations

* Permission-based rendering
* Print count limitation
* No data exposure in partial reloads
* User identity included in print audit

---

## Design Goals

* High performance for large datasets
* Clean separation of concerns
* Reusable partial views
* Audit-friendly printed reports

---

*Documented for ASP.NET Core MVC – Financial Management / Receipts Module*
