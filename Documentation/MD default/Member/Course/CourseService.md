# 📘 Course Service — Member Area (ASP.NET Core 9 MVC)

This file documents the **CourseService** class inside the **Member Area** of the application.  
It handles all logic related to **Courses** from the member's side — including subscription, rating, fetching, and CRUD operations.

---

## 🧩 Overview

- **Namespace:** `Application.Services.Member`  
- **Implements Interface:** `ICourseService`  
- **Injected Dependency:** `IUnitOfWork`  
- **Main Entity Used:** `Course`  
- **Related Entity:** `Subscription`  
- **Enum Used:** `SubscriptionType.Course`

---

## 🏗️ Purpose

The service provides methods for:
1. Subscribing members to courses.  
2. Rating courses after attendance.  
3. Getting all available courses for the logged-in user.  
4. Managing (Add, Update, Delete) course records from the database.

It acts as the **business logic layer** between the **Controller** and **Repositories**.

---

## 🧠 Key Responsibilities

| Method | Description | Returns |
|--------|--------------|----------|
| `SubscribeAsync(int CourseId, string username)` | Creates a new `Subscription` record linking a member to a course. | `Task` |
| `AddRateAsync(int CourseId, string email, int? rate)` | Updates the member's course rating inside their subscription. | `Task<bool>` |
| `GetAllAsync(string username)` | Retrieves all available courses and the logged-in user's related subscriptions. | `(IEnumerable<Course>, IEnumerable<Subscription>)` |
| `GetByIdAsync(int id)` | Gets a specific course by ID. | `Task<Course?>` |
| `AddAsync(Course Course)` | Adds a new course to the database. | `Task` |
| `UpdateAsync(Course Course)` | Updates an existing course record. | `Task` |
| `DeleteAsync(int id)` | Deletes a course by ID. | `Task` |
| `Exists(int id)` | Checks whether a course with a given ID exists. | `bool` |

---

---
## ⚙️⚙️⚙️ Course Subscribe Order Logic

| No. | Description | Then |
|----|------------|------|
| 1 | Member goes to subscribe in a course but his ID Card is expired | When he goes to subscribe to the course, he must enter a new ID Card for the same person and it must not be expired. **Checked by OCR but #Now Commented#** |
| 2 | Member goes to subscribe in a course but has a valid (not expired) ID Card | When he subscribes to the course, a new ID Card is **not required** |
| 3 | After the member subscribes to this course | In **SuperAdmin → CoursesSubs**, the subscription will appear, but **SuperAdmin must accept or reject** it. Once chosen, it **cannot be changed**. After acceptance, it will appear in **Member Area → Courses** |
| 4 | Only if SuperAdmin accepted the subscription | It will appear in `/Admin/Course/MembersCourse` to allow the **Trainer to mark students as present or absent**, and the member will be allowed to **rate the course** |
| 5 | Only after the Trainer marks the student as present | It will appear in `/Member/Course`, and the **certificate will be available for download only after**: the course time is finished, the member rated the course, and the member attended the course |

-----------

## ⚙️ Method Details

### 1. `SubscribeAsync`
- Gets the logged-in user using their email.
- Creates a new `Subscription` linking the **MemberId** to the **CourseId**.
- Sets `SubscribedInType = SubscriptionType.Course`.
- Saves the data to the database.

### 2. `AddRateAsync`
- Fetches the subscription for the current member and course.
- Updates the `Rate` field with the user's rating value.
- Returns `true` on success, `false` if subscription or rate is invalid.

### 3. `GetAllAsync`
- Retrieves all **courses** (with eager loading for Department and Trainer).
- Fetches all **subscriptions** belonging to the logged-in user.
- Returns both as a tuple for easy binding in the controller and view.

### 4. `AddAsync`, `UpdateAsync`, `DeleteAsync`
- Perform standard CRUD operations using the `UnitOfWork` pattern.
- Ensure transactional consistency via `_unitOfWork.CompleteAsync()`.

### 5. `Exists`
- Simple existence check using the database table query (`Any()`).

---

## 🧩 Dependencies Used

