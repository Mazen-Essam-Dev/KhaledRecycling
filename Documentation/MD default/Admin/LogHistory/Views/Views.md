# Logs Module - ASP.NET Core MVC

This module manages **log history** in the system, allowing filtering, pagination, printing, and exporting logs for users and actions performed.

---

## 1. Index Page (`Index.cshtml`)

### Purpose
- Main page for displaying **paginated logs**.
- Supports **filtering**, **searching**, **printing**, and **Excel export**.
- Uses `PaginatedList<LogsVM>` as the model.

### Key Features
1. **Breadcrumb and Page Title**
   - Displays the current page and hierarchy.
   - Dynamic titles from resources for multi-language support.

2. **Records Count Display**
   - Shows total number of records.
   - Updates dynamically after filtering.

3. **Actions**
   - **Print**: Opens a popup window for printing filtered logs.
   - **Export to Excel**: Downloads filtered logs as Excel.
   - **Timeline**: Redirects to a timeline view.

4. **Filtering Form**
   - Search by process name or username.
   - Filter by date range (`DateFrom` and `DateTo`).
   - Clear button to reset filters.
   - Filters are saved in **session storage** to persist user selection across page reloads.

5. **AJAX Partial Loading**
   - Partial view `_ListPartial` is loaded via AJAX.
   - Supports:
     - Pagination
     - Page size changes
     - Dynamic filtering
   - Provides smooth UX without full page reload.

6. **Scripts**
   - `confirmDeleteSelected` and `confirmDelete_All_Selected` for delete confirmation using SweetAlert.
   - AJAX scripts to load filtered data, handle pagination, and maintain last filter state.

---

## 2. Partial View (`_ListPartial.cshtml`)

### Purpose
- Renders the **logs table** inside the Index page.
- Supports **pagination**, **dynamic page size**, and displays logs with user info, date, time, and action description.

### Key Features
1. **Page Size Selector**
   - Dropdown to choose number of rows per page (50, 100, 150).
   - Updates the view dynamically via AJAX.

2. **Logs Table**
   - Columns:
     - Username
     - Date
     - Time
     - Description (Action performed and Controller)
   - Displays username dynamically:
     - If missing, attempts to fetch from member list.
     - Shows "Not Found" if username not available.
   - Descriptions combine **action performed**, **target entity**, and **controller** names from resource files for localization.

3. **Pagination**
   - Shows previous/next buttons.
   - Dynamically adjusts visible page numbers.
   - Uses AJAX to load selected page without refreshing.

4. **Empty State**
   - If no logs are available, displays a friendly "No Data" message with icon.

---

## 3. Print Log History (`PrintLogHistory.cshtml`)

### Purpose
- Provides a **printable version** of logs.
- Independent layout (no master layout) to optimize printing.

### Key Features
1. **Header**
   - Official club header in Arabic and English.
   - Includes logo centered.
   - Current date and time display.

2. **Logs Table**
   - Columns: Username, Date, Time, Description.
   - Follows same logic as `_ListPartial` for username and description resolution.
   - Supports multiple languages using resource files.
   - Displays action target and controller name.

3. **Footer**
   - Shows printed by user email and current timestamp.

4. **Print Automation**
   - Hides loading overlay once data is ready.
   - Automatically triggers browser print dialog.
   - Closes popup window after printing.

---

## 4. ViewModel (`LogsVM`)

- Represents each log entry.
- Properties:
  - `Id` – Unique identifier
  - `UserId` – User who performed the action
  - `UserFullName` – Display name
  - `RequestTime` – Date and time of action
  - `Controller` – Controller name
  - `Action` – Action performed
  - `LogTarget` – Entity affected by action

---

## 5. Features Summary

- **Filtering & Search**
  - By process name or username
  - By date range

- **Pagination**
  - Configurable page size
  - AJAX-based updates

- **Exporting**
  - Excel download
  - Print-ready version

- **Localization**
  - Multi-language support for usernames, actions, controllers, and UI text.

- **User Experience**
  - Loading indicators during AJAX requests
  - Session storage remembers filter state
  - Friendly empty-state display

- **Security**
  - Only authorized users can access logs
  - Confirmations for delete actions

---

## 6. Interactivity and Scripts

- Uses **jQuery and AJAX** for:
  - Partial list updates
  - Pagination clicks
  - Page size changes
  - Filter form submission
- Uses **SweetAlert** for confirmation dialogs.
- Session storage ensures user filters persist across page reloads.

---

**Overall**, the Logs module provides a fully interactive, localized, and user-friendly way to manage, view, and print system logs, integrated with ASP.NET Core MVC patterns and best practices.
