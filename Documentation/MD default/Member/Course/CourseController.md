# 🎓 Member Area — CourseController (Full Documentation)

This Markdown file describes the **CourseController** in the **Member area** of an ASP.NET Core 9 MVC application.  
It explains each region, service, and method — **without showing code** — to help developers understand functionality and dependencies.

---

## 🧩 Overview

The **CourseController** manages all **member interactions** related to courses, including:
- Listing available courses
- Subscribing to a course
- Uploading and validating ID or Passport images
- AI-based ID card classification using OCR and ML
- Sending real-time notifications via **SignalR**
- Generating printable course certificates

---

## 🏗️ Dependencies and Services

| Dependency | Purpose |
|-------------|----------|
| **IUnitOfWork** | Handles all repository operations and database commits. |
| **IHttpContextAccessor** | Accesses the current session and retrieves user email for identification. |
| **ICourseService (Member)** | Handles member-side course logic like subscription, rating, and listing. |
| **ICourseService (Admin)** | Used for admin-related tasks like certificate generation. |
| **IMapper** | Maps between domain models, DTOs, and ViewModels. |
| **IOCRService** | Extracts and processes text data from ID card images. |
| **IHubContext\<NotificationHub\>** | Enables real-time notifications through **SignalR** to admin or course managers. |
| **INotificationService** | Sends stored notifications to users with specific permissions. |
| **FileHelper & SessionHelper** | Assist in file handling and session management. |

All uploaded member images (ID/Passport) are saved under  
`/wwwroot/uploads/members`.

---

## 🔐 Attributes

| Attribute | Purpose |
|------------|----------|
| `[MemberAuthorize]` | Restricts access to authenticated members only. |
| `[Area("Member")]` | Registers this controller under the Member area. |
| `[Route("Member/[controller]/[action]")]` | Provides explicit routing for clean endpoint URLs. |

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


## ⚙️ Major Functional Regions

### 1️⃣ **IDClassification_Json (POST)**
Performs **AI-based ID image classification** and **OCR text extraction**.

- Receives an uploaded image file.  
- Runs ML model (`IDClassificationMLModel`) to detect if the image is a valid ID.  
- Checks confidence percentage — rejects if below 90%.  
- Calls `IOCRService.ReadGrayTextAsync()` and `ExtractAllTextDataFrom_IDCardGray_Async()` to read text from the card.  
- Maps results to `IDCardExtractedDataVM` and returns JSON with:
  - Label (e.g., "ID")
  - Probability
  - Extracted text fields (ID number, Name, Birthdate, etc.)
  - Status messages from `Resource1`

If any error occurs (invalid file or exception), a simple error JSON response is returned.

---

### 2️⃣ **Index (GET)**
Displays all **available courses** to the logged-in member.

- Retrieves current member from session.
- Calls `_courseService.GetAllAsync()` to fetch:
  - Available courses  
  - Subscribed courses  
- Maps data to `CourseVM` and adds flags for:
  - `Subscribed`
  - `Accepted`
  - `Attendance`
  - `Rating`
- Returns the populated ViewModel list to the view.

---

### 3️⃣ **TestSignalR (GET)**
Used for testing real-time notifications.

- Sends a test message (e.g., "New Student Joined") to groups:  
  `"CourseManagers"` and `"EventManagers"`.
- Also creates a system notification via `_notificationService`.
- Returns a JSON confirmation.

---

### 4️⃣ **GetCourse (GET)**
Fetches a **single course** by ID and returns its details as JSON.

- Loads course, department, and user information.
- Localizes text based on session language (`ar` or `en`).
- Returns:
  - Department name
  - Title
  - Dates
  - Description
  - Attachment link (if available)

Used for AJAX-based modal popups or course preview.

---

### 5️⃣ **Subscribe (GET)**
Prepares the **subscription page** for a selected course.

- Checks user identity via session.  
- Validates presence and expiration of user ID/Passport images.  
- Displays validation errors (expired or missing documents).  
- Populates `CourseVM` with basic course and user document info.

---

### 6️⃣ **Subscribe (POST)**
Handles actual course subscription logic.

#### ✅ Steps:
1. **Validate Uploaded Files**
   - Checks if files are images under 3MB.
   - Deletes old or invalid files.

2. **Load Current User**
   - Verifies session-based identity and ensures valid data.

3. **Validate Required Documents**
   - Requires valid ID and/or Passport images.
   - Ensures they exist and are not expired.

