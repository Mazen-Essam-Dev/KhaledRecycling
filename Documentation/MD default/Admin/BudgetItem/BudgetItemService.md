# BudgetItemService Documentation

## Overview
Service layer implementation for managing budget items with automatic code generation and unique number validation.

## Dependencies
- **IUnitOfWork**: Database repository access and transaction management

## Core CRUD Operations

### GetAllAsync()
- **Purpose**: Retrieves all budget item records
- **Returns**: `IEnumerable<BudgetItem>`
- **Implementation**: Direct delegation to repository

### GetByIdAsync(int id)
- **Purpose**: Fetches specific budget item by identifier
- **Returns**: `BudgetItem?` (nullable for not found cases)
- **Implementation**: Repository-based ID lookup with predicate

### AddAsync(BudgetItem entity)
- **Purpose**: Creates new budget item with unique number validation
- **Unique Number Validation**:
  - Checks for existing ItemNumber in database
  - Auto-generates new code if duplicate detected
- **Process**:
  1. ItemNumber uniqueness check
  2. Automatic code generation if needed
  3. Entity addition to repository
  4. Transaction commit
- **Returns**: Integer ID of created entity

### UpdateAsync(BudgetItem entity)
- **Purpose**: Modifies existing budget item
- **Process**:
  1. Fetches existing entity for existence check
  2. Updates values using repository helper method
  3. Commits changes to database
- **Safety**: Null-check prevents errors on missing entities

### DeleteAsync(int id)
- **Purpose**: Removes budget item record
- **Error Handling**: Try-catch with boolean result
- **Process**:
  1. Fetches entity for existence check
  2. Performs deletion
  3. Commits transaction
- **Returns**: Boolean indicating success/failure

## Code Generation System

### GenerateNewCode()
- **Purpose**: Creates unique sequential item numbers
- **Logic**:
  - Finds maximum existing ItemNumber
  - Increments by 1 for new code
  - Fallback to 1000 if no records exist or error occurs
- **Error Handling**: Exception fallback to base code 1000
- **Returns**: Next available integer code

### Number Validation in Add
```csharp
// Prevent duplicate item numbers
var itemNumberExists = _unitOfWork.BudgetItems.Table.Any(x => x.ItemNumber == entity.ItemNumber);
if (itemNumberExists)
{
    entity.ItemNumber = await GenerateNewCode();
}