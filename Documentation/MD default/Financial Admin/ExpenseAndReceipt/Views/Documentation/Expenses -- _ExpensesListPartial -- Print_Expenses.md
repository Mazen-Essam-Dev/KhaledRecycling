# Expenses Module Documentation

This document describes the **Expenses** feature in the Admin area, including:

* `Expenses` (Index view)
* `_ExpensesListPartial` (AJAX-loaded list)
* `Print_Expenses` (printable report)

The module is part of **Financial Management** and is built using **ASP.NET Core MVC**, **Razor Views**, **AJAX**, and **server-side pagination**.

---

## 1️⃣ Expenses – Index View

### Purpose

The **Expenses** index page allows administrators to:

* View all expense records
* Filter expenses using advanced criteria
* Create new expenses
* Print or export expense reports

---

### Page Metadata & Permissions

```csharp
ViewData["Title"] = Resource2.Expenses2;
var addPermission = PermissionScanner.ValidatePermission("ExpenseAndReceipt", "Add");
```

**Permissions used:**

* `Add` → Show **Create New** button
* `Edit` → Enable edit action
* `Delete` → Enable delete action

---

### UI Structure

#### 🔹 Breadcrumb & Record Count

* Shows navigation hierarchy
* Displays total records using `@Model.Count`

#### 🔹 Action Buttons

* **Create New Expense** (permission-based)
* **Print** → Opens `Print_Expenses`
* **Excel Export** → Downloads Excel report

---

### Advanced Filter Form

The filter form supports:

| Filter          | Description             |
| --------------- | ----------------------- |
| Budget Item     | Expense budget category |
| Expense Section | Gate / section          |
| Expense Source  | Funding source          |
| Supplier        | Destination / vendor    |
| Date From / To  | Date range              |

✔ Uses **Select2** for large dropdowns
✔ Uses **sessionStorage** to persist filter state

---

### AJAX List Loading

```js
$.ajax({
  url: '/Admin/ExpenseAndReceipt/Expenses',
  type: 'GET',
  data: { filters... }
});
```

**Behavior:**

* Loads `_ExpensesListPartial`
* Updates record count dynamically
* Supports pagination & page size change

---

## 2️⃣ _ExpensesListPartial

### Purpose

Renders the **expenses table** with:

* Pagination
* Totals
* Action buttons

Loaded dynamically via AJAX.

---

### Page Size Control

Allows switching between:

* 50 / 100 / 150 records per page

---

### Table Columns

| Column      | Description     |
| ----------- | --------------- |
| Item No     | Expense serial  |
| Amount      | Spent amount    |
| VAT         | VAT value       |
| Total       | Amount + VAT    |
| Notes       | Expense notes   |
| Budget Item | Linked budget   |
| Source      | Expense source  |
| Date        | Expense date    |
| Actions     | Control buttons |

---

### Action Buttons Logic

| Action          | Condition                          |
| --------------- | ---------------------------------- |
| View Attachment | Enabled if file exists             |
| Receipt         | Requires `Receipt` permission      |
| Edit            | Requires `Edit` permission         |
| Delete          | Disabled if receipt already signed |

✔ Delete is blocked once a receiving receipt is signed

---

### Totals Row

If records exist, the footer displays:

* Total Amount
* Total VAT
* Total Amount with VAT

Calculated server-side and exposed via the first item.

---

### Pagination

* Uses `PaginatedList<T>`
* AJAX-based navigation
* Preserves filters and page size

---

## 3️⃣ Print_Expenses View

### Purpose

Provides a **print-ready expenses report**:

* No layout
* Optimized for printing
* Automatically prints and closes

---

### Key Characteristics

* `Layout = null`
* RTL / LTR styles based on language
* Bootstrap + custom print CSS

---

### Report Header

Displays official branding:

* UAE / Fujairah Government
* Fujairah Science Club logo
* Arabic & English headers

---

### Printed Table

Columns:

* Item Number
* Amount
* VAT
* Total
* Notes
* Budget Item
* Date

---

### Totals Row (UI Only)

```html
<tr class="last-tr">
  <td>Totals</td>
  <td>@ViewBag.totalAmount</td>
  <td>@ViewBag.totalVat</td>
  <td>@ViewBag.totalWithVat</td>
</tr>
```

✔ Highlighted for print using `print-color-adjust`

---

### Footer Information

Includes:

* Print time
* Print date
* Printed by (current user)

---

### Auto Print Logic

```js
window.addEventListener('load', () => {
  window.print();
});

window.onafterprint = () => window.close();
```

✔ Prevents freezing main page
✔ Ensures clean user experience

---

## ✅ Summary

This Expenses module provides:

* Secure, permission-based actions
* Advanced filtering with persistence
* AJAX-powered performance
* Accurate financial totals
* Professional printable reports

It is a **complete, production-ready financial reporting component** in the Admin system.
