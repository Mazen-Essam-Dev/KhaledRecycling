# Digital Signature Information

## ViewModel Details
**Model Name:** `SignatureVM`  
**Namespace:** `FougeraClub.Areas.Admin.ViewModels.Account`

Represents a user's digital signature record and uploaded signature file.

---

## Fields & Purpose

| Property | Description | Notes / Validation |
|---------|-------------|-------------------|
| **UserId** | The unique ID of the user the signature belongs to | Required for linking signature to user |
| **ImagePath** | Stored file path of the uploaded signature image | Max 300 characters |
| **SignatureFile** | Signature image file uploaded by the user (`IFormFile`) | Used for upload handling, not stored directly |
| **CreatedAt** | Date the signature was created | Stored as `DateOnly` for clean date value |

---

## Usage Context
- Used when capturing or updating user signature
- Commonly integrated in workflows requiring signed approvals
- Displayed in print layouts and official system documents
