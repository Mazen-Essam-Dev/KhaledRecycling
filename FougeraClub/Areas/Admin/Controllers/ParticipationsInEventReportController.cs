using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Azure.Core;
using Domain.DTOs;
using Domain.Entities.ParticipationsInEventReport;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.ParticipationsInEventReport;
using FougeraClub.Areas.Admin.ViewModels.SMS;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;


namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class ParticipationsInEventReportController : Controller
    {
        private readonly IParticipationsInEventReportService _service;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IHubContext<Hub.NotificationHub> _hubContext;


        public ParticipationsInEventReportController(IParticipationsInEventReportService ParticipationsInEventReportService, IHttpContextAccessor httpContextAccessor, IMapper mapper, UserManager<Infrastructure.Identity.ApplicationUser> userManager , INotificationService notificationService, IHubContext<Hub.NotificationHub> hubContext)
        {
            _service = ParticipationsInEventReportService;
            _mapper = mapper;
            _userManager = userManager;
            _notificationService = notificationService;
            _hubContext = hubContext;
            _httpContextAccessor = httpContextAccessor;
        }


        [HttpPost]
        [NoLogging]
        [IgnoreAction]
        public async Task<IActionResult> SendOtp(int id, string role)
        {
            // role: "trainer" or "manager"
            var result = await _service.SendOtpAsync(id, role);
            return Json(new { success = result });
        }
        [IgnoreAction]
        [HttpPost]
        public async Task<IActionResult> ValidateOtp([FromBody] OtpValidationRequest request)
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
                          "مشاركة في حدث جديدة",
                          $"يوجد مشاركة في حدث رقم {(int)request.Id} جديدة جاهزة للإعتماد",
                          2
                      );
                }
            }
            return Json(new { success = result.success, message = result.message });
        }
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var allParticipationsInEventReports = await _service.GetAllAsync();

            var ParticipationsInEventReportVMs = _mapper.Map<List<ParticipationsInEventReportVM>>(allParticipationsInEventReports).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                ParticipationsInEventReportVMs = ParticipationsInEventReportVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.ReportTitle) && c.ReportTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }


            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                ParticipationsInEventReportVMs = ParticipationsInEventReportVMs.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                ParticipationsInEventReportVMs = ParticipationsInEventReportVMs.Where(c => c.Date <= dateTo.Value);
            }

            var paginated = PaginatedList<ParticipationsInEventReportVM>.Create(ParticipationsInEventReportVMs, page, pageSize, searchTerm);

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

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            TempData["FromParticipationsInEventReportIndex"] = "true";

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }


        public async Task<IActionResult> AddEdit(int? id)
        {
            var vm = new ParticipationsInEventReportVM();

            if (id.HasValue && id.Value != 0)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item == null) return NotFound();
                vm = _mapper.Map<ParticipationsInEventReportVM>(item);
            }
            vm.Old1_Path = vm.Image1Path;
            vm.Old2_Path = vm.Image2Path;
            vm.Old3_Path = vm.Image3Path;
            vm.Old4_Path = vm.Image4Path;
            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ParticipationsInEventReportVM model)
        {
            var FolderEntityWillSaveIn = "Participations In Event";
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

            if (file1_Text != "OK" && file1_Text != "null") ModelState.AddModelError("Image1", file1_Text ?? " ");
            if (file2_Text != "OK" && file2_Text != "null") ModelState.AddModelError("Image2", file2_Text ?? " ");
            if (file3_Text != "OK" && file3_Text != "null") ModelState.AddModelError("Image3", file3_Text ?? " ");
            if (file4_Text != "OK" && file4_Text != "null") ModelState.AddModelError("Image4", file4_Text ?? " ");

            #endregion Validate Is File is PDF And MG // Validate PDF

            #region validation fails // !ModelState.IsValid
            // If validation fails
            if (!ModelState.IsValid)
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
                #endregion Remove ModelState Image Errors To Rebind New Data Temp
                if (file1_Text != "OK" && file1_Text != "null") ModelState.AddModelError("Image1", file1_Text ?? " ");
                if (file2_Text != "OK" && file2_Text != "null") ModelState.AddModelError("Image2", file2_Text ?? " ");
                if (file3_Text != "OK" && file3_Text != "null") ModelState.AddModelError("Image3", file3_Text ?? " ");
                if (file4_Text != "OK" && file4_Text != "null") ModelState.AddModelError("Image4", file4_Text ?? " ");

                // If the user uploads a new file → cache it before returning
                if (model.Image1 != null)
                    model.TempFile1_Path = await FileHelper.SaveTempAsync(model.Image1);
                if (model.Image2 != null)
                    model.TempFile2_Path = await FileHelper.SaveTempAsync(model.Image2);
                if (model.Image3 != null)
                    model.TempFile3_Path = await FileHelper.SaveTempAsync(model.Image3);
                if (model.Image4 != null)
                    model.TempFile4_Path = await FileHelper.SaveTempAsync(model.Image4);

                // Re-fall and Re-populate enum select list

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
            #endregion PROCESS FINAL FILE Handeling Save in newpath From Temp
            // ---------------------------------------

            var entity = _mapper.Map<ParticipationsInEventReport>(model);
            entity.Image1Path = model.Image1Path;
            entity.Image2Path = model.Image2Path;
            entity.Image3Path = model.Image3Path;
            entity.Image4Path = model.Image4Path;
            if (model.Id == 0)
            {
                model.Id = await _service.AddAsync(entity);
                await _hubContext.Clients.Groups("ActivitiesSupervisor")
                  .SendAsync("ReceiveNotification", new
                  {
                      Title = "",
                      Message = ""
                  });
                await _notificationService.SendNotificationToRoleAsync(
                      "مشاركة في  حدث جديدة",
                      $"يوجد مشاركة في حدث رقم {model.Id} جديدة جاهزة للإعتماد",
                      3
                  );
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else // Edit
            {
                await _hubContext.Clients.Groups("ActivitiesSupervisor")
                  .SendAsync("ReceiveNotification", new
                  {
                      Title = "",
                      Message = ""
                  });
                await _notificationService.SendNotificationToRoleAsync(
                      "مشاركة في حدث تم تحديثها",
                      $"يوجد مشاركة في حدث رقم {model.Id} تم تحديثها جاهزة للإعتماد",
                      3
                  );
                await _service.UpdateAsync(entity);
            }

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id.HasValue && id.Value != 0)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item == null) return NotFound();
                var vm = _mapper.Map<ParticipationsInEventReportVM>(item);

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

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintDetails(int? id)
        {
            if (id.HasValue && id.Value != 0)
            {
                var item = await _service.GetByIdAsync(id.Value);
                if (item == null) return NotFound();
                var vm = _mapper.Map<ParticipationsInEventReportVM>(item);

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
        public async Task<IActionResult> Print(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var allParticipationsInEventReports = await _service.GetAllAsync();

            var ParticipationsInEventReportVMs = _mapper.Map<List<ParticipationsInEventReportVM>>(allParticipationsInEventReports).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                ParticipationsInEventReportVMs = ParticipationsInEventReportVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.ReportTitle) && c.ReportTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }


            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                ParticipationsInEventReportVMs = ParticipationsInEventReportVMs.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                ParticipationsInEventReportVMs = ParticipationsInEventReportVMs.Where(c => c.Date <= dateTo.Value);
            }

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            TempData["FromParticipationsInEventReportIndex"] = "true";
            return View(ParticipationsInEventReportVMs);
        }


        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var allParticipationsInEventReports = await _service.GetAllAsync();

            var ParticipationsInEventReportVMs = _mapper.Map<List<ParticipationsInEventReportVM>>(allParticipationsInEventReports).AsQueryable();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                ParticipationsInEventReportVMs = ParticipationsInEventReportVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.ReportTitle) && c.ReportTitle.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }


            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                ParticipationsInEventReportVMs = ParticipationsInEventReportVMs.Where(c => c.Date >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                ParticipationsInEventReportVMs = ParticipationsInEventReportVMs.Where(c => c.Date <= dateTo.Value);
            }

            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {
                var lang = SessionHelper.GetCurrentLanguage();
                var allData_list = ParticipationsInEventReportVMs;
                var ListTitles = new List<string>
                {
                    Resource1.ParticipatingTitle,Resource1.DepartManage,Resource1.Date
                };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = single.ReportTitle,
                        t2 = single.AdministrativeDepartment,
                        t3 = single.Date.HasValue ? single.Date.Value.ToString("d").Replace("/","-") : "",
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
                    var fileExcelName = Resource1.ParticipationsInEventList;
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