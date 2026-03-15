# 📄 AttachmentVM.cs

## 📦 Namespace
`FougeraClub.Areas.Admin.ViewModels.Employees`

## 🧩 Class: AttachmentVM
This ViewModel is used in the Admin area to represent individual file attachments for an employee.

### 🔹 Properties

| Property | Type | Description |
|----------|------|-------------|
| `Id` | `int` | The unique identifier of the attachment. |
| `Name` | `string?` | The display name of the attachment. |
| `Path` | `string?` | The file path of the attachment (max length 300). |
| `File` | `IFormFile?` | The uploaded file object. Null if no file is uploaded. |

### 📌 Usage
- Used to handle file uploads and display attachment details in the UI.
- Works with `EmployeeAttachmentsVM` to manage multiple attachments for a single employee.

### 💡 Notes
- `File` is used during upload and is not persisted in the database directly.
- `Path` stores the location where the file is saved.
