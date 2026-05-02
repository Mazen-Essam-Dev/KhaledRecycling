//using Application.Helpers;
//using Application.Interfaces.Admin;
//using Application.Services.Admin;
//using AutoMapper;
//using Domain.DTOs;
//using Domain.Entities;
//using Domain.Enums;
//using Domain.Resources;
//using KhaledTeamRecycling.Areas.Admin.ViewModels.MaterialOrder;
//using KhaledTeamRecycling.Attributes;
//using KhaledTeamRecycling.Helpers;
//using KhaledTeamRecycling.Middelware;
//using Infrastructure.Identity;
//using Infrastructure.Repositories.InterfacesDB;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.SignalR;


//namespace KhaledTeamRecycling.Areas.Admin.Controllers
//{
//    //[AdminAuthorize]
//    [Area("Admin")]
//    public class MaterialOrderController : Controller
//    {
//        private readonly IMaterialOrderService _service;
//        private readonly IMapper _mapper;
//        private readonly IUnitOfWork _unitOfWork;
//        private readonly IHubContext<Hub.NotificationHub> _hubContext;
//        private readonly INotificationService _notificationService;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;

//        public MaterialOrderController(
//            IMaterialOrderService monthlyAdministrativeReportService,
//            IMapper mapper,
//            IHubContext<Hub.NotificationHub> hubContext,
//            IHttpContextAccessor httpContextAccessor,
//            INotificationService notificationService,IUnitOfWork unitOfWork,
//            UserManager<Infrastructure.Identity.ApplicationUser> userManager)
//        {
//            _service = monthlyAdministrativeReportService;
//            _mapper = mapper;
//            _hubContext = hubContext;
//            _notificationService = notificationService;
//            _userManager = userManager;
//            _unitOfWork = unitOfWork;
//            _httpContextAccessor = httpContextAccessor;
//        }

//        [IgnoreAction]
//        [NoLogging]
//        public async Task<(string, int,int)> CheckLoggedUserIfTrainerAndReturnData()
//        {
//            // Get the UserEMail of User Logged in
//            var User = _httpContextAccessor.HttpContext?.User;
//            var UserEMail = User?.Identity?.Name;

//            int ThisTrainerId = 0;
//            int ThisTrainerDepartmentId = 0;
//            if (UserEMail == null) return (" ", 0,0);
//            var ThisUser = await _unitOfWork.Users.GetByIdAsync(x => x.UserName == UserEMail);

//            if (ThisTrainerId > 0) return (ThisUser?.Id != null ? ThisUser.Id : " ", ThisTrainerId, ThisTrainerDepartmentId);

//            return (ThisUser?.Id != null ? ThisUser.Id : " ", 0,0);
//        }

//        [YesGet]
//        public async Task<IActionResult> Index(string? selectedUser, int? selectedDepartment, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
//        {
//            (string userId, int TrainerId, int DepartmentId) = await CheckLoggedUserIfTrainerAndReturnData();

//            var allMaterialOrders = await _service.GetAllAsync();

//            if (TrainerId > 0)
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.UserId == userId);
//            }

//            if (selectedDepartment != null && selectedDepartment.HasValue)
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.DepartmentId == selectedDepartment.Value);
//            }
//            if (!string.IsNullOrWhiteSpace(selectedUser))
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.UserId == selectedUser.Trim().TrimStart());
//            }

//            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
//            if (dateFrom.HasValue)
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.Date >= dateFrom.Value);
//            }
//            if (dateTo.HasValue)
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.Date <= dateTo.Value);
//            }

//            var allMaterialOrderVM = _mapper.Map<IEnumerable<MaterialOrderVM>>(allMaterialOrders);
//            var paginated = PaginatedList<MaterialOrderVM>.Create(allMaterialOrderVM.ToList(), page, pageSize, null);
//            if (TrainerId > 0) paginated.flag1 = true;

//            var departments = await _unitOfWork.Departments.GetAllAsync();
//            ViewBag.DepartmentsList = SelectListHelper.BindSelectList(departments.ToList(), selectedDepartment).ToList();
//            ViewBag.SelectedType = selectedDepartment;
//            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
//            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

