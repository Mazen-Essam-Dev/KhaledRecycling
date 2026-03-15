# 📄 EmployeeAttachmentsDTO.cs

## Namespace

```csharp
Domain.DTOs.Admin.Employees
```

## Description

`EmployeeAttachmentsDTO` is a Data Transfer Object (DTO) used to transfer employee attachment data between layers (typically from service to controller or API). It encapsulates the `EmployeeId` and a list of associated attachments.

---

## Class: `EmployeeAttachmentsDTO`

### Properties

| Property      | Type                   | Description                                                                                                                                           |
| ------------- | ---------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------- |
| `EmployeeId`  | `int`                  | The unique identifier of the employee to whom the attachments belong.                                                                                 |
| `Attachments` | `List<AttachmentDTO>?` | A list of attachments associated with the employee. Each attachment is represented by `AttachmentDTO`. This list can be null if no attachments exist. |

### Usage Example

```csharp
var employeeAttachments = new EmployeeAttachmentsDTO
{
    EmployeeId = 123,
    Attachments = new List<AttachmentDTO>
    {
        new AttachmentDTO { Name = "Resume", Path = "files/resume.pdf" },
        new AttachmentDTO { Name = "Certificate", Path = "files/certificate.pdf" }
    }
};
```

---

### Notes

* This DTO is designed to be used in service methods like uploading or retrieving employee attachments.
* It works together with `AttachmentDTO` to represent each file associated with an employee.
