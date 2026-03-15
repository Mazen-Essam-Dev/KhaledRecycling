# 📄 EmployeeService.cs

## Namespace & Dependencies

```csharp
using Application.Helpers;
using Application.Interfaces.Admin;
using Domain.DTOs.Admin.Employees;
using Domain.Entities.Employees;
using Infrastructure.Repositories.InterfacesDB;
```

## Overview

The `EmployeeService` class implements the `IEmployeeService` interface and provides CRUD operations and attachment management for `Employee` entities.
It uses `IUnitOfWork` for database operations and `FileHelper` for image/file handling.

---

## Class: EmployeeService

```csharp
public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string FileName = "EmployeesAttachments";

    public EmployeeService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
```

### Methods

#### 1. `HasRelatedObjectsInDb(int empId)`

Checks if an employee has related records in other tables like `ExternalWorkMissions`, `SalaryManagements`, or `EmployeeAttachments`.

```csharp
public async Task<bool> HasRelatedObjectsInDb(int empId)
```

#### 2. `GetAllAsync()`

Returns all employees including their `Nationality`.

```csharp
public async Task<IEnumerable<Employee>> GetAllAsync()
```

#### 3. `GetByIdAsync(int id)`

Returns a single employee by ID.

```csharp
public async Task<Employee?> GetByIdAsync(int id)
```

#### 4. `AddAsync(Employee entity)`

Adds a new employee. Saves the employee photo if provided.

```csharp
public async Task<int> AddAsync(Employee entity)
```

#### 5. `UpdateAsync(Employee entity)`

Updates an existing employee. Handles image replacement if a new photo is uploaded.

```csharp
public async Task UpdateAsync(Employee entity)
```

#### 6. `DeleteAsync(int id)`

Deletes an employee by ID and removes the associated photo file from disk.

```csharp
public async Task DeleteAsync(int id)
```

#### 7. `UploadAttachmentsAsync(EmployeeAttachmentsDTO model)`

Handles employee attachments:

* Deletes old files that are removed from the list.
* Saves new uploaded files.

```csharp
public async Task UploadAttachmentsAsync(EmployeeAttachmentsDTO model)
```

#### 8. `GetAttachmentsAsync(int empId)`

Retrieves all attachments for a specific employee.

```csharp
public async Task<IEnumerable<EmployeeAttachment>> GetAttachmentsAsync(int empId)
```

---

### Notes

* Uses `FileHelper` to save, check, and delete image files.
* Uses `IUnitOfWork` repository pattern for database operations.
* Ensures old files are cleaned up when attachments are updated.
