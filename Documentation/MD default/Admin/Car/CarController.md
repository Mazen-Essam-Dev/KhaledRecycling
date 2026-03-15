# 📄 CarController.cs

## 📦 Namespace
`FougeraClub.Areas.Admin.Controllers`

## 🧩 Class: CarController
Handles all CRUD operations, filtering, and reporting for `Car` entities in the Admin area.

### 🔹 Attributes
- `[AdminAuthorize]` — Restricts access to admin users.
- `[Area("Admin")]` — Specifies the area for routing.

### 🔹 Dependencies
| Dependency | Purpose |
|------------|---------|
| `ICarService _CarService` | Service for car-related operations (CRUD). |
| `IUnitOfWork _unitOfWork` | Provides repository access to database tables. |
| `IMapper _mapper` | AutoMapper instance to map between `Car` entities and `CarVM` ViewModels. |

### 🔹 Constructor
`CarController(ICarService CarService, IUnitOfWork unitOfWork, IMapper mapper)`
- Injects required services and mapper.

---

## 🔹 Actions

### 1. `Index(...) : IActionResult`
- Returns the list of cars with optional filters: search term, type, and date range.
- Supports pagination and AJAX partial updates.
- Filters include:
  - PlateNumber, DriverName, Color, Model
  - Type filter
  - OwnershipExpiryDate range (`dateFrom`, `dateTo`)
- Sets `inService` flag based on `CarServices`.
- Returns a paginated `CarVM` list.

### 2. `AddEdit(int? id) : IActionResult`
- GET: Shows empty `CarVM` for adding or populated ViewModel for editing based on `id`.

### 3. `AddEdit(CarVM vm) : IActionResult`
- POST: Handles form submission for adding or editing a car.
- Validates file type and size for attachments.
- Maps `CarVM` to `Car` entity.
- Adds or updates the entity using `CarService`.
- Redirects appropriately after add or edit.

### 4. `Delete(int id) : IActionResult`
- Deletes a car by id using `CarService`.
- Also deletes the attached file from disk.

### 5. `Print(...) : IActionResult`
- Generates a print view of the filtered car list.
- Supports search term, type filter, and date range.
- Uses the same filtering logic as `Index`.

### 6. `createExcelReport_Download(...) : IActionResult`
- Generates an Excel report of the filtered car list.
- Supports both Arabic and English titles.
- Uses `ExcelStaticReport.ExcelReportArEn_` for report generation.
- Returns `FileContentResult` for download.
- Handles exceptions by redirecting to `Index`.

### 7. `GetAttachmentsAsync(...) : IActionResult`
- Get all this Car Attachments.
- Supports modal For Showing .
- Uses <CarAttachmentsVM> for server-side. 
- Supports both Arabic and English names.

### 8. `UploadAttachmentsAsync(...) : IActionResult`
- Supports both Arabic and English names.
- Uses <CarAttachmentsVM> for server-side. 
- All file uploads and deletions are managed via FileHelper.
- Upload all this Car Attachments and Remove.
---

## 🔹 Notes
- Uses `PaginatedList<CarVM>` for server-side pagination.
- AJAX support: Checks header `"X-Requested-With" == "XMLHttpRequest"` to return partial view.
- All file uploads and deletions are managed via `FileHelper`.
- Session language (`SessionHelper.GetCurrentLanguage()`) determines error messages and Excel report language.