| Dependency | Role |
|-------------|------|
| `IUnitOfWork` | Centralized data access to repositories for Courses and Subscriptions. |
| `Domain.Entities.Course` | Entity representing the course details. |
| `Domain.Entities.Subscription` | Entity linking Members to Courses. |
| `Domain.Enums.SubscriptionType` | Defines whether a subscription belongs to a course or other type. |

---

## 🧱 Layer Integration

| Layer | Interaction |
|--------|-------------|
| **Controller (Member.CourseController)** | Calls `SubscribeAsync()`, `AddRateAsync()`, and `GetAllAsync()`. |
| **Repository (via UnitOfWork)** | Handles all database-level CRUD operations. |
| **ViewModel (CourseVM)** | Receives the combined course and subscription data. |

---

## ✅ Summary

This service is the **core business layer** for handling member-side course actions.  
It ensures that:
- Each subscription and rating is tied to a specific authenticated member.  
- Database access remains clean through the `UnitOfWork` pattern.  
- The logic is **decoupled** from the controller for easier testing and maintainability.

---

## 🔄 Updating and Changes

### **Recent Updates:**
1. **Subscription Creation Logic**:
   - Streamlined subscription creation with direct entity initialization
   - Removed unnecessary validation steps handled by the controller
   - Maintained clean separation between subscription and rating logic

2. **Rating System Improvements**:
   - Added null safety checks for rating values
   - Enhanced error handling for non-existent subscriptions
   - Ensured atomic operations with proper `CompleteAsync()` calls

3. **Data Retrieval Optimization**:
   - Added eager loading for `Department` and `Trainer` navigation properties
   - Improved performance by loading related data in a single query
   - Separated user subscriptions from all courses for better data isolation

4. **Method Signature Consistency**:
   - Standardized async method naming with `Async` suffix
   - Maintained consistent parameter naming (e.g., `CourseId`, `username`, `email`)
   - Improved return type clarity for all public methods

### **Key Architectural Decisions:**
1. **Separation of Concerns**:
   - Course subscription logic isolated from rating logic
   - Data retrieval separated from data modification
   - Business rules enforced at service level, not repository level

2. **Error Handling Strategy**:
   - Silent failure for non-critical operations (returns `false` or empty collections)
   - Explicit validation for required parameters
   - Database transactions handled through UnitOfWork pattern

3. **Performance Considerations**:
   - Eager loading of related entities to prevent N+1 queries
   - Efficient filtering of subscriptions by user and type
   - Minimal data transfer between layers

### **Areas for Future Enhancement:**
1. **Caching Layer**: Implement caching for frequently accessed course lists
2. **Pagination Support**: Add pagination to `GetAllAsync()` for large datasets
3. **Advanced Filtering**: Support for filtering courses by date, department, or trainer
4. **Batch Operations**: Add support for bulk subscription or rating updates
5. **Audit Logging**: Track course subscription and rating changes

### **Integration Points:**
1. **Controller Integration**: 
   - Directly called from `CourseController.Subscribe()` and `CourseController.DoRate()`
   - Provides data for `CourseController.Index()` view
   - Integrates with session-based user authentication

2. **Repository Layer**:
   - Leverages `IUnitOfWork` for data access abstraction
   - Uses generic repository pattern for type safety
   - Maintains clean separation between data access and business logic

3. **Domain Model Alignment**:
   - Closely follows domain entity structures
   - Respects business constraints defined in domain layer
   - Maps cleanly to ViewModel structures in presentation layer

### **Code Structure Reference:**

```csharp
public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork) { /* DI setup */ }
    
    // Core business methods
    public async Task SubscribeAsync(int CourseId, string username) { /* ... */ }
    public async Task<bool> AddRateAsync(int CourseId, string email, int? rate) { /* ... */ }
    public async Task<(IEnumerable<Course>, IEnumerable<Subscription>)> GetAllAsync(string username) { /* ... */ }
    
    // Standard CRUD operations
    public async Task<Course?> GetByIdAsync(int id) { /* ... */ }
    public async Task AddAsync(Course Course) { /* ... */ }
    public async Task UpdateAsync(Course Course) { /* ... */ }
    public async Task DeleteAsync(int id) { /* ... */ }
    public bool Exists(int id) { /* ... */ }
}