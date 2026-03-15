# 📄 CarService.cs

## 📦 Namespace
`Application.Services.Admin`

## 🧩 Class: CarService
Service class responsible for handling business logic and data operations for `Car` entities. Implements `ICarService` interface.

### 🔹 Dependencies
- `IUnitOfWork _unitOfWork`: Repository abstraction for accessing database tables.
- `string FileName`: Folder name for storing car attachments (`Cars`).

### 🔹 Methods

| Method | Parameters | Return Type | Description |
|--------|------------|-------------|-------------|
| `GetAllAsync()` | None | `Task<IEnumerable<Car>>` | Retrieves all cars from the database. |
| `GetByIdAsync(int id)` | `int id` | `Task<Car?>` | Retrieves a specific car by its `Id`. Returns `null` if not found. |
| `AddAsync(Car entity, IFormFile? file)` | `Car entity`, `IFormFile? file` | `Task<int>` | Adds a new car to the database. If a file is provided, saves it using `FileHelper` and sets `AttachmentPath`. Returns the new car's Id. |
| `UpdateAsync(Car entity, IFormFile? file)` | `Car entity`, `IFormFile? file` | `Task` | Updates an existing car. If a new file is provided, deletes the old file and saves the new one. If no file is uploaded, keeps the old attachment. |
| `DeleteAsync(int id)` | `int id` | `Task` | Deletes a car by its Id. Also deletes the associated attachment file from disk. |
| `UploadAttachmentsAsync(CarAttachmentsDTO model)` | `CarAttachmentsDTO model` | `Task` | Save All uploads Attachments related to this car in Attachment table. |
| `GetAttachmentsAsync(int carId)` | `int carId` | `Task<CarAttachmentsDTO>` | Retrieves a specific car All Attachments related to it. Returns null if not found.|

### 📌 Usage
- Provides CRUD operations for `Car` entities.
- Handles file management (saving and deleting attachments) transparently.
- Works with the UnitOfWork pattern to ensure database transactions are committed.

### 💡 Notes
- File management is handled via `FileHelper`.
- Ensures that existing files are deleted when updating or removing a car.
- Designed to work with dependency injection in ASP.NET Core applications.
