# Member Add/Edit Module Documentation

This document explains the **Add/Edit Member** view (`AddEdit.cshtml`) in the Admin area of the FougeraClub ASP.NET Core MVC application. It focuses on structure, logic, and functionality, without including HTML/CSS.

---

## 1. Purpose

The `AddEdit` view is used for:

- Adding a new member
- Editing an existing member's details
- Uploading member images and documents
- Capturing basic, social, academic, login, and ID information
- Handling validation and conditional fields

---

## 2. ViewModel

The view uses the `MemberVM` ViewModel:

Key properties:

- `Id`, `Code`, `FullNameAr`, `FullNameEn`
- `DateOfBirth`, `Age`, `GenderId`, `NationalityId`
- Contact info: `PhoneNumber`, `FatherPhone`, `MotherPhone`, `Address`
- Academic info: `AcademicQualification`, `EducationInstitution`
- Social media info: `Facebook`, `Instagram`, `Xplatform`
- Login info: `Email`, `Password`, `ConfirmPassword`
- File uploads: `ProfileImage`, `IdImage`, `PassportImage`
- Enumerations: `Gender`, `Profession`, `HeardBySources`

---

## 3. Page Structure

### 3.1 Header and Breadcrumbs

- Displays the current page title (`Create Member` or `Edit Member`)
- Breadcrumb navigation shows:
  - Main module (`ActivitiesManagement`)
  - Current page (`Create/Edit Member`)
- Print button links to `PrintDetails` action for the member

### 3.2 Card Header

- Displays member **code** and **registration date**
- Includes page title with an icon

### 3.3 Profile Image Section

- Shows preview if `ProfileImagePath` exists
- Opens in a modal on click
- Upload field for a new profile image
- Maximum allowed size: 3MB

---

## 4. Form Sections

### 4.1 Basic Data

- Full name (Arabic & English)
- Nationality dropdown
- Gender radio buttons
- Date of birth input and **age calculation**
- Conditional guardian phone numbers:
  - Shown if age < 18
  - Hidden otherwise
- Phone number, city dropdown
- Profession:
  - Radio buttons for enum
  - Conditional input for user job or guardian job
- Address textarea

### 4.2 ID Information

- ID number
- ID expiry date input
- Validation for required fields

### 4.3 Qualifications

- Academic qualification
- Education institution
- Hobbies
- Languages spoken

### 4.4 Social Media

- Facebook, Instagram, Xplatform
- "Heard by" radio buttons for enum values
- Checkbox for license/permission related to social media photo

### 4.5 Login Data

- Email
- Password and Confirm Password
- Toggle visibility for passwords using eye icon
- Validation for required fields and format

### 4.6 Attached Files

- Profile image, ID image, passport image
- Each file has:
  - Upload input
  - Modal preview if file exists
  - Max size 3MB

---

## 5. Scripts

### 5.1 Password Toggle

- Function `togglePassword(inputId, button)` toggles input type between `password` and `text`
- Changes the eye icon accordingly

### 5.2 Age Calculation

- Automatically calculates age from `DateOfBirth`
- Shows/hides guardian phone fields based on age (<18)
- Clears inputs when hidden

### 5.3 Profession Toggle

- Shows either:
  - User profession input if first profession radio selected
  - Guardian profession input for other selections
- Clears hidden input values automatically

### 5.4 Profile Image Preview

- Displays selected profile image instantly before uploading
- Hides preview if no file selected

---

## 6. Validation

- Uses `asp-validation-for` for all input fields
- Summarizes errors at the top of the form with `asp-validation-summary="ModelOnly"`

---

## 7. Buttons

- **Save**: submits the form
- **Back**: returns to index or previous page
- **Print**: opens member details in a new tab for printing

---

## 8. Notes

- Conditional fields (age, profession) enhance user experience
- Modal previews for images provide quick visual confirmation
- Form uses `multipart/form-data` for file uploads
- Scripts ensure dynamic behavior without page reload

---

**End of Add/Edit Member Documentation**
