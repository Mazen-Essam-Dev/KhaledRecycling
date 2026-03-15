
using Domain.DTOs;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces.Member
{
    public interface IOCRService
    {
        Task<string> ReadAllTextAsync(IFormFile imageFile);
        Task<string> ReadCropped_MarginXY_TextAsync(/*IFormFile imageFile*/ string grayPath, float marginXRatio, float marginYRatio);
        Task<string> ReadCropped_Custom4_TextAsync(/*IFormFile imageFile*/ string grayPath, float xL, float xR, float yU, float yD, string? lang = null);
        Task<string> ReadGrayTextAsync(IFormFile imageFile);

        // Shortcuts to Crop images
        Task<string> ExtractTextFrom_Cropped_IDNumberAndBirth_Async(string grayPath);
        Task<string> ExtractTextFrom_Cropped_ExpiryDate_Async(string grayPath);
        Task<string> ExtractTextFrom_Cropped_FullEnName_Async(string grayPath);
        Task<string> ExtractTextFrom_Cropped_FullArName_Async(string grayPath);
        Task<string> ExtractTextFrom_Cropped_Gender_Async(string grayPath);
        Task<string> ExtractTextFrom_Cropped_Nationality_Async(string grayPath);
        Task<IDCardExtractedDataDTO> ExtractAllTextDataFrom_IDCardGray_Async(string grayPath);
        Task<int> DeleteTempFile1(string path);
    }
}
