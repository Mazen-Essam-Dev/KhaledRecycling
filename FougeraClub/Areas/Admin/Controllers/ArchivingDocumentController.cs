using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.ArchivingDocument;
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

    public class ArchivingDocumentController : Controller
    {
        private readonly ISupplierService _supplierService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IArchivingDocumentService _maintenanceClubService;

        public ArchivingDocumentController(ISupplierService supplierService, IUnitOfWork unitOfWork, IMapper mapper, IArchivingDocumentService maintenanceClubService)
        {
            _supplierService = supplierService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _maintenanceClubService = maintenanceClubService;
        }
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int? type, int? documentCategoryId, DateTime? dateFrom, DateTime? dateTo, int page = 1, int pageSize = 50)
        {
            var allRecords = await _maintenanceClubService.GetAllAsync();
            var vms = _mapper.Map<List<ArchivingDocumentVM>>(allRecords).AsQueryable();

            if (type.HasValue)
            {
                vms = vms.Where(c => c.Type.HasValue && (int?)c.Type.Value == type.Value);
            }

            if (documentCategoryId.HasValue)
            {
                vms = vms.Where(c => c.DocumentCategoryId.HasValue && c.DocumentCategoryId.Value == documentCategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                vms = vms.Where(s =>
                    (!string.IsNullOrEmpty(s.Title) && s.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Authority) && s.Authority.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.DocumentReferenceNumber) && s.DocumentReferenceNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
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
            var paginated = PaginatedList<ArchivingDocumentVM>.Create(vms.OrderByDescending(m => m.Id), page, pageSize, searchTerm);
            foreach (var item in paginated.Items)
            {
                if (item.Type != null)
                    item.TypeText = EnumHelper.GetDisplayName((DocumentType)item.Type);
            }
            paginated.dateFrom = dateFrom?.ToString("d")?.Replace("/","-");
            paginated.dateTo = dateTo?.ToString("d")?.Replace("/","-");

            ViewBag.TypeEnumList = SelectListHelper.GetEnumSelectList<DocumentType>(null);
            var categories = await _unitOfWork.DocumentCategories.GetAllAsync();
            ViewBag.DocumentCategoryList = SelectListHelper.BindSelectList(categories.ToList(), documentCategoryId, "Id", "NameAr", "NameEn").ToList();


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
            {
                var modelVM = new ArchivingDocumentVM();
                modelVM.TypeEnumList = SelectListHelper.GetEnumSelectList<DocumentType>();
                var categories = await _unitOfWork.DocumentCategories.GetAllAsync();
                modelVM.DocumentCategoryList = SelectListHelper.BindSelectList(categories.ToList(), null, "Id", "NameAr", "NameEn").ToList();
                return View(modelVM);
            }


            var entity = await _maintenanceClubService.GetByIdAsync(id.Value);
            if (entity == null) return NotFound();

            var vm = _mapper.Map<ArchivingDocumentVM>(entity);
            vm.TypeText = vm.Type.ToString();
            vm.TypeEnumList = SelectListHelper.GetEnumSelectList<DocumentType>((int?)vm.Type);

            vm.OldPath = entity.PdfFilePath;
            var cats = await _unitOfWork.DocumentCategories.GetAllAsync();
            vm.DocumentCategoryList = SelectListHelper.BindSelectList(cats.ToList(), vm.DocumentCategoryId, "Id", "NameAr", "NameEn").ToList();
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ArchivingDocumentVM model)
        {
            model.TypeEnumList = SelectListHelper.GetEnumSelectList<DocumentType>((int?)model.Type);
            var cats = await _unitOfWork.DocumentCategories.GetAllAsync();
            model.DocumentCategoryList = SelectListHelper.BindSelectList(cats.ToList(), model.DocumentCategoryId, "Id", "NameAr", "NameEn").ToList();

            var FolderEntityWillSaveIn = "Archiving Documents";
            #region Validate Is File is PDF And MG // Validate Images
            // Local function to validate the uploaded image size & type
            async Task<string?> ValidateImageAsync(string? TempPath, IFormFile? fileOrignal, string? filePath, string key)
            {

                IFormFile? Image_File_Temp = !string.IsNullOrEmpty(TempPath) ? FileHelper.ConvertToIFormFile(TempPath) : fileOrignal;
                string? NewImage_path = !string.IsNullOrEmpty(TempPath) ? TempPath : filePath;
                var result_Text = await FileHelper.CheckFileIsPdf_5Mg_Async(Image_File_Temp);
                if (result_Text != "OK" && result_Text != "null")
                {
                    ModelState.AddModelError(key, result_Text);
                    // Delete old actual image from Final folder if exists
                    FileHelper.DeleteImageFile(NewImage_path);
                }
                return result_Text;
            }

            var Photo_Text = await ValidateImageAsync(model.TempFilePath, model.PdfFile, model.PdfFilePath, "PdfFile");

            if (Photo_Text != "OK" && Photo_Text != "null") ModelState.AddModelError("PdfFile", Photo_Text);

            #endregion Validate Is File is PDF And MG // Validate PDF

            #region validation fails // !ModelState.IsValid
            // If validation fails
            if (!ModelState.IsValid)
            {
                ModelState.Remove("TempFilePath"); // its Important To Bind New Data Temp
                ModelState.Remove("OldPath"); // its Important To Bind New Data Temp
                ModelState.Remove("PdfFile"); // its Important To Bind New Data Temp
                if (Photo_Text != "OK" && Photo_Text != "null") { ModelState.AddModelError("PdfFile", Photo_Text ?? " "); }


                // If the user uploads a new file → cache it before returning
                if (model.PdfFile != null)
                    model.TempFilePath = await FileHelper.SaveTempAsync(model.PdfFile);

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
            if (model.PdfFile != null)
            {
                FileHelper.DeleteImageFile(model.OldPath);
                model.PdfFilePath = await FileHelper.SaveImageAsync(model.PdfFile, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFilePath))
            {
                FileHelper.DeleteImageFile(model.OldPath);
                model.PdfFilePath = FileHelper.MoveTempToFinal(
                    model.TempFilePath,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.PdfFilePath = model.OldPath;
            }

            #endregion PROCESS FINAL FILE Handeling Save in newpath From Temp
            // ---------------------------------------

            var entity = _mapper.Map<ArchivingDocument>(model);
            entity.PdfFilePath = model.PdfFilePath;


            if (model.Id == 0)
            {
                model.Id = await _maintenanceClubService.AddAsync(entity);
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else
                await _maintenanceClubService.UpdateAsync(entity);

            return RedirectToAction(nameof(AddEdit), new { id = model.Id }); // After Edit 
        }


        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _maintenanceClubService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, int? type, int? documentCategoryId, DateTime? dateFrom, DateTime? dateTo, int page = 1, int pageSize = 50)
        {
            var allRecords = await _maintenanceClubService.GetAllAsync();
            var vms = _mapper.Map<List<ArchivingDocumentVM>>(allRecords).AsQueryable();

            if (type.HasValue)
            {
                vms = vms.Where(c => c.Type.HasValue && (int?)c.Type.Value == type.Value);
            }

            if (documentCategoryId.HasValue)
            {
                vms = vms.Where(c => c.DocumentCategoryId.HasValue && c.DocumentCategoryId.Value == documentCategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                vms = vms.Where(s =>
                    (!string.IsNullOrEmpty(s.Title) && s.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Authority) && s.Authority.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.DocumentReferenceNumber) && s.DocumentReferenceNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
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
            foreach (var item in vms)
            {
                if (item.Type != null)
                    item.TypeText = EnumHelper.GetDisplayName((DocumentType)item.Type);
            }
            ViewBag.TotalCount = vms.Count();
            ViewBag.dateFrom = dateFrom?.ToString("d")?.Replace("/","-");
            ViewBag.dateTo = dateTo?.ToString("d")?.Replace("/","-");
            return View(vms.OrderByDescending(m => m.Id));
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm,int? type, int? documentCategoryId, DateTime? dateFrom, DateTime? dateTo, int page = 1, int pageSize = 50)
        {
            var allRecords = await _maintenanceClubService.GetAllAsync();
            var vms = _mapper.Map<List<ArchivingDocumentVM>>(allRecords).AsQueryable();

            if (type.HasValue)
            {
                vms = vms.Where(c => c.Type.HasValue && (int?)c.Type.Value == type.Value);
            }

            if (documentCategoryId.HasValue)
            {
                vms = vms.Where(c => c.DocumentCategoryId.HasValue && c.DocumentCategoryId.Value == documentCategoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                vms = vms.Where(s =>
                    (!string.IsNullOrEmpty(s.Title) && s.Title.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.Authority) && s.Authority.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(s.DocumentReferenceNumber) && s.DocumentReferenceNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
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
            foreach (var item in vms)
            {
                if (item.Type != null)
                    item.TypeText = EnumHelper.GetDisplayName((DocumentType)item.Type);
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
                var allData_list = vms.OrderByDescending(m => m.Id);
                var ListTitles = new List<string>
        {
            Resource1.DocumentReferenceNumber,Resource1.DocumentTitle1,Resource1.TypeDocument1,Resource1.Date,Resource1.DocumentAuthority, "التصنيف"
        };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = single.DocumentReferenceNumber,
                        t2 = single.Title,
                        t3 = single.TypeText,
                        t4 = single.Date.HasValue?single.Date.Value.ToString("d").Replace("/","-"):"",
                        t5 = single.Authority,
                        t6 = single.DocumentCategoryName
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
                    var fileExcelName = Resource1.ArchivingDocumentList;
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
