using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.DTOs.Admin.CarService;
using Domain.Entities;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.CarServices;
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
    public class CarServiceController : Controller
    {
        private readonly ICarServiceManager _CarServiceManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CarServiceController(ICarServiceManager CarServiceService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _CarServiceManager = CarServiceService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [YesGet]
        public async Task<IActionResult> Index(string? selectedType, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var carServices = await _CarServiceManager.GetAllAsync();
            var model = _mapper.Map<List<CarServiceVM>>(carServices).AsQueryable();

            var allCars = await _unitOfWork.Cars.GetAllAsync();
            var distinctTypes = allCars
                .Where(c => !string.IsNullOrWhiteSpace(c.Type))
                .Select(c => c.Type!)
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            ViewBag.CarTypes = distinctTypes
                .Select(t => new SelectListItem
                {
                    Value = t,
                    Text = t,
                    Selected = (t == selectedType)
                })
                .ToList();

            if (!string.IsNullOrWhiteSpace(selectedType))
                model = model.Where(c => c.Car.Type == selectedType);

            if (dateFrom.HasValue)
                model = model.Where(c => c.Date >= dateFrom.Value);

            if (dateTo.HasValue)
                model = model.Where(c => c.Date <= dateTo.Value);

            ViewBag.SelectedType = selectedType;
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            TempData["FromCarIndex"] = null;

            var paginated = PaginatedList<CarServiceVM>.Create(model.OrderByDescending(m => m.Id), page, pageSize);

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }

        public async Task<IActionResult> AddEdit(int? id, int? carId)
        {
            var cars = await _unitOfWork.Cars.GetAllAsync();

            var vm = new CarServiceVM();
            TempData.Keep(); // keep "FromCarIndex" for POST

            if (id == null || id == 0)
            {
                if (carId != null)
                {
                    vm.CarId = carId.Value;
                }
                else
                {
                    TempData["FromCarIndex"] = null;
                }
            }
            else
            {
                var carService = await _CarServiceManager.GetByIdAsync(id.Value);
                if (carService == null) return NotFound();

                vm = _mapper.Map<CarServiceVM>(carService);
                if (carId != null) vm.CarId = carId.Value;
            }

            vm.CarList = SelectListHelper.BindSelectList(cars.ToList(), vm.CarId, "Id", "Type", "Type").ToList();
            vm.Attachment_OldPath = vm.AttachmentPath;
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(CarServiceVM model)
        {
            var FolderEntityWillSaveIn = "CarServices";
            #region Validate Is File is PDF And MG // Validate PDF

            IFormFile? AttachmentFile_Temp = !string.IsNullOrEmpty(model.Attachment_TempFilePath) ? FileHelper.ConvertToIFormFile(model.Attachment_TempFilePath) : model.Attachment;
            string? Attachment_path = !string.IsNullOrEmpty(model.Attachment_TempFilePath) ? model.Attachment_TempFilePath : model.AttachmentPath;
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
                if (model.Attachment != null)
                    model.Attachment_TempFilePath = await FileHelper.SaveTempAsync(model.Attachment);

                // Refill dropdowns if validation fails
                var cars = await _unitOfWork.Cars.GetAllAsync();
                model.CarList = SelectListHelper.BindSelectList(cars.ToList(), model.CarId, "Id", "Type", "Type").ToList();

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
            if (model.Attachment != null)
            {
                FileHelper.DeleteImageFile(model.Attachment_OldPath);
                model.AttachmentPath = await FileHelper.SaveImageAsync(model.Attachment, FolderEntityWillSaveIn);
            }

            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.Attachment_TempFilePath))
            {
                FileHelper.DeleteImageFile(model.Attachment_OldPath);
                model.AttachmentPath = FileHelper.MoveTempToFinal(
                    model.Attachment_TempFilePath,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }

            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.AttachmentPath = model.Attachment_OldPath;
            }
            #endregion PROCESS FINAL FILE Handeling Save in newpath From Temp
            // ---------------------------------------
            // Map model → entity After Added New Path



            var entity = _mapper.Map<CarServiceEntity>(model);
            entity.AttachmentPath = model.AttachmentPath;
            if (model.Id == 0)
            {
                (model.Id, model.CarId) = await _CarServiceManager.AddAsync(entity, model.Attachment);

                return RedirectToAction("index"); // After Add 

            }
            else
                await _CarServiceManager.UpdateAsync(entity, model.Attachment);

            return RedirectToAction(nameof(AddEdit), new { model.Id, model.CarId }); // After Edit 
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _CarServiceManager.DeleteAsync(id);

            if (TempData["FromCarIndex"]?.ToString() == "true")
                return RedirectToAction(nameof(Index), nameof(Car));

            return RedirectToAction(nameof(Index));
        }
        [IgnoreAction]
        [Route("Admin/CarService/Print")]
        [YesGet]
        public async Task<IActionResult> Print(string? selectedType, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var carServices = await _CarServiceManager.GetAllAsync();
            var model = _mapper.Map<List<CarServiceVM>>(carServices).AsQueryable();

            var allCars = await _unitOfWork.Cars.GetAllAsync();
            var distinctTypes = allCars
                .Where(c => !string.IsNullOrWhiteSpace(c.Type))
                .Select(c => c.Type!)
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            ViewBag.CarTypes = distinctTypes
                .Select(t => new SelectListItem
                {
                    Value = t,
                    Text = t,
                    Selected = (t == selectedType)
                })
                .ToList();

            if (!string.IsNullOrWhiteSpace(selectedType))
                model = model.Where(c => c.Car.Type == selectedType);

            if (dateFrom.HasValue)
                model = model.Where(c => c.Date >= dateFrom.Value);

            if (dateTo.HasValue)
                model = model.Where(c => c.Date <= dateTo.Value);

            ViewBag.SelectedType = selectedType;
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            TempData["FromCarIndex"] = null;

            return View(model.OrderByDescending(m => m.Id));
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? selectedType, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var carServices = await _CarServiceManager.GetAllAsync();
            var model = _mapper.Map<List<CarServiceVM>>(carServices).AsQueryable();

            var allCars = await _unitOfWork.Cars.GetAllAsync();
            var distinctTypes = allCars
                .Where(c => !string.IsNullOrWhiteSpace(c.Type))
                .Select(c => c.Type!)
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            ViewBag.CarTypes = distinctTypes
                .Select(t => new SelectListItem
                {
                    Value = t,
                    Text = t,
                    Selected = (t == selectedType)
                })
                .ToList();

            if (!string.IsNullOrWhiteSpace(selectedType))
                model = model.Where(c => c.Car.Type == selectedType);

            if (dateFrom.HasValue)
                model = model.Where(c => c.Date >= dateFrom.Value);

            if (dateTo.HasValue)
                model = model.Where(c => c.Date <= dateTo.Value);

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
                var allData_list = model.OrderByDescending(m => m.Id);
                var ListTitles = new List<string>
        {
            Resource2.CarType,Resource1.MaintenanceDetails1,Resource2.Date,
        };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = ((single.Car != null) ? single.Car.Type : ""),
                        t2 = single.Details,
                        t3 = ((single.Date.HasValue) ? single.Date.Value.ToString("d").Replace("/", "-") : ""),
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
                    var fileExcelName = Resource1.CarsMaintenanceList;
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
        public async Task<IActionResult> GetCarServiceAttachments(int id)
        {
            var attachmentsDTO = await _CarServiceManager.GetAttachmentsAsync(id);
            var model = _mapper.Map<CarServiceAttachmentsVM>(attachmentsDTO);
            return PartialView("_CarServiceAttachmentsModal", model);
        }

        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadCarServiceFiles(CarServiceAttachmentsVM model)
        {
            if (model == null)
                return Json(new { success = false, message = "Files uploaded not done" });
            var dto = _mapper.Map<CarServiceAttachmentsDTO>(model);
            await _CarServiceManager.UploadAttachmentsAsync(dto);

            return RedirectToAction("Index");
        }
    }

}
