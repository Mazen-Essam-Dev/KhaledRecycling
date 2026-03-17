using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Resources;
using KhaledTeamRecycling.Areas.Admin.ViewModels.Member;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Helpers;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;


namespace KhaledTeamRecycling.Areas.Admin.Controllers
{
    //[AdminAuthorize]
    [Area("Admin")]
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public MemberController(IMemberService MemberService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _memberService = MemberService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int? selectedMemberType, int? selectedNationality, int? selectedGender, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var lang = SessionHelper.GetCurrentLanguage();

            var allMembers = await _memberService.GetAllAsync();
            var memberVMs = _mapper.Map<List<MemberVM>>(allMembers).AsQueryable();


            #region search by word
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                memberVMs = memberVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.FullNameAr) && c.FullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.FullNameEn) && c.FullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.IdNumber) && c.IdNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Code.ToString()) && c.Code.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            #endregion

            #region search by dropdown
            var allNationalities = await _unitOfWork.Nationalities.GetAllAsync();
            ViewBag.Nationalities = SelectListHelper.BindSelectList(allNationalities.ToList()).Distinct();

            if (selectedNationality != null)
            {
                memberVMs = memberVMs.Where(c => c.NationalityId == selectedNationality);
            }
            #endregion

            #region search by dropdown
            ViewBag.Genders = SelectListHelper.GetEnumSelectList<Gender>().Distinct();

            if (selectedGender != null)
            {
                memberVMs = memberVMs.Where(c => c.GenderId == selectedGender);
            }
            if (selectedMemberType != null && selectedMemberType != 33)
            {
                memberVMs = memberVMs.Where(c => c.MemberTypeId == selectedMemberType);
            }
            #endregion

            #region search by date
            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                memberVMs = memberVMs.Where(c => c.RegistrationDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                memberVMs = memberVMs.Where(c => c.RegistrationDate <= dateTo.Value);
            }
            #endregion


            var paginated = PaginatedList<MemberVM>.Create(memberVMs, page, pageSize, searchTerm);


            ViewBag.SelectedGender = selectedGender;
            ViewBag.SelectedNationality = selectedNationality;
            ViewBag.SelectedselectedMemberType = selectedMemberType;
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }

 

        public async Task<IActionResult> AddEdit(int? id)
        {
            var previousUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(previousUrl) && previousUrl.Contains("Admin/Member/Index", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Member_AddEdit = "Member.Index";
                SessionExtensions.SetString(HttpContext.Session, "Member_AddEdit", "Member.Index");
            }
            if (!string.IsNullOrEmpty(previousUrl) && previousUrl.Contains("Admin/Course/SubscribedMembersInCourses", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Member_AddEdit = "SubscribedMembersInCourses";
                SessionExtensions.SetString(HttpContext.Session, "Member_AddEdit", "SubscribedMembersInCourses");
            }

            var vm = new MemberVM();

            if (id.HasValue && id.Value != 0)
            {
                var member = await _memberService.GetByIdAsync(id.Value);
                if (member == null) return NotFound();
                vm = _mapper.Map<MemberVM>(member);
            }
            else
            {
                vm.Code = await _memberService.GenerateNewCode();
            }

            var cities = await _unitOfWork.Cities.GetAllAsync();
            if (cities != null && cities.Count() > 0) cities = cities.OrderBy(x => x.Id);

            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            //Select Only United Arab Emirates 
            var nationalitiesOne = nationalities.Where(n => (!string.IsNullOrEmpty(n.NameAr) && n.NameAr.Contains("مارات", StringComparison.OrdinalIgnoreCase)));
            //model.NationalityId = nationalities.FirstOrDefault()?.Id;

            vm.Nationalities = SelectListHelper.BindSelectList(nationalities.ToList(), vm.NationalityId).ToList();
            vm.NationalitiesOne = SelectListHelper.BindSelectList(nationalitiesOne.ToList(), vm.NationalityId).ToList();

            var citySharqa = await _unitOfWork.Cities.GetAllAsync(x => x.NameAr != null && x.NameAr.Contains("فجير"));
            var sharqaId = citySharqa.FirstOrDefault()?.Id;
            vm.CitiesList = SelectListHelper.BindSelectList(cities.ToList(), vm.CityId ?? sharqaId).ToList();
            if (vm.PhoneNumber != null && vm.PhoneNumber.StartsWith("971")) // Is PhoneNumber StartsWith 971 Remove it
                vm.PhoneNumber = vm.PhoneNumber.Substring(3);
            if (vm.FatherPhone != null && vm.FatherPhone.StartsWith("971")) // Is FatherPhone StartsWith 971 Remove it
                vm.FatherPhone = vm.FatherPhone.Substring(3);
            if (vm.MotherPhone != null && vm.MotherPhone.StartsWith("971")) // Is MotherPhone StartsWith 971 Remove it
                vm.MotherPhone = vm.MotherPhone.Substring(3);

            vm.IdImage_OldPath = vm.IdImagePath;
            vm.Passport_OldPath = vm.PassportImagePath;
            vm.ProfileImage_OldPath = vm.ProfileImagePath;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(MemberVM model)
        {
            var FolderEntityWillSaveIn = "Members";
            #region Validate Is File is PDF And MG // Validate Images
            // Local function to validate the uploaded image size & type
            async Task<string?> ValidateImageAsync(string? TempPath,IFormFile? fileOrignal, string? filePath, string key)
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

            var Passport_Text = await ValidateImageAsync(model.Passport_TempFilePath, model.PassportImage, model.PassportImagePath, "PassportImage");
            var IdImage_Text = await ValidateImageAsync(model.IdImage_TempFilePath, model.IdImage, model.IdImagePath, "IdImage");
            var ProfileImage_Text = await ValidateImageAsync(model.ProfileImage_TempFilePath, model.ProfileImage, model.ProfileImagePath, "ProfileImage");

            //IFormFile? ProfileImage_File_Temp = !string.IsNullOrEmpty(model.ProfileImage_TempFilePath) ? FileHelper.ConvertToIFormFile(model.ProfileImage_TempFilePath) : model.ProfileImage;
            //string? ProfileImage_path = !string.IsNullOrEmpty(model.ProfileImage_TempFilePath) ? model.ProfileImage_TempFilePath : model.ProfileImagePath;
            //var ProfileImage_Text = await FileHelper.CheckFileIsImage_3Mg_Async(ProfileImage_File_Temp);
            //if (ProfileImage_Text != "OK" && ProfileImage_Text != "null")
            //{
            //    ModelState.AddModelError("ProfileImage", ProfileImage_Text);
            //    FileHelper.DeleteImageFile(ProfileImage_path);
            //}


            #endregion Validate Is File is PDF And MG // Validate PDF

            #region validation fails // !ModelState.IsValid
            // If validation fails
            if (!ModelState.IsValid)
            {
                ModelState.Remove("Passport_TempFilePath"); // its Important To Bind New Data Temp
                ModelState.Remove("Passport_OldPath"); // its Important To Bind New Data Temp
                ModelState.Remove("PassportImage"); // its Important To Bind New Data Temp
                ModelState.Remove("IdImage_TempFilePath"); // its Important To Bind New Data Temp
                ModelState.Remove("IdImage_OldPath"); // its Important To Bind New Data Temp
                ModelState.Remove("IdImage"); // its Important To Bind New Data Temp
                ModelState.Remove("ProfileImage_TempFilePath"); // its Important To Bind New Data Temp
                ModelState.Remove("ProfileImage_OldPath"); // its Important To Bind New Data Temp
                ModelState.Remove("ProfileImage"); // its Important To Bind New Data Temp
                if (Passport_Text != "OK" && Passport_Text != "null") { ModelState.AddModelError("PassportImage", Passport_Text??" "); }
                if (IdImage_Text != "OK" && IdImage_Text != "null") { ModelState.AddModelError("IdImage", IdImage_Text ?? " "); }
                if (ProfileImage_Text != "OK" && ProfileImage_Text != "null") { ModelState.AddModelError("ProfileImage", ProfileImage_Text ?? " "); }

                // If the user uploads a new file → cache it before returning
                if (model.IdImage != null)
                    model.IdImage_TempFilePath = await FileHelper.SaveTempAsync(model.IdImage);
                if (model.PassportImage != null)
                    model.Passport_TempFilePath = await FileHelper.SaveTempAsync(model.PassportImage);
                if (model.ProfileImage != null)
                    model.ProfileImage_TempFilePath = await FileHelper.SaveTempAsync(model.ProfileImage);
                // Refill dropdowns if validation fails
                var cities = await _unitOfWork.Cities.GetAllAsync();
                if (cities != null && cities.Count() > 0) cities = cities.OrderBy(x => x.Id);

                var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
                //Select Only United Arab Emirates 
                var nationalitiesOne = nationalities.Where(n => (!string.IsNullOrEmpty(n.NameAr) && n.NameAr.Contains("مارات", StringComparison.OrdinalIgnoreCase)));
                //model.NationalityId = nationalities.FirstOrDefault()?.Id;

                model.Nationalities = SelectListHelper.BindSelectList(nationalities.ToList(), model.NationalityId).ToList();
                model.NationalitiesOne = SelectListHelper.BindSelectList(nationalitiesOne.ToList(), model.NationalityId).ToList();

                var citySharqa = await _unitOfWork.Cities.GetAllAsync(x => x.NameAr != null && x.NameAr.Contains("فجير"));
                var sharqaId = citySharqa.FirstOrDefault()?.Id;
                model.CitiesList = SelectListHelper.BindSelectList(cities.ToList(), model.CityId ?? sharqaId).ToList();

                if (model.PhoneNumber != null && model.PhoneNumber.StartsWith("971")) // Is PhoneNumber StartsWith 971 Remove it
                    model.PhoneNumber = model.PhoneNumber.Substring(3);
                if (model.FatherPhone != null && model.FatherPhone.StartsWith("971")) // Is FatherPhone StartsWith 971 Remove it
                    model.FatherPhone = model.FatherPhone.Substring(3);
                if (model.MotherPhone != null && model.MotherPhone.StartsWith("971")) // Is MotherPhone StartsWith 971 Remove it
                    model.MotherPhone = model.MotherPhone.Substring(3);

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
            if (model.IdImage != null)
            {
                FileHelper.DeleteImageFile(model.IdImage_OldPath);
                model.IdImagePath = await FileHelper.SaveImageAsync(model.IdImage, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.IdImage_TempFilePath))
            {
                FileHelper.DeleteImageFile(model.IdImage_OldPath);
                model.IdImagePath = FileHelper.MoveTempToFinal(
                    model.IdImage_TempFilePath,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.IdImagePath = model.IdImage_OldPath;
            }

            // 1) If there is a new file uploaded by the user
            if (model.PassportImage != null)
            {
                FileHelper.DeleteImageFile(model.Passport_OldPath);
                model.PassportImagePath = await FileHelper.SaveImageAsync(model.PassportImage, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.Passport_TempFilePath))
            {
                FileHelper.DeleteImageFile(model.Passport_OldPath);
                model.PassportImagePath = FileHelper.MoveTempToFinal(
                    model.Passport_TempFilePath,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.PassportImagePath = model.Passport_OldPath;
            }

            // 1) If there is a new file uploaded by the user
            if (model.ProfileImage != null)
            {
                FileHelper.DeleteImageFile(model.ProfileImage_OldPath);
                model.ProfileImagePath = await FileHelper.SaveImageAsync(model.ProfileImage, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.ProfileImage_TempFilePath))
            {
                FileHelper.DeleteImageFile(model.ProfileImage_OldPath);
                model.ProfileImagePath = FileHelper.MoveTempToFinal(
                    model.ProfileImage_TempFilePath,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.ProfileImagePath = model.ProfileImage_OldPath;
            }
            #endregion PROCESS FINAL FILE Handeling Save in newpath From Temp
            // ---------------------------------------
            // Map model → entity After Added New Path

            var entity = _mapper.Map<MemberEntity>(model);
            entity.IdImagePath = model.IdImagePath;
            entity.PassportImagePath = model.PassportImagePath;
            entity.ProfileImagePath = model.ProfileImagePath;

            // //PhoneNumber Dubai
            entity.PhoneNumber = await PhoneHelper.CheckAndDoPhoneStart971(entity.PhoneNumber);

            // //FatherPhone Dubai
            entity.FatherPhone = await PhoneHelper.CheckAndDoPhoneStart971(entity.FatherPhone);

            // //MotherPhone Dubai
            entity.MotherPhone = await PhoneHelper.CheckAndDoPhoneStart971(entity.MotherPhone);

            if (model.Id == 0)
            {
                model.Id = await _memberService.AddAsync(entity);
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else
                await _memberService.UpdateAsync(entity);


            var Member_AddEdit = SessionExtensions.GetString(HttpContext.Session, "Member_AddEdit");

            //if (Member_AddEdit == "SubscribedMembersInCourses")
            //{
            //    return RedirectToAction("SubscribedMembersInCourses","Course" ,new{ area = "admin" });

            //}
            //else if (Member_AddEdit == "Member.Index")
            //{
            //    return RedirectToAction(nameof(Index));
            //}
            //else
            //    return RedirectToAction(nameof(Index));

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit

        }
        [YesGet]
        public async Task<IActionResult> MemberDetails(int? id)
        {
            var vm = new MemberVM();

            if (id.HasValue && id.Value != 0)
            {
                var member = await _memberService.GetByIdAsync(id.Value);
                if (member == null) return NotFound();
                vm = _mapper.Map<MemberVM>(member);
            }
            else
            {
                vm.Code = await _memberService.GenerateNewCode();
            }

            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            var cities = await _unitOfWork.Cities.GetAllAsync();
            if (cities != null && cities.Count() > 0) cities = cities.OrderBy(x => x.Id);


            vm.NationalitiesList = SelectListHelper.BindSelectList(nationalities.ToList(), vm.NationalityId).ToList();
            vm.CitiesList = SelectListHelper.BindSelectList(cities.ToList(), vm.CityId).ToList();

            if (vm.PhoneNumber != null && vm.PhoneNumber.StartsWith("971")) // Is PhoneNumber StartsWith 971 Remove it
                vm.PhoneNumber = vm.PhoneNumber.Substring(3);
            if (vm.FatherPhone != null && vm.FatherPhone.StartsWith("971")) // Is FatherPhone StartsWith 971 Remove it
                vm.FatherPhone = vm.FatherPhone.Substring(3);
            if (vm.MotherPhone != null && vm.MotherPhone.StartsWith("971")) // Is MotherPhone StartsWith 971 Remove it
                vm.MotherPhone = vm.MotherPhone.Substring(3);

            vm.IdImage_OldPath = vm.IdImagePath;
            vm.Passport_OldPath = vm.PassportImagePath;
            vm.ProfileImage_OldPath = vm.ProfileImagePath;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _memberService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<JsonResult> Suspend(int id, bool suspend)
        {
            try
            {
                await _memberService.SuspendAsync(id, suspend);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
   

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, int? selectedMemberType, int? selectedGender, int? selectedNationality, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var allMembers = await _memberService.GetAllAsync();
            var memberVMs = _mapper.Map<List<MemberVM>>(allMembers).AsQueryable();

            #region search by word
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                memberVMs = memberVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.FullNameAr) && c.FullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.FullNameEn) && c.FullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.IdNumber) && c.IdNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Code.ToString()) && c.Code.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            #endregion

            #region search by dropdown
            if (selectedNationality != null)
            {
                memberVMs = memberVMs.Where(c => c.NationalityId == selectedNationality);
            }
            #endregion

            #region search by dropdown
            if (selectedGender != null)
            {
                memberVMs = memberVMs.Where(c => c.GenderId == selectedGender);
            }
            if (selectedMemberType != null && selectedMemberType != 33)
            {
                memberVMs = memberVMs.Where(c => c.MemberTypeId == selectedMemberType);
            }
            #endregion

            #region search by date
            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                memberVMs = memberVMs.Where(c => c.RegistrationDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                memberVMs = memberVMs.Where(c => c.RegistrationDate <= dateTo.Value);
            }
            #endregion

            return View(memberVMs);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, int? selectedMemberType, int? selectedGender, int? selectedNationality, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var allMembers = await _memberService.GetAllAsync();
            var memberVMs = _mapper.Map<List<MemberVM>>(allMembers).AsQueryable();

            #region search by word
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                memberVMs = memberVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.FullNameAr) && c.FullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.FullNameEn) && c.FullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.IdNumber) && c.IdNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Code.ToString()) && c.Code.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            #endregion

            #region search by dropdown
            if (selectedNationality != null)
            {
                memberVMs = memberVMs.Where(c => c.NationalityId == selectedNationality);
            }
            #endregion

            #region search by dropdown
            if (selectedGender != null)
            {
                memberVMs = memberVMs.Where(c => c.GenderId == selectedGender);
            }
            if (selectedMemberType != null && selectedMemberType!=33)
            {
                memberVMs = memberVMs.Where(c => c.MemberTypeId == selectedMemberType);
            }
            #endregion

            #region search by date
            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                memberVMs = memberVMs.Where(c => c.RegistrationDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                memberVMs = memberVMs.Where(c => c.RegistrationDate <= dateTo.Value);
            }
            #endregion
            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                var lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                var allData_list = memberVMs;
                var ListTitles = new List<string>
        {
            "St No.",@Resource1.Name,Resource2.IdNumber,
             Resource2.IdExpiryDate,Resource1.Phone,Resource2.Nationality,
              Resource2.Age,Resource2.RegistrationDate,Resource2.Type,
        };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 =  single.Code,
                        t2 = (lang == "ar" ? single.FullNameAr : single.FullNameEn),
                        t3 = single.IdNumber,
                        t4 = single.IdExpiryDate.HasValue ? single.IdExpiryDate.Value.ToString("d").Replace("/","-") :"",
                        t5 = single.PhoneNumber,
                        t6 = (single.Nationality!=null) ?(lang == "ar" ? single.Nationality.NameAr : single.Nationality.NameEn):"",
                        t7 = single.Age,
                        t8 = single.RegistrationDate.ToString("d").Replace("/","-"),
                        t9 = (single.MemberType != null) ? (lang == "ar" ? single.MemberType.NameAr : single.MemberType.NameEn) : "",
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
                    var fileExcelName = Resource1.Members2;
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



        //[IgnoreAction]
        //[YesGet]
        //public async Task<IActionResult> createExcelReport_Download_MemberCourse(int memberId)
        //{
        //    // ---- Start Get Data As Print
        //    //var memberId = Convert.ToInt32(TempData["memberId"]);
        //    var member = await _memberService.GetByIdAsync(memberId);

        //    ViewBag.MemberName = SessionHelper.GetCurrentLanguage() == "ar" ? member?.FullNameAr : member?.FullNameEn;
        //    // ---- End Get Data As Print

        //    var boolStatus = false;
        //    byte[]? fileBytes = null;
        //    var pathNewFile = "";
        //    try
        //    {

        //        var lang = SessionHelper.GetCurrentLanguage();
        //        //var allActivitys = await _ActivityService.GetAllAsync();
        //        var allData_list = new List<string>();
        //        var ListTitles = new List<string>
        //{
        //    Resource2.StartDate,Resource1.Department,Resource2.Location,Resource1.CourseTitle
        //};
        //        if (allData_list != null || allData_list?.Count() > 0)
        //        {
        //            var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
        //            {
        //                t1 = single.StartDate.HasValue?(single.StartDate.Value.ToString("d")?.Replace("/","-")) :"",
        //                t2 = (single.Department !=null)? (lang == "ar" ? single.Department.NameAr : single.Department.NameEn):"",
        //                t3 = single.Location,
        //                t4 = (lang == "ar" ? single.TitleAr : single.TitleEn),
        //            }).ToList();

        //            if (lang == "ar")
        //            {
        //                (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "ar");
        //            }
        //            else
        //            {
        //                (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "en");
        //            }
        //        }

        //        FileContentResult? Excelfile = null;
        //        if (fileBytes != null && fileBytes.Length > 0 && boolStatus == true)
        //        {
        //            var fileExcelName = Resource2.CoursesOfMember +" : "+ ViewBag.MemberName;
        //            Excelfile = File(fileBytes,
        //                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
        //                $"{fileExcelName}_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
        //        }
        //        return Excelfile;


        //    }
        //    catch (Exception ex)
        //    {
        //        return RedirectToAction("Index");
        //    }
        //}
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintDetails(int? id)
        {
            var vm = new MemberVM();

            if (id.HasValue && id.Value != 0)
            {
                var member = await _memberService.GetByIdAsync(id.Value);
                if (member == null) return NotFound();
                vm = _mapper.Map<MemberVM>(member);
            }

            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            var cities = await _unitOfWork.Cities.GetAllAsync();
            if (cities != null && cities.Count() > 0) cities = cities.OrderBy(x => x.Id);


            vm.NationalitiesList = SelectListHelper.BindSelectList(nationalities.ToList(), vm.NationalityId).ToList();
            vm.CitiesList = SelectListHelper.BindSelectList(cities.ToList(), vm.CityId).ToList();
            ViewBag.Professions = SelectListHelper.GetEnumSelectList<Profession>().Distinct();
            ViewBag.Genders = SelectListHelper.GetEnumSelectList<Gender>().Distinct();
            ViewBag.HeardBySources = SelectListHelper.GetEnumSelectList<HeardBySources>().Distinct();

            return View(vm);
        }

        [IgnoreAction]
        [HttpGet]
        public async Task<IActionResult> ViewData(int? id)
        {
            var vm = new MemberVM();

            if (id.HasValue && id.Value != 0)
            {
                var member = await _memberService.GetByIdAsync(id.Value);
                if (member == null) return NotFound();
                vm = _mapper.Map<MemberVM>(member);
            }
            else
            {
                vm.Code = await _memberService.GenerateNewCode();
            }

            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            var cities = await _unitOfWork.Cities.GetAllAsync();
            if (cities != null && cities.Count() > 0) cities = cities.OrderBy(x => x.Id);


            vm.NationalitiesList = SelectListHelper.BindSelectList(nationalities.ToList(), vm.NationalityId).ToList();
            vm.CitiesList = SelectListHelper.BindSelectList(cities.ToList(), vm.CityId).ToList();

            return View(vm);
        }
    }
}
