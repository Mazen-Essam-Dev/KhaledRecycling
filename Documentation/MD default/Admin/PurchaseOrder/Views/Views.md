# Purchase Order Module Documentation

This file documents the `PurchaseOrder` module in your ASP.NET Core 9 MVC application. It covers the main views: `AddEdit`, `Index`, `_ListPartial`, and `Details`. The focus is on functionality, ViewModel usage, permissions, and interactions.

---

## 1. AddEdit.cshtml

**Purpose:** Create or edit a purchase order.

**Key Features:**
- Conditional title and breadcrumb based on `Model.Id`.
- Checks if a manager signature exists (`boolManagerSignature`) to disable save if needed.
- Inputs for:
  - Purchase Order Code (read-only)
  - Date
  - Supplier (multilingual display)
  - VAT checkbox
- Dynamic item table:
  - Add multiple items with quantity, price, and name.
  - Client-side validation (integer/decimal only).
  - Delete confirmation modal.
- JavaScript functions:
  - `addRow()`: Adds a new item row.
  - `removeRow()`: Triggers delete modal.
  - `confirmDeleteRow()`: Deletes row and reindexes inputs.
- Scripts for validation, input restrictions, and dynamic table manipulation.

**Special Logic:**
- VAT calculation based on `HasVAT` and `VATValue`.
- Row index maintained dynamically to match model binding.
- Conditional disabling of "Save" button if signature exists.

---

## 2. Index.cshtml

**Purpose:** Display a list of all purchase orders with search, filter, and export options.

**Key Features:**
- Breadcrumb navigation.
- Records count display.
- Action buttons:
  - Create new order (permission-based)
  - Print page
  - Export to Excel
- Search panel with filters:
  - Date range (`dateFrom`, `dateTo`)
  - Supplier dropdown
- Partial AJAX refresh for filtered results.
- Pagination with session storage to retain filter and page state.
- JavaScript functions:
  - `saveFilters()`: Store current filters in `sessionStorage`.
  - `loadNewPartialList()`: Refresh `_ListPartial` via AJAX.

**Permissions:**
- Add, Edit, Delete determined via `PermissionScanner`.

---

## 3. _ListPartial.cshtml

**Purpose:** Render the filtered list of purchase orders in a table format (used by `Index.cshtml`).

**Key Features:**
- Page size selection (`50, 100, 150`) with persistence.
- Table columns:
  - Purchase Order Code
  - Supplier Name (multilingual)
  - Date (formatted based on language)
  - Actions: Details, Edit, Print, Delete (permission and signature-based)
- Empty state with icon and message if no records.

**Special Logic:**
- Disable actions if manager signature exists.
- Tooltips for actions.
- Form submission for delete with confirmation.

---

## 4. Details.cshtml

**Purpose:** View detailed information for a single purchase order.

**Key Features:**
- Breadcrumb navigation.
- Display order details:
  - Purchase Order Code
  - Date
  - Supplier
- Items table:
  - Quantity, Single Price, Item Name, Total per item
  - Total without VAT
  - VAT (if applicable)
  - Total with VAT in AED
  - Tafqeet (number-to-words) in Arabic and English
- Signature section:
  - Display uploaded signature if exists
  - Allow manager to sign with OTP modal
- Modal for OTP input:
  - 4-digit numeric input
  - Validation and confirmation buttons

**Special Logic:**
- VAT calculation.
- Conditional rendering of signature section based on role and permissions.

---

## 5. General Notes

- **ViewModels:** All views use `PurchaseOrderVM`. It includes properties for purchase order details, items list, suppliers, VAT, and signature.
- **Localization:** Language-specific rendering via `SessionHelper.GetCurrentLanguage()`.
- **Client-side validation:** Integer/decimal restrictions, required field checks, dynamic error messages.
- **Permissions:** Managed via `PermissionScanner.ValidatePermission()` and role-based logic.
- **AJAX:** Index view uses AJAX to refresh `_ListPartial` based on filter and page changes.

---

### Summary

This module handles full CRUD operations for purchase orders:

1. `AddEdit` – create or update an order.
2. `Index` – list, search, filter, print, and export.
3. `_ListPartial` – dynamic table rendering for AJAX.
4. `Details` – view order details with signature verification.

All actions are governed by user permissions and signature logic to ensure secure operations.
