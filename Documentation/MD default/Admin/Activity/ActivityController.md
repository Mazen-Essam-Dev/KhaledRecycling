# ActivityController.md

# ActivityController.cs

**Path:** `Areas/Admin/Controllers/ActivityController.cs`  
**Description:**  
This controller manages **Activities** in the Admin area of the FougeraClub application. It handles core operations such as listing, adding/editing, deleting, managing member subscriptions, and exporting/printing activity data with comprehensive filtering and reporting capabilities.

**Recent Updates:** Enhanced file attachment handling, improved Excel reporting, and optimized data filtering with better performance and user experience.

---

## Properties

- `_ActivityService`: Service for handling activity CRUD operations with business logic encapsulation.  
- `_unitOfWork`: Provides access to various repositories like Users, Departments, Subscriptions using Unit of Work pattern.  
- `_mapper`: AutoMapper instance for mapping between DTOs and ViewModels with configuration-based mapping.  
- `_excelReportService`: Service to generate Excel reports for activities with localization support.

---

## Constructor

Injects the above services via **Dependency Injection** when creating the controller, ensuring loose coupling and testability through interface-based service contracts.

---

## Actions

### 1. Index

- **Purpose:** Display a paginated list of activities with advanced search and date filtering capabilities.  
- **Features:**
  - Calculate activity duration (`ActivityDuration`) and subscription count (`SubscriptionCount`) dynamically for each activity.
  - Search by keyword (`searchTerm`) across both Arabic and English title fields with case-insensitive matching.
  - Filter by dates (`dateFrom`, `dateTo`) using `DateOnly` type for date-specific operations.
  - Pagination using `PaginatedList<ActivityVM>` with configurable page size (default: 50).
  - Supports Ajax requests to return a `PartialView` for dynamic UI updates.
  - **Recent Enhancement:** Added subscription count calculation directly in controller for real-time statistics.
  - **Performance Update:** Optimized query execution with AsQueryable() for deferred execution.

---

### 2. AddEdit

- **Purpose:** Add a new activity or edit an existing one with comprehensive file attachment management.  
- **Features:**
  - Load existing activity data for editing with proper ViewModel mapping.
  - Validate uploaded file attachments (PDF format, ≤ 5MB size limit) using `FileHelper` utilities.
  - Refill dropdown lists when validation fails to maintain form state.
  - **Recent File Handling Update:** Implemented temporary file storage during validation to prevent file loss.
  - **Attachment Management:** Three-phase file handling: new uploads, temporary files, and existing files.
  - **Validation Improvement:** Separate validation for file type and size with user-friendly error messages.

#### File Attachment Workflow:
1. **Temporary Storage:** New uploads saved temporarily during validation.
2. **File Validation:** PDF format and 5MB size limit enforced.
3. **Final Processing:** Files moved to permanent storage only after successful validation.
4. **Cleanup:** Old files deleted when replaced with new uploads.

---

### 3. Details

- **Purpose:** Display activity details in a `PartialView` for modal or quick-view display.  
- **Features:** 
  - Check if the attached file exists before displaying it using `FileHelper.IsFileExist()`.
  - Calculate activity duration dynamically for display.
  - **Recent Update:** Added null checking for file paths to prevent broken links.
  - **Performance:** Lightweight partial view for quick loading.

---

### 4. MembersActivity

- **Purpose:** Display members subscribed to a specific activity with pagination and filtering.  
- **Features:**  
  - Fetch activity and subscribed members with efficient data retrieval.
  - Map data to `ViewModel` using AutoMapper for consistency.
  - Pagination support with configurable page size.
  - Supports Ajax requests for partial view rendering without full page reload.
  - **Recent Enhancement:** Combined activity and member data in single ViewModel (`SubscribedMemberInActivitiesVM`).
  - **Performance Update:** Optimized member retrieval with service layer abstraction.

---

### 5. Delete

- **Purpose:** Delete a specific activity by `id` with proper error handling.  
- **Features:** 
  - Redirects back to the activity list after successful deletion.
  - **Recent Update:** Added async pattern for non-blocking operation.
  - **Security:** Admin authorization required for delete operations.

---

### 6. Print & ExportToExcel

- **Purpose:** Print or export the list of activities to Excel with comprehensive formatting.  
- **Features:**
  - Supports keyword and date filtering before export for targeted reporting.
  - Calculate activity duration and subscription count for export data.
  - Supports both Arabic and English languages with locale-specific formatting.
  - Generates Excel report using `ExcelStaticReport` utility with proper column headers.
  - **Recent Enhancement:** Added dynamic column generation based on resource files.
  - **Formatting Update:** Improved date formatting with proper locale handling.

