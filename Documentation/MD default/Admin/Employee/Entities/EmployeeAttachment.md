# 📄 EmployeeAttachment.cs

## 📂 Namespace & Dependencies

```csharp
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
```

## 📦 Namespace

```csharp
namespace Domain.Entities.Employees
```

## 🧩 Class: EmployeeAttachment

Represents a file attachment associated with an employee. This entity stores metadata about the attachment and optionally holds a file for upload.

### 🔹 Properties

| Property     | Type         | Description                                                  |
| ------------ | ------------ | ------------------------------------------------------------ |
| `Id`         | `int`        | Primary key for the attachment.                              |
| `Name`       | `string?`    | Name of the attachment. Maximum length: 100 characters.      |
| `Path`       | `string?`    | File path of the attachment. Maximum length: 300 characters. |
| `File`       | `IFormFile?` | Not mapped property used for uploading the file via forms.   |
| `EmployeeId` | `int`        | Foreign key referencing the associated employee.             |

### 📝 Notes

* The `File` property uses `[NotMapped]` because it is not persisted in the database; it is only used for handling uploads.
* Each `EmployeeAttachment` is linked to an employee, allowing multiple attachments per employee.