//            TempData["FromCarIndex"] = "true";

//            // Calling from Ajax Return PartialView
//            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
//            {
//                return PartialView("_ListPartial", paginated);
//            }

//            return View(paginated);
//        }

//        public async Task<IActionResult> AddEdit(int? id)
//        {
//            var vm = new MaterialOrderVM();
//            (string userLoggedInId,int TrainerId, int DepartmentId) = await CheckLoggedUserIfTrainerAndReturnData();

//            vm.MaterialOrderCode = await _service.GetNewCodeAsync();
//            if (id.HasValue && id.Value != 0) //Edit
//            {
//                var MaterialOrder = await _service.GetByIdAsync(id.Value);
//                if (MaterialOrder == null) return NotFound();
//                vm = _mapper.Map<MaterialOrderVM>(MaterialOrder);
//            }

//            if (TrainerId > 0)
//            {
//                vm.TrainerId = TrainerId;
//                vm.UserId = userLoggedInId;
//                vm.DepartmentId = DepartmentId;
//                vm.IsTrainerLogged = true;
//            }
//            if (!string.IsNullOrWhiteSpace(userLoggedInId)) vm.UserId = userLoggedInId;

//            // fill dropdowns 
//            var departments = await _unitOfWork.Departments.GetAllAsync();
//            vm.DepartmentsList = SelectListHelper.BindSelectList(departments.ToList(), vm.DepartmentId).ToList();
//            vm.DepartmentName = vm.DepartmentsList.FirstOrDefault(x => x.Selected == true)?.Text;
//            //var usersDTO = await _TrainerService.GetAllUsersNotTrainers_NamesAr_En_only();

//            return View(vm);
//        }

//        [HttpPost]
//        [ValidateAntiForgeryToken]
//        public async Task<IActionResult> AddEdit(MaterialOrderVM model)
//        {
//            (string userLoggedInId, int TrainerId, int DepartmentId) = await CheckLoggedUserIfTrainerAndReturnData();

//            if (TrainerId > 0)
//            {
//                model.TrainerId = TrainerId;
//                model.UserId = userLoggedInId;
//                model.DepartmentId = DepartmentId;
//                model.IsTrainerLogged = true;
//            }
//            if(!string.IsNullOrWhiteSpace(userLoggedInId)) model.UserId = userLoggedInId;

//            if (!ModelState.IsValid)
//            {
//                //model.suppliers = suppliers;
//                if (model.MaterialOrderCode == null)
//                    model.MaterialOrderCode = await _service.GetNewCodeAsync();

//                // Refill dropdowns if validation fails
//                var departments = await _unitOfWork.Departments.GetAllAsync();
//                model.DepartmentsList = SelectListHelper.BindSelectList(departments.ToList(), model.DepartmentId).ToList();
//                model.DepartmentName = model.DepartmentsList.FirstOrDefault(x => x.Selected == true)?.Text;
//                //var usersDTO = await _TrainerService.GetAllUsersNotTrainers_NamesAr_En_only();
//                return View(model);
//            }

//            var entity = _mapper.Map<MaterialOrder>(model);

//            if (model.Id == 0)
//            {
//                model.Id = await _service.AddAsync(entity);
//              //  await _hubContext.Clients.Groups("Manager")
//              // .SendAsync("ReceiveNotification", new
//              // {
//              //     Title = "",
//              //     Message = ""
//              // });
//              //  await _notificationService.SendNotificationToRoleAsync(
//              //    "طلب مواد جديد",
//              //    $"يوجد طلب مواد رقم {model?.MaterialOrderCode} جديد جاهز للإعتماد",
//              //    (int)RoleNumber.Manager
//              //);
//                return RedirectToAction(nameof(Index)); // After Add New
//            }
//            else
//            {
//                await _service.UpdateAsync(entity);
//            }

//            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit
//        }

