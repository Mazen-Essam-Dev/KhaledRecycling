# ActivityService.md

# ActivityService.cs

**Path:** `Application/Services/Admin/ActivityService.cs`  
**Description:**  
This service implements the `IActivityService` interface for managing **Activities**. It handles CRUD operations, file attachments, and fetching subscribed members with proper business logic encapsulation and data access abstraction.

**Recent Updates:** Enhanced file attachment handling, improved member subscription retrieval, and optimized data access patterns for better performance and maintainability.

---

## Properties

- `_unitOfWork`: Access to repositories such as Activities, Members, and Subscriptions using Unit of Work pattern for transactional consistency.  
- `FileName`: Default folder/file name for saving activity attachments (`"Activities"`) with consistent naming convention.
- **Recent Addition:** Standardized file naming convention for better organization of activity attachments.

---

## Constructor

- `ActivityService(IUnitOfWork unitOfWork)`  
  Injects `IUnitOfWork` to access data repositories through dependency injection, ensuring loose coupling and testability.
- **Design Pattern:** Follows Dependency Injection principle for service layer architecture.

---

## Methods

### 1. GetAllAsync

- **Purpose:** Retrieve all activities with efficient data access pattern.  
- **Returns:** `IEnumerable<Activity>` - Collection of all activity entities.
- **Performance:** Uses async/await pattern for non-blocking database operations.
- **Recent Enhancement:** No change - maintains simple retrieval pattern for complete activity lists.

### 2. GetByIdAsync

- **Purpose:** Retrieve a single activity by `id` with precise entity identification.  
- **Returns:** `Activity?` (nullable) - Single activity entity or null if not found.
- **Parameters:** `int id` - Unique identifier for activity lookup.
- **Error Handling:** Returns null for non-existent IDs rather than throwing exceptions.
- **Recent Update:** Uses expression-based lookup for type-safe query construction.

### 3. AddAsync

- **Purpose:** Add a new activity with optional file attachment support.  
- **Parameters:**  
  - `Activity entity` - Complete activity data entity.
  - `IFormFile? file` (optional attachment) - File upload with null safety.
- **Behavior:**  
  - Saves attachment if provided using `FileHelper.SaveImageAsync()`.
  - Adds the entity to the repository and saves changes with transactional consistency.
  - Returns the new record `Id` for immediate reference.
- **Returns:** `Task<int>` - Newly created activity ID for client-side reference.
- **File Handling:** 
  - Saves files to `wwwroot/uploads/Activities/` directory.
  - Uses consistent naming convention based on FileName property.
  - **Recent Update:** Maintains simple file save pattern without temporary storage for admin operations.

### 4. UpdateAsync

- **Purpose:** Update an existing activity with optional file replacement.  
- **Parameters:**  
  - `Activity entity` - Updated activity data.
  - `IFormFile? file` (optional attachment) - New file to replace existing.
- **Behavior:**  
  - Fetches existing activity to ensure entity existence.
  - **Recent Change:** Currently preserves old file handling logic (commented out section shows previous file replacement pattern).
  - Updates entity values using `UpdateValues()` method for efficient change tracking.
  - Saves changes with proper transaction management.
- **File Management Strategy:**
  - **Previous Approach:** Delete old file, save new file when provided.
  - **Current Approach:** File replacement logic commented out, preserving existing file paths.
  - **Rationale:** Allows controller-level file management for more complex scenarios.
- **Recent Refactoring:** Separated file handling from entity update to allow controller flexibility.

#### UpdateValues Pattern:
- Uses `_unitOfWork.Activities.UpdateValues(existing, entity)` for efficient updates.
- Tracks only changed properties rather than full entity replacement.
- Reduces database overhead for partial updates.

### 5. DeleteAsync

- **Purpose:** Delete an activity by `id` with complete cleanup.  
- **Behavior:**  
  - Retrieves entity to ensure existence before deletion.
  - Deletes the associated file attachment using `FileHelper.DeleteImageFile()`.
  - Removes the entity from the repository with proper data removal.
  - Saves changes with transactional safety.
- **Cleanup Process:**
  1. Entity retrieval for validation.
  2. File attachment deletion to prevent orphaned files.
  3. Entity removal from repository.
  4. Transaction commit via `CompleteAsync()`.
- **Recent Enhancement:** Maintains comprehensive cleanup including file system resources.

### 6. GetAllMembersOfActivityAsync

- **Purpose:** Get all members subscribed to a specific activity with related data.  
- **Parameters:** `int ActivityId` - Specific activity identifier for member lookup.
- **Returns:** Tuple `(IEnumerable<MemberEntity>?, IEnumerable<Subscription>?)` - Members and their subscriptions.
- **Behavior:**  
  - Retrieves all members with eager loading of Nationality data.
  - Retrieves subscriptions for the given activity filtered by `SubscriptionType.Activity`.
  - Filters members who are subscribed to the activity using LINQ join logic.
  - Returns both collections for flexible client usage.
- **Performance Considerations:**
  - Eager loading of Nationality prevents N+1 query problem.
  - Separate retrieval of members and subscriptions for clarity.
  - LINQ in-memory join for moderate dataset sizes.
- **Recent Optimization:** Added Nationality eager loading for complete member data.

#### Data Retrieval Pattern:
```csharp
// Step 1: Get all members with related Nationality data
var allMembers = await _unitOfWork.Members.GetAllAsync(x => x.Nationality);

// Step 2: Get subscriptions for specific activity
var membersSubscriptionsActivity = await _unitOfWork.Subscriptions
    .GetAllAsync(s => s.SubscribedInId == ActivityId && 
                     s.SubscribedInType == SubscriptionType.Activity);

// Step 3: Filter members by subscription
var activityMembers = allMembers
    .Where(member => membersSubscriptionsActivity.Any(s => s.MemberId == member.Id));