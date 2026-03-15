# FougeraClub ASP.NET Core MVC Documentation

This document summarizes the main **ViewModels** and **pages** in the admin area of the FougeraClub project, including Account, Activities, Courses, and Members management.

---

## 1. Account Module

### ResetPasswordVM
- **Purpose:** Used for resetting a user's password.
- **Properties:**
  - `Id` (string?): Optional user identifier.
  - `Email` (string): Required, validated as an email. Uses localized error messages.
  - `Password` (string): Required password.
  - `ConfirmPassword` (string): Must match `Password`. Uses localized error messages.

### SignatureVM
- **Purpose:** Handles user signature data.
- **Properties:**
  - `UserId` (string): Required user ID.
  - `ImagePath` (string?): Optional path of signature image (max 300 characters).
  - `SignatureFile` (IFormFile?): Optional uploaded file for signature.
  - `CreatedAt` (DateOnly): The date the signature was created.

---

## 2. Activity Module

### ActivityVM
- **Purpose:** Represents a single activity with validations and multilingual support.
- **Properties:**
  - `Questions` (IEnumerable<Question>): List of related questions.
  - `Id` (int): Activity identifier.
  - `TitleAr` / `TitleEn` (string?): Arabic and English titles. Required, max 200 characters, and specific character restrictions.
  - `StartDate` / `EndDate` (DateOnly?): Required. `StartDate` cannot be in the past; `EndDate` must be after `StartDate`.
  - `MinimumAge` (int?): Required minimum age for participation.
  - `Location` (string?): Required, max 200 characters.
  - `Description` / `Achievement` (string?): Optional activity description and achievement.
  - `AttachmentPath` (string?): Optional path to uploaded attachment (max 300 characters).
  - `Attachment` (IFormFile?): Optional uploaded file.
  - `IsSubscribed` (bool): Indicates if the current user is subscribed.
  - `ActivityDuration` / `SubscriptionCount` (int?): Optional duration and subscription count.

- **Validation Logic:**
  - Implements `IValidatableObject` to ensure `EndDate` is after `StartDate`.
  - Localization of error messages based on current session language.

### MembersActivityVM
- **Purpose:** Represents an activity along with its members and subscriptions.
- **Properties:**
  - `Activity` (ActivityVM): The activity details.
  - `Members` (IEnumerable<MemberVM>): List of members.
  - `Subscriptions` (IEnumerable<Subscription>): List of subscriptions.

### SubscribedMemberInActivitiesVM
- **Purpose:** Supports paginated member lists subscribed to a specific activity.
- **Properties:**
  - `Activity` (ActivityVM): Activity details.
  - `Members_Paginated` (PaginatedList<MemberVM>): Paginated list of members.
  - `Subscriptions` (IEnumerable<Subscription>): List of subscriptions.

---

## 3. Courses Module

### AddEdit Page
- **Purpose:** Allows adding or editing a course.
- **Key Features:**
  - Detects if the form is for editing or creating a new course.
  - Supports multilingual titles (`TitleAr`, `TitleEn`).
  - Validates required fields: Department, Trainer, StartDate, EndDate, Location.
  - Supports file upload for attachments (PDF) with max size check.
  - Dynamically loads trainers based on selected department via AJAX.
  - Shows existing attachment with a view option if editing.

### Index Page
- **Purpose:** Displays a paginated list of courses with filters and actions.
- **Key Features:**
  - Search and filter by course title, department, trainer, and date range.
  - Supports pagination and page size selection.
  - User permissions determine visibility of Add, Edit, Delete, Members, Print, and Excel actions.
  - Uses AJAX to reload filtered results without refreshing the page.
  - Stores last used filters in session storage to preserve user state.

### _ListPartial
- **Purpose:** Partial view used inside Index page to render the courses table.
- **Features:**
  - Displays course title, trainer name (if not a trainer user), start and end dates, specialization.
  - Shows action buttons for Members, Edit, Delete based on user permissions and subscription status.
  - Displays a message when no courses exist.
  - Includes pagination controls.
  - Updates the total record count dynamically.

---

## 4. Notes on Validation and Permissions
- **LocalizedRequired:** Custom attribute for localized required field messages.
- **NotInThePast:** Ensures that `StartDate` cannot be in the past.
- **Compare:** Ensures `ConfirmPassword` matches `Password`.
- **SessionHelper:** Provides current language for localized error messages.
- **PermissionScanner:** Checks user permissions to enable or disable certain actions (Add, Edit, Delete, Members).

---

## 5. Key Practices
- Use **ViewModels** to separate data presentation from domain entities.
- Use **IValidatableObject** for cross-field validation (e.g., StartDate < EndDate).
- Store user session data for filters to enhance UX.
- Use **AJAX** for dependent dropdowns and partial updates to improve performance.
- Localize all user-facing messages and validations to support multilingual users.

---

End of Documentation
