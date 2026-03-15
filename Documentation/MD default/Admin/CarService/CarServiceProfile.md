# 📄 CarServiceProfile.cs

## 📦 Namespace
`FougeraClub.Areas.Admin.Mappings`

## 🧩 Class: CarServiceProfile
AutoMapper profile for mapping between `CarServiceVM` and `CarServiceEntity`.

### 🔹 Purpose
Defines how the `CarServiceVM` (ViewModel) and `CarServiceEntity` (Domain/Database entity) are mapped to each other for CRUD operations.  
Handles conversion for all properties except the `AttachmentPath`, which is managed manually when files are uploaded.

### 🔹 Mappings

| Source | Destination | Notes |
|--------|------------|-------|
| `CarServiceVM` | `CarServiceEntity` | `AttachmentPath` is ignored and must be set manually in service layer. |
| `CarServiceEntity` | `CarServiceVM` | Standard property mapping for displaying data in forms. |

### 🔹 Usage
- Automatically used by AutoMapper when registered in DI container.
- Ensures consistency between view models and database entities.
- Simplifies controller code for Add/Edit operations by handling property mapping automatically.
