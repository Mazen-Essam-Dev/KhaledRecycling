# 📄 EmployeeVM Documentation

## **Namespace & Dependencies**

```csharp
using Domain.Entities;
using Domain.Entities.Employees;
using Domain.Resources;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
```

## **Class Definition**

```csharp
public class EmployeeVM : IValidatableObject
```

The `EmployeeVM` class is a ViewModel used for managing Employee data in the Admin area. It implements `IValidatableObject` to allow custom validation logic, such as ensuring the end date is after the start date.

---

## **Properties**

### **Index and Pagination**

* `DateOnly? StartDate` – Filter start date for index/search.
* `DateOnly? EndDate` – Filter end date for index/search.
* `IEnumerable<Employee>? all_EmployeeListVM` – List of employees for index view.
* `EmployeeAttachmentsVM? EmployeeAtachmentsVM` – Employee attachments view model.
* `string? SearchString` – Search term for filtering.
* `int CurrentPage` – Current page number for pagination.
* `int PageSize` – Number of records per page.
* `int TotalPages` – Total pages available.
* `int? TotalCount` – Total record count.
* `bool HasNextPage` – Flag if next page exists.
* `bool HasPreviousPage` – Flag if previous page exists.

---

### **Employee Identification**

* `int Id` – Employee ID.
* `int? Code` – Employee Code, required.
* `int? NationalityId` – Foreign key for nationality, required.
* `Nationality? Nationality` – Navigation property.
* `List<SelectListItem>? NationalitiesList` – Dropdown list for nationality selection.

---

### **Personal Details**

* `string? FullNameAr` – Arabic full name, required, max 100 chars.
* `string? FullNameEn` – English full name, optional, max 100 chars.
* `string? PhoneNumber` – Phone number, required, unique, max 20 chars.
* `string? Email` – Email, required, unique, max 100 chars.
* `string? Address` – Address, required, max 200 chars.
* `string? JobTitle1` – Display purpose only (optional).
* `string? Nationality1` – Display nationality (optional).

---

### **Passport & National ID**

* `string? PassportNumber` – Required, unique, max 50 chars.
* `DateOnly? PassportExpiryDate` – Required, cannot be in the past.
* `string? NationalIdNumber` – Required, unique, fixed length 18.
* `DateOnly? NationalIdExpiryDate` – Required, cannot be in the past.
* `string? NationalIdLocation` – Optional.

---

### **Banking & Salary**

* `string? BankAccountNumber` – Required, max 50 chars.
* `string? BankName` – Required, max 100 chars.
* `double? Salary` – Required.
* `string? Notes` – Optional.

---

### **Attachments & Photos**

* `IFormFile? Photo` – Upload personal photo.
* `string? PhotoPath` – Display saved photo path.
* `string? JobTitle` – Required, max 200 chars.

---

### **Extra Fields (Optional)**

* `string? g1`, `g2`, `g3`, `g4` – Custom fields for any extra data.

---

## **Validation**

* Implements `IValidatableObject.Validate` to ensure `EndDate > StartDate`.

```csharp
public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
{
    if (StartDate.HasValue && EndDate.HasValue)
    {
        if (EndDate <= StartDate)
        {
            yield return new ValidationResult(
                SessionHelper.GetCurrentLanguage() == "ar" ?
                "تاريخ النهاية يجب أن يكون بعد تاريخ البداية" :
                "The End Date must be after the Start Date",
                new[] { nameof(EndDate) }
            );
        }
    }
}
```

---

### **Data Annotations & Validation Attributes**

* `[Required]` – Required field validation.
* `[MaxLength]` – Maximum character length.
* `[EmailAddress]` – Email format validation.
* `[Unique]` – Custom attribute for checking uniqueness in DB.
* `[NotInThePast]` – Custom attribute to prevent past dates.
* `[LocalizedRequired/LocalizedMaxLength/LocalizedMinLength]` – Localized validation messages.

---

This ViewModel is designed for **CRUD operations, listing, filtering, and validation** for employees in the admin area.
