using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.MaintenanceClub;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Humanizer;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;

namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]

    public class MaintenanceClubController : Controller
    {
        private readonly ISupplierService _supplierService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IMaintenanceClubService _maintenanceClubService;

        public MaintenanceClubController(ISupplierService supplierService, IUnitOfWork unitOfWork, IMapper mapper, IMaintenanceClubService maintenanceClubService)
        {
            _supplierService = supplierService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _maintenanceClubService = maintenanceClubService;
        }
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, DateTime? dateFrom, DateTime? dateTo, int page = 1, int pageSize = 50)
        {
            var allRecords = await _maintenanceClubService.GetAllAsync();
            var vms = _mapper.Map<List<MaintenanceClubVM>>(allRecords).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                vms = vms.Where(s =>
                    (!string.IsNullOrEmpty(s.Title) && s.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Location) && s.Location.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Details) && s.Details.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                vms = vms.Where(c => c.Date >= dateFrom.Value.AtMidnight());
            }
            if (dateTo.HasValue)
            {
                vms = vms.Where(c => c.Date <= dateTo.Value.AtMidnight());
            }

            ViewBag.TotalCount = vms.Count();
            var paginated = PaginatedList<MaintenanceClubVM>.Create(vms, page, pageSize, searchTerm);
            paginated.dateFrom = dateFrom?.ToString("d")?.Replace("/","-");
            paginated.dateTo = dateTo?.ToString("d")?.Replace("/","-");


            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            if (id == null || id == 0)
                return View(new MaintenanceClubVM());

            var entity = await _maintenanceClubService.GetByIdAsync(id.Value);
            if (entity == null) return NotFound();

            var vm = _mapper.Map<MaintenanceClubVM>(entity);

            vm.Attachment_OldPath = vm.PdfFilePath;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(MaintenanceClubVM model)
        {
            var FolderEntityWillSaveIn = "Maintenance Club";
            #region Validate Is File is PDF And MG // Validate PDF

            IFormFile? AttachmentFile_Temp = !string.IsNullOrEmpty(model.Attachment_TempFilePath) ? FileHelper.ConvertToIFormFile(model.Attachment_TempFilePath) : model.PdfFile;
            string? Attachment_path = !string.IsNullOrEmpty(model.Attachment_TempFilePath) ? model.Attachment_TempFilePath : model.PdfFilePath;
            var Attachment_Text = await FileHelper.CheckFileIsPdf_5Mg_Async(AttachmentFile_Temp);
            if (Attachment_Text != "OK" && Attachment_Text != "null")
            {
                ModelState.AddModelError("PdfFile", Attachment_Text);
                FileHelper.DeleteImageFile(Attachment_path);
            }
            #endregion Validate Is File is PDF And MG // Validate PDF

            #region validation fails // !ModelState.IsValid
            // If validation fails
            if (!ModelState.IsValid)
            {
                ModelState.Remove("Attachment_TempFilePath"); // its Important To Bind New Data Temp
                ModelState.Remove("Attachment_OldPath"); // its Important To Bind New Data Temp
                ModelState.Remove("PdfFile"); // its Important To Bind New Data Temp
                if (Attachment_Text != "OK" && Attachment_Text != "null") { ModelState.AddModelError("PdfFile", Attachment_Text); }

                // If the user uploads a new file → cache it before returning
                if (model.PdfFile != null)
                    model.Attachment_TempFilePath = await FileHelper.SaveTempAsync(model.PdfFile);

                return View(model);
            }
            #endregion validation fails // !ModelState.IsValid

            // ---------------------------
            //  PROCESS FINAL FILE
            // ---------------------------
            #region PROCESS FINAL FILE Handeling Save in newpath From Temp
            string finalFileName = null;

            // 1) If there is a new file uploaded by the user
            if (model.PdfFile != null)
            {
                FileHelper.DeleteImageFile(model.Attachment_OldPath);
                model.PdfFilePath = await FileHelper.SaveImageAsync(model.PdfFile, FolderEntityWillSaveIn);
            }

            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.Attachment_TempFilePath))
            {
                FileHelper.DeleteImageFile(model.Attachment_OldPath);
                model.PdfFilePath = FileHelper.MoveTempToFinal(
                        model.Attachment_TempFilePath,
                        out finalFileName,
                        FolderEntityWillSaveIn
                    );
            }

            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.PdfFilePath = model.Attachment_OldPath;
            }
            #endregion PROCESS FINAL FILE Handeling Save in newpath From Temp
            // ---------------------------------------
            // Map model → entity After Added New Path


            var entity = _mapper.Map<MaintenanceClub>(model);
            entity.PdfFilePath = model.PdfFilePath;

            if (model.Id == 0)
            {
                model.Id = await _maintenanceClubService.AddAsync(entity);
                return RedirectToAction(nameof(Index)); // After Add New

            }
            else
                await _maintenanceClubService.UpdateAsync(entity);

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0) return NotFound();

            var entity = await _maintenanceClubService.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var vm = _mapper.Map<MaintenanceClubVM>(entity);
            return View(vm);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintDetails(int id)
        {
            if (id == 0) return NotFound();

            var entity = await _maintenanceClubService.GetByIdAsync(id);
            if (entity == null) return NotFound();

            var vm = _mapper.Map<MaintenanceClubVM>(entity);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _maintenanceClubService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, DateTime? dateFrom, DateTime? dateTo, int page = 1, int pageSize = 50)
        {
            var allRecords = await _maintenanceClubService.GetAllAsync();
            var vms = _mapper.Map<List<MaintenanceClubVM>>(allRecords).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                vms = vms.Where(s =>
                    (!string.IsNullOrEmpty(s.Title) && s.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Location) && s.Location.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Details) && s.Details.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                vms = vms.Where(c => c.Date >= dateFrom.Value.AtMidnight());
            }
            if (dateTo.HasValue)
            {
                vms = vms.Where(c => c.Date <= dateTo.Value.AtMidnight());
            }

            ViewBag.TotalCount = vms.Count();
            ViewBag.dateFrom = dateFrom?.ToString("d")?.Replace("/","-");
            ViewBag.dateTo = dateTo?.ToString("d")?.Replace("/","-");
            return View(vms);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, DateTime? dateFrom, DateTime? dateTo, int page = 1, int pageSize = 50)
        {
            var allRecords = await _maintenanceClubService.GetAllAsync();
            var vms = _mapper.Map<List<MaintenanceClubVM>>(allRecords).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                vms = vms.Where(s =>
                    (!string.IsNullOrEmpty(s.Title) && s.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Location) && s.Location.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Details) && s.Details.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                vms = vms.Where(c => c.Date >= dateFrom.Value.AtMidnight());
            }
            if (dateTo.HasValue)
            {
                vms = vms.Where(c => c.Date <= dateTo.Value.AtMidnight());
            }

            ViewBag.TotalCount = vms.Count();
            ViewBag.dateFrom = dateFrom?.ToString("d")?.Replace("/","-");
            ViewBag.dateTo = dateTo?.ToString("d")?.Replace("/","-");
            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                var lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                var allData_list = vms;
                var ListTitles = new List<string>
        {
            Resource1.MaintenanceLocation,Resource1.MaintenanceTitle,Resource1.MaintenanceDate,Resource1.MaintenanceTime,
        };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = single.Location,
                        t2 = single.Title,
                        t3 = single.Date.HasValue?single.Date.Value.ToString("d").Replace("/","-") :"",
                        t4 = single.Time.HasValue ? single.Time.Value.ToString(@"hh\:mm"):"",
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
                    var fileExcelName = Resource1.ClubMaintenanceManagementList;
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
        public async Task<IActionResult> GetMaintenanceClubAttachments(int maintenanceClubId)
        {
            var dto = await _maintenanceClubService.GetAttachmentsAsync(maintenanceClubId);
            var vm = _mapper.Map<MaintenanceClubAttachmentsVM>(dto);
            return PartialView("_MaintenanceClubAttachmentsModal", vm);
        }

        [IgnoreAction]
        [HttpPost]
        public async Task<IActionResult> UploadMaintenanceClubFiles(MaintenanceClubAttachmentsVM model)
        {
            if (model.Attachments != null && model.Attachments.Any())
            {
                var dto = _mapper.Map<MaintenanceClubAttachmentsDTO>(model);
                await _maintenanceClubService.UploadAttachmentsAsync(dto);
            }
            return RedirectToAction("Index");
        }
    }
}
