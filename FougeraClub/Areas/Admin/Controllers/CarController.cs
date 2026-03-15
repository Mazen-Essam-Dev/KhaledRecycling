using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.DTOs.Admin.Car;
using Domain.Entities;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.Cars;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class CarController : Controller
    {
        private readonly ICarService _CarService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CarController(ICarService CarService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _CarService = CarService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, string? selectedType, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var allCars = await _CarService.GetAllAsync();
            var allCarServices = await _unitOfWork.CarServices.Table.Select(x => x.CarId).Distinct().ToListAsync();

            var carVMs = _mapper.Map<List<CarVM>>(allCars).AsQueryable();
            ViewBag.CarTypes = allCars
                .Select(c => c.Type)
                .Where(type => !string.IsNullOrWhiteSpace(type))
                .Distinct()
                .OrderBy(type => type)
                .ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                carVMs = carVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.PlateNumber) && c.PlateNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.DriverName) && c.DriverName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Color) && c.Color.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Model) && c.Model.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (!string.IsNullOrWhiteSpace(selectedType))
            {
                carVMs = carVMs.Where(c => c.Type == selectedType);
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                carVMs = carVMs.Where(c => c.OwnershipExpiryDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                carVMs = carVMs.Where(c => c.OwnershipExpiryDate <= dateTo.Value);
            }
            foreach (var carVM in carVMs)
            {
                carVM.inService = allCarServices?.Any(x => x.Value == carVM.Id);
            }
            var paginated = PaginatedList<CarVM>.Create(carVMs.OrderByDescending(m => m.Id), page, pageSize, searchTerm);

            ViewBag.SelectedType = selectedType;
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            TempData["FromCarIndex"] = "true";

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            var vm = new CarVM();
            if (id == 0 || id == null)
            {
                return View(vm);
            }
            var Car = await _CarService.GetByIdAsync(id.Value);
            if (Car == null) return NotFound();

            vm = _mapper.Map<CarVM>(Car);
            vm.Attachment_OldPath = vm.AttachmentPath;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(CarVM modelVM)
        {
            var FolderEntityWillSaveIn = "Cars";
            #region Validate Is File is PDF And MG // Validate PDF

            IFormFile? AttachmentFile_Temp = !string.IsNullOrEmpty(modelVM.Attachment_TempFilePath) ? FileHelper.ConvertToIFormFile(modelVM.Attachment_TempFilePath) : modelVM.Attachment;
            string? Attachment_path = !string.IsNullOrEmpty(modelVM.Attachment_TempFilePath) ? modelVM.Attachment_TempFilePath : modelVM.AttachmentPath;
            var Attachment_Text = await FileHelper.CheckFileIsPdf_5Mg_Async(AttachmentFile_Temp);
            if (Attachment_Text != "OK" && Attachment_Text != "null")
            {
                ModelState.AddModelError("Attachment", Attachment_Text);
                FileHelper.DeleteImageFile(Attachment_path);
            }
            #endregion Validate Is File is PDF And MG // Validate PDF

            #region validation fails // !ModelState.IsValid
            // If validation fails
            if (!ModelState.IsValid)
            {
                ModelState.Remove("Attachment_TempFilePath"); // its Important To Bind New Data Temp
                ModelState.Remove("Attachment_OldPath"); // its Important To Bind New Data Temp
                ModelState.Remove("Attachment"); // its Important To Bind New Data Temp
                if (Attachment_Text != "OK" && Attachment_Text != "null") { ModelState.AddModelError("Attachment", Attachment_Text); }

                // If the user uploads a new file → cache it before returning
                if (modelVM.Attachment != null)
                    modelVM.Attachment_TempFilePath = await FileHelper.SaveTempAsync(modelVM.Attachment);

                return View(modelVM);
            }
            #endregion validation fails // !ModelState.IsValid

            // ---------------------------
            //  PROCESS FINAL FILE
            // ---------------------------
            #region PROCESS FINAL FILE Handeling Save in newpath From Temp
            string finalFileName = null;

            // 1) If there is a new file uploaded by the user
            if (modelVM.Attachment != null)
            {
                FileHelper.DeleteImageFile(modelVM.Attachment_OldPath);
                modelVM.AttachmentPath = await FileHelper.SaveImageAsync(modelVM.Attachment, FolderEntityWillSaveIn);
            }

            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(modelVM.Attachment_TempFilePath))
            {
                FileHelper.DeleteImageFile(modelVM.Attachment_OldPath);
                modelVM.AttachmentPath = FileHelper.MoveTempToFinal(
                        modelVM.Attachment_TempFilePath,
                        out finalFileName,
                        FolderEntityWillSaveIn
                    );
            }

            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                modelVM.AttachmentPath = modelVM.Attachment_OldPath;
            }
            #endregion PROCESS FINAL FILE Handeling Save in newpath From Temp
            // ---------------------------------------
            // Map model → entity After Added New Path

            var entity = _mapper.Map<Car>(modelVM);
            entity.AttachmentPath = modelVM.AttachmentPath;
            if (modelVM.Id == 0)
            {
                modelVM.Id = await _CarService.AddAsync(entity, modelVM.Attachment);

                return RedirectToAction("index"); // After Add 
            }
            else
                await _CarService.UpdateAsync(entity, modelVM.Attachment);

            return RedirectToAction(nameof(AddEdit), new {id= modelVM.Id }); // After Edit 
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _CarService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [IgnoreAction]
        [Route("Admin/Car/Print")]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm, string? selectedType, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var allCars = await _CarService.GetAllAsync();
            var carVMs = _mapper.Map<List<CarVM>>(allCars).AsQueryable();

            ViewBag.CarTypes = allCars
                .Select(c => c.Type)
                .Where(type => !string.IsNullOrWhiteSpace(type))
                .Distinct()
                .OrderBy(type => type)
                .ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                carVMs = carVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.PlateNumber) && c.PlateNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Type) && c.Type.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.DriverName) && c.DriverName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Color) && c.Color.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Model) && c.Model.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (!string.IsNullOrWhiteSpace(selectedType))
            {
                carVMs = carVMs.Where(c => c.Type == selectedType);
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                carVMs = carVMs.Where(c => c.OwnershipExpiryDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                carVMs = carVMs.Where(c => c.OwnershipExpiryDate <= dateTo.Value);
            }

            ViewBag.SelectedType = selectedType;
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            TempData["FromCarIndex"] = "true";
            return View(carVMs.OrderByDescending(m => m.Id));
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, string? selectedType, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var allCars = await _CarService.GetAllAsync();
            var carVMs = _mapper.Map<List<CarVM>>(allCars).AsQueryable();

            ViewBag.CarTypes = allCars
                .Select(c => c.Type)
                .Where(type => !string.IsNullOrWhiteSpace(type))
                .Distinct()
                .OrderBy(type => type)
                .ToList();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                carVMs = carVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.PlateNumber) && c.PlateNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Type) && c.Type.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.DriverName) && c.DriverName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Color) && c.Color.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Model) && c.Model.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            if (!string.IsNullOrWhiteSpace(selectedType))
            {
                carVMs = carVMs.Where(c => c.Type == selectedType);
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                carVMs = carVMs.Where(c => c.OwnershipExpiryDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                carVMs = carVMs.Where(c => c.OwnershipExpiryDate <= dateTo.Value);
            }

            ViewBag.SelectedType = selectedType;
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");
            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                var lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                var allData_list = carVMs.OrderByDescending(m => m.Id);
                var ListTitles = new List<string>
                {
                    Resource2.CarType,Resource2.DriverName,Resource2.CarNo,Resource2.Model , Resource2.Color,Resource2.OwnershipExpiryDate,
                };

                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = single.Type,
                        t2 = single.DriverName,
                        t3 = single.PlateNumber,
                        t4 = single.Model,
                        t5 = single.Color,
                        t6 = ((single.OwnershipExpiryDate.HasValue == true) ? single.OwnershipExpiryDate.Value.ToString("d").Replace("/","-") : ""),
                    }).ToList();

                    if (lang == "ar")
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_<ExcelDataDTO>(excelDataDTO, ListTitles, 0, "ar");
                    }
                    else
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_<ExcelDataDTO>(excelDataDTO, ListTitles, 0, "en");
                    }
                }

                FileContentResult? Excelfile = null;
                if (fileBytes != null && fileBytes.Length > 0 && boolStatus == true)
                {
                    var fileExcelName = Resource1.CarsList;
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
        public async Task<IActionResult> GetCarAttachments(int id)
        {
            var attachmentsDTO = await _CarService.GetAttachmentsAsync(id);
            var model = _mapper.Map<CarAttachmentsVM>(attachmentsDTO);
            return PartialView("_CarAttachmentsModal", model);
        }

        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCarFiles(CarAttachmentsVM model)
        {
            if (model == null)
                return Json(new { success = false, message = "Files uploaded not done" });
            var dto = _mapper.Map<CarAttachmentsDTO>(model);
            await _CarService.UploadAttachmentsAsync(dto);

            return RedirectToAction("Index");
        }

    }
}
