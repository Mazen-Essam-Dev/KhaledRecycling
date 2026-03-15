# 🗂 **EmployeeProfile Mapping Documentation**

## 📁 **Namespace**
`FougeraClub.Areas.Admin.Mappings`

---

## ⚙️ **Dependencies**
```csharp
using AutoMapper;
using Domain.DTOs.Admin.Employees;
using Domain.Entities.Employees;
using FougeraClub.Areas.Admin.ViewModels.Employees;
```

---

## 🧩 **Class Overview**
**EmployeeProfile** is an **AutoMapper Profile** class that defines the mappings between **Employee-related ViewModels and Entities/DTOs**.

It is used to automatically convert objects between layers in the application (ViewModel ↔ Entity ↔ DTO) to simplify data transfer and reduce manual mapping code.

**Key Roles:**
- Handles Employee entity mapping for CRUD operations.
- Manages attachment-related mappings between Employee entities and DTOs/ViewModels.

---

## 🔑 **Constructor**
```csharp
public EmployeeProfile()
```
**Purpose:**  
Defines all the mapping configurations for AutoMapper.

---

## 📜 **Mapping Configurations**

| Source | Destination | Direction |
|--------|------------|-----------|
| `EmployeeVM` | `Employee` | `ReverseMap()` – supports both ways |
| `EmployeeAttachmentsVM` | `EmployeeAttachmentsDTO` | `ReverseMap()` – supports both ways |
| `AttachmentVM` | `AttachmentDTO` | `ReverseMap()` – supports both ways |
| `EmployeeAttachment` | `AttachmentVM` | `ReverseMap()` – supports both ways |
| `EmployeeAttachment` | `EmployeeAttachmentsVM` | `ReverseMap()` – supports both ways |

**Explanation:**  
- `ReverseMap()` allows mapping **in both directions**, e.g., from `EmployeeVM` → `Employee` and `Employee` → `EmployeeVM`.  
- Separates DTOs from ViewModels to ensure proper data transfer between layers.  
- Simplifies controller and service logic by avoiding manual property assignments.

---

## 🧭 **Summary**
- Central place for all Employee-related mappings.  
- Ensures consistency and maintainability across the Admin module.  
- Supports attachments mapping between entities and DTOs/ViewModels.  

---

📚 **Author:** *FougeraClub Admin Team*  
🕒 **Last Updated:** *November 2025*  
🏗 **Framework:** ASP.NET Core + AutoMapper
