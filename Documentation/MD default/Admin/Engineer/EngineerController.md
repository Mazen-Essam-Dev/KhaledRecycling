# 🧑‍💼 **EngineerController**

## 📁 **Namespace**
`FougeraClub.Areas.Admin.Controllers`

---

## ⚙️ **Dependencies**
```csharp
using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.Engineer;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
```

---

## 🧩 **Controller Overview**
The **EngineerController** is responsible for managing all **Engineer**-related operations in the **Admin area**.  
It includes CRUD functionality, filtering, pagination, and report generation (Excel and print views).

The controller uses:
- `IEngineerService` → Handles business logic for engineers.  
- `IUnitOfWork` → Manages database operations and repositories.  
- `IMapper` → Maps between domain entities and view models.  

All actions are restricted to Admin users via `[AdminAuthorize]`.

---

## 🔑 **Constructor**
```csharp
public EngineerController(IEngineerService engineerService, IUnitOfWork unitOfWork, IMapper mapper)
```
**Purpose:**  
Injects dependencies for service, database, and mapping.

---

## 📜 **Action Methods**

### 1️⃣ **Index**
```csharp
public async Task<IActionResult> Index(string? searchTerm, int? selectedNationalityId, int? selectedGraduationYear, int page = 1, int pageSize = 50)
```
**Description:**  
Displays a paginated list of engineers with **filtering by search term**, **nationality**, and **graduation year**.

**Key Features:**
- Fetches dropdowns for Nationalities and Graduation Years.
- Supports **AJAX-based partial rendering** for search updates.
- Uses `PaginatedList<T>` for pagination.
- Language-dependent dropdowns (Arabic/English).

---

### 2️⃣ **AddEdit (GET)**
```csharp
public async Task<IActionResult> AddEdit(int? id)
```
**Description:**  
Displays the **Add/Edit form** for an Engineer.

**Behavior:**
- If `id` is null → Creates a new engineer with auto-generated code.
- If `id` exists → Loads existing engineer for editing.
- Populates `NationalityList` and `GraduationYears` dropdowns.

---

### 3️⃣ **AddEdit (POST)**
```csharp
[HttpPost]
public async Task<IActionResult> AddEdit(EngineerVM model)
```
**Description:**  
Handles **form submission** for creating or updating an Engineer.

**Logic:**
- Validates uploaded profile image (`FileHelper.CheckFileIsImage_3Mg_Async`).
- Maps `EngineerVM` to `Engineer` entity.
- If new engineer → Calls `AddAsync`.
- If existing → Calls `UpdateAsync`.
- Redirects to:
  - `Index` → After adding new engineer.
  - `AddEdit` → After editing.

---

### 4️⃣ **Delete**
```csharp
[HttpPost]
public async Task<IActionResult> Delete(int id)
```
**Description:**  
Deletes an engineer by ID using `_engineerService.DeleteAsync`.

---

### 5️⃣ **Print**
```csharp
[IgnoreAction]
public async Task<IActionResult> Print(string? searchTerm, int? selectedNationalityId, int? selectedGraduationYear)
```
**Description:**  
Generates a **printable view** of filtered engineers.

**Features:**
- Supports the same filters as `Index`.
- Maps `Engineer` entities to `EngineerVM`.
- Returns a **print-friendly View** (not a downloadable file).

---

### 6️⃣ **createExcelReport_Download**
```csharp
[IgnoreAction]
public async Task<IActionResult> createExcelReport_Download(string? searchTerm, int? selectedNationalityId, int? selectedGraduationYear)
```
**Description:**  
Generates and downloads an **Excel report** of filtered engineers.

**Process:**
1. Filters engineers by name, phone, specialization, nationality, or graduation year.
2. Maps to `ExcelDataDTO`.
3. Builds column titles based on localization (`Resource1`/`Resource2`).
4. Calls `ExcelStaticReport.ExcelReportArEn_` to generate file.
5. Returns file as `.xlsx`.

**Return:**  
`FileContentResult` (Excel file download)

---

### 7️⃣ **Details**
```csharp
[IgnoreAction]
public async Task<IActionResult> Details(int id)
```
**Description:**  
Displays full details of a single Engineer, including nationality list.

---

### 8️⃣ **PrintDetails**
```csharp
[IgnoreAction]
public async Task<IActionResult> PrintDetails(int id)
```
**Description:**  
Displays a **printable version** of the Engineer details page.

---

## 🧱 **Helper Classes Used**
| Helper | Purpose |
|--------|----------|
| `SessionHelper` | Retrieves current language (used for multilingual UI). |
| `SelectListHelper` | Generates dropdown lists for nationalities. |
| `FileHelper` | Validates uploaded images (type & size). |
| `PaginatedList<T>` | Handles pagination logic for large datasets. |
| `ExcelStaticReport` | Generates Excel reports for export. |

---

## 🧠 **Attributes**
| Attribute | Description |
|------------|--------------|
| `[AdminAuthorize]` | Restricts access to Admin users only. |
| `[Area("Admin")]` | Declares this controller belongs to the **Admin area**. |
| `[IgnoreAction]` | Marks actions that should be **excluded from permission scanning**. |

---

## 🧾 **Views Used**
| Action | View Name | Partial View |
|--------|------------|---------------|
| `Index` | `Index.cshtml` | `_ListPartial.cshtml` |
| `AddEdit` | `AddEdit.cshtml` | — |
| `Details` | `Details.cshtml` | — |
| `Print` | `Print.cshtml` | — |
| `PrintDetails` | `PrintDetails.cshtml` | — |

---

## 🗂 **Summary of Responsibilities**
| Feature | Description |
|----------|--------------|
| **Listing & Filtering** | Displays paginated engineers with filtering options. |
| **CRUD Operations** | Add, Edit, Delete engineers. |
| **File Upload** | Validates and uploads profile image. |
| **Localization** | Adjusts displayed data according to current language. |
| **Reports** | Generates printable and Excel reports. |
| **Dropdown Management** | Dynamically populates Nationalities and Graduation Years. |

---

## 🧾 **Example Flow**
1. **Admin navigates** to `/Admin/Engineer/Index`
2. The controller fetches engineers with optional filters.
3. Admin clicks **Add/Edit** to open form.
4. Admin submits form → validation and database save.
5. Admin can view **Details**, **Print**, or **Export Excel**.

---

## 🧭 **Controller Summary**
| Method | Type | Description |
|---------|------|--------------|
| `Index` | GET | List engineers with filtering & pagination |
| `AddEdit` | GET | Display form for Add/Edit |
| `AddEdit` | POST | Save new or updated engineer |
| `Delete` | POST | Delete engineer |
| `Print` | GET | Generate printable list |
| `createExcelReport_Download` | GET | Generate Excel file |
| `Details` | GET | View engineer details |
| `PrintDetails` | GET | Printable detail page |

---

📚 **Author:** *FougeraClub Admin Team*  
🕒 **Last Updated:** *November 2025*  
🏗 **Framework:** ASP.NET Core MVC (Clean Architecture)
