# Activity Management

## ViewModel Details
**Model Name:** `ActivityVM`  
**Namespace:** `FougeraClub.Areas.Admin.ViewModels.Activity`  
Used for creating, editing, and displaying activities within the Admin Panel.

---

## Core Fields

| Property | Description | Validation / Notes |
|---------|-------------|------------------|
| **Id** | Activity identifier | Auto-managed |
| **TitleAr** | Arabic activity title | Required, Max 200, Arabic letters & numbers only |
| **TitleEn** | English activity title | Required, Max 200, English letters & numbers only |
| **StartDate** | Activity start date | Required, Cannot be in the past (`NotInThePast`) |
| **EndDate** | Activity end date | Required, Must be after StartDate (custom validation) |
| **MinimumAge** | Minimum allowed age for participants | Required |
| **Location** | Activity location | Required, Max 200 |
| **Description** | Additional details about the activity | Optional |
| **Achievement** | Achievements or outcomes | Optional |

---

## Attachments

| Property | Description | Notes |
|---------|-------------|------|
| **AttachmentPath** | File storage path | Max length 300 |
| **Attachment** | File upload (`IFormFile`) | Used for uploading documents/images |

---

## Participation & Statistics

| Property | Type | Description |
|---------|------|-------------|
| **IsSubscribed** | `bool` | Indicates if current user is subscribed to this activity |
| **ActivityDuration** | `int?` | Activity duration in days (optional) |
| **SubscriptionCount** | `int?` | Number of participants |

---

## Questions

| Property | Description |
|---------|-------------|
| **Questions** | A list of activity-related questions (`IEnumerable<Question>`) used for quizzes, surveys, or requirements |

---

## Date Validation Logic

If both `StartDate` and `EndDate` are provided:
- `EndDate` **must be strictly after** `StartDate`
- Error message is **language-aware**, using `SessionHelper.GetCurrentLanguage()`

---

## Usage Context
- Admin → Activities Management
- Create / Edit / Display activities
- Activity subscription system
- Attaching documents and providing activity descriptions

