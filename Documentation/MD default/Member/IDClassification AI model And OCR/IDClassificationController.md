# IDClassificationController — Full Documentation

This controller is part of the **Member area** of FougeraClub.  
It handles **ID card processing** using **AI classification**, **OCR extraction**, and optional **data validation**.  

---

## Overview of Functionality

1. **File Upload Validation**:  
   - Checks if the uploaded file exists and is not empty.  
   - If invalid, returns a ViewModel with `lable = "Invalid File"` and `ProbabilityString = "0"`.

2. **AI Classification**:  
   - Converts the uploaded image to a byte array.  
   - Uses `IDClassificationMLModel.PredictAllLabels` to classify the image.  
   - Determines the highest prediction (`hightestPrediction`) and calculates probability.  
   - Validations:  
     - If the prediction is not `"ID"`, returns error `UploadIDCardThisIsNot`.  
     - If the probability is below threshold (90%), returns error `CaptureThisImageAgainFromFrontFace`.  

3. **OCR Extraction**:  
   - Converts the image to grayscale using `_iOCRService.ReadGrayTextAsync`.  
   - Extracts all text fields using `_iOCRService.ExtractAllTextDataFrom_IDCardGray_Async`.  
   - Maps the extracted DTO to `IDCardExtractedDataVM`.  
   - Sets flags `doneAI_bool`, `ProbabilityString`, `Probability_double`, and `lable`.  
   - Determines if extraction is successful and sets `DoneTextExtracted_Error_Str`.  

4. **Data Validation (Optional)**:  
   - Compares extracted text against reference or entered data using `_iCompareService.SimilarityPercentage`.  
   - Example: compares `matchFullEnName` to `"Muhammad Sajawal"` for similarity.  

5. **Error Handling**:  
   - Any exception during processing returns a ViewModel with the exception message in `lable` and `ProbabilityString = "0"`.  

---

## Endpoints

### GET: `IDClassification`
- Returns the view for uploading an ID image.

### POST: `IDClassification`
- Accepts `IFormFile file`.  
- Performs **validation → AI classification → OCR extraction → optional comparison**.  
- Returns `IDCardExtractedDataVM` populated with all extracted data and processing results.

---

## Key Properties in `IDCardExtractedDataVM`

| Property | Purpose |
|----------|---------|
| `lable` | The predicted label from AI (e.g., "ID"). |
| `ProbabilityString` | Probability percentage as a string. |
| `Probability_double` | Probability as a double. |
| `doneAI_bool` | Indicates if AI classification succeeded. |
| `doneOCR_bool` | Indicates if OCR extraction succeeded. |
| `doneValidation_bool` | Indicates if validation succeeded. |
| `DoneTextExtracted_Error_Str` | Status/error message for user display. |
| `textExtracted` | Raw text extracted from OCR. |
| `matchIDNumber`, `matchBirth`, `matchExpiryDate`, `matchFullEnName`, `matchFullArName`, `matchGender`, `matchNationality` | Individual fields extracted from the ID for comparison/validation. |
