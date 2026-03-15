# Employee Module Views Documentation

This document covers the **Employee module** views in ASP.NET Core MVC: **AddEdit**, **Index**, and **_ListPartial**. It includes purpose, ViewModel usage, layout, permissions, scripts, and special logic.

---

## 1. AddEdit View

**ViewModel:** `EmployeeVM`  
**Purpose:** Add or edit employee information, including basic, passport, ID, financial info, and photo upload.

### Key Features

- **Title & Breadcrumbs** dynamically change based on add/edit mode.
- **Photo Upload & Preview**
  - Shows current photo if available.
  - Allows change or add new photo.
  - Max size 3MB.
  - Opens modal preview for existing photo.
- **Form Sections**
  1. **Basic Data:** Name, EmployeeNo, JobTitle, Nationality, Phone, Email, Address.
  2. **Passport Info:** Number, Expiry Date.
  3. **ID Info:** National ID segmented input with auto-focus and merge on submit.
  4. **Financial Info:** Salary, Bank Name, Account Number.
  5. **Notes & Additional Photo Upload.**

### JavaScript Features

- **National ID input**
  - Segmented input: g1-g4.
  - Auto-forward on max length.
  - Backspace auto-focus to previous field.
  - Merges segments into hidden field on submit.
  - Prevents submit if incomplete.

- **Photo preview**
  - Shows selected image immediately.
  - Reverts if user removes file.
  - Hides user icon when image preview shown.

- **Validation**
  - Prevents non-numeric input on numeric fields.
  - Shows error messages via ASP.NET validation spans.

---

## 2. Index View

**ViewModel:** `EmployeeVM`  
**Purpose:** Display list of employees with search, filter, print, Excel export, and control actions.

### Key Features

- **Breadcrumbs**: Shows current page path.
- **Badges:** Highlight expired ID or IDs expiring within 30 days.
- **Records count** dynamically displayed.
- **Permissions**:
  - `Add`: Show "Create New" button.
  - `Edit`: Show edit icons.
  - `Delete`: Show delete button only if no related objects.
  - `Attachments`: Show attachment icon if permitted.

- **Filter Panel**
  - Search by name or phone.
  - Date range filter for NationalIdExpiryDate.
  - Clear filters button.
- **Print & Excel**
  - Print: Opens new window with current filters, max 3999 records.
  - Excel: Downloads filtered report.

### JavaScript Features

- Open Print and Excel URLs with filters.
- Delete modals:
  - Confirm delete.
  - Info modal for errors.
- Expiry highlighting: 
  - Red for expired.
  - Yellow for IDs expiring in 30 days.
- Attachments modal:
  - Dynamically loads employee files via AJAX.
  - Handles adding, previewing, and removing attachments with proper re-indexing.

---

## 3. _ListPartial View

**ViewModel:** `EmployeeVM`  
**Purpose:** Partial view for rendering employee table in Index view.

### Table Columns

1. Name (localized: Arabic/English)
2. Job Title
3. National ID Number
4. Salary
5. National ID Expiry Date (highlighted if expired or near expiry)
6. Phone Number
7. Control Tools (Details, Edit, Attachments, Delete)

### Features

- **Pagination & Page Size**
  - Page size selectable: 50, 100, 150.
- **Control Tools**
  - **Details:** Always available.
  - **Edit:** Shown if `editPermission`.
  - **Attachments:** Shown if `attachmentsPermission`.
  - **Delete:** Conditional based o
