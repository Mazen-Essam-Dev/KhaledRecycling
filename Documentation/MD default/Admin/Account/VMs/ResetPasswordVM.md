# Reset Password Management

## ViewModel Information
**Model Name:** `ResetPasswordVM`  
**Namespace:** `FougeraClub.Areas.Admin.ViewModels.Account`

Used to reset a user's password through the Admin panel.

---

## Fields & Validation

| Property | Description | Validation Rules |
|---------|-------------|------------------|
| **Id** | User Identifier | Hidden system value |
| **Email** | User account email | Required, Must be a valid email format, Localized error messages |
| **Password** | New password | Required, Input hidden (Password type) |
| **ConfirmPassword** | Confirmation of new password | Must match `Password`, Localized mismatch message |

---

## Behavior & Notes
- Ensures password reset is performed only with a valid user email.
- Validation error messages are retrieved from `Resource2` for multilingual support.
- Used typically in **Admin Panel → User Account Recovery / Reset**.
