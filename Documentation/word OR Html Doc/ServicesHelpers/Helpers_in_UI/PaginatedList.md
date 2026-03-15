# PaginatedList Documentation

## File Structure
- **PaginatedList**: PaginatedList.cs

## Core Class

### PaginatedList<T>
**Purpose**: Generic pagination utility for efficient data handling in web applications with support for search, filtering, and pagination metadata

## Properties

### Data Properties
- `Items`: List of paginated items (List<T>)
- `PageIndex`: Current page number (int)
- `PageSize`: Number of items per page (int)
- `TotalPages`: Total number of pages (int)
- `Count`: Number of items on current page (int)
- `TotalCount`: Total number of items across all pages (int)

### Filtering Properties
- `SearchTerm`: Search query for filtered results (string?)
- `dateTo`: End date for date range filtering (string?)
- `dateFrom`: Start date for date range filtering (string?)

### Navigation Properties
- `HasPreviousPage`: Indicates if previous page exists (bool)
- `HasNextPage`: Indicates if next page exists (bool)

## Constructors

### Primary Constructor
**Parameters**:
- `items`: Paginated item list
- `count`: Total item count for current filter
- `pageIndex`: Current page number (1-based)
- `pageSize`: Items per page
- `searchTerm`: Optional search filter
- `totalCount`: Optional total count across all filters

## Factory Methods

### Create from IQueryable<T>
**Purpose**: Creates paginated list from Entity Framework queryable source

**Parameters**:
- `source`: IQueryable data source
- `pageIndex`: Current page number
- `pageSize`: Items per page
- `searchTerm`: Optional search term
- `totalCount`: Optional total count

**Implementation**:
- Executes Count() on filtered query
- Applies Skip() and Take() for pagination
- Materializes results with ToList()

### Create from List<T>
**Purpose**: Creates paginated list from in-memory list source

**Parameters**:
- `source`: List data source
- `pageIndex`: Current page number
- `pageSize`: Items per page
- `searchTerm`: Optional search term
- `totalCount`: Optional total count

**Implementation**:
- Uses LINQ Skip() and Take() on list
- Efficient for pre-filtered collections

## Technical Implementation

### Pagination Logic
- **Page Calculation**: `Math.Ceiling(count / (double)pageSize)`
- **Skip Calculation**: `(pageIndex - 1) * pageSize`
- **1-based Indexing**: Page numbers start from 1

### Performance Features
- **Deferred Execution**: IQueryable support for database efficiency
- **In-memory Support**: List<T> support for cached data
- **Metadata Tracking**: Comprehensive pagination info

### Filtering Support
- Search term persistence
- Date range filtering capability
- Total count tracking for unfiltered data

## Usage Examples

### Basic Pagination
```csharp
var query = _context.Users.AsQueryable();
var paginated = PaginatedList<User>.Create(query, pageIndex: 1, pageSize: 10);