4. **Save Uploaded Files**
   - Generates GUID-based filenames.
   - Stores under `/uploads/members/`.

5. **(Optional Future Logic)**  
   - OCR + ID validation commented out for later activation:
     - Compares extracted text (from OCR) with member profile fields.
     - Confirms match in name, ID number, birthdate, etc.

6. **Create Subscription**
   - Calls `_courseService.SubscribeAsync()` to register the member.  
   - Sends **real-time notification** to course managers via SignalR.  
   - Sends stored notification to authorized admins.

7. **Redirects** back to **Index** page after success.

---

### 7️⃣ **Rate (POST)**
Records a **course rating** from the member.

- Takes `ratingValue` and `courseId`.  
- Ensures user is logged in.  
- Calls `_courseService.AddRateAsync()` to store the rating.  
- Returns `OK` JSON with success flag.

---

### 8️⃣ **PrintCertificate (GET)**
Generates and displays a **course completion certificate**.

- Fetches data via `_courseAdminService.GetCertificateData()`.  
- Maps to `CertificateVM` (Admin ViewModel).  
- Renders certificate as a printable Razor View.

---

## 🧠 Summary of Responsibilities

| Category | Description |
|-----------|--------------|
| **AI & OCR** | Handles image classification and text extraction for ID verification. |
| **File Management** | Validates, saves, and deletes uploaded member documents securely. |
| **Courses** | Displays, subscribes, and rates available training courses. |
| **Notifications** | Uses **SignalR** + **Database notifications** to inform admins in real time. |
| **Security** | Enforces member authorization and session-based data isolation. |
| **Localization** | Adapts text (titles, descriptions) based on selected language. |

---

## 💡 Design Notes

- The controller uses **multi-service composition** to separate business logic layers:
  - Member services → handle user-side logic  
  - Admin services → handle certificate & reporting logic  
- **SignalR integration** enhances interactivity.
- **Validation regions** ensure file integrity and prevent tampering.
- Future-ready for enabling **AI-based validation** of ID card data.

---

## 🧾 Summary

The `CourseController` serves as the **main hub for member-course interactions**, combining:
- OCR + AI logic  
- Course management  
- Notification broadcasting  
- Security and validation layers  

It demonstrates a **clean architecture pattern**, where controller logic delegates business responsibilities to services and repositories.

---

## 🔄 Updating and Changes

### **Recent Updates:**
1. **Temporary File Upload Handling** (Subscribe POST method):
   - New logic for saving uploaded files to temporary folder (`wwwroot/uploads/temp`)
   - Files are moved to final location only after validation passes
   - Prevents file loss during validation errors

2. **Enhanced Validation Logic**:
   - Improved file existence checks using `FileHelper.IsFileExist()`
   - Better handling of both temporary and existing file paths
   - Consolidated validation for ID and passport images

3. **Notification Improvements**:
   - Added Arabic notification messages for local users
   - Fixed notification target permission (`"Course.Index"`)
   - Consolidated SignalR notifications to specific groups

4. **Code Cleanup**:
   - Removed commented-out OCR validation code (still available for future implementation)
   - Simplified file path handling
   - Better separation of concerns in subscription logic

### **Key Changes in Subscription Flow:**
1. **Before**: Direct file upload to final destination
2. **After**: Temp file storage → Validation → Move to final destination

### **Areas for Future Enhancement:**
1. **AI Validation**: Uncomment and implement OCR-based ID validation
2. **Multi-language Support**: Expand beyond Arabic/English
3. **File Compression**: Add image optimization before storage
4. **Async Optimization**: Further parallelize file operations

### **Dependencies Updated:**
- `FileHelper` now supports temporary file conversion (`ConvertToIFormFile`)
- Enhanced `DeleteImageFile` to handle various path formats
- Added directory creation utilities in critical paths

---

## 📋 Code Structure Reference

```csharp
// Main dependencies and constructor
private readonly IUnitOfWork _unitOfWork;
private readonly IHttpContextAccessor _httpContextAccessor;
private readonly Application.Interfaces.Member.ICourseService _courseService;
// ... other services

public CourseController(IUnitOfWork UnitOfWork, /* ... */)
{
    // Dependency injection setup
}

// Key action methods:
public async Task<IActionResult> IDClassification_Json(IFormFile file) { }
public async Task<IActionResult> Index() { }
public async Task<IActionResult> Subscribe(int? id) { }
[HttpPost] public async Task<IActionResult> Subscribe(CourseVM model) { }
// ... other actions