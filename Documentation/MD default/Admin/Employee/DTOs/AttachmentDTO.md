# 📄 AttachmentDTO.cs Documentation

## 📂 Namespace

```csharp
Domain.DTOs.Admin.Employees
```

## 🧩 Class: `AttachmentDTO`

Represents a data transfer object (DTO) for a single employee attachment.
Used to transfer attachment data between the service, controller, and view layers.

### Properties

| Property | Type         | Description                                                                                    |
| -------- | ------------ | ---------------------------------------------------------------------------------------------- |
| `Id`     | `int`        | Unique identifier of the attachment.                                                           |
| `Name`   | `string?`    | Name or title of the attachment. Nullable because a name may not be required.                  |
| `Path`   | `string?`    | Path to the saved file. Maximum length: 300 characters. Nullable if the file is not yet saved. |
| `File`   | `IFormFile?` | The actual uploaded file. Nullable because it may not always be provided.                      |

### Notes

* Used when uploading, updating, or displaying employee attachments.
* Supports both metadata (`Name` and `Path`) and the actual file (`IFormFile`) for upload.

### Example Usage

```csharp
var attachment = new AttachmentDTO
{
    Name = "Passport Copy",
    File = uploadedFile
};
```

### Purpose

* Encapsulates the information for a single file attachment.
* Can be used individually or as part of `EmployeeAttachmentsDTO` for managing multiple attachments.
