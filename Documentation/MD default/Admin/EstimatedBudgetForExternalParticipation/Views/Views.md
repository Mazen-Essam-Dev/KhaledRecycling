# Estimated Budget for External Participation Module Views Documentation

This document covers the **Estimated Budget for External Participation module** views in ASP.NET Core MVC: **AddEdit**, **Index**, and **_ListPartial**. It includes purpose, ViewModel usage, layout, permissions, scripts, and special logic.

---

## 1. AddEdit View

**ViewModel:** `EstimatedBudgetVM`  
**Purpose:** Add or edit estimated budget records for external participation, capturing basic details, participation info, and financial fees.

### Key Features

- **Title & Breadcrumbs** dynamically adjust depending on whether the user is adding a new record or editing an existing one.
- **Form Sections**
  1. **Basic Data:** Participation title, regulator, country, participation date, creation date.
  2. **Participation Details:** Participation type, required participants, number of managers, technical supervisors, activity monitors, and team members.
  3. **Financial Fees:** Fees for administrators, heads of delegation, entire team, subsidies, or special allowances.
  4. **Notes:** Optional remarks or instructions.

### JavaScript Features

- **Dynamic Form Behavior**
  - Sections may show/hide based on participation type or role selection.
- **Validation**
  - Required fields enforced client-side for instant feedback.
  - Numeric validation for fees and participant counts.
- **Date Pickers**
  - Ensures correct date format and prevents invalid dates.

---

## 2. Index View

**ViewModel:** `PaginatedList<EstimatedBudgetVM>`  
**Purpose:** Display a list of all estimated budget records with search, filtering, print, and export options.

### Key Features

- **Breadcrumbs:** Shows current page path.
- **Records Count:** Displays number of filtered records dynamically.
- **Permissions**
  - `Add`: Show "Create New" button.
  - `Edit`: Show edit icons per row.
  - `Delete`: Show delete button only if allowed.
- **Filter Panel**
  - Search by participation title, country, regulator, or participation type.
  - Filter by participation date or budget range.
  - Clear filters button.
- **Print & Excel**
  - Print: Opens a printable report of current filtered records, with maximum record limit applied.
  - Excel: Downloads a filtered report as an Excel file.

### JavaScript Features

- AJAX-based filters save last state in sessionStorage.
- Dynamic loading of partial list on search, filter, pagination, or page size change.
- Handles Print and Excel buttons with current filters.
- Clear filter button resets inputs and reloads the list.
- Pagination links load data via AJAX without full page reload.

---

## 3. _ListPartial View

**ViewModel:** `PaginatedList<EstimatedBudgetVM>`  
**Purpose:** Partial view for rendering the estimated budget table inside the Index view.

### Table Columns

1. Participation Title (localized)
2. Regulator / Organization
3. Country
4. Participation Type
5. Number of Participants
6. Fees / Financial Details
7. Control Tools (Details, Edit, Attachments, Delete)

### Features

- **Pagination & Page Size**
  - Page size selectable: 50, 100, 150.
- **Control Tools**
  - **Details:** Always available.
  - **Edit:** Shown if `editPermission`.
  - **Attachments:** Shown if `attachmentsPermission`.
  - **Delete:** Conditional based on `deletePermission`.
- **Highlighting**
  - Highlight entries with upcoming deadlines or unusual budgets if applicable.
- **AJAX Handling**
  - Supports search, filter, pagination, and dynamic updates without page reload.

---

## Notes

- All views support **multilanguage** where applicable.  
- Form validation uses both **ASP.NET server-side validation** and **client-side JavaScript** for instant feedback.  
- Permission checks determine visibility of buttons and actions in Index and partial views.  
- Financial data inputs support **dynamic calculations** and numeric validation.  
- Dynamic filtering and pagination are **AJAX-enabled** for smoother user experience.
