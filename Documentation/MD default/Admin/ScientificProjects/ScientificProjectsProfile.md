# ScientificProjectsProfile Mapping Documentation

## 📂 Namespace
`FougeraClub.Areas.Admin.Mappings`

## 📄 Description
The `ScientificProjectsProfile` class defines an AutoMapper profile for mapping between the `ScientificProjectsVM` ViewModel and the `ScientificProjects` entity. This allows easy transformation of data between the domain entity and the ViewModel used in the admin area, supporting both directions.

---

## 🔑 Mapping Configuration

| Source | Destination | Notes |
|--------|------------|-------|
| `ScientificProjectsVM` | `ScientificProjects` | Maps all matching properties from the ViewModel to the entity. |
| `ScientificProjects` | `ScientificProjectsVM` | Reverse mapping enabled via `.ReverseMap()`, allowing data from the entity to populate the ViewModel. |

---

## 🏗 Usage

- Enables automatic mapping when using services or controllers that work with both entities and ViewModels.
- Reduces manual property assignment and ensures consistency across the application.

### Example:

```csharp
// Mapping ViewModel to Entity
var projectEntity = _mapper.Map<ScientificProjects>(scientificProjectsVM);

// Mapping Entity to ViewModel
var projectVM = _mapper.Map<ScientificProjectsVM>(projectEntity);
