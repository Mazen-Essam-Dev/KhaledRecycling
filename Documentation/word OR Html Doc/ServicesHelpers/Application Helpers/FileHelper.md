# FileHelper Documentation

## File Structure
- **FileHelper**: FileHelper.cs

## Core Helper

### FileHelper
**Purpose**: Comprehensive file management utility for handling image and PDF file operations with validation and storage

**Configuration**:
- `Configure()`: Initializes web hosting environment for path resolution

**File Operations**:
- `SaveImageAsync()`: Saves uploaded images with GUID filename generation
- `CheckFileIsPdf_5Mg_Async()`: Validates PDF files (5MB maximum size)
- `CheckFileIsImage_3Mg_Async()`: Validates image files (3MB maximum size)  
- `DeleteImageFile()`: Deletes files by relative path
- `IsFileExist()`: Checks file existence by relative path

## Methods

### Configure
**Purpose**: Initializes FileHelper with web hosting environment

**Parameters**:
- `env`: IWebHostEnvironment for web root path access

**Usage**: Must be called once at application startup

### SaveImageAsync
**Purpose**: Asynchronously saves uploaded image files with unique naming

**Parameters**:
- `file`: IFormFile to save
- `folderName`: Target folder within uploads directory

**Returns**: Relative file path string

**Features**:
- Creates directory if not exists
- Uses GUID for unique filenames
- Preserves original file extension
- Returns web-relative path

### CheckFileIsPdf_5Mg_Async
**Purpose**: Validates PDF file type and size

**Parameters**:
- `file`: IFormFile to validate

**Returns**: 
- "OK" for valid files
- "null" for null/empty files
- Localized error messages for invalid files

**Validation Rules**:
- Extension must be .pdf
- Maximum size: 5MB
- Returns Resource1.PdfOnly for wrong type
- Returns Resource1.pdfFileMore5mg for oversized files

### CheckFileIsImage_3Mg_Async
**Purpose**: Validates image file type and size

**Parameters**:
- `file`: IFormFile to validate

**Returns**:
- "OK" for valid files  
- "null" for null/empty files
- Localized error messages for invalid files

**Validation Rules**:
- Extensions: .jpg, .jpeg, .png
- Maximum size: 3MB
- Returns Resource1.ImageOnly for wrong type
- Returns Resource1.ImageFileMore3mg for oversized files

### DeleteImageFile
**Purpose**: Deletes file from server storage

**Parameters**:
- `relativePath`: Web-relative path to file

**Features**:
- Handles null/empty paths gracefully
- Converts web paths to system paths
- Checks existence before deletion

### IsFileExist
**Purpose**: Checks if file exists on server

**Parameters**:
- `relativePath`: Web-relative path to check

**Returns**: Boolean indicating file existence

**Features**:
- Handles null/empty paths
- Converts web paths to system paths
- Safe existence checking

## Technical Implementation

### Path Management
- Uses IWebHostEnvironment.WebRootPath for base directory
- Stores files in "uploads/{folderName}" structure
- Handles cross-platform path separators
- Returns web-relative paths for client use

### File Validation
- Extension-based type checking
- Size limits in bytes (5MB PDF, 3MB images)
- Localized error messages from Resource1
- Async operation support

### Security Features
- GUID filename generation prevents name conflicts
- Directory creation with proper permissions
- Safe file deletion with existence checks
- Input validation for all parameters

## Usage Context

### Application Setup
```csharp
// In Startup.cs or Program.cs
FileHelper.Configure(env);