#### Export Features:
- **Localized Headers:** Column titles from Resource1 for multi-language support.
- **Date Formatting:** Consistent date display across languages.
- **Dynamic Data:** Real-time calculation of duration and subscription counts.
- **File Naming:** Timestamp-based file names for easy organization.

---

### 7. createExcelReport_Download

- **Purpose:** Download filtered activity data as an Excel file with advanced formatting.  
- **Features:**
  - Convert activity data to `ExcelDataDTO` for consistent Excel generation.
  - Support for Arabic and English with proper RTL/LTR formatting.
  - Return file as `FileContentResult` for direct browser download.
  - **Recent Enhancement:** Added try-catch error handling for robust file generation.
  - **File Naming:** Includes timestamp and localized activity list name.

#### Excel Generation Process:
1. **Data Filtering:** Apply search and date filters to activity data.
2. **DTO Conversion:** Map to `ExcelDataDTO` for Excel compatibility.
3. **Localization:** Apply language-specific formatting.
4. **File Generation:** Create Excel file with proper MIME type.
5. **Download:** Return as file stream for browser download.

---

### 8. PrintMembersActivity & createExcelReport_Download_MembersActivity

- **Purpose:** Print or export the list of members subscribed to a specific activity.  
- **Features:**
  - Fetch member and subscription data with related nationality information.
  - Support filtering by specific activity ID.
  - Support both Arabic and English with proper localization.
  - Generate downloadable Excel report with member details.
  - **Recent Enhancement:** Added activity title in exported file name for context.
  - **Data Enrichment:** Includes member age, contact information, and subscription dates.

#### Member Export Features:
- **Comprehensive Data:** Name, nationality, age, contact details, subscription date.
- **Activity Context:** Activity title included in export for reference.
- **Localized Formatting:** Proper date and text formatting based on language.
- **Error Resilience:** Graceful fallback for missing data.

---

## Validation System

### File Validation
- **PDF Validation:** Ensures uploaded files are PDF format using `FileHelper.CheckFileIsPdf_5Mg_Async()`.
- **Size Limit:** 5MB maximum file size for attachments.
- **Temporary Storage:** Files saved temporarily during validation to prevent data loss.
- **Cleanup:** Old files deleted when replaced to prevent storage bloat.

### Model Validation
- **Required Fields:** Title (both Arabic and English), dates, and location.
- **Date Validation:** Start date before end date validation.
- **File Path Handling:** Proper management of file paths during edit operations.
- **Recent Enhancement:** Improved validation feedback with specific error messages.

---

## Session and Localization

### Language Support
- **Dynamic Language:** Uses `SessionHelper.GetCurrentLanguage()` for locale detection.
- **Resource Files:** Localized strings from Resource1 and Resource2.
- **Bilingual UI:** Support for both Arabic (RTL) and English (LTR) interfaces.
- **Recent Update:** Consistent language handling across all views and exports.

### Session Management
- **Language Preference:** Session-based language storage.
- **Temp Data:** Temporary file paths during multi-step operations.
- **Recent Enhancement:** Improved session handling for better user experience.

---

## Excel Reporting System

### Architecture
- **Service Layer:** `IExcelReportService<T>` interface for report generation.
- **Static Utility:** `ExcelStaticReport` class for common Excel operations.
- **DTO Pattern:** `ExcelDataDTO` for consistent data structure.
- **Recent Enhancement:** Unified Excel generation across all report types.

### Features
- **Localized Reports:** Arabic and English support with proper formatting.
- **Dynamic Columns:** Column headers from resource files.
- **Error Handling:** Graceful fallback on report generation failures.
- **Performance:** Efficient memory usage for large datasets.

---

## Pagination System

### Implementation
- **PaginatedList<T>:** Custom pagination class for consistent paging.
- **Configurable Page Size:** Default 50 items per page, adjustable.
- **Search Integration:** Maintains search terms across pagination.
- **Ajax Support:** Partial view updates for dynamic pagination.

### Features
- **Efficient Queries:** Queryable interface for deferred execution.
- **UI Integration:** Seamless integration with Bootstrap pagination.
- **State Preservation:** Maintains filter state across pages.
- **Recent Enhancement:** Improved performance with optimized queries.

---

## File Management System

### Directory Structure