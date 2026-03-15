
using Application.Interfaces.Member;
using Application.Services.Member;
using AutoMapper;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels;
using FougeraClub.Areas.Member.ViewModels;
using Infrastructure;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;

namespace FougeraClub.Areas.Member.Controllers
{
    [Area("Member")]
    [Route("Member/[controller]/[action]")]
    public class IDClassificationController : Controller
    {
        private readonly IOCRService _iOCRService;
        private readonly IMapper _mapper;
        private readonly ICompareService _iCompareService;
        public IDClassificationController(IOCRService iOCRService,IMapper mapper, ICompareService iCompareService)
        {
            _iOCRService = iOCRService;
            _mapper = mapper;
            _iCompareService = iCompareService;
        }
       
        
        [HttpGet]
        public async Task<IActionResult> IDClassification()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> IDClassification(IFormFile file)
        {
            try
            {
                #region AI Model Classification
                if (file == null || file.Length==0) 
                {
                    return View(new IDCardExtractedDataVM { lable = "Invalid File", ProbabilityString = "0" });
                }
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                IDClassificationMLModel.ModelInput IDClassificationModel = new IDClassificationMLModel.ModelInput()
                {
                    ImageSource = fileBytes,
                };
                // Make a single prediction on the sample data and print results.
                var sortedScoresWithLabel = IDClassificationMLModel.PredictAllLabels(IDClassificationModel);

                var hightestPrediction = sortedScoresWithLabel.FirstOrDefault();
                string probabilityString = $"{hightestPrediction.Value * 100:0.##}%";
                double propapility = hightestPrediction.Value * 100;
                int maxPercent = 90; // For Know this image Is ID Card

                if (!(hightestPrediction.Key.ToUpper()=="ID")) // NotID
                {
                    return View(new IDCardExtractedDataVM { doneAI_bool = false , DoneTextExtracted_Error_Str = Resource1.UploadIDCardThisIsNot, lable = hightestPrediction.Key.ToUpper(), });
                }
                if (propapility < maxPercent) // ID but image Not Sured
                {
                    return View(new IDCardExtractedDataVM { doneAI_bool = false, DoneTextExtracted_Error_Str = Resource1.CaptureThisImageAgainFromFrontFace, lable = hightestPrediction.Key.ToUpper(), ProbabilityString = probabilityString, Probability_double = propapility });
                }
                #endregion

                #region OCR Extracted Data 
                var grayPath = await _iOCRService.ReadGrayTextAsync(file);
                var iDCardExtractedDataDTO = await _iOCRService.ExtractAllTextDataFrom_IDCardGray_Async(grayPath);
                IDCardExtractedDataVM iDCardExtractedDataVM = _mapper.Map<IDCardExtractedDataVM>(iDCardExtractedDataDTO);
                
                iDCardExtractedDataVM.doneAI_bool = true;
                iDCardExtractedDataVM.lable = hightestPrediction.Key.ToUpper();
                iDCardExtractedDataVM.ProbabilityString = probabilityString;
                iDCardExtractedDataVM.Probability_double = propapility;
                if (hightestPrediction.Key.ToUpper() == "ID" && propapility >= maxPercent && iDCardExtractedDataVM.doneOCR_bool==true)
                {
                    iDCardExtractedDataVM.DoneTextExtracted_Error_Str = Resource1.DataExtractedCorrectly;
                }
                else
                {
                    iDCardExtractedDataVM.DoneTextExtracted_Error_Str = Resource1.CaptureThisImageAgainWithHighQuality;
                }
                #endregion

                #region Validation That Extracted Data == Entered Data in Inputs
                var FullEnCompaire_percentage = await _iCompareService.SimilarityPercentage(iDCardExtractedDataVM.matchFullEnName, " Muhammad Sajawal");

                #endregion


                return View(iDCardExtractedDataVM);
            }
            catch (Exception ex)
            {
                return View(new IDCardExtractedDataVM  { lable = ex.ToString() // أو ex.StackTrace
                                    , ProbabilityString = "0" });
                }

        }
       
    }
}
