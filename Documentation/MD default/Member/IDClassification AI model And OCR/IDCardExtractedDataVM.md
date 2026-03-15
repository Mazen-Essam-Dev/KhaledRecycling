# Member IDCardExtractedDataVM — Full Documentation (Explanation Only)

This ViewModel is used to **represent the results of ID card extraction and classification** in the Member area.  
It is typically returned after processing an ID card image using AI/ML and OCR services.

---

## Purpose

`IDCardExtractedDataVM` is designed to hold **all extracted data from an ID card**, along with metadata about the extraction process and validation results.  
It is primarily used in:
- ID card classification (`IDClassification_Json` in `CourseController`)
- Frontend display of extracted data for verification
- Conditional workflow, e.g., subscription validation before enrolling in courses.

---

## Properties

| Property | Type | Description |
|----------|------|-------------|
| `lable` | `string?` | The type or label predicted by the AI classifier (e.g., "ID"). |
| `ProbabilityString` | `string?` | The classification confidence as a formatted string (e.g., "92.5%"). |
| `Probability_double` | `double?` | The classification confidence as a numeric value (0–100). Default: `0`. |
| `doneAI_bool` | `bool?` | Indicates if the AI classification step was completed successfully. |
| `doneOCR_bool` | `bool?` | Indicates if OCR extraction of text from the ID was successful. |
| `doneValidation_bool` | `bool?` | Indicates if the extracted data matches expected validation rules (e.g., matches database). |
| `DoneTextExtracted_Error_Str` | `string?` | Message describing success or failure of AI/OCR/validation steps. |
| `textExtracted` | `string?` | Raw text extracted from the ID image using OCR. |
| `matchIDNumber` | `string?` | Extracted ID number from the ID card. |
| `matchBirth` | `string?` | Extracted date of birth. |
| `matchExpiryDate` | `string?` | Extracted expiry date of the ID card. |
| `matchFullEnName` | `string?` | Extracted full name in English. |
| `matchFullArName` | `string?` | Extracted full name in Arabic. |
| `matchGender` | `string?` | Extracted gender from the ID card. |
| `matchNationality` | `string?` | Extracted nationality from the ID card. |

---

## Usage Notes

1. **AI Classification:**  
   - The `lable` and `Probability_double` are set by the machine learning model that classifies the image as an ID or other document.
   - `ProbabilityString` is a human-readable format of the confidence.

2. **OCR Extraction:**  
   - `textExtracted` contains all recognized text.
   - Individual fields (`matchIDNumber`, `matchBirth`, etc.) are parsed from `textExtracted`.

3. **Validation:**  
   - `doneValidation_bool` is used to indicate whether the extracted fields match expected values from the database (e.g., ID number matches the member record).

4. **Error Handling:**  
   - `DoneTextExtracted_Error_Str` provides descriptive messages, which can be displayed on the UI if extraction or validation fails.

5. **Integration Example:**  
   - In `CourseController.IDClassification_Json`, this ViewModel is populated and returned as JSON to the frontend for further processing and user display.

---

## Summary

`IDCardExtractedDataVM` encapsulates:
- **Classification results** (`lable`, `Probability_double`)
- **OCR results** (`textExtracted`, `match*` fields)
- **Validation and workflow flags** (`doneAI_bool`, `doneOCR_bool`, `doneValidation_bool`)
- **Error messages** (`DoneTextExtracted_Error_Str`)

It is a **central data structure** for ID verification workflows in the Member area.
