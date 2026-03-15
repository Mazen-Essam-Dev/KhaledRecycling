# EmployeesController Documentation

## 📂 Namespace & Dependencies

```csharp
using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.DTOs.Admin.Employees;
using Domain.Entities.Employees;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.Employees;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
```

## 🏷️ Controller Overview

**Class:** `EmployeesController`
**Area:** `Admin`
**Authorization:** `[AdminAuthorize]`

Handles CRUD operations for employees, including listing, adding/editing, deleting, viewing details, and generating Excel reports. It also supports AJAX partial updates and file uploads for employee attachments.

**Dependencies:**

* `IUnitOfWork` – Access database repositories.
* `IEmployeeService` – Business logic for employees.
* `IMapper` – Mapping between ViewModels and Entities.
* `IWebHostEnvironment` – Web hosting environment, used for file paths.

---

## 🔹 Actions

### 1. `Index`

**Signature:**

```csharp
public async Task<IActionResult> Index(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
```

**Description:**
Displays a paginated list of employees with optional search and date filters. Supports AJAX requests to return partial views for dynamic updates.

**Parameters:**

* `searchTerm` – Filters by name, national ID, or phone number.
* `dateFrom` – Filter employees whose NationalIdExpiryDate is after this date.
* `dateTo` – Filter employees whose NationalIdExpiryDate is before this date.
* `page` – Current page for pagination.
* `pageSize` – Number of employees per page.

**Returns:**
`View` or `PartialView` with `EmployeeVM` containing:

* List of employees
* Pagination info
* Search and filter parameters

---

### 2. `AddEdit (GET)`

**Signature:**

```csharp
public async Task<IActionResult> AddEdit(int? id)
```

**Description:**
Displays the form for adding a new employee or editing an existing one. Populates nationality dropdowns.

**Parameters:**

* `id` – Employee ID for editing. Null or 0 for adding new employee.

**Returns:**
`View` with `EmployeeVM`.

---

### 3. `AddEdit (POST)`

**Signature:**

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> AddEdit(EmployeeVM model)
```

**Description:**
Handles form submission for creating or updating an employee. Validates file uploads for employee photo and ensures model is valid.

**Behavior:**

* If `model.Id` is 0/null → Add new employee.
* If `model.Id` exists → Update existing employee.
* Redirects to `Index` after add or back to `AddEdit` after edit.

---

### 4. `Details`

**Signature:**

```csharp
[IgnoreAction]
public async Task<IActionResult> Details(int id)
```

**Description:**
Displays full details of a single employee. Populates nationality dropdown and preserves previous URL for navigation.

---

### 5. `PrintDetails`

**Signature:**

```csharp
[IgnoreAction]
public async Task<IActionResult> PrintDetails(int id)
```

**Description:**
Returns a printable view of employee details.

---

### 6. `GetEmployeeAttachments`

**Signature:**

```csharp
public async Task<IActionResult> GetEmployeeAttachments(int id)
```

**Description:**
Returns a partial view containing employee attachments.

**Parameters:**

* `id` – Employee ID

**Returns:**
`PartialView` with `EmployeeAttachmentsVM`.

---

### 7. `UploadEmployeeFiles`

**Signature:**

```csharp
[IgnoreAction]
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> UploadEmployeeFiles(EmployeeAttachmentsVM model)
```

**Description:**
Handles uploading of employee attachments.

---

### 8. `Delete`

**Signature:**

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(int id)
```

**Description:**
Deletes an employee by ID. Redirects back to `Index`.

---

### 9. `Print`

**Signature:**

```csharp
[IgnoreAction]
public async Task<IActionResult> Print(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
```

**Description:**
Returns a printable list of employees filtered by search term and dates.

---

### 10. `createExcelReport_Download`

**Signature:**

```csharp
[IgnoreAction]
public async Task<IActionResult> createExcelReport_Download(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
```

**Description:**
Generates and downloads an Excel report of employees filtered by search term and date range. Uses `ExcelStaticReport` helper to create Excel in Arabic or English.

**Returns:**
`FileContentResult` containing the Excel file.

---

### 🔹 Notes:

* Uses `SessionHelper.GetCurrentLanguage()` to switch between Arabic and English views or reports.
* Validates uploaded images using `FileHelper.CheckFileIsImage_3Mg_Async`.
* Supports partial rendering for AJAX with `X-Requested-With` header check.
* `[IgnoreAction]` attributes mark actions not directly exposed for
