# Subscribed Members In Courses Module Documentation

This document explains the **Subscribed Members In Courses** page in the FougeraClub ASP.NET Core MVC admin area. It includes key ViewModel usage, pagination, filtering, attendance handling, and printing/exporting logic.

---

## 1. Page Overview

**ViewModel:** `SubscribedMemberCourseVMTrainer`  
**Purpose:** Display a list of course participants with their details, subscription status, and support for pagination, filtering, printing, and Excel export.

**Key Features:**
- Shows course details: title, department, trainer, start/end dates.
- Lists members subscribed to the course with optional attendance/approval checkboxes.
- Pagination and page size selection.
- AJAX-based filtering and list updates.
- Printing and exporting participant lists to Excel.
- Localization support for Arabic and English based on session language.

---

## 2. Page Structure

**Breadcrumb & Record Count:**
- Shows hierarchy navigation and current section.
- Displays total participant count (`Model.paginated.Count`).
- Color-coded badges for subscription status: Approval, In Progress, Expired, Rejection.

**Card Layout:**
- Header includes page title and action buttons (Print, Excel Export).
- Main content wrapped in rounded/shadowed card.
- Filter section allows text search, date range, and nationality selection.

---

## 3. Filters & Search

**Search:**
- Text input for name, ID number, or other identifiers.
- Placeholder dynamically switches based on localization.

**Date Filters:**
- `From` / `To` subscription dates.
- Uses calendar input with icon.

**Nationality Filter:**
- Dropdown populated from `ViewBag.Nationalities`.
- Retains selected value after submission.

**Filter Form:**
- Submits via GET method.
- Can be tied to AJAX reload to dynamically update the participant list.

---

## 4. Participant Table

**Columns:**
- Subscriber Name
- Nationality ID
- Mobile
- Email
- Attendance / Approval (optional based on permission)

**Permissions:**
- `PermissionScanner.ValidatePermission("Course", "DeleteSubscription")` → Delete
- `PermissionScanner.ValidatePermission("Course", "Accept")` → Approve / Reject
- `PermissionScanner.ValidatePermission("Course", "Rate")` → Rating
- `PermissionScanner.ValidatePermission("Member", "Edit")` → Edit member data

**Behavior:**
- Attendance/approval checkboxes trigger AJAX updates using `data-member-id` and `data-course-id`.
- Handles empty datasets with a user-friendly message.
- Supports Arabic/English display dynamically.

---

## 5. Pagination & Page Size

- `Model.paginated` provides data for current page.
- Previous/Next buttons and page number highlighting.
- Page size options: 50, 100, 150.
- Updates maintain filter and state across reloads.

---

## 6. AJAX Filtering & Updates

**AJAX Functionality:**
- Partial view container: `#PartialNewListContainer`
- Updates `#countRecords` on each reload.
- Re-initializes tooltips after load.

**Example Filter Object:**
```text
filters = {
    courseId,
    page,
    pageSize
}
