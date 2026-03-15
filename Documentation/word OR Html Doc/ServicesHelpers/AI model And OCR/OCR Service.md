# OCRService Documentation

## Overview
`OCRService` is responsible for extracting text from images using **Tesseract OCR** and **ImageSharp** for image processing.  

```     
    OCR (Optical Character Recognition) Service
    This service is responsible for reading text from images + cropping areas before extracting text.
    It uses Tesseract OCR engine and ImageSharp for image processing.
```

It supports:
- Full image text extraction
- Grayscale transformation
- Custom region cropping for ID Card recognition
- Extracting specific fields such as:
  - ID Number
  - Birth Date
  - Expiry Date
  - Full English & Arabic Name
  - Gender
  - Nationality

## Namespaces Used
```csharp
using Tesseract;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using Microsoft.AspNetCore.Http;
```

# All Actions

 - Task<string> ReadAllTextAsync(IFormFile imageFile);
  ```Reads all text directly from the uploaded image without cropping.```

 - Task<string> ReadCropped_MarginXY_TextAsync(/*IFormFile imageFile*/ string grayPath, float marginXRatio, float marginYRatio);
    ```Reads text after cropping margins based on percentage values (marginXRatio, marginYRatio).```
 
 - Task<string> ReadCropped_Custom4_TextAsync(/*IFormFile imageFile*/ string grayPath, float xL, float xR, float yU, float yD, string? lang = null);
    ```Crops by custom (Left, Right, Top, Bottom) ratios, then applies OCR. Useful for extracting specific zones of ID Card.```

 - Task<string> ReadGrayTextAsync(IFormFile imageFile);
    ```Converts uploaded image to grayscale and returns saved grayscale image path.```

## Shortcuts to Crop images
 - Task<string> ExtractTextFrom_Cropped_IDNumberAndBirth_Async(string grayPath);
 - Task<string> ExtractTextFrom_Cropped_ExpiryDate_Async(string grayPath);
 - Task<string> ExtractTextFrom_Cropped_FullEnName_Async(string grayPath);
 - Task<string> ExtractTextFrom_Cropped_FullArName_Async(string grayPath);
 - Task<string> ExtractTextFrom_Cropped_Gender_Async(string grayPath);
 - Task<string> ExtractTextFrom_Cropped_Nationality_Async(string grayPath);
 - Task<IDCardExtractedDataDTO> ExtractAllTextDataFrom_IDCardGray_Async(string grayPath);
 - Task<int> DeleteTempFile1(string path);
 
