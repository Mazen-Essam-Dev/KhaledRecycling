# 📄 CarServiceManager.cs

## 📦 Namespace
`Application.Services.Admin`

## 🧩 Class: CarServiceManager
Manages CRUD operations for `CarServiceEntity` and handles file attachments for car service records.

### 🔹 Dependencies
| Dependency | Purpose |
|------------|---------|
| `IUnitOfWork _unitOfWork` | Provides repository access to `Cars` and `CarServices` tables. |
| `string FileName` | Folder name for saving attachments: `"CarServices"`. |

### 🔹 Constructor
`CarServiceManager(IUnitOfWork unitOfWork)`
- Injects `IUnitOfWork` for database operations.

---

## 🔹 Methods

### 1. `GetAllAsync() : Task<IEnumerable<CarServiceEntity>>`
- Returns all car service records including the related `Car` entity.

### 2. `GetByIdAsync(int id) : Task<CarServiceEntity?>`
- Returns a single car service record by `id`.
- Includes the related `Car`.

### 3. `AddAsync(CarServiceEntity entity, IFormFile? file) : Task<(int, int)>`
- Adds a new car service record.
- Saves attached file (if any) using `FileHelper.SaveImageAsync`.
- Updates related `Car` entity (currently placeholder logic for modifications).
- Returns a tuple: `(CarServiceEntity.Id, CarServiceEntity.CarId)`.

### 4. `UpdateAsync(CarServiceEntity entity, IFormFile? file)`
- Updates an existing car service record.
- Deletes old attachment file if a new file is uploaded.
- Keeps the old attachment if no new file is provided.
- Calls `_unitOfWork.CarServices.UpdateValues` to update entity values.
- Commits changes via `_unitOfWork.CompleteAsync()`.

### 5. `DeleteAsync(int id)`
- Deletes a car service record by `id`.
- Deletes attached file from disk if it exists.
- Calls `_unitOfWork.CarServices.Delete` and commits changes.

---

## 🔹 Notes
- File attachments are handled through `FileHelper` (save/delete).
- Relies on `IUnitOfWork` for all database operations, maintaining unit-of-work pattern.
- Ensures attachment integrity by updating or removing files in sync with database changes.
