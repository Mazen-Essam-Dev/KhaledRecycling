# MembersCourse Module Documentation

This document explains the **MembersCourse** page and partial view in the FougeraClub ASP.NET Core MVC admin area. It includes key ViewModel usage, pagination, filtering, attendance handling, and printing/exporting logic.

---

## 1. Page Overview

**ViewModel:** `MembersCourseVM`  
**Purpose:** Display a list of course participants with their details, attendance status, and support for pagination, filtering, printing, and Excel export.

**Key Features:**
- Shows course details: title, department, trainer, start/end dates.
- Lists members subscribed to the course with optional attendance checkboxes.
- Pagination and page size selection.
- AJAX-based filtering and list updates.
- Printing and exporting participant lists to Excel.
- Localization support for Arabic and English based on session language.

---

## 2. Course Details Section

Displays main course information using ViewModel properties:

- `Course.TitleAr` / `Course.TitleEn` → Course title
- `Course.Department.NameAr` / `Course.Department.NameEn` → Department name
- `Trainer.FullNameAr` / `Trainer.FullNameEn` → Trainer name
- `Course.StartDate` / `Course.EndDate` → Course duration

Each field dynamically switches between Arabic and English based on the session language. Visual sections are displayed as cards (`icon_box_all`) for clarity.

---

## 3. Records Count and Actions

- Displays total participant count (`Members_Paginated.Count`).
- Print button:
  - If record count > 3999 → Shows toaster warning “Cannot print more than 3999 records”.
  - Otherwise → Opens print view in a new tab.
- Excel export button → Downloads participants as an Excel file.
- Uses `PermissionScanner.ValidatePermission("Course", "Attendance")` to determine if the attendance column should be visible.

---

## 4. Participant Table (_ListPartialMemberSubsInCourse)

**Key Features:**
- Columns: Subscriber Name, Nationality ID, Mobile, Email, Attendance (optional)
- Attendance column only visible if user has permission.
- Supports Arabic/English display for names and nationality.
- Attendance checkboxes:
  - Triggers AJAX POST on change to update attendance.
  - Uses `data-member-id` and `data-course-id` to identify the participant and course.
- Handles empty dataset with a user-friendly message.

**Pagination:**
- Uses `Members_Paginated` ViewModel.
- Supports Previous/Next buttons.
- Highlights current page.
- Maintains state across AJAX reloads.

**Page Size Selection:**
- Options: 50, 100, 150
- Updates the partial view when changed.

---

## 5. AJAX Filtering & Partial Updates

- Filters and pagination use sessionStorage to store the last state.
- `loadNewPartialList(page, pageSizeOverride)` fetches the updated partial view without full page reload.
- Updates:
  - `#PartialNewListContainer` → new HTML
  - `#countRecords` → total record count
  - Tooltips are re-initialized after AJAX load.

**Filter Save Logic:**
```text
filters = {
    courseId,
    page,
    pageSize
}
