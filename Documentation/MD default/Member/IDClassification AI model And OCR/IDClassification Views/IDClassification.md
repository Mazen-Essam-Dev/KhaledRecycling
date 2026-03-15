# 🪪 ID Classification Page — Documentation (ASP.NET Core 9 MVC)

This file documents the **ID Classification** page that allows users to upload an ID image and view the extracted and classified data.  
It focuses on explaining the **structure**, **logic**, and **flow** — without HTML or CSS.

---

## 🎯 Page Purpose

This page is designed to:
- Let the user upload an image of an **ID card**.
- Send that image to a backend ML or OCR service.
- Display the **classification result**, including extracted text fields such as:
  - ID Type  
  - Confidence Score  
  - Extracted text details (Name, Birth, Gender, etc.)

---

## 🧩 View Overview

| Element | Description |
|----------|--------------|
| **Model** | `IDCardExtractedDataVM` — ViewModel containing all classification results. |
| **Title** | Sets the page title to “ID Classification”. |
| **Form** | Uploads an ID image via `POST` to the `IDClassification` action. |
| **Validation** | Uses both client-side and server-side validation to ensure file selection. |

---

## 🧱 Core Functional Sections

### 1. **File Upload Form**
- Method: `POST`  
- Action: `IDClassification`  
- Enctype: `multipart/form-data` (for image upload)
- Input:  
  - File input (`<input type="file">`) — accepts only image types.  
  - Submit button — triggers classification process.

### 2. **Client-Side Validation**
- JavaScript checks if the user selected a file before submission.
- Displays an alert message if no file is selected.
- Prevents accidental empty uploads.

### 3. **Model Check and Result Display**
- After the backend processes the uploaded file, a new instance of `IDCardExtractedDataVM` is returned.
- If the `Model` is not null → classification results are displayed.

---

## 🧠 ViewModel: `IDCardExtractedDataVM`

| Property | Type | Description |
|-----------|------|-------------|
| `Lable` | `string` | The predicted ID type (e.g., National ID, Passport, License). |
| `ProbabilityString` | `string` | The confidence score of classification (e.g., "98.5%"). |
| `DoneTextExtracted_Error_Str` | `string` | Status of OCR extraction (e.g., “Success” or error details). |
| `TextExtracted` | `string` | The raw text extracted from the uploaded image. |
| `MatchIDNumber` | `string` | Extracted ID number. |
| `MatchBirth` | `string` | Extracted date of birth. |
| `MatchExpiryDate` | `string` | Extracted expiry date. |
| `MatchFullArName` | `string` | Full Arabic name extracted from the ID. |
| `MatchFullEnName` | `string` | Full English name extracted from the ID. |
| `MatchGender` | `string` | Extracted gender value. |
| `MatchNationality` | `string` | Extracted nationality. |

---

## ⚙️ Backend Processing Flow

1. **User uploads file** → Form posts image to controller action `IDClassification`.
2. **Controller logic** (in `MemberController` or similar):
   - Validates the uploaded file.
   - Sends the image to an OCR or classification service.
   - Populates the `IDCardExtractedDataVM` with results.
3. **View renders the model**:
   - Displays label, probability, and all extracted data fields.
   - Uses badges or indicators for better visual feedback.

---

## 🛡️ Validation & Error Handling

| Level | Validation | Behavior |
|-------|-------------|-----------|
| **Client-Side** | Ensures image is selected before upload. | Alerts user immediately. |
| **Server-Side** | Validates image format and size. | Returns error or re-renders form. |
| **Model State** | Uses `ModelState.IsValid` in controller. | Prevents invalid data from processing. |

---

## 🌐 Possible Extensions

- Integrate **loading animation** while waiting for classification.
- Add **preview of uploaded image** before submission.
- Store extracted data to database for later verification.
- Allow **multiple file uploads** (batch classification).
- Integrate **error messages** from backend OCR service.

---

## 🧾 Summary

The **ID Classification** page provides a simple and secure interface for users to upload ID images and view AI-extracted data.  
It’s fully built on:
- **ASP.NET Core 9 MVC**
- **Razor views**
- **Strongly typed ViewModel**
- **Client-side validation**
- **Multilingual-ready structure (if resources used)**

It forms part of an OCR/AI-based identity verification module.
