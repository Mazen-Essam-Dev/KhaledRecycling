using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities.MonthlyAdministrativeReport;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.MonthlyAdministrativeReport;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Hub;
using FougeraClub.Middelware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;


namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class MonthlyAdministrativeReportController : Controller
    {
        private readonly IMonthlyAdministrativeReportService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;
        private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationService _notificationService; 
        public MonthlyAdministrativeReportController(IMonthlyAdministrativeReportService monthlyAdministrativeReportService, IHttpContextAccessor httpContextAccessor, IMapper mapper, UserManager<Infrastructure.Identity.ApplicationUser> userManager, IHubContext<NotificationHub> hubContext, INotificationService notificationService)
        {
            _service = monthlyAdministrativeReportService;
            _mapper = mapper;
            _userManager = userManager;
            _hubContext = hubContext;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
        }


        [IgnoreAction]
        [NoLogging]
        [HttpPost]
        public async Task<IActionResult> SendOtp(int id, string role)
        {
            // role: "trainer" or "manager"
            var result = await _service.SendOtpAsync(id, role);
            return Json(new { success = result });
        }
        [IgnoreAction]
        [HttpPost]
        public async Task<IActionResult> ValidateOtp([FromBody] FougeraClub.Areas.Admin.ViewModels.SMS.OtpValidationRequest request)
        {
            // request: { id, code, role }
            var result = await _service.ValidateOtpAsync((int)request.Id, request.Code, request.Role, User);

            if (result.success == true)
            {
                if (request.Role == "trainer")
                {
                    await _hubContext.Clients.Groups("Manager")
                        .SendAsync("ReceiveNotification", new
                        {
                            Title = "",
                            Message = ""
                        });
                    await _notificationService.SendNotificationToRoleAsync(
                          "تقرير إداري/نشاط شهري جديد",
                          "يوجد تقرير إداري/نشاط شهري جديد جاهز للإعتماد",
                          (int)RoleNumber.Manager
                      );
                }
            }
            return Json(new { success = result.success, message = result.message });
        }
        [YesGet]
        public async Task<IActionResult> Index(int? selectedYear, int? selectedMonth, int page = 1, int pageSize = 50)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var monthlyAdministrativeReports = await _service.GetAllAsync();

            // Graduation Year Dropdown
            var years = await _service.GetAllYearsInDb();
            var months = Enumerable.Range(1, 12).Select(m => new SelectListItem
            {
                Value = m.ToString(),
                Text = new DateTime(1, m, 1).ToString("MMMM")
            }).ToList();

            ViewBag.Years = years.ToList();
            ViewBag.Months = months;

            var model = _mapper.Map<List<MonthlyAdministrativeReportVM>>(monthlyAdministrativeReports).AsQueryable();
            foreach (var item in model)
            {
                if (item.Type != null)
                    item.TypeText = EnumHelper.GetDisplayName((MonthlyAdministrativeReportType)item.Type);
            }

            if (selectedYear != null)
                model = model.Where(c => c.Date.HasValue && c.Date.Value.Year == selectedYear);

            if (selectedMonth != null)
                model = model.Where(c => c.Date.HasValue && c.Date.Value.Month == selectedMonth);

            var paginated = PaginatedList<MonthlyAdministrativeReportVM>.Create(model, page, pageSize);

            RoleNumber ValidateRoleNumber()
            {
                var roleNumber = _httpContextAccessor.HttpContext?.Session.GetInt32("RoleNumber");
                if (roleNumber != null)
                {
                    return (RoleNumber)roleNumber;
                }
                return RoleNumber.NormalUser;
            }

            var IsUserLogedInISManager = ValidateRoleNumber() == RoleNumber.Manager;

            paginated.IsUserLogedInISManager = IsUserLogedInISManager;

            ViewBag.SelectedYear = selectedYear;
            ViewBag.SelectedMonth = selectedMonth;

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }


        public async Task<IActionResult> AddEdit(int? id)
        {
            var vm = new MonthlyAdministrativeReportVM();

            if (id.HasValue && id.Value != 0)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item == null) return NotFound();
                vm = _mapper.Map<MonthlyAdministrativeReportVM>(item);
            }

            vm.TypeEnumList = SelectListHelper.GetEnumSelectList<MonthlyAdministrativeReportType>();

            vm.Old1_Path = vm.Image1Path;
            vm.Old2_Path = vm.Image2Path;
            vm.Old3_Path = vm.Image3Path;
            vm.Old4_Path = vm.Image4Path;
            vm.Old5_Path = vm.Image5Path;
            vm.Old6_Path = vm.Image6Path;
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(MonthlyAdministrativeReportVM model)
        {
            var type = (MonthlyAdministrativeReportType)(model.Type ?? 0);
            var isMonthRegistedBefore = await _service.CheckIsMonthRegistedBefore(model.Id, model.Date, type);

            var FolderEntityWillSaveIn = "Monthly Administrative";
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

            var file1_Text = await ValidateImageAsync(model.TempFile1_Path, model.Image1, model.Image1Path, "Image1");
            var file2_Text = await ValidateImageAsync(model.TempFile2_Path, model.Image2, model.Image2Path, "Image2");
            var file3_Text = await ValidateImageAsync(model.TempFile3_Path, model.Image3, model.Image3Path, "Image3");
            var file4_Text = await ValidateImageAsync(model.TempFile4_Path, model.Image4, model.Image4Path, "Image4");
            var file5_Text = await ValidateImageAsync(model.TempFile5_Path, model.Image5, model.Image5Path, "Image5");
            var file6_Text = await ValidateImageAsync(model.TempFile6_Path, model.Image6, model.Image6Path, "Image6");

            if (file1_Text != "OK" && file1_Text != "null") ModelState.AddModelError("Image1", file1_Text??" ");
            if (file2_Text != "OK" && file2_Text != "null") ModelState.AddModelError("Image2", file2_Text ?? " ");
            if (file3_Text != "OK" && file3_Text != "null") ModelState.AddModelError("Image3", file3_Text ?? " ");
            if (file4_Text != "OK" && file4_Text != "null") ModelState.AddModelError("Image4", file4_Text ?? " ");
            if (file5_Text != "OK" && file5_Text != "null") ModelState.AddModelError("Image5", file5_Text ?? " ");
            if (file6_Text != "OK" && file6_Text != "null") ModelState.AddModelError("Image6", file6_Text ?? " ");

            #endregion Validate Is File is PDF And MG // Validate PDF

            #region validation fails // !ModelState.IsValid
            // If validation fails
            if (!ModelState.IsValid || isMonthRegistedBefore)
            {
                #region Remove ModelState Image Errors To Rebind New Data Temp
                ModelState.Remove("TempFile1_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old1_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Image1"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile2_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old2_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Image2"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile3_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old3_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Image3"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile4_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old4_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Image4"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile5_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old5_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Image5"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile6_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old6_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Image6"); // its Important To Bind New Data Temp
                #endregion Remove ModelState Image Errors To Rebind New Data Temp
                if (file1_Text != "OK" && file1_Text != "null") ModelState.AddModelError("Image1", file1_Text ?? " ");
                if (file2_Text != "OK" && file2_Text != "null") ModelState.AddModelError("Image2", file2_Text ?? " ");
                if (file3_Text != "OK" && file3_Text != "null") ModelState.AddModelError("Image3", file3_Text ?? " ");
                if (file4_Text != "OK" && file4_Text != "null") ModelState.AddModelError("Image4", file4_Text ?? " ");
                if (file5_Text != "OK" && file5_Text != "null") ModelState.AddModelError("Image5", file5_Text ?? " ");
                if (file6_Text != "OK" && file6_Text != "null") ModelState.AddModelError("Image6", file6_Text ?? " ");

                // If the user uploads a new file → cache it before returning
                if (model.Image1 != null)
                    model.TempFile1_Path = await FileHelper.SaveTempAsync(model.Image1);
                if (model.Image2 != null)
                    model.TempFile2_Path = await FileHelper.SaveTempAsync(model.Image2);
                if (model.Image3 != null)
                    model.TempFile3_Path = await FileHelper.SaveTempAsync(model.Image3);
                if (model.Image4 != null)
                    model.TempFile4_Path = await FileHelper.SaveTempAsync(model.Image4);
                if (model.Image5 != null)
                    model.TempFile5_Path = await FileHelper.SaveTempAsync(model.Image5);
                if (model.Image6 != null)
                    model.TempFile6_Path = await FileHelper.SaveTempAsync(model.Image6);

                // Re-fall and Re-populate enum select list
                model.TypeEnumList = SelectListHelper.GetEnumSelectList<MonthlyAdministrativeReportType>();
                ModelState.AddModelError("Type", Resource2.ReportIsExist);

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
            if (model.Image1 != null)
            {
                FileHelper.DeleteImageFile(model.Old1_Path);
                model.Image1Path = await FileHelper.SaveImageAsync(model.Image1, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile1_Path))
            {
                FileHelper.DeleteImageFile(model.Old1_Path);
                model.Image1Path = FileHelper.MoveTempToFinal(
                    model.TempFile1_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.Image1Path = model.Old1_Path;
            }


            // 1) If there is a new file uploaded by the user
            if (model.Image2 != null)
            {
                FileHelper.DeleteImageFile(model.Old2_Path);
                model.Image2Path = await FileHelper.SaveImageAsync(model.Image2, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile2_Path))
            {
                FileHelper.DeleteImageFile(model.Old2_Path);
                model.Image2Path = FileHelper.MoveTempToFinal(
                    model.TempFile2_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.Image2Path = model.Old2_Path;
            }

            // 1) If there is a new file uploaded by the user
            if (model.Image3 != null)
            {
                FileHelper.DeleteImageFile(model.Old3_Path);
                model.Image3Path = await FileHelper.SaveImageAsync(model.Image3, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile3_Path))
            {
                FileHelper.DeleteImageFile(model.Old3_Path);
                model.Image3Path = FileHelper.MoveTempToFinal(
                    model.TempFile3_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.Image3Path = model.Old3_Path;
            }

            // 1) If there is a new file uploaded by the user
            if (model.Image4 != null)
            {
                FileHelper.DeleteImageFile(model.Old4_Path);
                model.Image4Path = await FileHelper.SaveImageAsync(model.Image4, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile4_Path))
            {
                FileHelper.DeleteImageFile(model.Old4_Path);
                model.Image4Path = FileHelper.MoveTempToFinal(
                    model.TempFile4_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.Image4Path = model.Old4_Path;
            }

            // 1) If there is a new file uploaded by the user
            if (model.Image5 != null)
            {
                FileHelper.DeleteImageFile(model.Old5_Path);
                model.Image5Path = await FileHelper.SaveImageAsync(model.Image5, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile5_Path))
            {
                FileHelper.DeleteImageFile(model.Old5_Path);
                model.Image5Path = FileHelper.MoveTempToFinal(
                    model.TempFile5_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.Image5Path = model.Old5_Path;
            }

            // 1) If there is a new file uploaded by the user
            if (model.Image6 != null)
            {
                FileHelper.DeleteImageFile(model.Old6_Path);
                model.Image6Path = await FileHelper.SaveImageAsync(model.Image6, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile6_Path))
            {
                FileHelper.DeleteImageFile(model.Old6_Path);
                model.Image6Path = FileHelper.MoveTempToFinal(
                    model.TempFile6_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.Image6Path = model.Old6_Path;
            }
            #endregion PROCESS FINAL FILE Handeling Save in newpath From Temp
            // ---------------------------------------


            var entity = _mapper.Map<MonthlyAdministrativeReport>(model);
            entity.Image1Path = model.Image1Path;
            entity.Image2Path = model.Image2Path;
            entity.Image3Path = model.Image3Path;
            entity.Image4Path = model.Image4Path;
            entity.Image5Path = model.Image5Path;
            entity.Image6Path = model.Image6Path;

            int? month = model.Date.HasValue ? model.Date.Value.Month : 0;
            string quarter = " ";
            string annualy = " ";
            string typetext = " ";
            bool isQuarterNotify = false;
            if (model.Type != null)
                typetext = EnumHelper.GetDisplayName((QuarterlyReportType)model.Type);

            if (month == 3)
            {
                quarter = EnumHelper.GetDisplayName((QuartersYear)QuartersYear.Quareter1);
                isQuarterNotify = true;
            }
            else if (month == 6)
            {
                quarter = EnumHelper.GetDisplayName((QuartersYear)QuartersYear.Quareter2);
                isQuarterNotify = true;
            }
            else if (month == 9)
            {
                quarter = EnumHelper.GetDisplayName((QuartersYear)QuartersYear.Quareter3);
                isQuarterNotify = true;
            }
            else if (month == 12)
            {
                quarter = EnumHelper.GetDisplayName((QuartersYear)QuartersYear.Quareter4);
                annualy = EnumHelper.GetDisplayName((QuartersYear)QuartersYear.yearFull);
                isQuarterNotify = true;
            }

            if (model.Id == 0)
            {
                model.Id = await _service.AddAsync(entity);
                await _hubContext.Clients.Groups("ActivitiesSupervisor")
                  .SendAsync("ReceiveNotification", new
                  {
                      Title = "",
                      Message = ""
                  });


                //await _hubContext.Clients.Groups("ActivitiesSupervisor")
                //  .SendAsync("ReceiveNotification", new
                //  {
                //      Title = "",
                //      Message = ""
                //  });
                

                await _notificationService.SendNotificationToRoleAsync(
                      $"تقرير {typetext} شهري جديد",
                      $"يوجد تقرير {typetext} شهري جديد جاهز للإعتماد",
                    (int)RoleNumber.ActivitiesSupervisor
                  );
                
                if (isQuarterNotify)
                {
                    await _notificationService.SendNotificationToRoleAsync(
                        $"تقرير {quarter} {typetext} جديد",
                        $"يوجد تقرير {quarter} {typetext} جديد جاهز للإعتماد",
                        (int)RoleNumber.Manager
                    );
                }
                
                if (month == 12)
                {
                    await _notificationService.SendNotificationToRoleAsync(
                        $"تقرير {annualy} مجمع جديد",
                        $"يوجد تقرير {annualy} مجمع جديد جاهز للإعتماد",
                        (int)RoleNumber.Manager
                    );
                }
              
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else // Edit
            {
                // Update existing entity — do NOT call AddAsync for edits (this caused duplicate tracking)
                await _service.UpdateAsync(entity);

                await _hubContext.Clients.Groups("ActivitiesSupervisor")
                  .SendAsync("ReceiveNotification", new
                  {
                      Title = "",
                      Message = ""
                  });

                await _notificationService.SendNotificationToRoleAsync(
                      $"تقرير {typetext} شهري تم تحديثه",
                      $"يوجد تقرير {typetext} شهري تم تحديثه جاهز للإعتماد",
                    (int)RoleNumber.ActivitiesSupervisor
                  );

                if (isQuarterNotify)
                {
                    await _notificationService.SendNotificationToRoleAsync(
                        $"تقرير {quarter} {typetext} تم تحديثه",
                        $"يوجد تقرير {quarter} {typetext} تم تحديثه جاهز للإعتماد",
                        (int)RoleNumber.Manager
                    );
                }

                if (month == 12)
                {
                    await _notificationService.SendNotificationToRoleAsync(
                        $"تقرير {annualy} مجمع تم تحديثه",
                        $"يوجد تقرير {annualy} مجمع تم تحديثه جاهز للإعتماد",
                        (int)RoleNumber.Manager
                    );
                }
            }

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit
        }
        [YesGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id.HasValue && id.Value != 0)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item == null) return NotFound();
                var vm = _mapper.Map<MonthlyAdministrativeReportVM>(item);
                if (vm.Type != null)
                    vm.TypeText = EnumHelper.GetDisplayName((MonthlyAdministrativeReportType)vm.Type);

                // Set SupervisorFullName if TrainerSignature exists
                if (vm.TrainerSignature != null && !string.IsNullOrEmpty(vm.TrainerSignature.UserId))
                {
                    var supervisor = await _userManager.FindByIdAsync(vm.TrainerSignature.UserId);
                    vm.SupervisorFullName = supervisor?.FullNameAr ?? supervisor?.FullNameEn ?? supervisor?.UserName ?? supervisor?.Email ?? "";
                }
                // Set ManagerFullName if ManagerSignature exists
                if (vm.ManagerSignature != null && !string.IsNullOrEmpty(vm.ManagerSignature.UserId))
                {
                    var manager = await _userManager.FindByIdAsync(vm.ManagerSignature.UserId);
                    vm.ManagerFullName = manager?.FullNameAr ?? manager?.FullNameEn ?? manager?.UserName ?? manager?.Email ?? "";
                }

                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(int? selectedYear, int? selectedMonth)
        {
            var monthlyAdministrativeReports = await _service.GetAllAsync();
            var model = _mapper.Map<List<MonthlyAdministrativeReportVM>>(monthlyAdministrativeReports).AsQueryable();
            if (selectedYear != null)
                model = model.Where(c => c.Date.HasValue && c.Date.Value.Year == selectedYear);

            if (selectedMonth != null)
                model = model.Where(c => c.Date.HasValue && c.Date.Value.Month == selectedMonth);

            foreach (var item in model)
            {
                if (item.Type != null)
                    item.TypeText = EnumHelper.GetDisplayName((MonthlyAdministrativeReportType)item.Type);
            }

            return View(model);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintDetails(int? id)
        {
            if (id.HasValue && id.Value != 0)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item == null) return NotFound();
                var vm = _mapper.Map<MonthlyAdministrativeReportVM>(item);
                if (vm.Type != null)
                    vm.TypeText = Enum.GetName(typeof(MonthlyAdministrativeReportType), vm.Type);
                return View(vm);
            }
            else
            {
                return NotFound();
            }
        }

        [IgnoreAction]
        [NoLogging]
        [HttpPost]
        public async Task<IActionResult> CheckDate(int id, DateOnly date, MonthlyAdministrativeReportType type)
        {

            // Example logic: only allow future dates
            var isMonthRegistedBefore = await _service.CheckIsMonthRegistedBefore(id, date, type);

            return Json(isMonthRegistedBefore);
        }


        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(int? selectedYear, int? selectedMonth)
        {
            var monthlyAdministrativeReports = await _service.GetAllAsync();
            var model = _mapper.Map<List<MonthlyAdministrativeReportVM>>(monthlyAdministrativeReports).AsQueryable();

            if (selectedYear != null)
                model = model.Where(c => c.Date.HasValue && c.Date.Value.Year == selectedYear);

            if (selectedMonth != null)
                model = model.Where(c => c.Date.HasValue && c.Date.Value.Month == selectedMonth);

            foreach (var item in model)
            {
                if (item.Type != null)
                    item.TypeText = EnumHelper.GetDisplayName((MonthlyAdministrativeReportType)item.Type);
            }


            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {
                var lang = SessionHelper.GetCurrentLanguage();
                var allData_list = model;
                var ListTitles = new List<string>
                {
                    Resource2.Year,Resource2.Month,Resource2.Type
                };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = single.Date.HasValue ? single.Date.Value.ToString("yyyy") : "",
                        t2 = single.Date.HasValue ? single.Date.Value.ToString("MMMM").ToString() : "",
                        t3 = single.TypeText
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
                    var fileExcelName = Resource1.MonthlyAdministrativeReportList;
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