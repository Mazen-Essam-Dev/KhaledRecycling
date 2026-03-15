# 📄 EmployeeAttachmentsVM.cs

## 📦 Namespace
`FougeraClub.Areas.Admin.ViewModels.Employees`

## 🧩 Class: EmployeeAttachmentsVM
This ViewModel is used in the Admin area to manage employee attachments in the UI.  
It acts as a container for an employee's attachment list and the associated employee ID.

### 🔹 Properties

| Property | Type | Description |
|----------|------|-------------|
| `EmployeeId` | `int` | The unique identifier of the employee. |
| `Attachments` | `List<AttachmentVM>?` | A list of attachment view models associated with the employee. Can be `null` if no attachments exist. |

### 📌 Usage
- Used to display and manage a collection of attachments for a specific employee.
- Typically sent to views or partial views in the Admin area for upload or display operations.

### 💡 Notes
- This is a simple DTO/ViewModel; it does not contain any methods or business logic.
- The `Attachments` list references `AttachmentVM`, which represents individual file attachments.
