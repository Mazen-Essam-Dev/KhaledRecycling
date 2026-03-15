# 📄 CarServiceVM.cs

## 📦 Namespace
`FougeraClub.Areas.Admin.ViewModels.CarServices`

## 🧩 Class: CarServiceVM
Represents the ViewModel for creating or editing a car service record in the Admin area.  
Includes data for binding to forms, validations, and optional file attachment.

### 🔹 Properties

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Primary key of the car service record (used for editing). |
| `CarId` | `int` | Foreign key reference to the related `Car`. Marked as required with localized validation. |
| `Car` | `Car?` | Navigation property to the related `Car` entity. Optional for display purposes. |
| `CarList` | `List<SelectListItem>?` | Dropdown list of cars for form selection. Initialized with empty list by default. |
| `Date` | `DateOnly?` | Date of the car service. Required and stored as SQL `date` type. |
| `Details` | `string?` | Description of the service. Required with a maximum length of 500 characters. |
| `AttachmentPath` | `string?` | Path to an optional attachment file (e.g., PDF or image), max length 255. |
| `Attachment` | `IFormFile?` | Optional uploaded file for the service record. |

### 🔹 Notes
- Uses localized validation attributes (`LocalizedRequired`, `LocalizedMaxLength`) for multi-language support.
- `CarList` is intended for populating dropdowns in Razor views.
- `Attachment` is handled in the service layer to save or delete files on disk.
