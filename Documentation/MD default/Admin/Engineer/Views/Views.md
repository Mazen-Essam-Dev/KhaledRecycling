# Engineer Module Views Documentation

This document covers the **Engineer module** views in ASP.NET Core MVC: **AddEdit**, **Index**, and **_ListPartial**. It includes purpose, ViewModel usage, layout, permissions, scripts, and special logic.

---

## 1. AddEdit View

**ViewModel:** `EngineerVM`  
**Purpose:** Add or edit engineer information, including basic, passport, ID, financial info, and photo upload.

### Key Features

- **Title & Breadcrumbs** dynamically change based on add/edit mode.
- **Photo Upload & Preview**
  - Shows current photo if available.
  - Allows change or add new photo.
  - Max size 3MB.
  - Opens modal preview for existing photo.
- **Form Sections**
  1. **Basic Data:** Full Name, Nationality, Specialization, Graduation Year, Date of Birth, Email, Position, Phone, Mobile, Work Address, Home Address.
  2. **Passport Info:** Passport Number, Passport Expiry Date.
  3. **ID Info:** National ID segmented input (g1-g4) with auto-focus and merge on submit.
  4. **Financial Info:** Salary, Bank Name, Bank Account Number.
  5. **Notes & Additional Photo Upload.**

### JavaScript Features

- **National ID Input**
  - Segmented inputs: g1-g4.
  - Auto-forward to next segment when max length reached.
  - Backspace auto-focus to previous segment.
  - Merge segments into hidden field on form submit.
  - Prevent form submission if incomplete.
- **Photo Preview**
  - Shows selected image immediately.
  - Hides user icon when preview shown.
  - Clears preview if file removed.
- **Validation**
  - Numeric-only validation for segmented ID inputs.
  - ASP.NET validation messages displayed inline.

---

## 2. Index View

**ViewModel:** `PaginatedList<EngineerVM>`  
**Purpose:** Display list of engineers with search, filters, print, Excel export, and action buttons.

### Key Features

- **Breadcrumbs:** Shows current page path.
- **Records Count:** Dynamically displayed.
- **Permissions**
  - `Add`: Show "Create New" button.
  - `Edit`: Show edit icons.
  - `Delete`: Show delete button only if allowed.
- **Filter Panel**
  - Search by name, phone, or specialization.
  - Filter by Nationality and Graduation Year.
  - Clear filters button.
- **Print & Excel**
  - Print: Opens new window with current filters, max 3999 records.
  - Excel: Downloads filtered report.

### JavaScript Features

- AJAX-based filters save last state in sessionStorage.
- Dynamic loading of partial list on search, filter, pagination, or page size change.
- Handles Print and Excel buttons with current filters.
- Clear filter button resets inputs and reloads list.
- Pagination links load data via AJAX without full page reload.

---

## 3. _ListPartial View

**ViewModel:** `PaginatedList<EngineerVM>`  
**Purpose:** Partial view for rendering engineer table inside Index view.

### Table Columns

1. Full Name (localized Arabic/English)
2. Specialization
3. National ID Number
4. Graduation Year
5. Date of Birth
6. Phone / Mobile
7. Control Tools (Details, Edit, Attachments, Delete)

### Features

- **Pagination & Page Size**
  - Page size selectable: 50, 100, 150.
- **Control Tools**
  - **Details:** Always available.
  - **Edit:** Shown if `editPermission`.
  - **Attachments:** Shown if `attachmentsPermission`.
  - **Delete:** Conditional based on `deletePermission`.
- **Expiry Highlighting**
  - Highlight near graduation or certificate expiry if applicable.
- **AJAX Handling**
  - Supports search, filter, pagination, and dynamic updates without page reload.

---

## Notes

- All views use **multilanguage support** where applicable.  
- Form validation relies on both **ASP.NET server-side validation** and **client-side JS** for instant feedback.  
- Permission checks determine visibility of buttons and actions in Index and partial views.  
- Image and file uploads are managed with **preview**, **remove**, and **modal viewing** features.  
- Dynamic filtering and pagination are **AJAX-enabled** for better user experience.
