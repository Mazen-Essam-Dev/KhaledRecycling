# 📄 CarServiceEntity.cs

## 📦 Namespace
`Domain.Entities`

## 🧩 Class: CarServiceEntity
Represents a car service record, including service details, date, and optional attachment.

### 🔹 Properties

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | Primary key of the car service record. |
| `CarId` | `int?` | Foreign key referencing the related `Car`. Nullable to allow optional association. |
| `Car` | `Car?` | Navigation property to the related `Car` entity. |
| `Date` | `DateOnly?` | Date of the car service. Stored as SQL `date` type. |
| `Details` | `string?` | Additional description or notes about the service. |
| `AttachmentPath` | `string?` | File path of an optional attachment (e.g., service report), max length 255 characters. |

### 🔹 Notes
- Uses `ForeignKey` attribute to link `CarId` with the `Car` entity.
- Attachment files are managed externally via services (e.g., saving/deleting in `CarServiceManager`).
- Nullable properties allow partial records and flexibility in data entry.
