using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.DTOs.Admin.Employees;
using Domain.Entities.Employees;
using Domain.Resources;
using KhaledTeamRecycling.Areas.Admin.ViewModels.Employees;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Helpers;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KhaledTeamRecycling.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class EmployeesController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmployeeService _employeeService;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _webHostEnvironment;


        public EmployeesController(IUnitOfWork unitOfWork, IEmployeeService employeeService, IMapper mapper, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _employeeService = employeeService;
            _mapper = mapper;
            _webHostEnvironment = webHostEnvironment;
        }
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var query = _unitOfWork.Employees.Table;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.FullNameAr.Contains(searchTerm) || e.FullNameEn.Contains(searchTerm) || e.NationalIdNumber.Contains(searchTerm) || e.PhoneNumber.Contains(searchTerm));
            }
            if (dateFrom.HasValue)
            {
                query = query.Where(e => e.NationalIdExpiryDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(e => e.NationalIdExpiryDate <= dateTo.Value);
            }
            foreach(var emp in query)
            {
                emp.HasRelatedObjects = await _employeeService.HasRelatedObjectsInDb(emp.Id);
            }

            var totalRecords = await query.CountAsync();
            var employees = await query
                .OrderBy(e => e.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new EmployeeVM
            {
                all_EmployeeListVM = employees,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                SearchString = searchTerm,
                StartDate = dateFrom,
                EndDate = dateTo,
                TotalCount = totalRecords,
                PageSize = pageSize,
                HasNextPage = false,
                HasPreviousPage = false
            };
            if (totalRecords > pageSize * page)
            {
                model.HasNextPage = true;
            }
            if (page > 1) model.HasPreviousPage = true;

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", model);
            }

            return View(model);
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            var vm = new EmployeeVM();

            if (id.HasValue && id.Value != 0)
            {
                var member = await _employeeService.GetByIdAsync(id.Value);
                if (member == null) return NotFound();
                vm = _mapper.Map<EmployeeVM>(member);
            }

            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            vm.NationalitiesList = SelectListHelper.BindSelectList(nationalities.ToList(), vm.NationalityId).ToList();

            if (vm.PhoneNumber != null && vm.PhoneNumber.StartsWith("971")) // Is Phone StartsWith 971 Remove it
                vm.PhoneNumber = vm.PhoneNumber.Substring(3);

            vm.Photo_OldPath = vm.PhotoPath;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(EmployeeVM model)
        {
            //var jobs = await _unitOfWork.Jobs.GetAllAsync();
            //var alljobsVM = _mapper.Map<IEnumerable<JobVM>>(jobs);
            //ViewBag.allJobs = alljobsVM;

            var FolderEntityWillSaveIn = "EmployeesAttachments";
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

            var Photo_Text = await ValidateImageAsync(model.Photo_TempFilePath, model.Photo, model.PhotoPath, "Photo");

            if (Photo_Text != "OK" && Photo_Text != "null") ModelState.AddModelError("Photo", Photo_Text);

            #endregion Validate Is File is PDF And MG // Validate PDF

            #region validation fails // !ModelState.IsValid
            // If validation fails
            if (!ModelState.IsValid)
            {
                ModelState.Remove("Photo_TempFilePath"); // its Important To Bind New Data Temp
                ModelState.Remove("Photo_OldPath"); // its Important To Bind New Data Temp
                ModelState.Remove("Photo"); // its Important To Bind New Data Temp
                if (Photo_Text != "OK" && Photo_Text != "null") { ModelState.AddModelError("Photo", Photo_Text ?? " "); }


                // If the user uploads a new file → cache it before returning
                if (model.Photo != null)
                    model.Photo_TempFilePath = await FileHelper.SaveTempAsync(model.Photo);

                // Refill dropdowns if validation fails
                var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
                model.NationalitiesList = SelectListHelper.BindSelectList(nationalities.ToList(), model.NationalityId).ToList();

                if (model.PhoneNumber != null && model.PhoneNumber.StartsWith("971")) // Is Phone StartsWith 971 Remove it
                    model.PhoneNumber = model.PhoneNumber.Substring(3);

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
            if (model.Photo != null)
            {
                FileHelper.DeleteImageFile(model.Photo_OldPath);
                model.PhotoPath = await FileHelper.SaveImageAsync(model.Photo, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.Photo_TempFilePath))
            {
                FileHelper.DeleteImageFile(model.Photo_OldPath);
                model.PhotoPath = FileHelper.MoveTempToFinal(
                    model.Photo_TempFilePath,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.PhotoPath = model.Photo_OldPath;
            }

            #endregion PROCESS FINAL FILE Handeling Save in newpath From Temp
            // ---------------------------------------


            // //Phone Dubai
            model.PhoneNumber = await PhoneHelper.CheckAndDoPhoneStart971(model.PhoneNumber);
            
            var entity = _mapper.Map<Employee>(model);
            entity.PhotoPath = model.PhotoPath;
            if (model.Id == 0 || model.Id == null)
            {
                model.Id = await _employeeService.AddAsync(entity);
                return RedirectToAction("index"); // After Add 
            }
            else
                await _employeeService.UpdateAsync(entity);

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit 
        }


        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Details(int id)
        {
            // for get The Previous URL
            var refererURL = Request.Headers["Referer"].ToString();
            ViewBag.refererURL = refererURL;

            ViewBag.id = id;
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null) return NotFound();
            var thisNation = await _unitOfWork.Nationalities.GetByIdAsync(employee.NationalityId);
            var lang = SessionHelper.GetCurrentLanguage();
            var Nationality = lang == "ar" ? thisNation?.NameAr : thisNation?.NameEn;

            var model = new EmployeeVM
            {
                Id = employee.Id,
                Code = employee.Code,
                FullNameAr = employee.FullNameAr,
                FullNameEn = employee.FullNameEn,
                PhoneNumber = employee.PhoneNumber,
                Email = employee.Email,
                Address = employee.Address,
                NationalityId = employee.NationalityId,
                Nationality1 = Nationality,
                JobTitle1 = employee.JobTitle,
                PassportNumber = employee.PassportNumber,
                PassportExpiryDate = employee.PassportExpiryDate,
                NationalIdNumber = employee.NationalIdNumber,
                NationalIdExpiryDate = employee.NationalIdExpiryDate,
                BankAccountNumber = employee.BankAccountNumber,
                BankName = employee.BankName,
                Salary = employee.Salary,
                Notes = employee.Notes,
                NationalIdLocation = employee.NationalIdLocation,
                PhotoPath = "../../" + employee.PhotoPath
            };

            // لو Model يحتوي Lists
            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            model.NationalitiesList = SelectListHelper.BindSelectList(nationalities.ToList(), model.NationalityId).ToList();

            return View(model);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintDetails(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null) return NotFound();
            var thisNation = await _unitOfWork.Nationalities.GetByIdAsync(employee.NationalityId);
            var lang = SessionHelper.GetCurrentLanguage();
            var Nationality = lang == "ar" ? thisNation?.NameAr : thisNation?.NameEn;

            var model = new EmployeeVM
            {
                Id = employee.Id,
                Code = employee.Code,
                FullNameAr = employee.FullNameAr,
                FullNameEn = employee.FullNameEn,
                PhoneNumber = employee.PhoneNumber,
                Email = employee.Email,
                Address = employee.Address,
                NationalityId = employee.NationalityId,
                Nationality1 = Nationality,
                JobTitle1 = employee.JobTitle,
                PassportNumber = employee.PassportNumber,
                PassportExpiryDate = employee.PassportExpiryDate,
                NationalIdNumber = employee.NationalIdNumber,
                NationalIdExpiryDate = employee.NationalIdExpiryDate,
                BankAccountNumber = employee.BankAccountNumber,
                BankName = employee.BankName,
                Salary = employee.Salary,
                Notes = employee.Notes,
                NationalIdLocation = employee.NationalIdLocation,
                PhotoPath = "../../" + employee.PhotoPath
            };

            // لو Model يحتوي Lists
            var nationalities = await _unitOfWork.Nationalities.GetAllAsync();
            model.NationalitiesList = SelectListHelper.BindSelectList(nationalities.ToList(), model.NationalityId).ToList();
            return View(model);
        }

        [YesGet]
        public async Task<IActionResult> GetEmployeeAttachments(int id)
        {
            var employeeAttachments = await _employeeService.GetAttachmentsAsync(id);
            var attachmentsVM = _mapper.Map<List<AttachmentVM>>(employeeAttachments);
            var model = new EmployeeAttachmentsVM
            {
                EmployeeId = id,
                Attachments = attachmentsVM
            };
            return PartialView("_EmployeeAttachmentsModal", model);
        }
        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadEmployeeFiles(EmployeeAttachmentsVM model)
        {
            if (model == null)
                return Json(new { success = false, message = "Files uploaded not done" });
            var dto = _mapper.Map<EmployeeAttachmentsDTO>(model);
            await _employeeService.UploadAttachmentsAsync(dto);

            return RedirectToAction("Index");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == 0) return NotFound();

            await _employeeService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }


        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var query = _unitOfWork.Employees.Table;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.FullNameAr.Contains(searchTerm) || e.FullNameEn.Contains(searchTerm) || e.NationalIdNumber.Contains(searchTerm) || e.PhoneNumber.Contains(searchTerm));
            }
            if (dateFrom.HasValue)
            {
                query = query.Where(e => e.NationalIdExpiryDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(e => e.NationalIdExpiryDate <= dateTo.Value);
            }

            var totalRecords = await query.CountAsync();
            var employees = await query
                .OrderBy(e => e.Id)
                .ToListAsync();

            var model = new EmployeeVM
            {
                all_EmployeeListVM = employees,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                SearchString = searchTerm,
                StartDate = dateFrom,
                EndDate = dateTo,
                TotalCount = totalRecords
            };

            return View(model);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var query = _unitOfWork.Employees.Table;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(e => e.FullNameAr.Contains(searchTerm) || e.FullNameEn.Contains(searchTerm) || e.NationalIdNumber.Contains(searchTerm) || e.PhoneNumber.Contains(searchTerm));
            }
            if (dateFrom.HasValue)
            {
                query = query.Where(e => e.NationalIdExpiryDate >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(e => e.NationalIdExpiryDate <= dateTo.Value);
            }

            var totalRecords = await query.CountAsync();
            var employees = await query
                .OrderBy(e => e.Id)
                .ToListAsync();

            var model = new EmployeeVM
            {
                all_EmployeeListVM = employees,
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                SearchString = searchTerm,
                StartDate = dateFrom,
                EndDate = dateTo,
                TotalCount = totalRecords
            };
            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                var lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                var allData_list = model.all_EmployeeListVM;
                var ListTitles = new List<string>
        {
            Resource1.Name,Resource1.JobTitle1,Resource1.NationalIdNumber1,Resource1.Salary,Resource1.NationalIdExpiryDate,Resource1.PhoneNumber,
        };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = (lang == "ar" ? single.FullNameAr : single.FullNameEn),
                        t2 = single.JobTitle,
                        t3 = single.NationalIdNumber,
                        t4 = single.Salary,
                        t5 = (single.NationalIdExpiryDate.HasValue) ? single.NationalIdExpiryDate.Value.ToString("d")?.Replace("/","-") : "",
                        t6 = single.PhoneNumber,
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
                    var fileExcelName = Resource1.EmployeesList;
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
    }
}
