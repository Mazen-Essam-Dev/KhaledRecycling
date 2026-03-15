# ScientificProjectsVM ViewModel Documentation

## 📂 Namespace
`FougeraClub.Areas.Admin.ViewModels.ScientificProject`

## 📄 Description
The `ScientificProjectsVM` class is a ViewModel for handling scientific projects in the admin area. It is designed to support both the index/listing page and create/edit forms. The ViewModel includes filtering, pagination, project details, validation rules, file uploads, and navigation properties.

---

## 🔑 Properties

### Index/List Page Properties
| Property | Type | Description |
|----------|------|-------------|
| `StartDate` | `DateOnly?` | Filter start date for listing projects. |
| `EndDate` | `DateOnly?` | Filter end date for listing projects. |
| `all_ScientificProjectListVM` | `IEnumerable<ScientificProjects>?` | Collection of projects for display. |
| `SearchString` | `string?` | Text search filter. |
| `CurrentPage` | `int` | Current page number in pagination. |
| `TotalPages` | `int` | Total number of pages available. |
| `TotalCount` | `int?` | Total count of projects. |
| `PageSize` | `int` | Number of projects per page. |

### Project Details Properties
| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| `CurrentDate` | `string?` | — | Current date string (optional display). |
| `Id` | `int?` | — | Project ID. |
| `SerialCode` | `string?` | `[MaxLength(50)]` | Project serial code. |
| `DateByCalander` | `DateOnly?` | — | Project date. |
| `DepartmentId` | `int?` | `[Required]` | Foreign key to department (required). |
| `all_Departments` | `IEnumerable<Department>?` | — | List of departments for dropdowns. |
| `SingleDepartment` | `Department?` | — | Selected department navigation property. |
| `ProjectNameAr` | `string?` | `[Required, MaxLength(200), RegularExpression(@"^[\u0621-\u064A0-9 ]+$")]` | Project name in Arabic, must contain Arabic letters and numbers. |
| `ProjectNameEn` | `string?` | `[Required, MaxLength(200), RegularExpression(@"^[a-zA-Z0-9 ]+$")]` | Project name in English, must contain English letters and numbers. |
| `IdeaOwnerAr` | `string?` | `[Required, MaxLength(200), RegularExpression(@"^[\u0621-\u064A0-9 ]+$")]` | Idea owner in Arabic. |
| `IdeaOwnerEn` | `string?` | `[Required, MaxLength(200), RegularExpression(@"^[a-zA-Z0-9 ]+$")]` | Idea owner in English. |
| `ProjectIdeaAr` | `string?` | `[Required, MaxLength(500), RegularExpression(@"^[\u0621-\u064A0-9 ]+$")]` | Project idea description in Arabic. |
| `ProjectIdeaEN` | `string?` | `[Required, MaxLength(500), RegularExpression(@"^[a-zA-Z0-9 ]+$")]` | Project idea description in English. |
| `ProjectElement1`–`ProjectElement8` | `string?` | `[MaxLength(200)]` | Optional project elements. |
| `InstallationRecommendations` | `string?` | `[Required, MaxLength(500)]` | Recommendations for installation. |
| `DeliveryData` | `string?` | `[Required, MaxLength(500)]` | Delivery-related data. |
| `Participant1`–`Participant4` | `string?` | `[MaxLength(200)]` | Participants’ names. |
| `Supervisor1`–`Supervisor6` | `string?` | `[MaxLength(200)]` | Supervisors’ names. |
| `ExpectedCost` | `double?` | `[Required]` | Expected project cost. |
| `File1`–`File6` | `IFormFile?` | — | File uploads for the project. |
| `FilePath1`–`FilePath6` | `string?` | — | File path storage for uploaded files. |
| `Notes` | `string?` | `[MaxLength(500)]` | Optional notes about the project. |
#### Sign Information
| `TrainerSignatureId` | int? | Signature ID of the Trainer who approved |
| `TrainerSignature` | Signature | Trainer's signature details |
| `ActivitySupervisorSignatureId` | int? | Signature ID of the ActivitySupervisor who approved |
| `ActivitySupervisorSignature` | Signature | ActivitySupervisor's signature details |
| `ManagerSignatureId` | int? | Signature ID of the manager who approved |
| `ManagerSignature` | Signature | Manager's signature details |


---

## 🏗 Relationships

- **Department**: Supports dropdown selection and navigation via `DepartmentId` and `SingleDepartment`.
- **Signatures**: Tracks approval via `ActivityMonitorSigniture` and `ManagerSignature`.

---

## 📝 Validation

- Uses `[Required]` to enforce mandatory fields.
- `[MaxLength]` enforces maximum string lengths.
- `[RegularExpression]` ensures content matches language-specific or alphanumeric requirements.
- Error messages are localized using `Resource1`.

---

## ⚡ Usage Example

```csharp
var viewModel = new ScientificProjectsVM
{
    ProjectNameAr = "مشروع علمي",
    ProjectNameEn = "Scientific Project",
    IdeaOwnerAr = "أحمد محمد",
    IdeaOwnerEn = "Ahmed Mohamed",
    DepartmentId = 1,
    ExpectedCost = 10000,
    StartDate = DateOnly.FromDateTime(DateTime.Now),
    EndDate = DateOnly.FromDateTime(DateTime.Now.AddMonths(1)),
    PageSize = 10,
    CurrentPage = 1
};
