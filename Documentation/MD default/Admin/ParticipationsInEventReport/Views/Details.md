# Participations In Event Report – Documentation

This document explains the structure, logic, and functionality of the **ParticipationsInEventReport** views in the ASP.NET Core MVC application.

---

## 1. Details View (`Details.cshtml`)

**Purpose:**  
Displays detailed information for a single participation report, including report metadata, activities, images, and digital signatures.

### Model

- **ViewModel:** `ParticipationsInEventReportVM`
- **Key Properties:**
  - `ReportTitle` – The title of the report.
  - `Date` – The date of the report.
  - `AdministrativeDepartment` – Name of the responsible department.
  - `Details` – Collection of activities (name, action, reason).
  - `Image1Path`, `Image2Path`, `Image3Path`, `Image4Path` – Paths for uploaded participation images.
  - `TrainerSignature` / `ManagerSignature` – Objects containing image paths for signatures.

### Breadcrumb Navigation

- Shows a navigation path:
  - Parent module: `ExternalWorkMissionManagement`
  - Current view: `DetailsParticipationsInEventReport`

### Print Button Logic

- Enabled only if **both** manager and trainer signatures exist.
- Opens a popup window for printing the report using `PrintDetails` view.
- Prevents freezing the main page by using a standalone window (`window.open()`).

### Data Sections

1. **Report Info**
   - Displays `ReportTitle`, `Date`, and `AdministrativeDepartment`.
   - Inputs are readonly for display purposes.

2. **Activities Table**
   - Columns: `ActivityName`, `ActivityAction`, `ActivityReason`.
   - Iterates over `Model.Details` to display each activity.
   - If no activities exist, shows a placeholder message with an icon.

3. **Images**
   - Displays all non-empty images from `Image1Path` to `Image4Path`.
   - Images arranged 2 per row.

4. **Signatures**
   - Trainer and Manager signature sections.
   - If signature exists, displays the image.
   - If missing and the current user has permission, allows signing via OTP modal.

### OTP Modal

- Shared for both Trainer and Manager roles.
- Collects 4-digit OTP for signature verification.
- Includes validation logic (digits only, length = 4).
- AJAX call to `SendOtp` endpoint to send OTP.
- AJAX call to `ValidateOtp` endpoint to confirm signature.

---

## 2. Print Details View (`PrintDetails.cshtml`)

**Purpose:**  
Provides a printer-friendly version of a single participation report.

### Layout

- No layout is used (`Layout = null`).
- Right-to-left direction (`direction: rtl`).
- Header contains UAE and club information with a logo.
- Footer prints current user and timestamp.

### Data Display

1. **Report Info Table**
   - Displays `Date` and `AdministrativeDepartment`.

2. **Activities Table**
   - Columns: `ActivityName`, `ActivityAction`, `ActivityReason`.
   - Iterates over `Model.Details`.

3. **Images**
   - Displays non-empty images in rows of 2 per row.
   - Images have max-height restriction to prevent page break issues.

4. **Signatures**
   - Trainer and Manager signatures are displayed if available.

### Printing Script

- Automatically hides loading overlay and calls `window.print()` after page load.
- Closes the print window after printing.

---

## 3. Print List View (`Print.cshtml`)

**Purpose:**  
Prints a list of multiple participation reports.

### Layout

- No layout (`Layout = null`).
- Chooses CSS based on language (RTL or LTR).
- Header with official club information and logo.
- Footer prints current user and timestamp.

### Table Display

- Columns: `ParticipatingTitle`, `DepartManage`, `Date`.
- Iterates over `Model` (list of reports).
- If no reports exist, shows a placeholder message.

### Printing Script

- Shows loading overlay until content is ready.
- Calls `window.print()` automatically.
- Closes the print window after printing.

---

## 4. Common Features Across Views

1. **Localization**
   - Uses `Resource1` and `Resource2` for labels and messages.
   - Supports RTL (Arabic) and LTR (English) displays.

2. **Conditional Logic**
   - Signature display depends on existing images and permissions.
   - Print button depends on presence of signatures.

3. **JavaScript Interactivity**
   - OTP input handling (focus, validation, digit-only input).
   - Print popup handling to avoid freezing main page.
   - AJAX calls for sending and validating OTP.

4. **Responsive Layout**
   - Images and tables adapt to screen width.
   - Ensures readability on print and screen.

---

## 5. Summary

- **Details View:** User-facing detailed display with editable signature options.  
- **PrintDetails View:** Printer-friendly single report.  
- **Print View:** Printer-friendly list of reports.  
- OTP system ensures secure signature validation.  
- Views leverage localization, permissions, and responsive design for a robust reporting experience.
