# SupplierService Documentation

## Overview
Service layer implementation for managing suppliers with advanced data integrity checks, file attachment handling, and comprehensive relationship tracking.

## Dependencies
- **IUnitOfWork**: Database repository access and transaction management
- **IWebHostEnvironment**: File system access for supplier attachments

## Core Supplier Operations

### GetAllAsync()
- **Purpose**: Retrieves all supplier records with relationship status tracking
- **Advanced Features**:
  - **HasRelatedData Property**: Dynamically checks if supplier has related records
  - **Multi-Entity Checking**: Verifies relationships across Expenses, Receipts, and PurchaseOrders
  - **Performance Optimized**: Uses AsNoTracking() for read-only operations
- **Returns**: `IEnumerable<Supplier>` with relationship status

### GetByIdAsync(int id)
- **Purpose**: Fetches specific supplier by identifier
- **Returns**: `Supplier?` entity

### AddAsync(Supplier entity)
- **Purpose**: Creates new supplier
- **Process**: Entity addition with transaction commit
- **Returns**: Integer ID of created supplier

### UpdateAsync(Supplier entity)
- **Purpose**: Modifies existing supplier information
- **Process**: Entity fetch → Value update → Transaction commit
- **Safety**: Null-check prevents errors on missing entities

### DeleteAsync(int id)
- **Purpose**: Removes supplier record
- **Process**: Entity fetch → Database removal → Transaction commit
- **Safety**: Null-check prevents deletion attempts on non-existent entities

## Advanced Data Integrity Features

### Relationship Status Tracking
```csharp
HasRelatedData =
    context.Expenses.Any(e => e.SupplierId == s.Id) ||
    context.Receipts.Any(r => r.SupplierId == s.Id) ||
    context.PurchaseOrders.Any(po => po.SupplierId == s.Id)