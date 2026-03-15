# Member HomeController — Full Documentation (Explanation Only)

This controller manages the **main dashboard** for the **Member area** in the application.  
It handles displaying courses, activities, user information, and also manages language switching and error handling.

---

## Overview

**Namespace:** `FougeraClub.Areas.Member.Controllers`  
**Controller Name:** `HomeController`  
**Area:** `Member`  
**Base Route:** `Member/Home/[action]`

The `HomeController` is responsible for:
- Rendering the **home dashboard** for logged-in members.
- Fetching **courses** and **activities** related to the member.
- Managing **localization** (Arabic/English language switch).
- Handling **custom error pages** for different HTTP codes.

---

## Dependencies (Injected Services)

| Dependency | Type | Purpose |
|-------------|------|----------|
| `_unitOfWork` | `IUnitOfWork` | Centralized access to all repositories. |
| `_accountService` | `IAccountService` | Manages member account details and authentication. |
| `_activityService` | `IActivityService` | Provides access to member-related activity data. |
| `_courseService` | `ICourseService` | Provides access to member-related course data. |
| `_httpContextAccessor` | `IHttpContextAccessor` | Allows access to session data and HTTP context. |
| `_mapper` | `IMapper` | AutoMapper instance for converting entities to ViewModels. |

---

## Attributes Used

| Attribute | Description |
|------------|-------------|
| `[Area("Member")]` | Declares this controller belongs to the *Member* area. |
| `[Route("Member/[controller]/[action]")]` | Defines a structured route pattern for all controller actions. |
| `[YesGet]` | Custom attribute (likely for logging or validation). |
| `[MemberAuthorize]` | Restricts access to authenticated member users. |
| `[IgnoreAction]`, `[NoLogging]`, `[AllowAnonymous]` | Used to exclude specific actions from authorization or logging. |

---

## Main Actions

### 1. `Index()` — Dashboard Home
**Type:** `GET`  
**Attributes:** `[YesGet]`, `[MemberAuthorize]`  
**Returns:** `View(HomeActivityCourseVM)`

#### Function:
Loads the member's personalized dashboard, including:
- Upcoming **Courses**
- Current **Activities**
- Member’s **name**
- Optional **Toastr message** after login success

#### Key Steps:
1. **Detect language** using `SessionHelper.GetCurrentLanguage()`.
2. **Check if login was recent**, based on the referrer URL (to show a success toast).
3. **Retrieve the logged-in member’s email** from the session.
4. **Fetch all Courses** and **Activities** for this member.
5. **Filter courses** to only show those starting today or later.
6. **Map data** using AutoMapper → convert entities to `CourseVM` and `ActivityVM`.
7. **Assign related subscription info** (attendance, acceptance, rate, etc.).
8. **Build HomeActivityCourseVM** to combine both Courses and Activities.
9. **Return View** with the complete dashboard data.

#### ViewData Used:
| Key | Purpose |
|------|----------|
| `ShowToastrLoginSuccesfullyLoggedIn` | Boolean flag for displaying a success notification post-login. |

#### Example:
```csharp
var model = new HomeActivityCourseVM
{
    Activities = activitiesVM,
    Courses = coursesVM,
    MemberName = lang == "ar" ? user?.FullNameAr : user?.FullNameEn
};
return View(model);
