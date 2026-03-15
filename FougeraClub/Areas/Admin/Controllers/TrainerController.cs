using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.DTOs;
using Domain.Entities;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.Trainers;
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
    public class TrainerController : Controller
    {
        private readonly ITrainerService _TrainerService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TrainerController(ITrainerService TrainerService, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _TrainerService = TrainerService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int page = 1, int pageSize = 50)
        {
            var allDepartments = await _unitOfWork.Departments.GetAllAsync();

            var trainersWithUsers = await _unitOfWork.Trainers
            .Table
            .Include(t => t.Courses) // Include related courses first
            .Join(
                _unitOfWork.Users.Table,
                trainer => trainer.UserId,
                user => user.Id,
                (trainer, user) => new TrainerWithUserVM
                {
                    Trainer = trainer,
                    User = user
                }
            )
            .ToListAsync();


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                trainersWithUsers = trainersWithUsers.Where(c =>
                    (!string.IsNullOrEmpty(c.User.FullNameAr) && c.User.FullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.User.FullNameEn) && c.User.FullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.User.PhoneNumber) && c.User.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            var paginated = PaginatedList<TrainerWithUserVM>.Create(trainersWithUsers, page, pageSize, searchTerm);

            TrainerIndexVM trainerIndexVM = new TrainerIndexVM()
            {
                TrainerWithUserVM_Paginated = paginated,
                Departments = allDepartments.ToList(),
            };

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", trainerIndexVM);
            }
            return View(trainerIndexVM);
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            var usersDTO = await _TrainerService.GetAllUsersNotTrainers_NamesAr_En_only();
            var departments = await _unitOfWork.Departments.GetAllAsync();

            var vm = new TrainerVM();

            if (id.HasValue && id.Value != 0)
            {
                var trainer = await _TrainerService.GetByIdAsync(id.Value);
                if (trainer == null) return NotFound();
                vm = _mapper.Map<TrainerVM>(trainer);
                usersDTO = await _TrainerService.GetAllUsersNotTrainers_NamesAr_En_only(trainer.UserId);
            }


            vm.UsersList = SelectListHelper.BindSelectListIdString(usersDTO.ToList(), vm.UserId, "UserId").ToList();
            vm.DepartmentsList = SelectListHelper.BindSelectList(departments.ToList(), vm.DepartmentId).ToList();

            vm.Attachment_OldPath = vm.AttachmentPath;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(TrainerVM model)
        {
            var FolderEntityWillSaveIn = "Trainers";
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
                    var usersDTO = await _TrainerService.GetAllUsersNotTrainers_NamesAr_En_only();
                    var departments = await _unitOfWork.Departments.GetAllAsync();

                    model.UsersList = SelectListHelper.BindSelectListIdString(usersDTO.ToList(), model.UserId, "UserId").ToList();
                    model.DepartmentsList = SelectListHelper.BindSelectList(departments.ToList(), model.DepartmentId).ToList();

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

           
            var entity = _mapper.Map<Trainer>(model);
            entity.AttachmentPath = model.AttachmentPath;

            if (model.Id == 0)
            {
                model.Id = await _TrainerService.AddAsync(entity, model.Attachment);
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else
                await _TrainerService.UpdateAsync(entity, model.Attachment);

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit
        }

        [IgnoreAction]
        [HttpGet]
        [YesGet]
        public async Task<IActionResult> TrainerEditMyData2(string? id)
        {
            var userId = id;
            var usersDTO = await _TrainerService.GetAllUsersNotTrainers_NamesAr_En_only();
            var departments = await _unitOfWork.Departments.GetAllAsync();

            var vm = new TrainerVM();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var trainer = await _TrainerService.GetByIdAsync_byuserId(userId);
                if (trainer == null) return NotFound();
                vm = _mapper.Map<TrainerVM>(trainer);
                usersDTO = await _TrainerService.GetAllUsersNotTrainers_NamesAr_En_only(trainer.UserId);
            }


            vm.UsersList = SelectListHelper.BindSelectListIdString(usersDTO.ToList(), vm.UserId, "UserId").ToList();
            vm.DepartmentsList = SelectListHelper.BindSelectList(departments.ToList(), vm.DepartmentId).ToList();

            vm.Attachment_OldPath = vm.AttachmentPath;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TrainerEditMyData2(TrainerVM model)
        {
            var trainer = await _TrainerService.GetByIdAsync_byuserId(model.UserId);
            model.Id = trainer!=null ? trainer.Id :0;
            var FolderEntityWillSaveIn = "Trainers";
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
            ModelState.Remove("Id");
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
                var usersDTO = await _TrainerService.GetAllUsersNotTrainers_NamesAr_En_only();
                var departments = await _unitOfWork.Departments.GetAllAsync();

                model.UsersList = SelectListHelper.BindSelectListIdString(usersDTO.ToList(), model.UserId, "UserId").ToList();
                model.DepartmentsList = SelectListHelper.BindSelectList(departments.ToList(), model.DepartmentId).ToList();

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


            var entity = _mapper.Map<Trainer>(model);
            entity.AttachmentPath = model.AttachmentPath;

            //if (model.Id == 0)
            //{
            //    model.Id = await _TrainerService.AddAsync(entity, model.Attachment);
            //    return RedirectToAction(nameof(Index)); // After Add New
            //}
            //else
                await _TrainerService.UpdateAsync(entity, model.Attachment);

            return RedirectToAction(nameof(TrainerEditMyData2), new { model.UserId }); // After Edit
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _TrainerService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string? searchTerm)
        {
            var allDepartments = await _unitOfWork.Departments.GetAllAsync();

            var trainersWithUsers = await _unitOfWork.Trainers
                .Table
                .Include(t => t.Courses) // Include related courses first
                .Join(
                    _unitOfWork.Users.Table,
                    trainer => trainer.UserId,
                    user => user.Id,
                    (trainer, user) => new TrainerWithUserVM
                    {
                        Trainer = trainer,
                        User = user
                    }
                )
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                trainersWithUsers = trainersWithUsers.Where(c =>
                    (!string.IsNullOrEmpty(c.User.FullNameAr) && c.User.FullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.User.FullNameEn) && c.User.FullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.User.PhoneNumber) && c.User.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            return View(trainersWithUsers);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm)
        {
            var allDepartments = await _unitOfWork.Departments.GetAllAsync();

            var trainersWithUsers = await _unitOfWork.Trainers
                .Table
                .Include(t => t.Courses) // Include related courses first
                .Join(
                    _unitOfWork.Users.Table,
                    trainer => trainer.UserId,
                    user => user.Id,
                    (trainer, user) => new TrainerWithUserVM
                    {
                        Trainer = trainer,
                        User = user
                    }
                )
                .ToListAsync();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                trainersWithUsers = trainersWithUsers.Where(c =>
                    (!string.IsNullOrEmpty(c.User.FullNameAr) && c.User.FullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.User.FullNameEn) && c.User.FullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.User.PhoneNumber) && c.User.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }


            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {
                var lang = SessionHelper.GetCurrentLanguage();
                var allData_list = trainersWithUsers;
                var ListTitles = new List<string>
                {
                    Resource2.TrainerName,Resource1.Phone,Resource2.Email,Resource1.Specialization
                };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = lang == "ar" ? single.User?.FullNameAr : single.User?.FullNameEn,
                        t2 = single.User?.PhoneNumber,
                        t3 = single.User?.UserName,
                        t4 = lang == "ar" ? single.Trainer?.Department?.NameAr : single.Trainer?.Department?.NameEn
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
                    var fileExcelName = Resource1.TrainersList;
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
