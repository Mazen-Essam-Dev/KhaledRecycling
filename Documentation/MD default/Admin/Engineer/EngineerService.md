## 🧰 Service: EngineerService
✅ Packages Used:
```
using Application.Interfaces;
using Application.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
```
✅ Method:
```
| Method                      | Description                                 |
| --------------------------- | ------------------------------------------- |
GetAllAsync() – Gets all engineers with nationalities.

GetByIdAsync(id) – Fetches specific engineer + nationality.

AddAsync(entity, file) – Saves image (if provided), stores engineer.

UpdateAsync(entity, file) – Updates details, replaces image.

DeleteAsync(id) – Deletes record and related image.

SaveImageAsync(file) – Saves image to /uploads/engineers.

DeleteImageFile(path) – Deletes image from disk.

GenerateNewCode() – Generates sequential Code.
```
