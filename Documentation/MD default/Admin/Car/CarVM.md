# 📄 CarVM.cs

## 📦 Namespace
`FougeraClub.Areas.Admin.ViewModels.Cars`

## 🧩 Class: CarVM
This ViewModel represents a car in the Admin area. It is used for displaying, validating, and managing car information, including attachments and ownership expiry dates.

### 🔹 Properties

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Unique identifier of the car. |
| `PlateNumber` | `string?` | Car plate number. Required and limited to 20 characters. |
| `Type` | `string?` | Type of the car (e.g., Sedan, SUV). Required and limited to 100 characters. |
| `DriverName` | `string?` | Name of the driver assigned to the car. Required and limited to 100 characters. |
| `OwnershipExpiryDate` | `DateOnly?` | Expiration date of the car ownership. Required and stored as `date` type in DB. |
| `Model` | `string?` | Car model. Required and limited to 100 characters. |
| `Color` | `string?` | Car color. Required and limited to 100 characters. |
| `Notes` | `string?` | Additional notes about the car. |
| `AttachmentPath` | `string?` | Path to an uploaded attachment (e.g., registration document). Max length 255. |
| `CarAttachments` | `ICollection<CarAttachment>` | Uploaded attachments files. ICollection<CarAttachment>. |
| `inService` | `bool?` | Indicates whether the car is currently in service. |

### 🔹 Validation
- Implements `IValidatableObject` for custom validation.
- Checks if `OwnershipExpiryDate` has expired.
- Provides localized error messages depending on the current language (`ar` or `en`).

### 📌 Usage
- Used in forms for adding, editing, or displaying car information in the admin panel.
- Validates critical data like ownership expiry date and required fields.
- Handles file uploads for car-related documents.

### 💡 Notes
- `Attachment` is only used for file uploads; `AttachmentPath` stores the location of the saved file.
- `OwnershipExpiryDate` is validated against the current date using Dubai time (`AppDubaiTime.Now.AtMidnight()`).
