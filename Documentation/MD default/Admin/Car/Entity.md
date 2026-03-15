# 📄 Car.cs

## 📦 Namespace
`Domain.Entities`

## 🧩 Class: Car
Represents the `Car` entity in the domain model. This class is used to persist car information in the database.

### 🔹 Properties

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Primary key of the car entity. |
| `PlateNumber` | `string?` | Car plate number. Maximum length 20 characters. |
| `Type` | `string?` | Type of the car (e.g., Sedan, SUV). Maximum length 100 characters. |
| `DriverName` | `string?` | Name of the driver assigned to the car. Maximum length 100 characters. |
| `OwnershipExpiryDate` | `DateOnly?` | Expiration date of car ownership. Stored as `date` type in the database. |
| `Model` | `string?` | Car model. Maximum length 100 characters. |
| `Color` | `string?` | Car color. Maximum length 100 characters. |
| `Notes` | `string?` | Additional notes about the car. |
| `CarAttachments` | `ICollection<CarAttachment>` | Uploaded attachments files. ICollection<CarAttachment>. Maximum 
length 255 characters. |


||- CarAttachment -||
| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Primary key of the car entity. |
| `Name` | `string` |Name Entered From Modal. |
| `File` | `IFormFile?` | Store File Path |
| `CarId` | `int` | Forign key from Cars Table |
| `Car` | `Car` | Navigation property |


### 📌 Usage
- Represents cars in the domain layer.
- Used by the database context to persist and query car information.
- Serves as the source entity for mapping to ViewModels like `CarVM` for the UI.

### 💡 Notes
- No business logic or validation is included in the entity; validation is handled in the corresponding ViewModel (`CarVM`) or service layer.
- `CarAttachments` stores files (path , Name); actual file management is done outside this entity.
