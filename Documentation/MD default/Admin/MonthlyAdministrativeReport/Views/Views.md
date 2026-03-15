# Monthly Administrative Report Module Documentation

## Overview
This module manages **monthly administrative reports** in the FougeraClub admin area. It allows users to **create, edit, view, print, and delete reports**, with full **validation, dynamic activities, image uploads, and filtering**.

The module uses **ASP.NET Core MVC**, **ViewModels**, **partial views**, **AJAX**, and **Bootstrap modals**.

---

## Add/Edit Page (`AddEdit.cshtml`)

### Purpose
- Create a new report or edit an existing one.
- Add multiple activities dynamically.
- Upload participation images.
- Validate form data, including report dates and activity dates.

### Key Features

1. **Dynamic Titles**
   - Uses `ViewData["Title"]`, `ViewData["Title pakage"]`, and `ViewData["Title Single"]` to dynamically set page headings.

2. **Breadcrumb Navigation**
   - Shows the module and current page for user orientation.

3. **Form Structure**
   - **Hidden Field** for report `Id`.
   - **Date Field** for the report date.
   - **Type Field** using radio buttons (`TypeEnumList`).
   - **Administrative Department** text input.
   - **Report Title** text input.
   - **Activities Section**
     - Add multiple activities with name, number of participants, start/end dates, and reason.
     - Dynamic table for activity entries.
     - Validation for empty fields, dates within the same month/year as the report, and end date after start date.

4. **Images Upload**
   - Up to 4 images per report.
   - Shows preview in Bootstrap modals if editing.
   - Validates file type and size (max 3MB).

5. **Form Actions**
   - **Save Button** submits form.
   - **Back Button** navigates to the index page.

6. **JavaScript**
   - Validates report date against type via AJAX.
   - Adds and removes activity rows dynamically.
   - Reindexes rows after deletion.
   - Handles delete confirmation modal.

---

## Index Page (`Index.cshtml`)

### Purpose
- Display a **paginated list** of monthly administrative reports.
- Filter by **year** and **month**.
- Provide **Print** and **Excel Export** functionality.
- Manage permissions for **Add/Edit/Delete** actions.

### Key Features

1. **Breadcrumbs and Titles**
   - Shows the module name and page name.

2. **Records Count**
   - Displays total number of reports.

3. **Action Buttons**
   - **Create New** (if `addPermission` is true)
   - **Print** – opens printable page in a popup.
   - **Export to Excel** – downloads Excel file for filtered data.

4. **Filter Panel**
   - **Year Select** – list of years from `ViewBag.Years`.
   - **Month Select** – list of months from `ViewBag.Months`.
   - **Search Button** – filters data using AJAX.
   - **Clear Button** – resets filters.

5. **Partial View for Report List**
   - `_ListPartial` loads the table dynamically.
   - Supports AJAX reloading for filtering, pagination, and page size changes.

6. **JavaScript**
   - Stores last filter, page, and page size in `sessionStorage`.
   - Reloads partial view when filters, page, or page size change.
   - Updates record count dynamically.
   - Initializes tooltips after AJAX calls.

---

## List Partial View (`_ListPartial.cshtml`)

### Purpose
- Render the **table of reports** for the index page.

### Structure

1. **Hidden Field**
   - `NewCountRecords` to track total items.

2. **Page Size Selector**
   - Dropdown for 50, 100, or 150 rows per page.

3. **Table Columns**
   - Year, Month, Type, and Actions.

4. **Actions Column**
   - **View Details** – opens details page.
   - **Print** – prints individual report.
   - **Edit** – enabled if permission allows and report not signed by trainer.
   - **Delete** – enabled if permission allows and report not signed by trainer.

5. **Empty State**
   - Displays a message when no reports exist.

---

## Flow Summary

1. User opens **Index Page**:
   - Filters reports by year and month.
   - Table loads via `_ListPartial`.
   - Can change page size and paginate using AJAX.

2. **Add/Edit Page**:
   - User creates or edits a report.
   - Adds multiple activities dynamically.
   - Uploads images.
   - Form validates required fields and date consistency.

3. **JavaScript**:
   - Handles dynamic activity addition and deletion.
   - Validates dates via AJAX.
   - Maintains last filters using session storage.

4. **Printing & Export**:
   - Prints full report list or individual report details.
   - Downloads filtered data as Excel.

---

## Notes

- **Permissions** control which buttons/actions are visible.
- **Validation** uses ASP.NET MVC validation and client-side checks.
- **AJAX** improves responsiveness when filtering and paginating.
- **Bootstrap modals** used for delete confirmation and image previews.
- **Localization** supports multiple languages via `Resource1`, `Resource2`, `Resource3`.

---

This Markdown file provides a **full, single-page explanation** of your module with all key features, flows, and logic, without requiring any HTML/CSS copy.
