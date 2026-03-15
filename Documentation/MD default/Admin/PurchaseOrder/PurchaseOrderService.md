# PurchaseOrderService Documentation

## Overview
C# service class implementing purchase order management with automatic calculations, OTP verification, and supplier integration in an ASP.NET Core application.

## Service Configuration
- **Namespace**: Application.Services.Admin
- **Interface**: IPurchaseOrderService
- **Dependencies**:
  - IUnitOfWork: Database operations and repository management
  - IWebHostEnvironment: Web hosting environment information
  - IHttpContextAccessor: HTTP context and user information access

## Core Purchase Order Operations

### Data Retrieval Methods
- **GetAllAsync()**: Retrieves all purchase orders with signature information
- **GetByIdAsync(long id)**: Gets specific purchase order by ID including items and signature
- **GetAllSuppliersAsync()**: Fetches supplier list with Arabic/English names and IDs
- **GetAllYearsInDb()**: Returns distinct years from all purchase order dates

### Purchase Order Management
- **AddAsync(PurchaseOrder entity)**: Creates new purchase order with automatic total calculations
- **UpdateAsync(PurchaseOrder entity)**: Updates existing order with complete item replacement
- **DeleteAsync(long id)**: Removes purchase order and associated items

## Auto-Code Generation

### GetNewCodeAsync
- **Purpose**: Generates sequential purchase order codes
- **Logic**: Increments last used code or starts from 188
- **Collision Detection**: Checks for existing codes before assignment

```csharp
// Example: 188 → 189 → 190
var lastId = await _unitOfWork.PurchaseOrders.Table.Select(x => (long?)x.Id).MaxAsync() ?? 0;
return (numericCode + 1).ToString();