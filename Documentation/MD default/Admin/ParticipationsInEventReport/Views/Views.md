# ParticipationsInEventReport Module Documentation

This document explains the structure, functionality, and flow of the `ParticipationsInEventReport` feature in the Admin area.

---

## AddEdit Page

### Purpose
- Used for creating or editing a participation report in an event.
- Displays a form with multiple fields and allows adding multiple activity entries dynamically.
- Handles file uploads (images) for the report.

### Key ViewModel
`ParticipationsInEventReportVM` containing:
- `Id` (int)
- `Date` (DateTime)
- `AdministrativeDepartment` (string)
- `ReportTitle` (string)
- `Details` (List of activity details)
- `Image1Path`, `Image2Path`, `Image3Path`, `Image4Path` (for uploaded images)

### Page Logic
1. **Title Logic**
   - If `Model.Id > 0` → editing mode, else → adding mode.
   - Sets multiple `ViewData` entries for breadcrumb and page titles.

2. **Form Fields**
   - `Date`, `AdministrativeDepartment`, `ReportTitle`
   - Input validation using `asp-validation-for`.

3. **Activities Section**
   - Allows dynamic addition of rows for activities:
     - `ActivityName`
     - `ActivityAction`
     - `ActivityReason`
   - Uses JavaScript to validate inputs and add rows dynamically.
   - Supports deletion with confirmation modal.
   - Re-indexes rows after deletion to maintain correct model binding.

4. **Images Section**
   - Supports 4 image uploads with preview functionality.
   - Each image has:
     - File input
     - Max size validation
     - Modal preview if editing

5. **Actions**
   - `Save` button submits the form.
   - `Back` button redirects to index.

6. **JavaScript**
   - Handles dynamic row addition and deletion.
   - Validates inputs before adding rows.
   - Uses Bootstrap modal for delete confirmation.

---

## Index Page

### Purpose
- Displays a paginated list of participation reports.
- Supports searching, filtering by date, printing, and exporting to Excel.

### Key Features
1. **Breadcrumb & Title**
   - Uses `ViewData` for breadcrumbs.
   - Displays package name and single-page title.

2. **Permissions**
   - Add, Edit, Delete permissions are checked using `PermissionScanner`.

3. **Filter Panel**
   - Search by text (`searchTerm`)
   - Filter by date range (`dateFrom`, `dateTo`)
   - Persist filters in `sessionStorage` to maintain state across page loads.

4. **Actions**
   - Create new report (if permission allows)
   - Print report
   - Export as Excel
   - Clear filters

5. **AJAX-based Pagination and Filters**
   - Uses jQuery to load partial lists dynamically.
   - Updates record count after AJAX calls.
   - Maintains last state using `sessionStorage`.

6. **Modals**
   - Delete confirmation modal
   - Info modal for error messages

---

## _ListPartial

### Purpose
- Partial view used by Index for displaying the table of participation reports.
- Handles pagination, permissions, and actions for each row.

### Key Features
1. **Table Structure**
   - Columns:
     - Participating Title
     - Department Manage
     - Date
     - Actions
   - Actions:
     - View Details
     - Edit (if permissions and signatures allow)
     - Delete (if permissions and signatures allow)

2. **Permissions**
   - Edit and delete buttons are shown based on permissions and status of signatures.

3. **Pagination**
   - Displays page numbers with `Previous` and `Next` links.
   - Uses AJAX for page navigation without full page reload.

4. **No Data Handling**
   - Shows a friendly "No Data" message with instructions to create a new record if none exist.

---

## JavaScript Overview

### AddEdit Page
- `addRow()` → Adds a new activity row to the table.
- `removeRow(button)` → Marks a row for deletion and opens confirmation modal.
- `confirmDeleteRow()` → Removes the row from the DOM and re-indexes remaining rows.

### Index Page
- Filters and pagination are handled via AJAX.
- `saveFilters(page, pageSize)` → Stores current filter and pagination state in `sessionStorage`.
- `loadNewPartialList(page, pageSize)` → Loads the updated list via AJAX.
- Tooltips are initialized dynamically after AJAX calls.

---

## Notes
- All resources (`Resource1`, `Resource2`, `Resource3`) are used for localization.
- `PaginatedList<T>` is used to handle pagination for Index and `_ListPartial`.
- File uploads and images are bound to the ViewModel properties and handled with preview modals.
- Delete actions use confirmation modals to prevent accidental deletions.

---

## Summary

- **AddEdit Page**: Form with dynamic activities and image uploads, supports create/edit operations.
- **Index Page**: List view with search, filters, pagination, print/export options.
- **_ListPartial**: Reusable table view with actions, permissions, and pagination.
- **JS Functions**: Handle dynamic UI behaviors (add/remove rows, modals, AJAX).

This modular structure allows maintainable and scalable management of participations in events within the Admin area.