//        [IgnoreAction]
//        [YesGet]
//        public async Task<IActionResult> Details(int? id)
//        {
//            if (id.HasValue && id.Value != 0)
//            {
//                var item = await _service.GetByIdAsync(id.Value);
//                if (item == null) return NotFound();
//                var vm = _mapper.Map<MaterialOrderVM>(item);
//                if (vm != null && vm.UserId != null)
//                {
//                    ApplicationUser? Applicant = await _userManager.FindByIdAsync(vm.UserId);
//                    vm.TrainerName = Applicant?.FullNameAr;
//                }
//                // Fetch trainer name from signed user (if already signed)
//                if (vm.SignatureUser != null && !string.IsNullOrEmpty(vm.SignatureUser.UserId))
//                {
//                    var trainerUser = await _userManager.FindByIdAsync(vm.SignatureUser.UserId);
//                    vm.TrainerSignedName = trainerUser?.FullNameAr ?? trainerUser?.FullNameEn ?? trainerUser?.Email ?? "";
//                }
//                // Fetch manager full name from ApplicationUser using SignatureManager.UserId
//                if (vm.SignatureManager != null && !string.IsNullOrEmpty(vm.SignatureManager.UserId))
//                {
//                    var mgrUser = await _userManager.FindByIdAsync(vm.SignatureManager.UserId);
//                    vm.ManagerUserName = mgrUser?.FullNameAr ?? mgrUser?.FullNameEn ?? mgrUser?.Email ?? "";
//                }
//                return View(vm);
//            }
//            else
//            {
//                return NotFound();
//            }
//        }

//        [HttpPost]
//        public async Task<IActionResult> Delete(int id)
//        {
//            await _service.DeleteAsync(id);
//            return RedirectToAction(nameof(Index));
//        }

//        [IgnoreAction]
//        [YesGet]
//        public async Task<IActionResult> Print(string? selectedUser, int? selectedDepartment, DateOnly? dateFrom, DateOnly? dateTo)
//        {
//            (string userId, int TrainerId, int DepartmentId) = await CheckLoggedUserIfTrainerAndReturnData();

//            var allMaterialOrders = await _service.GetAllAsync();

//            if (TrainerId > 0)
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.UserId == userId);
//            }

//            if (selectedDepartment != null && selectedDepartment.HasValue)
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.DepartmentId == selectedDepartment.Value);
//            }
//            if (!string.IsNullOrWhiteSpace(selectedUser))
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.UserId == selectedUser.Trim().TrimStart());
//            }

//            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
//            if (dateFrom.HasValue)
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.Date >= dateFrom.Value);
//            }
//            if (dateTo.HasValue)
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.Date <= dateTo.Value);
//            }


//            var allMaterialOrderVM = _mapper.Map<IEnumerable<MaterialOrderVM>>(allMaterialOrders);
//            var departments = await _unitOfWork.Departments.GetAllAsync();
//            ViewBag.DepartmentsList = SelectListHelper.BindSelectList(departments.ToList(), selectedDepartment).ToList();
//            ViewBag.SelectedType = selectedDepartment;
//            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
//            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

//            return View(allMaterialOrderVM);
//        }

//        [IgnoreAction]
//        [YesGet]
//        public async Task<IActionResult> PrintDetails(int? id)
//        {
//            if (id.HasValue && id.Value != 0)
//            {
//                var item = await _service.GetByIdAsync(id.Value);
//                if (item == null) return NotFound();
//                var vm = _mapper.Map<MaterialOrderVM>(item);
//                //vm.suppliers = suppliers;
//                return View(vm);
//            }
//            else
//            {
//                return NotFound();
//            }
//        }
//        [IgnoreAction]
//        [YesGet]
//        public async Task<IActionResult> createExcelReport_Download(string? selectedUser, int? selectedDepartment, DateOnly? dateFrom, DateOnly? dateTo)
//        {
//            (string userId, int TrainerId, int DepartmentId) = await CheckLoggedUserIfTrainerAndReturnData();

//            var allMaterialOrders = await _service.GetAllAsync();

