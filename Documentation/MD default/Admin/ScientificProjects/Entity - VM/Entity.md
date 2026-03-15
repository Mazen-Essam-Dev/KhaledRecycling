# ScientificProjects Entity Documentation

## 📂 Namespace
`Domain.Entities`

## 📄 Description
The `ScientificProjects` class represents a scientific project within the domain model. It includes details about the project such as serial code, names, idea owners, project elements, participants, supervisors, related files, and approval signatures. This entity also maintains relationships with the `Department` and `Signature` entities.

---

## 🔑 Properties

| Property | Type | Attributes | Description |
|----------|------|------------|-------------|
| `Id` | `int` | `[Key]` | Primary key of the project. |
| `SerialCode` | `string?` | `[MaxLength(50)]` | Optional serial code of the project. |
| `DateByCalander` | `DateOnly?` | — | Optional date of the project according to the calendar. |
| `DepartmentId` | `int?` | `[ForeignKey(nameof(Department))]` | Optional foreign key to the related `Department`. |
| `ProjectNameAr` | `string?` | `[MaxLength(200)]` | Project name in Arabic. |
| `ProjectNameEn` | `string?` | `[MaxLength(200)]` | Project name in English. |
| `IdeaOwnerAr` | `string?` | `[MaxLength(200)]` | Name of the idea owner in Arabic. |
| `IdeaOwnerEn` | `string?` | `[MaxLength(200)]` | Name of the idea owner in English. |
| `ProjectIdeaAr` | `string?` | — | Description of the project idea in Arabic. |
| `ProjectIdeaEN` | `string?` | — | Description of the project idea in English. |
| `ProjectElement1`–`ProjectElement8` | `string?` | `[MaxLength(200)]` | Optional elements/components of the project (up to 8). |
| `InstallationRecommendations` | `string?` | — | Recommendations for project installation. |
| `DeliveryData` | `string?` | — | Data related to project delivery. |
| `Participant1`–`Participant4` | `string?` | `[MaxLength(200)]` | Names of project participants. |
| `Supervisor1`–`Supervisor6` | `string?` | `[MaxLength(200)]` | Names of project supervisors. |
| `ExpectedCost` | `double?` | — | Expected cost of the project. |
| `FilePath1`–`FilePath6` | `string?` | `[MaxLength(200)]` | File paths associated with the project. |
| `Department` | `Department?` | `virtual` | Navigation property to the related `Department` entity. |
| `Notes` | `string?` | `[MaxLength(500)]` | Additional notes about the project. |
#### Sign Information
| `TrainerSignatureId` | int? | Signature ID of the Trainer who approved |
| `TrainerSignature` | Signature | Trainer's signature details |
| `ActivitySupervisorSignatureId` | int? | Signature ID of the ActivitySupervisor who approved |
| `ActivitySupervisorSignature` | Signature | ActivitySupervisor's signature details |
| `ManagerSignatureId` | int? | Signature ID of the manager who approved |
| `ManagerSignature` | Signature | Manager's signature details |

---

## 🏗 Relationships

- **Department**: Each `ScientificProjects` may belong to a single `Department`.
- **Signatures**: Tracks approval or monitoring through `ActivityMonitorSigniture` and `ManagerSignature` references.

---

## 📝 Notes

- The entity makes heavy use of nullable types (`?`) to allow optional data fields.
- `MaxLength` annotations enforce string length constraints in the database.
- `ForeignKey` attributes define relationships with other entities to maintain referential integrity.
- The class uses `virtual` navigation properties to support Entity Framework lazy loading.

---

## ⚡ Usage Example

```csharp
var project = new ScientificProjects
{
    SerialCode = "SP-2025-001",
    ProjectNameAr = "مشروع علمي",
    ProjectNameEn = "Scientific Project",
    IdeaOwnerAr = "أحمد محمد",
    IdeaOwnerEn = "Ahmed Mohamed",
    DepartmentId = 1,
    ExpectedCost = 5000.0,
    Notes = "Initial project notes."
};
