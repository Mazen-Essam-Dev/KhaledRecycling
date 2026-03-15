# External Work Mission Module (ASP.NET Core MVC)

This module manages **external work missions** for employees, including creating, editing, listing, filtering, and deleting missions. It uses ASP.NET Core MVC with ViewModels, DTOs, and permission-based access.

---

## 1. Overview

The module has three main components:

1. **Add/Edit Page** (`AddEdit.cshtml`)
   - Purpose: Create a new mission or edit an existing one.
   - Uses `ExternalWorkMissionVM` as the main ViewModel.
   - Dynamically populates employee dropdowns and job titles.
   - Multilanguage support (Arabic/English) for employee names.
   - Handles financial details and candidate participation fields.
   - Locks saving if the mission is already approved (`SignatureIdApproved`).

2. **Index/List Page** (`Index.cshtml`)
   - Purpose: Display a paginated and filterable list of missions.
   - Features:
     - Search by employee name or keyword.
     - Filter by date range.
     - Display counts of total records.
     - Buttons for **Add**, **Edit**, **Delete**, **Print**, and **Export to Excel**.
     - Pagination and page-size selection.
     - Permissions control which actions are available to the user.

3. **Partial View for List** (`_ListPartial.cshtml`)
   - Used to render the mission table inside the Index page.
   - Supports AJAX updates for filtering, pagination, and page-size changes.
   - Handles dynamic display of mission types and employee info.
   - Includes tooltips for actions.

---

## 2. Key ViewModels and DTOs

- **`ExternalWorkMissionVM`**
  - Main ViewModel for creating/editing missions.
  - Key properties:
    - `Id` – Mission identifier.
    - `MissionDate` – Date of the mission.
    - `EmployeeId` – Selected employee.
    - `MissionLocation`, `MissionCountry` – Mission details.
    - `MissionWorkDescription` – Description of the work.
    - `PetroleumFees`, `FoodFees`, `MissionAllowance` – Financial fees.
    - `CandidateName1-4`, `CandidateAdj1-4` – Candidate participation details.
    - `SignatureIdApproved` – Locks editing if approved.
    - `Missions` – List of mission types (forum, conference, exhibition, etc.).

- **`EmployeesNameDTO`**
  - Provides employee info for dropdowns.
  - Properties:
    - `Id` – Employee ID.
    - `FullNameAr` – Arabic name.
    - `FullNameEn` – English name.
    - `JobTitle` – Employee job.

---

## 3. Permissions

Permissions are enforced via `PermissionScanner`:

- **AddPermission** – Can the user create a mission.
- **EditPermission** – Can the user edit a mission.
- **DeletePermission** – Can the user delete a mission.

Actions like editing or deleting are disabled if `SignatureIdApproved` is set.

---

## 4. Filtering & Pagination

- Filter options:
  - `SearchTerm` – Filter by employee name or keyword.
  - `DateFrom` / `DateTo` – Filter by mission date range.
- Pagination:
  - Page size selection (50, 100, 150 records per page).
  - AJAX updates to reload table without refreshing the whole page.
  - Stored in `sessionStorage` to preserve the last filter state when navigating.

---

## 5. Dynamic Features

1. **Employee Job Auto-Fill**
   - When selecting an employee, the job field is automatically populated via AJAX (`GetJob` action).

2. **Mission Types**
   - Checkbox selection for multiple types:
     - Forum, Conference, Exhibition, Competition, Club Equipment, Exchange/Delivery.
   - Pre-checked if editing an existing mission with selected types.

3. **Candidate Fields**
   - Displays up to 4 candidates.
   - Hidden if empty in edit mode.

4. **Financial Fields**
   - Petroleum fees, food fees, and mission allowance are numeric inputs.
   - Validation messages are displayed using `asp-validation-for`.

---

## 6. Printing & Export

- Print button opens a new popup window for the printable version.
- Export to Excel button triggers file download via a dedicated action.
- Both respect current filters applied on the list.

---

## 7. JavaScript Interactions

- Handles:
  - AJAX-based filtering and pagination.
  - Employee selection to fetch job title.
  - Session storage to save last filter state.
  - Tooltips for action buttons.
  - Conditional display of candidates and mission types.

---

## 8. Summary

This module efficiently manages external missions by combining:

- Strongly typed **ViewModels**.
- **Permissions** for secure operations.
- **AJAX partials** for smooth UX.
- **Multilanguage support** for employee names.
- **Dynamic data interactions** like auto-fill and pre-checked checkboxes.

It is fully integrated into ASP.NET Core MVC patterns and ensures maintainable, scalable code for HR-related workflows.
