# CarServiceController Documentation

## 📂 Namespace & Dependencies

```csharp
using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.CarServices;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
```

This controller manages **Car Service** operations in the Admin area, including listing, adding, editing, deleting, printing, and exporting reports to Excel.

---

## 🏛 Controller: `CarServiceController`

**Attributes:**

* `[AdminAuthorize]` — Ensures only authorized admin users can access.
* `[Area("Admin")]` — Defines this controller under the Admin area.

**Dependencies:**

* `ICarServiceManager` — Service for business logic related to `CarServiceEntity`.
* `IUnitOfWork` — Database repository unit of work.
* `IMapper` — AutoMapper for mapping between `CarServiceVM` and `CarServiceEntity`.

**Constructor:**

```csharp
public CarServiceController(ICarServiceManager CarServiceService, IUnitOfWork unitOfWork, IMapper mapper)
```

Initializes the controller with service, unit of work, and AutoMapper.

---

## 📄 Actions

### 1. `Index`

```csharp
public async Task<IActionResult> Index(string? selectedType, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
```

* **Purpose:** Lists all car services with optional filters by car type and date range.
* **Features:**

  * Pagination support using `PaginatedList<CarServiceVM>`.
  * Filtering by car type, `DateFrom`, and `DateTo`.
  * AJAX partial view rendering for dynamic updates.
  * Populates `ViewBag.CarTypes` for car type selection dropdown.

---

### 2. `AddEdit` (GET)

```csharp
public async Task<IActionResult> AddEdit(int? id, int? carId)
```

* **Purpose:** Returns the Add/Edit view for a car service.
* **Parameters:**

  * `id` — CarService ID for editing, `null` for adding.
  * `carId` — Optional car ID to pre-select when adding.
* **Behavior:**

  * Loads car list for dropdown (`vm.CarList`).
  * Maps existing `CarServiceEntity` to `CarServiceVM` when editing.
  * Preserves TempData for redirection after POST.

---

### 3. `AddEdit` (POST)

```csharp
[HttpPost]
public async Task<IActionResult> AddEdit(CarServiceVM model)
```

* **Purpose:** Handles submission of Add/Edit form.
* **Features:**

  * Validates uploaded attachment (PDF < 5 MB).
  * Uses AutoMapper to convert VM to Entity.
  * Adds a new record or updates an existing one using `ICarServiceManager`.
  * Redirects to Index after Add, or stays on Add/Edit for Edit.

---

### 4. `Delete`

```csharp
[HttpPost]
public async Task<IActionResult> Delete(int id)
```

* **Purpose:** Deletes a car service record by ID.
* **Behavior:**

  * Calls `ICarServiceManager.DeleteAsync`.
  * Redirects to `Index` or back to `Car` index if triggered from a car page.

---

### 5. `Print`

```csharp
[IgnoreAction]
[Route("Admin/CarService/Print")]
public async Task<IActionResult> Print(string? selectedType, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
```

* **Purpose:** Displays a printable list of car services.
* **Features:**

  * Supports filtering by type and date range.
  * Returns a normal View (no partial or AJAX).

---

### 6. `createExcelReport_Download`

```csharp
[IgnoreAction]
public async Task<IActionResult> createExcelReport_Download(string? selectedType, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
```

* **Purpose:** Generates and downloads an Excel report of car services.
* **Features:**

  * Filters data by type and date range.
  * Maps data to `ExcelDataDTO`.
  * Uses `ExcelStaticReport.ExcelReportArEn_` for multi-language export (Arabic/English).
  * Returns `FileContentResult` containing the Excel file.
  * Handles exceptions and redirects to Index if error occurs.

---

## 🔧 Internal Helpers

* `SelectListHelper.BindSelectList` — Populates `SelectListItem` lists.
* `FileHelper.CheckFileIsPdf_5Mg_Async` — Validates uploaded PDF attachments.
* `SessionHelper.GetCurrentLanguage()` — Gets current session language for reports.

---

## 🗂 ViewModel

* `CarServiceVM`:

  * Properties: `Id`, `CarId`, `Car`, `CarList`, `Date`, `Details`, `AttachmentPath`, `Attachment`.
  * Supports form validation and file uploads.

---

## 📦 Notes

* All database access is handled via `IUnitOfWork` and `ICarServiceManager`.
* AutoMapper is used to map between entity and view model.
* AJAX is supported for list updates to improve user experience.
* Export functionality is language-aware and supports Arabic and English.
* Attachment handling includes validation and saving/removing files.
