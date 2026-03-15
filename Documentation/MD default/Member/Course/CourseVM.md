# Course ViewModel (`CourseVM`) — Full Documentation (Explanation Only)

This ViewModel represents the **Course** entity as used in the **Member area** of the ASP.NET Core MVC application.  
It connects data between the UI (views) and backend services, handling **validation**, **file uploads**, **subscriptions**, and **language support**.

---

## Overview

- Used in the Member area to **display, create, and update courses**.
- Implements `IValidatableObject` to perform **custom validation** (e.g., checking date logic).
- Supports **multilingual fields** (Arabic & English titles).
- Includes **file upload properties** for ID and Passport images.
- Works alongside `CourseService` and `Department` / `Subscription` entities.

---

## Key Properties

| Property | Type | Description |
|-----------|------|-------------|
| `Id` | `int` | Primary identifier of the course. |
| `DepartmentId` | `int?` | References the department associated with the course. |
| `TrainerId` | `string?` | References the trainer assigned to the course. |
| `TitleAr` | `string?` | Arabic course title (validated to allow only Arabic letters and digits). |
| `TitleEn` | `string?` | English course title (validated to allow only Latin letters and digits). |
| `StartDate` | `DateOnly?` | The date when the course begins. |
| `EndDate` | `DateOnly?` | The date when the course ends. |
| `Location` | `string?` | The venue or platform where the course is held. |
| `Description` | `string?` | Full details or overview of the course. |
| `AttachmentPath` | `string?` | Path of any attached document (e.g., syllabus, outline). |
| `Department` | `Department?` | Navigation property for related department entity. |
| `selectedRate` | `int?` | The user’s chosen rating for this course (if rated). |
| `all_CoursesListVM` | `IEnumerable<CourseVM>?` | List of all available courses (used in list pages). |
| `all_SubscriptionsList` | `IEnumerable<Subscription>?` | List of user’s subscriptions (for course membership logic). |
| `Attendance` | `bool?` | Indicates if the member attended the course. |
| `Subscribed` | `bool?` | Indicates if the user has already subscribed. |
| `Accepted` | `bool?` | Indicates if the course application was accepted. |
| `SubscriptionId` | `int?` | Links to the subscription entry. |
| `Notes` | `string?` | Any internal or administrative notes. |
| `RejectionNotes` | `string?` | Reason for rejection (if any). |

---

## Image Upload Fields

| Property | Type | Purpose |
|-----------|------|----------|
| `IdImage` | `IFormFile?` | The uploaded ID card image file. |
| `IdImagePath` | `string?` | Stored path for ID image. |
| `PassportImage` | `IFormFile?` | The uploaded passport image file. |
| `PassportImagePath` | `string?` | Stored path for passport image. |

These are used when the course requires identity verification (e.g., ID card or passport proof).

---

## Validation Rules

### Arabic & English Title Validation
- `TitleAr` → Allows **Arabic letters and numbers** only.  
  Regex: `^[\u0621-\u064A0-9 ]+$`
- `TitleEn` → Allows **English letters and numbers** only.  
  Regex: `^[a-zA-Z0-9 ]+$`

Each validation has a localized error message for user clarity.

---

## Custom Validation — Date Logic

Implements `IValidatableObject.Validate()` to ensure:
> **End Date must be after Start Date**

If validation fails:
- Arabic message → "تاريخ النهاية يجب أن يكون بعد تاريخ البداية"  
- English message → "The End Date must be after the Start Date"

The error message appears **under the `EndDate` field** in the form.

---

## Additional Flags

| Property | Type | Meaning |
|-----------|------|----------|
| `isHasCode` | `int?` | Indicates if the course has an access or registration code. |
| `isHasIDCard` | `bool?` | Whether an ID card is required. |
| `isHasPassport` | `bool?` | Whether a passport is required. |
| `isNotExpired` | `bool?` | Ensures the course is not past its end date. |

These flags help manage conditional form logic (e.g., enabling/disabling file upload inputs).

---

## Summary

- `CourseVM` bridges the gap between **database entities** and **UI forms**.
- Handles **multilanguage support**, **custom validation**, and **file upload binding**.
- Works with the **CourseService** to manage subscriptions, ratings, and updates.
- Ensures **data consistency** (e.g., valid date range) and **localized error feedback** for users.
