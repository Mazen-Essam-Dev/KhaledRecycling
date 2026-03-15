# 📄 ScientificProjectsController.cs

## 📦 Namespace
`FougeraClub.Areas.Admin.Controllers`

---

## 🧩 Class: ScientificProjectsController
Handles **CRUD operations and workflow for Scientific Projects** in the Admin module, including:

- Listing, filtering, searching, and paginating projects
- Creating and editing projects with **file uploads**
- Viewing details, printing, and project cards
- OTP-based signing/approval workflow for Trainers, Activity Monitors, and Managers
- Sending notifications via SignalR and `_notificationService`

### 🔹 Attributes
| Attribute | Purpose |
|-----------|---------|
| `[AdminAuthorize]` | Restrict access to admin users |
| `[Area("Admin")]` | Specify the MVC area for routing |
| `[Route("Admin/[controller]/[action]")]` | Custom route pattern |

---

## 🔹 Dependencies
| Dependency | Purpose |
|------------|---------|
| `IUnitOfWork _unitOfWork` | Repository & database unit of work |
| `IWebHostEnvironment _env` | Host environment (used for file handling) |
| `IScientificProjectsService _service` | Business logic service for Scientific Projects |
| `UserManager<ApplicationUser> _userManager` | Identity user management |
| `IHubContext<NotificationHub> _hubContext` | SignalR hub for real-time notifications |
| `INotificationService _notificationService` | Sending notifications to users/roles |
| `IHttpContextAccessor _httpContextAccessor` | Access session & HTTP context for role validation |

---

## 🔹 Helper Methods

### ValidateRoleNumber
```csharp
[IgnoreAction]
        [NoLogging]
        public RoleNumber ValidateRoleNumber()
        {
            var roleNumber = _httpContextAccessor.HttpContext?.Session.GetInt32("RoleNumber");
            if (roleNumber != null)
            {
                return (RoleNumber)roleNumber;
            }
            return RoleNumber.NormalUser;
        }
```
## 📌 Actions Overview

### 0. OTP
* OTP Roles `1(Trainer)` --> `2(activityMonitor)` --> `3(Manager)` Arranged
* This `1(Trainer)` has OTP Who is selected by DropDown.
* OTP First sign `1(Trainer)` then close edit for him and sending Notification to `2(activityMonitor)`
and then Open Sign OTP for `2(activityMonitor)` and if he Submit Edit (Update) --> Not remove his sign or Any Sign pervious him.
* else if `2(activityMonitor)` Validate otp then close edit for him and sending Notification to `3(Manager)`
* if `3(Manager)` Validate otp then close edit for Any one and Open `ProjectCard Enabled And his Print `
* else if `3(Manager)` Submit Edit (Update) --> Not remove his sign or Any Sign pervious him.
* if First User Sign --> Delete Button will be disabled for all users.
---

### 1. Index
```csharp
public async Task<IActionResult> Index(string searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
````

* Displays a paginated list of `ScientificProjects`.
* Supports search by project name or department, and date filtering.
* Returns partial view if AJAX request.

---

### 2. Create (GET)

```csharp
public async Task<IActionResult> Create()
```

* Prepares a new `ScientificProjectsVM` with the next `SerialCode`.
* Fetches all departments.
* Sets current date.

### 3. Create (POST)

```csharp
[HttpPost]
public async Task<IActionResult> Create(ScientificProjectsVM model)
```

* Validates files (`File1` to `File4`) are images and under 3MB.
* Saves uploaded files to `/uploads/Admin/ScientificProjects/`.
* Adds new `ScientificProjects` entity to the database.
* Checks for unique `SerialCode` and auto-increments if duplicate.
* Redirects to `Index`.

---

### 4. Details

```csharp
[HttpGet]
public async Task<IActionResult> Details(int id)
```

* Retrieves a single project by ID including `ManagerSignature` and `ActivityMonitorSigniture`.
* Returns project details in `ScientificProjectsVM`.

---

### 5. PrintDetails

```csharp
[IgnoreAction]
public async Task<IActionResult> PrintDetails(int id)
```

* Similar to `Details` but intended for printable view.

---

### 6. ProjectCard / PrintProjectCard

```csharp
public async Task<IActionResult> ProjectCard(int id)
[IgnoreAction]
public async Task<IActionResult> PrintProjectCard(int id)
```

* Retrieves essential project information for a "Project Card" view.
* Printable version via `PrintProjectCard`.

---

### 7. Edit (GET)

```csharp
public async Task<IActionResult> Edit(int? id)
```

* Loads existing project into `ScientificProjectsVM` for editing.
* Includes all departments for dropdown selection.

### 8. Edit (POST)

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(ScientificProjectsVM model, IFormFile File1, IFormFile File2, IFormFile File3, IFormFile File4)
```

* Validates uploaded files and updates existing project.
* Saves new uploaded files and updates file paths.
* Persists changes in the database.
* Redirects back to `Edit`.

---

### 9. Delete

```csharp
[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Delete(int id)
```

* Deletes the specified project from the database.
* Redirects to `Index`.

---

### 10. Print

```csharp
[IgnoreAction]
public async Task<IActionResult> Print(string searchTerm, DateOnly? dateFrom, DateOnly? dateTo)
```

* Filters projects by search term and date.
* Returns view suitable for printing.

---

### 11. createExcelReport_Download

```csharp
[IgnoreAction]
public async Task<IActionResult> createExcelReport_Download(string searchTerm, DateOnly? dateFrom, DateOnly? dateTo)
```

* Generates an Excel report of filtered `ScientificProjects`.
* Supports Arabic and English localization for report content.
* Returns Excel file as `FileContentResult`.

---

### 12. SendOtp

```csharp
[IgnoreAction]
[NoLogging]
[HttpPost]
public async Task<IActionResult> SendOtp(int id, string role)
```

* Sends OTP to user for specified role (`trainer` or `manager`) via `_service`.

### 13. ValidateOtp

```csharp
[IgnoreAction]
[HttpPost]
public async Task<IActionResult> ValidateOtp([FromBody] OtpValidationRequest request)
```

* Validates OTP for given `Id` and `role`.
* Returns JSON response with success status and message.

## ⚡ Notes

* File uploads are stored in `/wwwroot/uploads/Admin/ScientificProjects/`.
* Date filtering and search are applied dynamically in queries.
* Ajax support for partial views in `Index`.
* Uses `UnitOfWork` for all database interactions.
* Uses `ScientificProjectsVM` ViewModel for all forms and detail views.
* Supports OTP verification workflow for project approvals or confirmations.
* OTP Roles `1(Trainer)` --> `2(activityMonitor)` --> `3(Manager)` Arranged


```
```