//            if (TrainerId > 0)
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.UserId == userId);
//            }

//            if (selectedDepartment != null && selectedDepartment.HasValue)
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.DepartmentId == selectedDepartment.Value);
//            }
//            if (!string.IsNullOrWhiteSpace(selectedUser))
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.UserId == selectedUser.Trim().TrimStart());
//            }

//            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
//            if (dateFrom.HasValue)
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.Date >= dateFrom.Value);
//            }
//            if (dateTo.HasValue)
//            {
//                allMaterialOrders = allMaterialOrders.Where(c => c.Date <= dateTo.Value);
//            }

//            var departments = await _unitOfWork.Departments.GetAllAsync();

//            // ---- End Get Data As Print

//            var boolStatus = false;
//            byte[]? fileBytes = null;
//            var pathNewFile = "";
//            try
//            {
//                var lang = SessionHelper.GetCurrentLanguage();
//                var allData_list = allMaterialOrders;
//                var ListTitles = new List<string>
//                {
//                    ".No",Resource1.Date,"القسم"
//                };
//                if (allData_list != null || allData_list?.Count() > 0)
//                {
//                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
//                    {
//                        t1 = single.MaterialOrderCode,
//                        t2 = (single.Date.HasValue ? (lang == "ar" ? single.Date.Value.ToString("d")?.Replace("/","-") : single.Date.Value.ToString("d")?.Replace("/","-")) : ""),
//                        t3 = (single.Department != null) ? (lang == "ar" ? single.Department.NameAr : single.Department.NameEn) : "",
//                    }).ToList();

//                    if (lang == "ar")
//                    {
//                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "ar");
//                    }
//                    else
//                    {
//                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "en");
//                    }
//                }

//                FileContentResult? Excelfile = null;
//                if (fileBytes != null && fileBytes.Length > 0 && boolStatus == true)
//                {
//                    var fileExcelName = Resource1.MaterialOrderList;
//                    Excelfile = File(fileBytes,
//                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
//                        $"{fileExcelName}_{AppDubaiTime.Now:yyyyMMdd_HHmmss}.xlsx");
//                }
//                return Excelfile;

//            }
//            catch (Exception ex)
//            {
//                return RedirectToAction("Index");
//            }
//        }

//        //#region sms approval
//        //[IgnoreAction]
//        //[NoLogging]
//        //[HttpPost]
//        //public async Task<IActionResult> SendOtp() // GetSignature
//        //{
//        //    try
//        //    {
//        //        var status= await _service.SendOtpAsync();
//        //        return Json(new { success = status });
//        //    }
//        //    catch (Exception ex)
//        //    {
//        //        return Json(new { success = false });
//        //    }
//        //}


//        //[HttpPost]
//        //[IgnoreAction]
//        //public async Task<IActionResult> ValidateOtp([FromBody] OtpValidationRequest request)    // ValidateOTPSignature
//        //{
//        //    if (request == null || string.IsNullOrEmpty(request.Code))
//        //        return Json(new { success = false, message = "Invalid data." });

//        //    var report = await _unitOfWork.MaterialOrders.GetByColumnAsync(
//        //            e => e.MaterialOrderCode != null && e.Id == (int)request.Id);

//        //    var result = await _service.ValidateOtpAsync((int)request.Id, request.Code, request.Role ?? "Trainer");
//        //    if (result.success==true)
//        //    {
//        //        if (request.Role == "Trainer")
//        //        {
//        //            await _hubContext.Clients.Groups("Manager")
//        //           .SendAsync("ReceiveNotification", new
//        //           {
//        //               Title = "",
//        //               Message = ""
//        //           });
//        //            await _notificationService.SendNotificationToRoleAsync(
//        //              "طلب مواد جديد",
//        //              $"يوجد طلب مواد رقم {report?.MaterialOrderCode} جديد جاهز للإعتماد",
//        //              (int)RoleNumber.Manager
//        //          );
//        //        }
//        //    }
            

//        //    return Json(new { success = result.success, message = result.message });
//        //}

//        //#endregion

//    }
//}