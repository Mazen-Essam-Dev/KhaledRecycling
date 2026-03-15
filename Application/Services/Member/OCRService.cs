using Application.Interfaces.Member;
using Domain.DTOs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using System.Text.RegularExpressions;
using Tesseract;

namespace Application.Services.Member
{ 
    public class OCRService : IOCRService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly string _tessdataPath;
        private readonly string _tempFolderPath;
        public OCRService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _tessdataPath = Path.Combine(Directory.GetCurrentDirectory(), "tessdata");
            _tempFolderPath = Path.Combine(_webHostEnvironment.WebRootPath, "temp");
        }

        public async Task<string> ReadAllTextAsync(IFormFile imageFile)
        {
            var imagePath = await SaveTempImageAsync(imageFile);
            try
            {
                using var engine = new TesseractEngine(_tessdataPath, "ara+eng", EngineMode.Default);
                using var pix = Pix.LoadFromFile(imagePath);
                using var page = engine.Process(pix);
                return page.GetText();
            }
            finally
            {
                DeleteTempFile(imagePath);
            }
        }

        public async Task<string?> ReadCropped_MarginXY_TextAsync(/*IFormFile imageFile*/ string grayPath, float marginXRatio, float marginYRatio)
        {
            Directory.CreateDirectory(_tempFolderPath); // Ensures the folder exists

            var fileName = Guid.NewGuid().ToString() + "_cropped" + ".jpg";
            var croppedPath = Path.Combine(_tempFolderPath, fileName);

            try
            {
                using var image = Image.Load<Rgba32>(grayPath);

                // Check that the ROI is within the image dimensions
                int imageWidth = image.Width;
                int imageHeight = image.Height;

                //we calculate how much we want to Cut on each side
                int marginX = (int)(marginXRatio * imageWidth);
                int marginY = (int)(marginYRatio * imageHeight);

                //Image dimensions after cropping
                int newWidth = imageWidth - 2 * marginX;
                int newHeight = imageHeight - 2 * marginY;

                // Crop rectangle
                var cropRect = new Rectangle(marginX, marginY, newWidth, newHeight);

                var cropped = image.Clone(ctx => ctx.Crop(cropRect));
                cropped.Save(croppedPath);

                //using var engine = new TesseractEngine(_tessdataPath, "ara+eng", EngineMode.Default);
                using var engine = new TesseractEngine(_tessdataPath, "eng", EngineMode.Default);
                using var pix = Pix.LoadFromFile(croppedPath);
                using var page = engine.Process(pix);

                ////DeleteTempFile(imagePath);
                //DeleteTempFile(croppedPath);

                return page.GetText();
            }
            catch (Exception ex)
            {
                return " ";
            }

        }

        public async Task<string?> ReadCropped_Custom4_TextAsync(/*IFormFile imageFile*/ string grayPath, float xL, float xR, float yU, float yD,string? lang=null)
        {
            if (lang == null) lang = "eng";

            Directory.CreateDirectory(_tempFolderPath); // Ensures the folder exists

            var fileName = Guid.NewGuid().ToString() + "_cropped" + ".jpg";
            var croppedPath = Path.Combine(_tempFolderPath, fileName);
            try
            {
                using var image = Image.Load<Rgba32>(grayPath);

                // Check that the ROI is within the image dimensions
                int imageWidth = image.Width;
                int imageHeight = image.Height;

                // Calculate all margin as a percentage
                int left = (int)(xL * imageWidth);
                int right = (int)(xR * imageWidth);
                int top = (int)(yU * imageHeight);
                int bottom = (int)(yD * imageHeight);

                // Calculate the remaining width and height
                int newWidth = imageWidth - left - right;
                int newHeight = imageHeight - top - bottom;

                if (newWidth <= 0 || newHeight <= 0)
                    throw new Exception("Crop dimensions are invalid. Margins too large.");

                var cropRect = new Rectangle(left, top, newWidth, newHeight);

                var cropped = image.Clone(ctx => ctx.Crop(cropRect));
                cropped.Save(croppedPath);

                //using var engine = new TesseractEngine(_tessdataPath, "ara+eng", EngineMode.Default);
                //using var engine = new TesseractEngine(_tessdataPath, "eng", EngineMode.Default);
                using var engine = new TesseractEngine(_tessdataPath, lang, EngineMode.Default);
                using var pix = Pix.LoadFromFile(croppedPath);
                using var page = engine.Process(pix);

                ////DeleteTempFile(imagePath);
                DeleteTempFile(croppedPath);
                return page.GetText();
            }
            catch (Exception ex) 
            {
                return " ";
            }
        }

        public async Task<string> ReadGrayTextAsync(IFormFile imageFile)
        {
            var imagePath = await SaveTempImageAsync(imageFile);
            Directory.CreateDirectory(_tempFolderPath); // Ensures the folder exists

            var fileName = Guid.NewGuid().ToString() + "_gray" + ".jpg";
            var grayPath = Path.Combine(_tempFolderPath, fileName);
            try
            {
                using var image = Image.Load<Rgba32>(imagePath);
                image.Mutate(x => x.Grayscale());
                image.Save(grayPath);
                DeleteTempFile(imagePath);
                return grayPath;


            }
            catch(Exception ex)
            {
                return grayPath;
                //DeleteTempFile(imagePath);
                //DeleteTempFile(grayPath);
            }
        }

        // Shortcuts to Cropp image

        public async Task<string> ExtractTextFrom_Cropped_IDNumberAndBirth_Async(string grayPath)
        {
            float xL = 0.25f;  // Delete 5% from the left of 0.05f
            float xR = 0.25f; //  Delete 15% from the right of 0.15f
            float yU = 0.20f;   // Delete 20% from above 0.2ff
            float yD = 0.40f;   // Delete 20% from below 0.2f         
            string textExtracted = await ReadCropped_Custom4_TextAsync(grayPath, xL, xR, yU, yD);
            return (textExtracted!=null)? textExtracted:" ";
        }
        public async Task<string> ExtractTextFrom_Cropped_ExpiryDate_Async(string grayPath)
        {
            float xL = 0.35f; // Delete 5% from the left of 0.05f
            float xR = 0.35f; //  Delete 15% from the right of 0.15f
            float yU = 0.81f;  // Delete 20% from above 0.2f
            float yD = 0.03f; // Delete 20% from below 0.2f
            string textExtracted = await ReadCropped_Custom4_TextAsync(grayPath, xL, xR, yU, yD);
            return (textExtracted != null) ? textExtracted : " ";
        }
        public async Task<string> ExtractTextFrom_Cropped_FullEnName_Async(string grayPath)
        {
            float xL = 0.29f; // Delete 5% from the left of 0.05f
            float xR = 0.0f; //  Delete 15% from the right of 0.15f
            float yU = 0.35f;  // Delete 20% from above 0.2f
            float yD = 0.40f;  // Delete 20% from below 0.2f         
            string textExtracted = await ReadCropped_Custom4_TextAsync(grayPath, xL, xR, yU, yD);
            return (textExtracted != null) ? textExtracted : " ";
        }
        public async Task<string> ExtractTextFrom_Cropped_FullArName_Async(string grayPath)
        {
            float xL = 0.29f; // Delete 5% from the left of 0.05f
            float xR = 0.0f; //  Delete 15% from the right of 0.15f
            float yU = 0.35f;  // Delete 20% from above 0.2f
            float yD = 0.40f;  // Delete 20% from below 0.2f         
            string textExtracted = await ReadCropped_Custom4_TextAsync(grayPath, xL, xR, yU, yD,"ara");
            return (textExtracted != null) ? textExtracted : " ";
        }

        public async Task<string> ExtractTextFrom_Cropped_Gender_Async(string grayPath)
        {
            float xL = 0.75f; // Delete 5% from the left of 0.05f
            float xR = 0.0f; //  Delete 15% from the right of 0.15f
            float yU = 0.77f;  // Delete 20% from above 0.2f
            float yD = 0.08f;  // Delete 20% from below 0.2f         
            string textExtracted = await ReadCropped_Custom4_TextAsync(grayPath, xL, xR, yU, yD);
            return (textExtracted != null) ? textExtracted : " ";
        }
        public async Task<string> ExtractTextFrom_Cropped_Nationality_Async(string grayPath)
        {
            float xL = 0.30f;  // Delete 5% from the left of 0.05f
            float xR = 0.25f; //  Delete 15% from the right of 0.15f
            float yU = 0.45f;   // Delete 20% from above 0.2f
            float yD = 0.23f;   // Delete 20% from below 0.2f         
            string textExtracted = await ReadCropped_Custom4_TextAsync(grayPath, xL, xR, yU, yD);
            return (textExtracted != null) ? textExtracted : " ";
        }

        //the most important function that Extract all data from the image and returns it in a model
        public async Task<IDCardExtractedDataDTO> ExtractAllTextDataFrom_IDCardGray_Async(string grayPath)
        {
        
            var iDCardExtractedDataDTO = new IDCardExtractedDataDTO();
            string textExtracted_IDNumberAndBirth = await ExtractTextFrom_Cropped_IDNumberAndBirth_Async(grayPath);

            var matchIDNumber = Regex.Match(textExtracted_IDNumberAndBirth, @"\b\d{3}-\d{4}-\d{7}-\d\b");
            var matchBirth = Regex.Match(textExtracted_IDNumberAndBirth, @"\b\d{2}/\d{2}/\d{4}\b");
            if (!matchBirth.Success)
            {
                matchBirth = Regex.Match(textExtracted_IDNumberAndBirth, @"\b\d{1}/\d{2}/\d{4}\b");
            }
            string textExtracted_ExpiryDate = await ExtractTextFrom_Cropped_ExpiryDate_Async(grayPath);
            var matchExpiryDate = Regex.Match(textExtracted_ExpiryDate, @"\b\d{2}/\d{2}/\d{4}\b");
            if (!matchExpiryDate.Success)
            {
                matchExpiryDate = Regex.Match(textExtracted_ExpiryDate, @"\b\d{1}/\d{2}/\d{4}\b");
            }
            string textExtracted_FullEnName = await ExtractTextFrom_Cropped_FullEnName_Async(grayPath);
            var matchFullEnName = Regex.Match(textExtracted_FullEnName, @"Name\s*([:;A-Za-z ]+)\s*Dat");
            string matchFullEnName_Str = matchFullEnName.Value;
            matchFullEnName_Str = matchFullEnName_Str.Replace("Name", "").Replace(";", "").Replace(":", "").Replace("\n", "").Replace("\\n", "").Replace("Dat", "");

            string textExtracted_FullArName = await ExtractTextFrom_Cropped_FullArName_Async(grayPath);
            var matchFullArName = Regex.Match(textExtracted_FullArName, @"الاسم\s*([:;\p{IsArabic} ]+)\n");
            if (!matchFullArName.Success)
            {
                matchFullArName = Regex.Match(textExtracted_FullArName, @"الإسم\s*([:;\p{IsArabic} ]+)\n");
            }
            string matchFullArName_Str = matchFullArName.Value;
            matchFullArName_Str = matchFullArName_Str.Replace("الاسم", "").Replace("الإسم", "").Replace("\n", "").Replace("\\n", "").Replace(";", "").Replace(":", "").Replace("Name", "");


            string textExtracted_Gender = await ExtractTextFrom_Cropped_Gender_Async(grayPath);
            var matchGender = Regex.Match(textExtracted_Gender, @"Sex\s*([:;A-Za-z ]+)\s*");
            string matchGender_Str = matchGender.Value;
            matchGender_Str = matchGender_Str.Replace("Sex", "").Replace("\n", "").Replace("\\n", "").Replace(";", "").Replace(":", "");

            string textExtracted_Nationality = await ExtractTextFrom_Cropped_Nationality_Async(grayPath);
            var matchNationality = Regex.Match(textExtracted_Nationality, @"National\s*([:;A-Za-z ]+)\s*");
            if (!matchNationality.Success)
            {
                matchNationality = Regex.Match(textExtracted_Nationality, @"lity\s*([:;A-Za-z ]+)\s*");
            }
            string matchNationality_Str = matchNationality.Value;
            matchNationality_Str = matchNationality_Str.Replace("Nationality", "").Replace("Nationalily", "").Replace("lity", "").Replace("\n", "").Replace("\\n", "").Replace(";", "").Replace(":", "");

            string IDNumber = "";
            if (matchIDNumber.Success)
            {
                IDNumber = matchIDNumber.Value;
                //Console.WriteLine($"Extracted: {IDNumber}");
            }
            //return IDNumber;
            iDCardExtractedDataDTO.textExtracted = textExtracted_IDNumberAndBirth + " -==- " + textExtracted_ExpiryDate + " -===- " + textExtracted_FullEnName + " -==- " + textExtracted_FullArName + " -===- " + textExtracted_Nationality + " -==- " + textExtracted_Gender;
            iDCardExtractedDataDTO.matchIDNumber = matchIDNumber.Value;
            iDCardExtractedDataDTO.matchBirth = matchBirth.Value;
            iDCardExtractedDataDTO.matchExpiryDate = matchExpiryDate.Value;
            iDCardExtractedDataDTO.matchFullEnName = matchFullEnName_Str;
            iDCardExtractedDataDTO.matchFullArName = matchFullArName_Str;
            iDCardExtractedDataDTO.matchGender = matchGender_Str;
            iDCardExtractedDataDTO.matchNationality = matchNationality_Str;

            if (matchIDNumber.Success && matchExpiryDate.Success && matchBirth.Success && matchFullEnName.Success)
            {
                iDCardExtractedDataDTO.doneOCR_bool = true;
            }
            else
            {
                iDCardExtractedDataDTO.doneOCR_bool = false;
            }

            return iDCardExtractedDataDTO;
        }

        //Help tools
        private async Task<string> SaveTempImageAsync(IFormFile imageFile)
        {
            Directory.CreateDirectory(_tempFolderPath); // Ensures the folder exists
            var path = Path.Combine(_tempFolderPath, Path.GetFileName(imageFile.FileName));
            using var stream = new FileStream(path, FileMode.Create);
            await imageFile.CopyToAsync(stream);
            return path;
        }

        private void DeleteTempFile(string path)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
        }
        public async Task<int> DeleteTempFile1(string path)
        {
            if (System.IO.File.Exists(path))
                System.IO.File.Delete(path);
            
            return 1; 
        }
    }
}

