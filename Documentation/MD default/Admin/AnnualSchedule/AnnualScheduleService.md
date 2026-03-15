# AnnualScheduleService Documentation

## Overview
Service layer implementation for managing annual schedule entities with basic CRUD operations and image file management.

## Dependencies
- **IUnitOfWork**: Database repository access and transaction management
- **IWebHostEnvironment**: File system access for image storage

## Core CRUD Operations

### GetAllAsync()
- **Purpose**: Retrieves all annual schedule records
- **Returns**: `IEnumerable<AnnualSchedule>`
- **Implementation**: Direct delegation to repository

### GetByIdAsync(int id)
- **Purpose**: Fetches specific schedule by identifier
- **Returns**: `AnnualSchedule?` (nullable for not found cases)
- **Implementation**: Repository-based ID lookup

### AddAsync(AnnualSchedule entity)
- **Purpose**: Creates new annual schedule record
- **Process**:
  1. Adds entity to repository
  2. Commits transaction
  3. Returns generated ID
- **Returns**: Integer ID of created entity

### UpdateAsync(AnnualSchedule entity)
- **Purpose**: Modifies existing schedule record
- **Process**:
  1. Verifies entity existence
  2. Updates values using repository helper
  3. Commits changes to database
- **Safety**: Null-check prevents errors on missing entities

### DeleteAsync(int id)
- **Purpose**: Removes schedule record by ID
- **Process**:
  1. Fetches entity for existence check
  2. Performs soft/hard delete based on repository implementation
  3. Commits transaction
- **Safety**: Null-check prevents deletion attempts on non-existent records

## File Management Operations

### SaveImageAsync(IFormFile file)
- **Purpose**: Stores uploaded image files for schedules
- **Process**:
  1. Creates "uploads/AnnualSchedules" directory if missing
  2. Generates unique filename using GUID
  3. Saves file to web root path
  4. Returns relative path for database storage
- **Security**: GUID prevents filename conflicts
- **Returns**: Relative path string (e.g., "uploads/AnnualSchedules/guid.jpg")

### DeleteImageFile(string? relativePath)
- **Purpose**: Removes physical image files from storage
- **Process**:
  1. Validates path is not null/empty
  2. Converts relative to absolute path
  3. Deletes file if exists
- **Safety**: Null-check and existence verification prevent errors

## Architecture Patterns

### Repository Pattern
- All data access through UnitOfWork
- Separation of concerns between service and data layers
- Consistent transaction management via CompleteAsync()

### File Management
- Centralized image handling logic
- Cross-platform path handling
- Proper resource disposal with using statements

## Error Handling
- Graceful null handling in Update/Delete operations
- Directory creation for missing upload folders
- File existence checks before deletion
- No exceptions thrown for missing entities

## Transaction Management
- Explicit transaction commits via CompleteAsync()
- Atomic operations for data consistency
- Proper resource cleanup