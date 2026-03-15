# Member Index & ListPartial Documentation

This document explains the **Member module Index page** and its `_ListPartial` in the Admin area of the FougeraClub ASP.NET Core MVC application.

---

## 1. Purpose

The **Member Index page** allows administrators to:

- View a list of all members with paging and filters
- Search members by code, name, ID, phone number
- Filter members by registration date, nationality, and gender
- Perform actions such as Add/Edit, Suspend/Activate, Delete, View Courses
- Export data to Excel or print member list

The `_ListPartial` is used for **AJAX updates** of the table content without reloading the entire page.

---

## 2. Page Structure (Index.cshtml)

### 2.1 Header and Breadcrumbs

- Breadcrumbs show:
  - Module: ActivitiesManagement
  - Page: Members
- Records count badges:
  - `IdExpired` – members with expired IDs
  - `ExpireIn30Days` – members whose IDs expire in 30 days
- Create New button:
  - Visible only if the user has `Create` permission
  - Opens the `AddEdit` page for new member

### 2.2 Card Header

- Displays page title with icon
- Action buttons:
  - **Print**: Opens a popup for printing the filtered member list
  - **Export to Excel**: Downloads Excel report with current filters

### 2.3 Search & Filter Panel

- **Search Term**: Search by Code, Full Name, ID Number, Phone Number
- **Date Filters**: Registration Date From / To
- **Nationality Filter**: Dropdown populated dynamically
- **Gender Filter**: Radio buttons (Male, Female, All)
- **Clear Filters Button**: Resets all filters
- **Search Button**: Reloads table with filtered results

### 2.4 Scripts for Filters

- `sessionStorage` remembers last filters and page state
- `loadNewPartialList` AJAX function reloads `_ListPartial` with filters and paging
- `saveFilters` stores filter values in session storage
- Expiry date badges are applied dynamically:
  - **Expired**: red highlight
  - **Expiring in 30 days**: yellow highlight
- Pop-up date picker handled by `input.showPicker()` for better UX

---

## 3. Table Display (_ListPartial.cshtml)

### 3.1 Table Structure

- Columns:
  1. Code
  2. Full Name
  3. ID Number
  4. ID Expiry Date
  5. Phone Number
  6. Nationality
  7. Age
  8. Registration Date
  9. Actions

### 3.2 Table Rows

- Each member row displays data from `MemberVM`
- Full Name clickable to open Add/Edit page
- Expiry dates styled with **badges**:
  - `badge-expired` for past dates
  - `badge-warning` for upcoming expirations (30 days)
- Permissions applied per action button:
  - **Edit**: only if `editPermission` is true
  - **Suspend/Activate**: only if `suspendPermission` is true
  - **Delete**: only if `deletePermission` is true and member has no courses
- Actions use tooltips for clarity

### 3.3 Suspend/Activate Modals

- Two modals:
  - `suspendModal` for suspended members
  - `notSuspendModal` for active members
- AJAX POST toggles `Suspended` status
- Modal shows confirmation, then reloads table after dismissal

### 3.4 Pagination

- Bottom pagination uses AJAX to load new pages without full reload
- Supports dynamic page size selection (50, 100, 150 rows)
- Page navigation buttons (`Previous`, numbered pages, `Next`) respect current filters

---

## 4. AJAX Functionality

- Handles:
  - Paging
  - Page size change
  - Search and filter submissions
- Reloads only the `_ListPartial` container
- Maintains consistent UI state and tooltips
- Displays loading indicator while fetching new data
- Handles errors gracefully with alerts

---

## 5. Summary of Key Features

- **Dynamic filtering** (search, gender, nationality, registration date)
- **AJAX-powered table** with partial reloads
- **Permission-based actions** (Edit, Suspend, Delete)
- **Visual indicators** for expired and soon-to-expire IDs
- **Session storage** maintains filters and page state
- **Print & Excel export** with current filters applied
- **Responsive design** for mobile and desktop layouts
- **Tooltips** and modals enhance usability

---

**End of Member Index & ListPartial Documentation**
