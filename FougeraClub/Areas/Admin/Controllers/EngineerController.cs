using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.Engineer;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;


namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class EngineerController : Controller
    {
        private readonly IEngineerService _engineerService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public EngineerController(IEngineerService engineerService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _engineerService = engineerService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int? selectedNationalityId, int? selectedGraduationYear, int page = 1, int pageSize = 50)
        {
            var lang = SessionHelper.GetCurrentLanguage();

            // Nationalities Dropdown
            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            ViewBag.Nationalities = nationalities
                .Where(c => !string.IsNullOrWhiteSpace(lang == "ar" ? c.NameAr : c.NameEn))
                .Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = lang == "ar" ? c.NameAr : c.NameEn
                })
                .OrderBy(c => c.Text)
                .ToList();


            // Graduation Year Dropdown
            var engineers = await _engineerService.GetAllAsync();
            var years = await _engineerService.GetAllGraduationYears_index();

            ViewBag.GraduationYears = years;
            ViewBag.selectedGraduationYear = selectedGraduationYear;
            ViewBag.selectedNationalityId = selectedNationalityId;

            var model = _mapper.Map<List<EngineerVM>>(engineers).AsQueryable();

            // Filtering
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                model = model.Where(c =>
                    (!string.IsNullOrEmpty(c.FullName) && c.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Mobile) && c.Mobile.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Specialization) && c.Specialization.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Position) && c.Position.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (selectedNationalityId != null)
                model = model.Where(c => c.NationalityId == selectedNationalityId);

            if (selectedGraduationYear != null)
                model = model.Where(c => c.GraduationYear == selectedGraduationYear);

            var paginated = PaginatedList<EngineerVM>.Create(model?.OrderByDescending(m => m.Id), page, pageSize, searchTerm);

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }
        
        public async Task<IActionResult> AddEdit(int? id)
        {
            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();

            var years = await _engineerService.GetAllGraduationYears();
            ViewBag.GraduationYears = years;

            EngineerVM vm;

            if (id == null || id == 0)
            {
                vm = new EngineerVM
                {
                    Code = await _engineerService.GenerateNewCode()
                };
            }
            else
            {
                var engineer = await _engineerService.GetByIdAsync(id.Value);
                if (engineer == null)
                    return NotFound();

                vm = _mapper.Map<EngineerVM>(engineer);
                ViewBag.selectedGraduationYear = engineer?.GraduationYear;
            }

            vm.NationalityList = SelectListHelper.BindSelectList(nationalities.ToList(), vm.NationalityId).ToList();

            if (vm.PhoneNumber != null && vm.PhoneNumber.StartsWith("971")) // Is Phone StartsWith 971 Remove it
                vm.PhoneNumber = vm.PhoneNumber.Substring(3);
            if (vm.Mobile != null && vm.Mobile.StartsWith("971")) // Is Mobile StartsWith 971 Remove it
                vm.Mobile = vm.Mobile.Substring(3);

            vm.OldPath = vm.ProfileImagePath;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(EngineerVM model)
        {
            var FolderEntityWillSaveIn = "Engineers";
            #region Validate Is File is PDF And MG // Validate Images
            // Local function to validate the uploaded image size & type
            async Task<string?> ValidateImageAsync(string? TempPath, IFormFile? fileOrignal, string? filePath, string key)
            {

                IFormFile? Image_File_Temp = !string.IsNullOrEmpty(TempPath) ? FileHelper.ConvertToIFormFile(TempPath) : fileOrignal;
                string? NewImage_path = !string.IsNullOrEmpty(TempPath) ? TempPath : filePath;
                var result_Text = await FileHelper.CheckFileIsImage_3Mg_Async(Image_File_Temp);
                if (result_Text != "OK" && result_Text != "null")
                {
                    ModelState.AddModelError(key, result_Text);
                    // Delete old actual image from Final folder if exists
                    FileHelper.DeleteImageFile(NewImage_path);
                }
                return result_Text;
            }

            var Photo_Text = await ValidateImageAsync(model.TempFilePath, model.ProfileImage, model.ProfileImagePath, "ProfileImage");

            if (Photo_Text != "OK" && Photo_Text != "null") ModelState.AddModelError("ProfileImage", Photo_Text);

            #endregion Validate Is File is PDF And MG // Validate PDF

            #region validation fails // !ModelState.IsValid
            // If validation fails
            if (!ModelState.IsValid)
            {
                ModelState.Remove("TempFilePath"); // its Important To Bind New Data Temp
                ModelState.Remove("OldPath"); // its Important To Bind New Data Temp
                ModelState.Remove("ProfileImage"); // its Important To Bind New Data Temp
                if (Photo_Text != "OK" && Photo_Text != "null") { ModelState.AddModelError("ProfileImage", Photo_Text ?? " "); }


                // If the user uploads a new file → cache it before returning
                if (model.ProfileImage != null)
                    model.TempFilePath = await FileHelper.SaveTempAsync(model.ProfileImage);

                // Refill dropdowns if validation fails
                var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
                model.NationalityList = SelectListHelper.BindSelectList(nationalities.ToList(), model.NationalityId).ToList();

                var years = await _engineerService.GetAllGraduationYears();
                ViewBag.GraduationYears = years;

                if (model.PhoneNumber != null && model.PhoneNumber.StartsWith("971")) // Is Phone StartsWith 971 Remove it
                    model.PhoneNumber = model.PhoneNumber.Substring(3);
                if (model.Mobile != null && model.Mobile.StartsWith("971")) // Is Mobile StartsWith 971 Remove it
                    model.Mobile = model.Mobile.Substring(3);

                TempData.Keep(); // for safety if re-rendered
                return View(model);
            }
            #endregion validation fails // !ModelState.IsValid

            // ---------------------------
            //  PROCESS FINAL FILE
            // ---------------------------
            #region PROCESS FINAL FILE Handeling Save in newpath From Temp
            string finalFileName = null;

            // 1) If there is a new file uploaded by the user
            if (model.ProfileImage != null)
            {
                FileHelper.DeleteImageFile(model.OldPath);
                model.ProfileImagePath = await FileHelper.SaveImageAsync(model.ProfileImage, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFilePath))
            {
                FileHelper.DeleteImageFile(model.OldPath);
                model.ProfileImagePath = FileHelper.MoveTempToFinal(
                    model.TempFilePath,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.ProfileImagePath = model.OldPath;
            }

            #endregion PROCESS FINAL FILE Handeling Save in newpath From Temp
            // ---------------------------------------


            // //Phone Dubai
            model.PhoneNumber = await PhoneHelper.CheckAndDoPhoneStart971(model.PhoneNumber);

            // //Phone Dubai
            model.Mobile = await PhoneHelper.CheckAndDoPhoneStart971(model.Mobile);

            var entity = _mapper.Map<Engineer>(model);
            entity.ProfileImagePath = model.ProfileImagePath;
            if (model.Id == 0)
            {
                model.Id = await _engineerService.AddAsync(entity, model.ProfileImage);
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else
                await _engineerService.UpdateAsync(entity, model.ProfileImage);

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit 
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _engineerService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, int? selectedNationalityId, int? selectedGraduationYear)
        {
            var lang = SessionHelper.GetCurrentLanguage();

            var engineers = await _engineerService.GetAllAsync();

            // Filtering
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                engineers = engineers.Where(c =>
                    (!string.IsNullOrEmpty(c.FullName) && c.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Mobile) && c.Mobile.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Specialization) && c.Specialization.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Position) && c.Position.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (selectedNationalityId != null)
                engineers = engineers.Where(c => c.NationalityId == selectedNationalityId);

            if (selectedGraduationYear != null)
                engineers = engineers.Where(c => c.GraduationYear == selectedGraduationYear);

            var model = _mapper.Map<List<EngineerVM>>(engineers).AsQueryable();
            model = model?.OrderByDescending(m => m.Id);
            return View(model);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, int? selectedNationalityId, int? selectedGraduationYear)
        {
            var lang = SessionHelper.GetCurrentLanguage();

            var engineers = await _engineerService.GetAllAsync();

            // Filtering
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                engineers = engineers.Where(c =>
                    (!string.IsNullOrEmpty(c.FullName) && c.FullName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Mobile) && c.Mobile.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Specialization) && c.Specialization.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Position) && c.Position.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (selectedNationalityId != null)
                engineers = engineers.Where(c => c.NationalityId == selectedNationalityId);

            if (selectedGraduationYear != null)
                engineers = engineers.Where(c => c.GraduationYear == selectedGraduationYear);

            var model = _mapper.Map<List<EngineerVM>>(engineers).AsQueryable();
            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                var allData_list = model?.OrderByDescending(m => m.Id);
                var ListTitles = new List<string>
{
    Resource2.Name,Resource2.Profession,Resource2.Nationality,Resource2.GraduationYear,Resource2.Specialization,Resource1.MobileNumber,
};
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = single.FullName,
                        t2 = single.Position,
                        t3 = lang == "ar" ? ((single.Nationality!=null)? single.Nationality.NameAr:"") : ((single.Nationality != null) ? single.Nationality.NameEn : ""),
                        t4 =single.GraduationYear,
                        t5=single.Specialization,
                        t6= single.PhoneNumber,
                    }).ToList();

                    if (lang == "ar")
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "ar");
                    }
                    else
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "en");
                    }
                }

                FileContentResult? Excelfile = null;
                if (fileBytes != null && fileBytes.Length > 0 && boolStatus == true)
                {
                    var fileExcelName = Resource1.EngineerList2;
                    Excelfile = File(fileBytes,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        $"{fileExcelName}_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
                }
                return Excelfile;


            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Details(int id)
        {
            var engineer = await _engineerService.GetByIdAsync(id);
            if (engineer == null)
                return NotFound();
            var vm = _mapper.Map<EngineerVM>(engineer);
            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            vm.NationalityList = SelectListHelper.BindSelectList(nationalities.ToList(), vm.NationalityId).ToList();
            return View(vm);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintDetails(int id)
        {
            var engineer = await _engineerService.GetByIdAsync(id);
            if (engineer == null)
                return NotFound();
            var vm = _mapper.Map<EngineerVM>(engineer);
            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            vm.NationalityList = SelectListHelper.BindSelectList(nationalities.ToList(), vm.NationalityId).ToList();
            return View(vm);
        }


    }

}
