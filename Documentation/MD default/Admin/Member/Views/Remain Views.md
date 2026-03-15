# Member Courses Module Documentation

## Overview
This module manages **courses associated with a member** in the FougeraClub application. It includes listing courses, viewing details, filtering, pagination, printing, and exporting to Excel. The implementation uses **ASP.NET Core MVC**, **ViewModels**, **AJAX**, and **partial views**.

---

## MemberCourses.cshtml

### Purpose
- Display all courses linked to a member.
- Provide filtering, pagination, print, and export functionality.
- Show detailed course information in a modal.

### Structure
1. **ViewData**
   - `ViewData["Title"]`, `ViewData["Title pakage"]`, `ViewData["Title Single"]` dynamically set page titles.
2. **Breadcrumb & Record Count**
   - Navigation breadcrumbs for context.
   - Shows the total number of courses for the member.
3. **Hidden Fields**
   - `memberId` for AJAX requests and page state persistence.
4. **Card Layout**
   - Header shows member’s name.
   - Conditional printing if records > 3999.
   - Export to Excel button.
5. **Partial View**
   - `@await Html.PartialAsync("_ListPartialMemberCourses", Model)` dynamically loads the course list.

### Modals
- **Course Details Modal**
  - Displays department, title, start/end dates, location, description, and attachments.
  - Read-only inputs and dynamic attachment handling.
  - Populated via AJAX call to `/Admin/Member/GetCourse`.

### Scripts
- AJAX for fetching course details and populating modal.
- Session storage for filters, pagination, and page size persistence.
- Functions: `saveFilters(page, pageSize)` and `loadNewPartialList(page, pageSize)`.

---

## _ListPartialMemberCourses.cshtml

### Purpose
- Render a **table of courses** for a member.
- Support pagination and page size selection.

### Structure
1. **Hidden Inputs**
   - `NewCountRecords` and `NewMemberId` for updating parent view after AJAX.
2. **Page Size Selector**
   - Dropdown to select 50, 100, or 150 rows per page.
3. **Courses Table**
   - Columns: Start Date, Department, Location, Course Title, Actions.
   - Language support for Arabic/English via `SessionHelper.GetCurrentLanguage()`.
   - "View Details" button opens course modal.
4. **Empty State**
   - Shows a message when there are no courses.
5. **Pagination**
   - Previous/Next links and numbered pages.
   - Uses AJAX to load the selected page.

---

## _MemberCourseDetailsPartial.cshtml

### Purpose
- Display **course details** in a read-only format.

### Fields
- Course Title (Arabic/English)
- Start and End Dates
- Location
- Description

---

## PrintMemberCourses.cshtml

### Purpose
- Generate a **printable view of member courses**.
- Supports **RTL and LTR** depending on session language.

### Features
- Official header with club name and logo.
- Table listing courses with Start Date, Department, Location, and Course Title.
- Footer includes user info and timestamp.
- Loading overlay while the report is generated.
- Auto-print and close window after printing via JavaScript.

---

## PrintDetails.cshtml

### Purpose
- Generate a **printable member profile**.

### Features
- Displays profile image, ID image, passport image.
- Sections: Code, Full Name (AR/EN), Nationality, Gender, Date of Birth, Age, Profession/Guardian Job, Address, Email, ID Number, ID Expiry, Academic Qualification, Education Institution, Hobby, Languages, Social Media, Heard By.
- RTL layout for Arabic.
- Footer with printed by and timestamp.
- JavaScript handles date formatting and printing.

---

## General Notes

- **AJAX Partial Loading**: Used to update the courses list dynamically without refreshing the page.
- **Session Storage**: Stores filters, page number, and page size to persist state.
- **Localization**: Supports Arabic and English text dynamically.
- **Modals**: Bootstrap modals for displaying detailed course information.
- **Printing**: Separate views without layout for clean printing.
- **Export**: Excel export via dedicated action.

---

## Flow Summary

1. Load `MemberCourses` page.
2. Display breadcrumbs and record count.
3. Load `_ListPartialMemberCourses` partial view via server.
4. User can:
   - View course details in modal.
   - Change page size or page via AJAX.
   - Filter results using form.
   - Print or export courses.
5. `PrintMemberCourses` and `PrintDetails` render printable reports.
6. Modals and AJAX ensure seamless user experience.

