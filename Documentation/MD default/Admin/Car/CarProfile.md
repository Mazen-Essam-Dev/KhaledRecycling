# 📄 CarProfile.cs

## 📦 Namespace
`FougeraClub.Areas.Admin.Mappings`

## 🧩 Class: CarProfile
An AutoMapper profile that defines mappings between `CarVM` (ViewModel) and `Car` (Entity).
An AutoMapper profile that defines mappings between `CarAttachment` (ViewModel) and `CarAttachmentDTO` (Entity).
An AutoMapper profile that defines mappings between `CarAttachmentDTO` (ViewModel) and `CarAttachmentsVM` (Entity).
An AutoMapper profile that defines mappings between `CarAttachmentsVM` (ViewModel) and `CarAttachment` (Entity).

### 🔹 Base Class
- Inherits from `Profile` (AutoMapper).

### 🔹 Constructor: `CarProfile()`
Defines the mapping rules between `CarVM` and `Car`.

### 🔹 Mappings
| Source | Destination | Notes |
|--------|-------------|-------|
| `CarVM` | `Car` | Maps all properties except `AttachmentPath`, which is ignored. This is because `AttachmentPath` is set manually when a file is uploaded. |
| `Car` | `CarVM` | Maps all properties automatically from entity to ViewModel. |
| `CarAttachment` | `CarAttachmentDTO` | Maps all properties automatically from entity to DTO And Revers. |
| `CarAttachmentDTO` | `CarAttachmentsVM` | Maps all properties automatically from DTO to ViewModel And Revers. |
| `CarAttachmentsVM` | `CarAttachment` | Maps all properties automatically from entity to ViewModel And Revers. |

### 📌 Usage
- Enables automatic conversion between `Car` entities and `CarVM` ViewModels.
- Used in ASP.NET Core with AutoMapper for simplifying data transformations between layers.
- Ensures that image paths are handled explicitly rather than automatically mapped.
