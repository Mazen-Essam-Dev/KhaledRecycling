using Domain.Resources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Application.Helpers
{
    public static class FileHelper
    {
        private static IWebHostEnvironment? _env;

        public static void Configure(IWebHostEnvironment env)
        {
            _env = env;
        }

        public static async Task<string> SaveImageAsync(IFormFile file, string folderName)
        {
            if (_env == null)
                throw new InvalidOperationException("FileHelper is not configured. Call Configure() first.");

            if (file == null || string.IsNullOrWhiteSpace(folderName))
                throw new ArgumentException("Invalid file or folder name");

            var folder = Path.Combine(_env.WebRootPath, "uploads", folderName);
            Directory.CreateDirectory(folder);

            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var fullPath = Path.Combine(folder, fileName);

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Path.Combine("uploads", folderName, fileName).Replace("\\", "/");
        }

        public static async Task<string> CheckFileIsPdf_5Mg_Async(IFormFile? file)
        {
            if (file != null && file.Length > 0)
            {
                #region Only PDF files are allowed
                bool isPdf = true;
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension != ".pdf")
                {
                    isPdf = false;
                    return Resource1.PdfOnly;
                }

                // If you want to make sure of the size, for example (5 MB)
                if (file.Length > 5 * 1024 * 1024)
                {
                    isPdf = false;
                    return Resource1.pdfFileMore5mg;
                }

                return "OK";
                #endregion
            }
            else
            {
                return "null";
            }
        }

        public static async Task<string> CheckFileIsImage_3Mg_Async(IFormFile? file)
        {
            if (file != null && file.Length > 0)
            {
                #region Only Image files are allowed
                bool isPdf = true;
                var extension = Path.GetExtension(file.FileName).ToLower();
                if (extension != ".jpg" && extension != ".jpeg" && extension != ".png")
                {
                    isPdf = false;
                    return Resource1.ImageOnly;
                    //ModelState.AddModelError("PdfFilePath", "يُسمح فقط بملفات PDF");

                }

                // If you want to make sure of the size, for example (3 MB)
                if (file.Length > 3 * 1024 * 1024)
                {
                    isPdf = false;
                    return Resource1.ImageFileMore3mg;
                }

                return "OK";
                #endregion
            }
            else
            {
                return "null";
            }
        }

        public static void DeleteImageFile(string? relativePath)
        {
            if (_env == null)
                throw new InvalidOperationException("FileHelper is not configured. Call Configure() first.");

            if (string.IsNullOrWhiteSpace(relativePath)) return;

            var fullPath = _env.WebRootPath +"\\"+ relativePath.Replace("/", Path.DirectorySeparatorChar.ToString());
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }
        }

        public static bool IsFileExist(string? relativePath)
        {
            if (_env == null)
                throw new InvalidOperationException("FileHelper is not configured. Call Configure() first.");


            if (string.IsNullOrWhiteSpace(relativePath)) return false;

            var fullPath = _env.WebRootPath + "\\" + relativePath.Replace("/", Path.DirectorySeparatorChar.ToString());
            if (File.Exists(fullPath))
            {
                return true;
            }

            return false;
        }

        public static async Task<string> SaveTempAsync(IFormFile file)
        {
            if (file == null) return null;

            if (_env == null)
                throw new InvalidOperationException("FileHelper is not configured. Call Configure() SaveTempAsync.");

            var ext = Path.GetExtension(file.FileName);
            var fileName = Guid.NewGuid() + ext;
            var postTempPath = "uploads/temp" + "/" + fileName;
            var tempPath = _env.WebRootPath + "\\"+ "uploads/temp";
            if (!Directory.Exists(tempPath))
            {
                Directory.CreateDirectory(tempPath);
            }
            tempPath = tempPath + "\\" + fileName;
            tempPath = tempPath.Replace("/", Path.DirectorySeparatorChar.ToString());

            using (var stream = new FileStream(tempPath, FileMode.Create))
                await file.CopyToAsync(stream);

            return postTempPath;
        }

        public static string MoveTempToFinal(string tempFileName, out string finalFileName,string folderName)
        {
            finalFileName = null;

            if (string.IsNullOrEmpty(tempFileName)) return "";

            if (_env == null)
                throw new InvalidOperationException("FileHelper is not configured. Call Configure() MoveTempToFinal.");

            var tempPath = Path.Combine(_env.WebRootPath, tempFileName);
            if (!File.Exists(tempPath)) return "";

            finalFileName = Guid.NewGuid() + Path.GetExtension(tempFileName);
            var newPath = "uploads/"+ folderName +"/"+ finalFileName;
            //var finalPath = Path.Combine(_env.WebRootPath,"uploads/", folderName, finalFileName);
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", folderName);

            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var finalPath = Path.Combine(uploadsFolder, finalFileName);

            File.Move(tempPath, finalPath);
            return newPath;
        }

        public static IFormFile? ConvertToIFormFile(string? filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return null;
            var filePath1 = filePath.Replace("wwwroot/", "").Replace("wwwroot", "");
            filePath1 = filePath1.TrimStart('/');
            if (string.IsNullOrEmpty(filePath1)|| string.IsNullOrWhiteSpace(filePath1)) return null;

            filePath = Path.Combine(_env.WebRootPath, filePath1);

            var bytes = System.IO.File.ReadAllBytes(filePath);
            var stream = new MemoryStream(bytes);

            return new FormFile(stream, 0, bytes.Length, "file", Path.GetFileName(filePath))
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };
        }
    }

}
