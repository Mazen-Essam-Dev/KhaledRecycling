# ArchivingDocumentService Documentation

## Overview
Service layer implementation for managing archiving documents with integrated file handling for PDF documents.

## Dependencies
- **IUnitOfWork**: Database repository access and transaction management
- **IWebHostEnvironment**: File system access for document storage
- **FileName Constant**: "ArchivingDocuments" for organized file storage

## Core CRUD Operations

### GetAllAsync()
- **Purpose**: Retrieves all archiving document records
- **Returns**: `IEnumerable<ArchivingDocument>`
- **Implementation**: Direct delegation to repository

### GetByIdAsync(int id)
- **Purpose**: Fetches specific document by identifier
- **Returns**: `ArchivingDocument?` (nullable for not found cases)
- **Implementation**: Repository-based ID lookup with predicate

### AddAsync(ArchivingDocument entity)
- **Purpose**: Creates new archiving document with file handling
- **File Processing**:
  - Saves PDF file using FileHelper
  - Stores relative path in PdfFilePath
  - Handles null files gracefully
- **Process**:
  1. File upload and path generation
  2. Entity addition to repository
  3. Transaction commit
  4. Returns generated ID
- **Returns**: Integer ID of created entity

### UpdateAsync(ArchivingDocument entity)
- **Purpose**: Modifies existing document with file management
- **File Update Logic**:
  - Preserves existing file path if no new file uploaded
  - Deletes old file and saves new one when file is replaced
  - Validates file has content before processing
- **Process**:
  1. Fetches existing entity
  2. Handles file replacement if applicable
  3. Updates entity values
  4. Commits changes

### DeleteAsync(int id)
- **Purpose**: Removes document record and associated file
- **Process**:
  1. Fetches entity for existence check
  2. Deletes physical PDF file from storage
  3. Removes database record
  4. Commits transaction
- **Cleanup**: Ensures file system cleanup on deletion

## File Management Architecture

### File Storage Strategy
- **Directory Organization**: Uses "ArchivingDocuments" folder structure
- **Path Handling**: FileHelper manages relative path generation
- **File Naming**: Automatic unique filename generation

### File Update Pattern
```csharp
// Preserve existing path
entity.PdfFilePath = existing.PdfFilePath;

// Replace file only when new file provided
if (entity.PdfFile != null && entity.PdfFile.Length > 0)
{
    FileHelper.DeleteImageFile(existing.PdfFilePath);
    entity.PdfFilePath = await FileHelper.SaveImageAsync(entity.PdfFile, FileName);
}