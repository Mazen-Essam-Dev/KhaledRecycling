using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Interfaces.Member;
using Application.Services.Admin;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.DTOs;
using Domain.DTOs.Admin;
using Domain.DTOs.Admin.Course;
using Domain.Entities;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.Course;
using FougeraClub.Areas.Admin.ViewModels.Member;
using FougeraClub.Areas.Admin.ViewModels.Trainers;
using FougeraClub.Areas.Member.ViewModels;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Hub;
using FougeraClub.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;


namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class CourseController : Controller
    {
        #region Properties
        private readonly Application.Interfaces.Admin.ICourseService _courseService;
        private readonly Application.Interfaces.Member.ICourseService _courseMemberService;
        private readonly ITrainerService _trainerService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorizationService;
        private readonly IOCRService _iOCRService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;
        private readonly string _imageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "members");
        private readonly IServiceProvider _serviceProvider;
        #endregion

        #region Constructor
        public CourseController(Application.Interfaces.Admin.ICourseService CourseService, IServiceProvider serviceProvider, Application.Interfaces.Member.ICourseService CourseMemberService,
            IOCRService iOCRService,
            IHubContext<NotificationHub> hubContext,
            INotificationService notificationService,
            ITrainerService TrainerService,
            IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor httpContextAccessor, IAuthorizationService authorizationService)
        {
            _courseService = CourseService;
            _courseMemberService = CourseMemberService;
            _trainerService = TrainerService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _authorizationService = authorizationService;
            _iOCRService = iOCRService;
            _hubContext = hubContext;
            _notificationService = notificationService;
            _serviceProvider = serviceProvider;
        }
        #endregion


        #region Actions
        [YesGet]
        public async Task<IActionResult> Index(string? searchTerm, int? selectedDept, int? selectedTrainer, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            // Get the UserEMail of User Logged in
            var User = _httpContextAccessor.HttpContext?.User;
            var UserEMail = User?.Identity?.Name;

            var ThisTrainerId = await _trainerService.GetThisTrainerId_IfTrainer_else_0(UserEMail);

            var UserIsTrainer = false;
            IEnumerable<Course>? allCourses;
            List<TrainersNameVM>? trainersWithUsers = new List<TrainersNameVM>();

            if (ThisTrainerId == null || ThisTrainerId == 0) // Admin or This User Not Trainer
            {
                UserIsTrainer = false;
                allCourses = await _courseService.GetAllAsync();

                trainersWithUsers = await _unitOfWork.Trainers.Table.Join(_unitOfWork.Users.Table,
                  trainer => trainer.UserId,
                  user => user.Id,
                  (trainer, user) => new TrainersNameVM
                  {
                      Id = trainer.Id,
                      UserId = trainer.UserId,
                      FullNameAr = user.FullNameAr,
                      FullNameEn = user.FullNameEn
                  })
                   .ToListAsync();


            }
            else  // This User IS Trainer
            {
                UserIsTrainer = true;
                allCourses = await _unitOfWork.Courses.GetAllAsync(x => x.TrainerId == ThisTrainerId, t => t.Trainer, t => t.Department);
            }

            var courseVMs = _mapper.Map<List<ViewModels.Course.CourseVM>>(allCourses).AsQueryable();

            var allSubscriptions = await _unitOfWork.Subscriptions.GetAllAsync();

            foreach (var course in courseVMs)
            {
                course.IsSubscribed = allSubscriptions.Any(s =>
                    s.SubscribedInType == SubscriptionType.Course &&
                    s.SubscribedInId == course.Id 
                    && s.Acceptance.HasValue && s.Acceptance.Value == true
                );
            }


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                courseVMs = courseVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.TitleAr) && c.TitleAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.TitleEn) && c.TitleEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            var allDepartments = await _unitOfWork.Departments.GetAllAsync();
            ViewBag.Departments = SelectListHelper.BindSelectList(allDepartments.ToList()).Distinct();

            if (selectedDept != null)
            {
                courseVMs = courseVMs.Where(c => c.DepartmentId == selectedDept);
            }

            ViewBag.Trainers = allCourses
                .Select(c => c.Trainer?.Id)
                .Distinct()
                .ToList();

            if (selectedTrainer != null)
            {
                courseVMs = courseVMs.Where(c => c.TrainerId == selectedTrainer);
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                courseVMs = courseVMs.Where(c => c.StartDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                courseVMs = courseVMs.Where(c => c.StartDate <= dateTo.Value);
            }

            var paginated = PaginatedList<ViewModels.Course.CourseVM>.Create(courseVMs?.OrderByDescending(m => m.Id), page, pageSize, searchTerm);


            ViewBag.SelectedDept = selectedDept;
            ViewBag.SelectedTrainer = selectedTrainer;
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");
            IndexCoursesVM indexCoursesVM = new IndexCoursesVM()
            {
                UserIsTrainer = UserIsTrainer,
                CoursesVM_Paginated = paginated,
                TrainersNameVM_List = trainersWithUsers
            };

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", indexCoursesVM);
            }

            return View(indexCoursesVM);
        }

        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            var previousUrl = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(previousUrl) && previousUrl.Contains("Admin/Course/Index", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Course_AddEdit = "Course.Index";
                SessionExtensions.SetString(HttpContext.Session, "Course_AddEdit", "Course.Index");
            }
            if (!string.IsNullOrEmpty(previousUrl) && previousUrl.Contains("Admin/Course/SubscribedMembersInCourses", StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Course_AddEdit = "SubscribedMembersInCourses";
                SessionExtensions.SetString(HttpContext.Session, "Course_AddEdit", "SubscribedMembersInCourses");
            }


            var vm = new ViewModels.Course.CourseVM();

            if (id.HasValue && id.Value != 0)
            {
                var Course = await _courseService.GetByIdAsync(id.Value);
                if (Course == null) return NotFound();
                vm = _mapper.Map<ViewModels.Course.CourseVM>(Course);
            }


            var departments = await _unitOfWork.Departments.GetAllAsync();

            vm.DepartmentsList = SelectListHelper.BindSelectList(departments.ToList(), vm.DepartmentId).ToList();
            vm.Attachment_OldPath = vm.AttachmentPath;
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ViewModels.Course.CourseVM model)
        {
            var FolderEntityWillSaveIn = "Courses";
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
                var departments = await _unitOfWork.Departments.GetAllAsync();
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


            var entity = _mapper.Map<Course>(model);
            entity.AttachmentPath = model.AttachmentPath;

            if (model.Id == 0)
            {
                model.Id = await _courseService.AddAsync(entity, model.Attachment);
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else
                await _courseService.UpdateAsync(entity, model.Attachment); 

            var Course_AddEdit = SessionExtensions.GetString(HttpContext.Session, "Course_AddEdit");

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit 

            //if (Course_AddEdit == "SubscribedMembersInCourses")
            //{
            //    return RedirectToAction(nameof(SubscribedMembersInCourses));
            //}
            //else if (Course_AddEdit == "Course.Index")
            //{
            //    return RedirectToAction(nameof(Index));
            //}
            //else
            //    return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _courseService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }
        [HttpPost]
        public async Task<IActionResult> DeleteSubscription(int id)
        {
            await _courseService.DeleteSubscriptionAsync(id);
            return RedirectToAction(nameof(SubscribedMembersInCourses));
        }
        [YesGet]
        public async Task<IActionResult> MembersCourse(int courseId , int  page = 1 , int pageSize = 50)
        {
            // Get course
            var course = await _unitOfWork.Courses.GetByIdAsync(c => c.Id == courseId, c => c.Department);
            if (course == null)
                return NotFound();

            // Get members and their subscriptions
            var (members, subscriptions) = await _courseService.GetAllMembersOfCourseAsync(courseId);
            members = members?.OrderByDescending(m => m.Id).ToList();
            subscriptions = subscriptions?.OrderByDescending(s => s.Id).ToList();
            var membersVM = _mapper.Map<List<ViewModels.Member.MemberVM>>(members);

            // Get trainer info (join Trainers and Users)
            var trainer = await (from t in _unitOfWork.Trainers.Table
                                 join u in _unitOfWork.Users.Table on t.UserId equals u.Id
                                 where t.Id == course.TrainerId
                                 select new TrainersNameVM
                                 {
                                     Id = t.Id,
                                     UserId = t.UserId,
                                     FullNameAr = u.FullNameAr,
                                     FullNameEn = u.FullNameEn
                                 }).FirstOrDefaultAsync();


            var paginated = PaginatedList<ViewModels.Member.MemberVM>.Create(membersVM, page, pageSize, null);
            // Compose view model
            var MembersActivityVM = new MembersCourseVM
            {
                Course = _mapper.Map<ViewModels.Course.CourseVM>(course),
                Trainer = trainer,
                Members_Paginated = paginated,
                Subscriptions = subscriptions
            };

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartialMemberSubsInCourse", MembersActivityVM);
            }
            return View(MembersActivityVM);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintMembersCourse(int courseId)
        {
            // Get course
            var course = await _unitOfWork.Courses.GetByIdAsync(c => c.Id == courseId, c => c.Department);
            if (course == null)
                return NotFound();

            // Get members and their subscriptions
            var (members, subscriptions) = await _courseService.GetAllMembersOfCourseAsync(courseId);
            members = members?.OrderByDescending(m => m.Id).ToList();
            subscriptions = subscriptions?.OrderByDescending(s => s.Id).ToList();
            var membersVM = _mapper.Map<List<ViewModels.Member.MemberVM>>(members);

            // Get trainer info (join Trainers and Users)
            var trainer = await (from t in _unitOfWork.Trainers.Table
                                 join u in _unitOfWork.Users.Table on t.UserId equals u.Id
                                 where t.Id == course.TrainerId
                                 select new TrainersNameVM
                                 {
                                     Id = t.Id,
                                     UserId = t.UserId,
                                     FullNameAr = u.FullNameAr,
                                     FullNameEn = u.FullNameEn
                                 }).FirstOrDefaultAsync();

            // Compose view model
            var viewModel = new MembersCourseVM
            {
                Course = _mapper.Map<ViewModels.Course.CourseVM>(course),
                Trainer = trainer,
                Members = membersVM,
                Subscriptions = subscriptions
            };

            return View(viewModel);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download_MembersupscriptiosToCourse(int courseId)
        {
            // Get course
            var course = await _unitOfWork.Courses.GetByIdAsync(c => c.Id == courseId, c => c.Department);
            if (course == null)
                return NotFound();

            // Get members and their subscriptions
            var (members, subscriptions) = await _courseService.GetAllMembersOfCourseAsync(courseId);
            members = members?.OrderByDescending(m => m.Id).ToList();
            subscriptions = subscriptions?.OrderByDescending(s => s.Id).ToList();
            var membersVM = _mapper.Map<List<ViewModels.Member.MemberVM>>(members);

            // Get trainer info (join Trainers and Users)
            var trainer = await (from t in _unitOfWork.Trainers.Table
                                 join u in _unitOfWork.Users.Table on t.UserId equals u.Id
                                 where t.Id == course.TrainerId
                                 select new TrainersNameVM
                                 {
                                     Id = t.Id,
                                     UserId = t.UserId,
                                     FullNameAr = u.FullNameAr,
                                     FullNameEn = u.FullNameEn
                                 }).FirstOrDefaultAsync();

            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                var lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                var allData_list = members;
                var ListTitles = new List<string>();

                var PermissionScanner = new PermissionScanner(_httpContextAccessor, _authorizationService, _unitOfWork);
                var AttendancePermission = PermissionScanner.ValidatePermission("Course", "Attendance");
                var excelDataDTO = new List<ExcelDataDTO>();
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    if (AttendancePermission)
                    {
                        ListTitles = new List<string>
                            {
                                Resource1.subscriberName,Resource1.NationalityId,/*Resource1.SubscriptionHistory,*/
                                /*Resource1.Age,*/Resource1.Mobile,Resource1.Email,
                                Resource2.Attendance2,
                            };
                        excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                        {
                            t1 = (lang == "ar" ? single.FullNameAr : single.FullNameEn),
                            t2 = single.Nationality != null ? (lang == "ar" ? single.Nationality.NameAr : single.Nationality.NameEn) : "",
                            //t3 = (subscriptions != null && subscriptions.Count() > 0 && subscriptions.Where(x => x.MemberId == single.Id).FirstOrDefault() != null && subscriptions.Where(x => x.MemberId == single.Id).FirstOrDefault().ParticipationDate.HasValue) ? (subscriptions.Where(x => x.MemberId == single.Id).FirstOrDefault().ParticipationDate.Value.ToString("d")?.Replace("/","-")) : "",
                            //t4 = single.Age,
                            t3 = single.PhoneNumber,
                            t4 = single.Email,
                            t5 = (subscriptions != null && subscriptions.Count() > 0 && subscriptions.Where(x => x.MemberId == single.Id).FirstOrDefault() != null && subscriptions.Where(x => x.MemberId == single.Id).FirstOrDefault().Attendance.HasValue) ? ((subscriptions.Where(x => x.MemberId == single.Id).FirstOrDefault().Attendance.Value==true)? Resource1.At : Resource1.Ab) : Resource1.Ab,
                        }).ToList();
                    }
                    else
                    {
                        ListTitles = new List<string>
                            {
                                Resource1.subscriberName,Resource1.NationalityId,/*Resource1.SubscriptionHistory,*/
                                /*Resource1.Age,*/Resource1.Mobile,Resource1.Email,
                            };
                        excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                        {
                            t1 = (lang == "ar" ? single.FullNameAr : single.FullNameEn),
                            t2 = single.Nationality != null ? (lang == "ar" ? single.Nationality.NameAr : single.Nationality.NameEn) : "",
                            //t3 = (subscriptions != null && subscriptions.Count() > 0 && subscriptions.Where(x => x.MemberId == single.Id).FirstOrDefault() != null && subscriptions.Where(x => x.MemberId == single.Id).FirstOrDefault().ParticipationDate.HasValue) ? (subscriptions.Where(x => x.MemberId == single.Id).FirstOrDefault().ParticipationDate.Value.ToString("d")?.Replace("/","-")) : "",
                            //t4 = single.Age,
                            t3 = single.PhoneNumber,
                            t4 = single.Email,
                        }).ToList();
                    }
                        

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
                    var fileExcelName = Resource2.ParticipantsInCourse +" : "+ (lang == "ar" ? course?.TitleAr : course?.TitleEn);
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

        [HttpPost]
        public async Task<IActionResult> Attendance(int courseId, int memberId, bool attendance)
        {
            var result = await _courseService.UpdateAttendance(courseId, memberId, attendance);
            return result ? Ok() : BadRequest();
        }
        [YesGet]
        public async Task<IActionResult> SubscribedMembersInCourses(string? searchTerm, int? selectedNationality, int? selectedMemberType, int? selectedGender, int? selectedDept, string? selectedTrainer, DateTime? dateFrom, DateTime? dateTo, int page = 1, int pageSize = 50)
        {
            // Get the UserEMail of User Logged in
            var User = _httpContextAccessor.HttpContext?.User;
            var UserEMail = User?.Identity?.Name;

            var ThisTrainerId = await _trainerService.GetThisTrainerId_IfTrainer_else_0(UserEMail);

            var UserIsTrainer = false;
            IEnumerable<SubscribedMemberCourseDTO>? data;
            List<TrainersNameDTO>? allTrainersNames = new List<TrainersNameDTO>();

            if (ThisTrainerId != null && ThisTrainerId > 0) // User Logged in IS Trainner
            {
                UserIsTrainer = true;
                data = await _courseService.SubscribedMembersInCourses_Trainers(ThisTrainerId ?? 0);
            }
            else   // User or Admin Logged in IS Not Trainner
            {
                UserIsTrainer = false;
                data = await _courseService.SubscribedMembersInCourses_Admin();
                allTrainersNames = await _trainerService.GetAllTrainerNamesAr_En_only();
            }

            var dataVM = _mapper.Map<List<SubscribedMemberCourseVM>>(data);

            #region search by dropdown Trainers Names
            
            ViewBag.allTrainersNames = allTrainersNames;
            ViewBag.selectedTrainer = selectedTrainer;

            if (!string.IsNullOrEmpty(selectedTrainer))
            {
                dataVM = dataVM
                    .Where(c => c.TrainerFullNameAr == selectedTrainer || c.TrainerFullNameEn == selectedTrainer)
                    .ToList();
            }

            #endregion


            #region search by word
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                dataVM = dataVM
                    .Where(c =>
                        (!string.IsNullOrWhiteSpace(c.MemberFullNameAr) && c.MemberFullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(c.MemberFullNameEn) && c.MemberFullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(c.MemberIdNumber) && c.MemberIdNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    )
                    .ToList();
            }
            #endregion

            #region search by dropdown nationality

            var allNationalities = await _unitOfWork.Nationalities.GetAllAsync();
            ViewBag.Nationalities = SelectListHelper.BindSelectList(allNationalities.ToList()).Distinct();

            if (selectedNationality.HasValue)
            {
                dataVM = dataVM
                    .Where(c => c.NationalityId.HasValue && c.NationalityId.Value == selectedNationality.Value)
                    .ToList();
            }
            if (selectedMemberType != null && selectedMemberType != 33)
            {
                dataVM = dataVM.Where(c => c.MemberTypeId == selectedMemberType).ToList();
            }

            #endregion

            #region search by dropdown gender
            ViewBag.Genders = SelectListHelper.GetEnumSelectList<Gender>().Distinct();

            if (selectedGender != null && selectedGender != 3)// 3 is All Gender (male+Female)
            {
                dataVM = dataVM
                    .Where(c => c.MemberGenderId.HasValue && c.MemberGenderId.Value == selectedGender.Value)
                    .ToList();
            }
            #endregion

            #region search by dropdown department

            var allDepartments = await _unitOfWork.Departments.GetAllAsync();
            ViewBag.Departments = SelectListHelper.BindSelectList(allDepartments.ToList()).Distinct();

            if (selectedDept.HasValue)
            {
                dataVM = dataVM
                    .Where(c => c.DepartmentId.HasValue && c.DepartmentId.Value == selectedDept.Value)
                    .ToList();
            }

            #endregion

            #region search by date
            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                dataVM = dataVM
                    .Where(c => c.SubscriptionDate.HasValue && c.SubscriptionDate.Value.Date >= dateFrom.Value.Date)
                    .ToList();
            }

            if (dateTo.HasValue)
            {
                dataVM = dataVM
                    .Where(c => c.SubscriptionDate.HasValue && c.SubscriptionDate.Value.Date <= dateTo.Value.Date)
                    .ToList();
            }
            #endregion
            dataVM = dataVM?.OrderByDescending(m => m.SubscriptionId).ToList();

            var paginated = PaginatedList<SubscribedMemberCourseVM>.Create(dataVM, page, pageSize, searchTerm);


            ViewBag.SelectedGender = selectedGender;
            ViewBag.SelectedDept = selectedDept;
            ViewBag.selectedMemberType = selectedMemberType;
            ViewBag.SelectedNationality = selectedNationality;
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            // If the logged-in user is a trainer, ensure the trainer dropdown (which is disabled)
            // contains the trainer's own name and is selected so it shows properly in the view.
            List<SelectListItem>? trainersSelectList = null;
            if (UserIsTrainer && ThisTrainerId != null && ThisTrainerId > 0)
            {
                var trainerInfo = await _unitOfWork.Trainers.Table.Join(_unitOfWork.Users.Table,
                    t => t.UserId,
                    u => u.Id,
                    (t, u) => new { t.Id, t.DepartmentId, u.FullNameAr, u.FullNameEn })
                    .FirstOrDefaultAsync(x => x.Id == ThisTrainerId);

                if (trainerInfo != null)
                {
                    var name = SessionHelper.GetCurrentLanguage() == "ar" ? trainerInfo.FullNameAr : trainerInfo.FullNameEn;
                    trainersSelectList = new List<SelectListItem>
                    {
                        new SelectListItem { Value = name ?? "", Text = name ?? "", Selected = true }
                    };

                    // Set selected dept so the disabled department dropdown shows the trainer's dept
                    ViewBag.SelectedDept = trainerInfo.DepartmentId;
                    // Also set both casings just in case the view checks one or the other
                    ViewBag.selectedTrainer = name;
                    ViewBag.SelectedTrainer = name;
                }
            }

            var SubscribedMemberCourseVMTrainer = new SubscribedMemberCourseVMTrainer()
            {
                paginated = paginated,
                UserIsTrainer = UserIsTrainer,
                trainerSelect = selectedTrainer,
                TrainersList = trainersSelectList
            };

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartialMember", SubscribedMemberCourseVMTrainer);
            }

            return View(SubscribedMemberCourseVMTrainer);
        }

        [YesGet]
        [HttpPost]
        public async Task<IActionResult> Accept([FromBody] AcceptanceVM model)  // <-- important
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data." });

            var dto = _mapper.Map<AcceptanceDTO>(model);
            var result = await _courseService.UpdateAcceptance(dto);

            return Json(new { success = result, message = result ? "" : "Failed to update acceptance." });
        }

        [YesGet]
        [IgnoreAction]
        public async Task<IActionResult> PrintCertificate(int subscriptionId)
        {
            var config = _serviceProvider.GetRequiredService<IConfiguration>();
            var CertificateURLWebsite = config["CertificateURL:BaseUrl"]; // must be set in appsettings.json or secrets

            var data = await _courseService.GetCertificateData(subscriptionId);
            var model = _mapper.Map<CertificateVM>(data);
            var certificateSerialHashed = HashHelper.Encrypt(model.CertificateSerial ?? "0");
            string encodedCertificateSerialHashed = Uri.EscapeDataString(certificateSerialHashed); // save + , % وهكذا 
            model.CertificateSerialHashed = encodedCertificateSerialHashed;
            var url = CertificateURLWebsite + "/Certificate/CertificateVerified?serialHashed=" + encodedCertificateSerialHashed;
            // Change to Remote URL
            var qrCode = QrCodeHelper.GenerateQrBase64(url);
            model.QrCodeBase64 = qrCode;
            return View(model);
        }


        [HttpGet]
        public IActionResult Rate()
        {
            return Json(new { success = true });
        }


        [Route("Admin/Course/PrintSubscribedMembersInCourses")]
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintSubscribedMembersInCourses(string? searchTerm, int? selectedNationality, int? selectedMemberType, int? selectedGender, int? selectedDept,string? selectedTrainer, DateTime? dateFrom, DateTime? dateTo)
        {
            // Get the UserEMail of User Logged in
            var User = _httpContextAccessor.HttpContext?.User;
            var UserEMail = User?.Identity?.Name;

            var ThisTrainerId = await _trainerService.GetThisTrainerId_IfTrainer_else_0(UserEMail);

            var UserIsTrainer = false;
            IEnumerable<SubscribedMemberCourseDTO>? data;
            List<TrainersNameDTO>? allTrainersNames = new List<TrainersNameDTO>();

            if (ThisTrainerId != null && ThisTrainerId > 0) // User Logged in IS Trainner
            {
                UserIsTrainer = true;
                data = await _courseService.SubscribedMembersInCourses_Trainers(ThisTrainerId ?? 0);
            }
            else   // User or Admin Logged in IS Not Trainner
            {
                UserIsTrainer = false;
                data = await _courseService.SubscribedMembersInCourses_Admin();
                allTrainersNames = await _trainerService.GetAllTrainerNamesAr_En_only();
            }

            var dataVM = _mapper.Map<List<SubscribedMemberCourseVM>>(data);

            #region search by dropdown Trainers Names

            ViewBag.allTrainersNames = allTrainersNames;
            ViewBag.selectedTrainer = selectedTrainer;

            if (!string.IsNullOrEmpty(selectedTrainer))
            {
                dataVM = dataVM
                    .Where(c => c.TrainerFullNameAr == selectedTrainer || c.TrainerFullNameEn == selectedTrainer)
                    .ToList();
            }

            #endregion


            #region search by word
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                dataVM = dataVM
                    .Where(c =>
                        (!string.IsNullOrWhiteSpace(c.MemberFullNameAr) && c.MemberFullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(c.MemberFullNameEn) && c.MemberFullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(c.MemberIdNumber) && c.MemberIdNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    )
                    .ToList();
            }
            #endregion

            #region search by dropdown nationality

            var allNationalities = await _unitOfWork.Nationalities.GetAllAsync();
            ViewBag.Nationalities = SelectListHelper.BindSelectList(allNationalities.ToList()).Distinct();

            if (selectedNationality.HasValue)
            {
                dataVM = dataVM
                    .Where(c => c.NationalityId.HasValue && c.NationalityId.Value == selectedNationality.Value)
                    .ToList();
            }

            #endregion

            #region search by dropdown gender
            ViewBag.Genders = SelectListHelper.GetEnumSelectList<Gender>().Distinct();

            if (selectedGender != null && selectedGender != 3)// 3 is All Gender (male+Female)
            {
                dataVM = dataVM
                    .Where(c => c.MemberGenderId.HasValue && c.MemberGenderId.Value == selectedGender.Value)
                    .ToList();
            }
            if (selectedMemberType != null && selectedMemberType != 33)
            {
                dataVM = dataVM.Where(c => c.MemberTypeId == selectedMemberType).ToList();
            }
            #endregion

            #region search by dropdown department

            var allDepartments = await _unitOfWork.Departments.GetAllAsync();
            ViewBag.Departments = SelectListHelper.BindSelectList(allDepartments.ToList()).Distinct();

            if (selectedDept.HasValue)
            {
                dataVM = dataVM
                    .Where(c => c.DepartmentId.HasValue && c.DepartmentId.Value == selectedDept.Value)
                    .ToList();
            }

            #endregion

            #region search by date
            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                dataVM = dataVM
                    .Where(c => c.SubscriptionDate.HasValue && c.SubscriptionDate.Value.Date >= dateFrom.Value.Date)
                    .ToList();
            }

            if (dateTo.HasValue)
            {
                dataVM = dataVM
                    .Where(c => c.SubscriptionDate.HasValue && c.SubscriptionDate.Value.Date <= dateTo.Value.Date)
                    .ToList();
            }
            #endregion
            if(dataVM!= null && dataVM.Count>0)
                dataVM[0].UserIsTrainer = UserIsTrainer;

            dataVM = dataVM?.OrderByDescending(m => m.SubscriptionId).ToList();


            ViewBag.SelectedGender = selectedGender;
            ViewBag.SelectedDept = selectedDept;
            ViewBag.selectedMemberType = selectedMemberType;
            ViewBag.SelectedNationality = selectedNationality;
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            return View(dataVM);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download_SubscripeMember(string? searchTerm, int? selectedNationality, int? selectedMemberType, int? selectedGender, int? selectedDept, string? selectedTrainer, DateTime? dateFrom, DateTime? dateTo)
        {
            // Get the UserEMail of User Logged in
            var User = _httpContextAccessor.HttpContext?.User;
            var UserEMail = User?.Identity?.Name;

            var ThisTrainerId = await _trainerService.GetThisTrainerId_IfTrainer_else_0(UserEMail);

            var UserIsTrainer = false;
            IEnumerable<SubscribedMemberCourseDTO>? data;
            List<TrainersNameDTO>? allTrainersNames = new List<TrainersNameDTO>();

            if (ThisTrainerId != null && ThisTrainerId > 0) // User Logged in IS Trainer
            {
                UserIsTrainer = true;
                data = await _courseService.SubscribedMembersInCourses_Trainers(ThisTrainerId ?? 0);
            }
            else   // User or Admin Logged in IS Not Trainer
            {
                UserIsTrainer = false;
                data = await _courseService.SubscribedMembersInCourses_Admin();
                allTrainersNames = await _trainerService.GetAllTrainerNamesAr_En_only();
            }

            var dataVM = _mapper.Map<List<SubscribedMemberCourseVM>>(data);

            #region search by dropdown Trainers Names

            ViewBag.allTrainersNames = allTrainersNames;
            ViewBag.selectedTrainer = selectedTrainer;

            if (!string.IsNullOrEmpty(selectedTrainer))
            {
                dataVM = dataVM
                    .Where(c => c.TrainerFullNameAr == selectedTrainer || c.TrainerFullNameEn == selectedTrainer)
                    .ToList();
            }

            #endregion


            #region search by word
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                dataVM = dataVM
                    .Where(c =>
                        (!string.IsNullOrWhiteSpace(c.MemberFullNameAr) && c.MemberFullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(c.MemberFullNameEn) && c.MemberFullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrWhiteSpace(c.MemberIdNumber) && c.MemberIdNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                    )
                    .ToList();
            }
            #endregion

            #region search by dropdown nationality

            var allNationalities = await _unitOfWork.Nationalities.GetAllAsync();
            ViewBag.Nationalities = SelectListHelper.BindSelectList(allNationalities.ToList()).Distinct();

            if (selectedNationality.HasValue)
            {
                dataVM = dataVM
                    .Where(c => c.NationalityId.HasValue && c.NationalityId.Value == selectedNationality.Value)
                    .ToList();
            }

            #endregion

            #region search by dropdown gender
            ViewBag.Genders = SelectListHelper.GetEnumSelectList<Gender>().Distinct();

            if (selectedGender != null && selectedGender != 3)// 3 is All Gender (male+Female)
            {
                dataVM = dataVM
                    .Where(c => c.MemberGenderId.HasValue && c.MemberGenderId.Value == selectedGender.Value)
                    .ToList();
            }
            if (selectedMemberType != null && selectedMemberType != 33)
            {
                dataVM = dataVM.Where(c => c.MemberTypeId == selectedMemberType).ToList();
            }
            #endregion

            #region search by dropdown department

            var allDepartments = await _unitOfWork.Departments.GetAllAsync();
            ViewBag.Departments = SelectListHelper.BindSelectList(allDepartments.ToList()).Distinct();

            if (selectedDept.HasValue)
            {
                dataVM = dataVM
                    .Where(c => c.DepartmentId.HasValue && c.DepartmentId.Value == selectedDept.Value)
                    .ToList();
            }

            #endregion

            #region search by date
            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                dataVM = dataVM
                    .Where(c => c.SubscriptionDate.HasValue && c.SubscriptionDate.Value.Date >= dateFrom.Value.Date)
                    .ToList();
            }

            if (dateTo.HasValue)
            {
                dataVM = dataVM
                    .Where(c => c.SubscriptionDate.HasValue && c.SubscriptionDate.Value.Date <= dateTo.Value.Date)
                    .ToList();
            }
            dataVM = dataVM?.OrderByDescending(m => m.SubscriptionId).ToList();

            #endregion

            ViewBag.SelectedGender = selectedGender;
            ViewBag.SelectedDept = selectedDept;
            ViewBag.selectedMemberType = selectedMemberType;
            ViewBag.SelectedNationality = selectedNationality;
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
                var allData_list = dataVM;
                var ListTitles = new List<string>();
                var excelDataDTO = new List<ExcelDataDTO>();
                ListTitles = new List<string>
                {
                    Resource1.subscriberName,Resource2.Nationality,Resource1.Department,Resource2.CourseName,Resource2.SubscriptionDate,Resource2.Type,Resource2.Trainer,
                };
                //if (UserIsTrainer)
                //{
                    ListTitles.Insert(0, "CLS No");
                //}
                                    
                if (lang == "ar")
                {
                    if (allData_list != null || allData_list?.Count() > 0)
                    {
                        //if (UserIsTrainer)
                        //{
                            excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                            {
                                t1 = single.MemberCode,
                                t2 = single.MemberFullNameAr,
                                t3 = single.NationalityNameAr,
                                t4 = single.DepartmentNameAr,
                                t5 = single.CourseTitleAr,
                                t6 = single.SubscriptionDate.HasValue ? single.SubscriptionDate.Value.ToString("d")?.Replace("/","-") : "",
                                t7 = single.MemberTypeAr,
                                t8 = single.TrainerFullNameAr,
                            }).ToList();
                        //}
                        //else
                        //{
                        //    excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                        //    {
                        //        t1 = single.MemberFullNameAr,
                        //        t2 = single.NationalityNameAr,
                        //        t3 = single.DepartmentNameAr,
                        //        t4 = single.CourseTitleAr,
                        //        t5 = single.SubscriptionDate.HasValue ? single.SubscriptionDate.Value.ToString("d")?.Replace("/","-") : "",
                        //        t6 = single.TrainerFullNameAr,
                        //    }).ToList();
                        //}

                    }
                    (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_<ExcelDataDTO>(excelDataDTO, ListTitles, 0, "ar");
                }
                else
                {
                    if (allData_list != null || allData_list?.Count() > 0)
                    {
                        //if (UserIsTrainer)
                        //{
                            excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                            {
                                t1 = single.MemberCode,
                                t2 = single.MemberFullNameEn,
                                t3 = single.NationalityNameEn,
                                t4 = single.DepartmentNameEn,
                                t5 = single.CourseTitleEn,
                                t6 = single.SubscriptionDate,
                                t7 = single.TrainerFullNameEn,
                            }).ToList();
                        //}
                        //else
                        //{
                        //    excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                        //    {
                        //        t1 = single.MemberFullNameEn,
                        //        t2 = single.NationalityNameEn,
                        //        t3 = single.DepartmentNameEn,
                        //        t4 = single.CourseTitleEn,
                        //        t5 = single.SubscriptionDate,
                        //        t6 = single.TrainerFullNameEn,
                        //    }).ToList();
                        //}
                           
                    }
                    (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_<ExcelDataDTO>(excelDataDTO, ListTitles, 0, "en");
                }

                FileContentResult? Excelfile = null;
                if (fileBytes != null && fileBytes.Length > 0 && boolStatus == true)
                {
                    var fileExcelName = Resource2.RegisteredCourses;
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
        public async Task<IActionResult> GetTrainersInDept(int deptId,int? trainerId)
        {
            var trainers = await _trainerService.GetAllTrainerNamesAr_En_only(); // Or pass courseId if needed
            var filtered = trainers?.Where(t => t.DepartmentId == deptId).ToList();
            if (filtered != null)
            {
                var selectList = SelectListHelper.BindSelectList(
                    filtered,
                    selected: trainerId,
                    valueProperty: "Id",
                    nameAr: "FullNameAr",
                    nameEn: "FullNameEn"
                );
                return Json(new { trainers = selectList });

            }
            return Json(new { trainers = "" });
        }

        [IgnoreAction]
        [Route("Admin/Course/PrintIndex")]
        [YesGet]
        public async Task<IActionResult> PrintIndex(string? searchTerm, int? selectedDept, int? selectedTrainer, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            // Get the UserEMail of User Logged in
            var User = _httpContextAccessor.HttpContext?.User;
            var UserEMail = User?.Identity?.Name;

            var ThisTrainerId = await _trainerService.GetThisTrainerId_IfTrainer_else_0(UserEMail);

            var UserIsTrainer = false;
            IEnumerable<Course>? allCourses;
            List<TrainersNameVM>? trainersWithUsers = new List<TrainersNameVM>();

            if (ThisTrainerId == null || ThisTrainerId == 0) // Admin or This User Not Trainer
            {
                UserIsTrainer = false;
                allCourses = await _courseService.GetAllAsync();

                trainersWithUsers = await _unitOfWork.Trainers.Table.Join(_unitOfWork.Users.Table,
                  trainer => trainer.UserId,
                  user => user.Id,
                  (trainer, user) => new TrainersNameVM
                  {
                      Id = trainer.Id,
                      UserId = trainer.UserId,
                      FullNameAr = user.FullNameAr,
                      FullNameEn = user.FullNameEn
                  })
                   .ToListAsync();


            }
            else  // This User IS Trainer
            {
                UserIsTrainer = true;
                allCourses = await _unitOfWork.Courses.GetAllAsync(x => x.TrainerId == ThisTrainerId, t => t.Trainer, t => t.Department);
            }

            var courseVMs = _mapper.Map<List<ViewModels.Course.CourseVM>>(allCourses).AsQueryable();

            var allSubscriptions = await _unitOfWork.Subscriptions.GetAllAsync();

            foreach (var course in courseVMs)
            {
                course.IsSubscribed = allSubscriptions.Any(s =>
                    s.SubscribedInType == SubscriptionType.Course &&
                    s.SubscribedInId == course.Id
                );
            }


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                courseVMs = courseVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.TitleAr) && c.TitleAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.TitleEn) && c.TitleEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            var allDepartments = await _unitOfWork.Departments.GetAllAsync();
            ViewBag.Departments = SelectListHelper.BindSelectList(allDepartments.ToList()).Distinct();

            if (selectedDept != null)
            {
                courseVMs = courseVMs.Where(c => c.DepartmentId == selectedDept);
            }

            ViewBag.Trainers = allCourses
                .Select(c => c.Trainer?.Id)
                .Distinct()
                .ToList();

            if (selectedTrainer != null)
            {
                courseVMs = courseVMs.Where(c => c.TrainerId == selectedTrainer);
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                courseVMs = courseVMs.Where(c => c.StartDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                courseVMs = courseVMs.Where(c => c.StartDate <= dateTo.Value);
            }

            courseVMs = courseVMs?.OrderByDescending(x => x.Id);

            ViewBag.SelectedDept = selectedDept;
            ViewBag.SelectedTrainer = selectedTrainer;
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");
            IndexCoursesVM indexCoursesVM = new IndexCoursesVM()
            {
                UserIsTrainer = UserIsTrainer,
                CoursesVM = courseVMs,
                TrainersNameVM_List = trainersWithUsers
            };

            return View(indexCoursesVM);
        }
        #endregion
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, int? selectedDept, int? selectedTrainer, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            // Get the UserEMail of User Logged in
            var User = _httpContextAccessor.HttpContext?.User;
            var UserEMail = User?.Identity?.Name;

            var ThisTrainerId = await _trainerService.GetThisTrainerId_IfTrainer_else_0(UserEMail);

            var UserIsTrainer = false;
            IEnumerable<Course>? allCourses;
            List<TrainersNameVM>? trainersWithUsers = new List<TrainersNameVM>();

            if (ThisTrainerId == null || ThisTrainerId == 0) // Admin or This User Not Trainer
            {
                UserIsTrainer = false;
                allCourses = await _courseService.GetAllAsync();

                trainersWithUsers = await _unitOfWork.Trainers.Table.Join(_unitOfWork.Users.Table,
                  trainer => trainer.UserId,
                  user => user.Id,
                  (trainer, user) => new TrainersNameVM
                  {
                      Id = trainer.Id,
                      UserId = trainer.UserId,
                      FullNameAr = user.FullNameAr,
                      FullNameEn = user.FullNameEn
                  })
                   .ToListAsync();


            }
            else  // This User IS Trainer
            {
                UserIsTrainer = true;
                allCourses = await _unitOfWork.Courses.GetAllAsync(x => x.TrainerId == ThisTrainerId, t => t.Trainer, t => t.Department);
            }

            var courseVMs = _mapper.Map<List<ViewModels.Course.CourseVM>>(allCourses).AsQueryable();

            var allSubscriptions = await _unitOfWork.Subscriptions.GetAllAsync();

            foreach (var course in courseVMs)
            {
                course.IsSubscribed = allSubscriptions.Any(s =>
                    s.SubscribedInType == SubscriptionType.Course &&
                    s.SubscribedInId == course.Id
                );
            }


            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                courseVMs = courseVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.TitleAr) && c.TitleAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.TitleEn) && c.TitleEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }

            var allDepartments = await _unitOfWork.Departments.GetAllAsync();
            ViewBag.Departments = SelectListHelper.BindSelectList(allDepartments.ToList()).Distinct();

            if (selectedDept != null)
            {
                courseVMs = courseVMs.Where(c => c.DepartmentId == selectedDept);
            }

            ViewBag.Trainers = allCourses
                .Select(c => c.Trainer?.Id)
                .Distinct()
                .ToList();

            if (selectedTrainer != null)
            {
                courseVMs = courseVMs.Where(c => c.TrainerId == selectedTrainer);
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                courseVMs = courseVMs.Where(c => c.StartDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                courseVMs = courseVMs.Where(c => c.StartDate <= dateTo.Value);
            }



            ViewBag.SelectedDept = selectedDept;
            ViewBag.SelectedTrainer = selectedTrainer;
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");
            IndexCoursesVM indexCoursesVM = new IndexCoursesVM()
            {
                UserIsTrainer = UserIsTrainer,
                CoursesVM = courseVMs,
                TrainersNameVM_List = trainersWithUsers
            };
            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                var allData_list = (indexCoursesVM?.CoursesVM != null && indexCoursesVM?.CoursesVM.Count()>0) ? indexCoursesVM.CoursesVM?.OrderByDescending(m => m.Id).ToList():new List<ViewModels.Course.CourseVM>();
                var ListTitles = new List<string>();
                var excelDataDTO = new List<ExcelDataDTO>();
                if (indexCoursesVM?.UserIsTrainer != null && indexCoursesVM.UserIsTrainer) // User Is Trainer
                {
                    ListTitles = new List<string>
                    {
                        Resource1.CourseTitle,Resource2.StartDate,Resource2.EndDate,Resource1.Time,
                    };
                    if (allData_list != null || allData_list?.Count() > 0)
                    {
                        excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                        {
                            t1 = (lang == "ar" ? single.TitleAr : single.TitleEn),
                            t2 = single.StartDate.HasValue ? single.StartDate.Value.ToString("d")?.Replace("/","-") : "",
                            t3 = single.EndDate.HasValue ? single.EndDate.Value.ToString("d")?.Replace("/","-") : "",
                            t4 = single.Time,
                        }).ToList();
                    }
                }
                else  // User Isn't Trainer
                {
                    ListTitles = new List<string>
                    {
                        Resource1.CourseTitle,Resource2.TainerName,Resource2.StartDate,Resource2.EndDate,Resource1.Time,Resource1.Specialization,
                    };
                    if (allData_list != null || allData_list?.Count() > 0)
                    {
                        excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                        {
                            t1 = (lang == "ar" ? single.TitleAr : single.TitleEn),
                            t2 = ((lang == "ar") ? ((indexCoursesVM?.TrainersNameVM_List?.Where(x => x.Id == single.TrainerId).FirstOrDefault()?.FullNameAr) ?? "") : ((indexCoursesVM?.TrainersNameVM_List?.Where(x => x.Id == single.TrainerId).FirstOrDefault()?.FullNameEn) ?? "")),
                            t3 = single.StartDate,
                            t4 = single.EndDate,
                            t5 = single.Time,
                            t6 = (lang == "ar" ? (single.Department?.NameAr) ?? "" : (single.Department?.NameEn) ?? ""),
                        }).ToList();
                    }
                }


                if (lang == "ar")
                {
                    (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "ar");
                }
                else
                {
                    (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "en");
                }
                
                FileContentResult? Excelfile = null;
                if (fileBytes != null && fileBytes.Length > 0 && boolStatus == true)
                {
                    var fileExcelName = Resource1.CoursesList;
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
        public async Task<IActionResult> UpdateSubscribe(int? id)
        {
            if (id == null) return NotFound();

            var subscription = await _unitOfWork.Subscriptions.GetByIdAsync(id.Value);
            if (subscription == null) return NotFound();

            var course = await _unitOfWork.Courses.GetByIdAsync(subscription.SubscribedInId);
            if (course == null) return NotFound();

            CourseSubVM coursesVM = new CourseSubVM
            {
                Id = course.Id,
                Title = SessionHelper.GetCurrentLanguage() == "ar" ? course.TitleAr : course.TitleEn,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
                SubscriptionId = subscription.Id,
                memberId = subscription.MemberId,
            };

            var user = await _unitOfWork.Members.GetByColumnAsync(u => u.Id == subscription.MemberId);
            if (user == null || coursesVM.Id == null)
            {
                //ModelState.AddModelError("IdImage", Resource1.NoData);
                coursesVM.isHasIDCard = false;
                coursesVM.isHasPassport = false;
                coursesVM.isNotExpired = false;
                //return View(coursesVM);
                return PartialView("_UpdateSubscripeModal", coursesVM);
            }
            coursesVM.IdImagePath = user.IdImagePath;
            coursesVM.PassportImagePath = user.PassportImagePath;
            if ((string.IsNullOrEmpty(user.IdImagePath)) && (string.IsNullOrEmpty(user.PassportImagePath)))
            {
                coursesVM.isHasCode = 3;
            }

            if (string.IsNullOrEmpty(user.IdImagePath) || !FileHelper.IsFileExist(user.IdImagePath))
            {
                //ModelState.AddModelError("IdImage", Resource1.UploadIDCard);
                coursesVM.isHasIDCard = false;
            }
            if (string.IsNullOrEmpty(user.PassportImagePath) || !FileHelper.IsFileExist(user.PassportImagePath))
            {
                coursesVM.isHasPassport = false;
                //ModelState.AddModelError("PassportImage", Resource1.UploadPassportImage);
            }
            if (user.IdExpiryDate < DateOnly.FromDateTime(AppDubaiTime.Now))
            {
                //ModelState.AddModelError("IdImage", Resource1.UploadIDCardNew);
                // //FileHelper.DeleteImageFile(coursesVM.IdImagePath); // Delete old
                coursesVM.isNotExpired = false;
                coursesVM.isHasIDCard = false;
            }
            //return View(coursesVM);
            return PartialView("_UpdateSubscripeModal", coursesVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateSubscribe(CourseSubVM model)
        {
            #region Validate Files are Images and Size
            async Task ValidateImageAsync(IFormFile file, string oldPath, string key)
            {
                var result = await FileHelper.CheckFileIsImage_3Mg_Async(file);
                if (result != "OK" && result != "null")
                {
                    ModelState.AddModelError(key, result);
                    FileHelper.DeleteImageFile(oldPath); // Delete old
                }
            }

            await ValidateImageAsync(model.IdImage, model.IdImagePath, "IdImage");
            await ValidateImageAsync(model.PassportImage, model.PassportImagePath, "PassportImage");
            #endregion

            #region Get current user
            var user = await _unitOfWork.Members.GetByColumnAsync(u => u.Id == model.memberId);

            if (user == null || model.Id == null)
            {
                model.isHasIDCard = false;
                model.isHasPassport = false;
                model.isNotExpired = false;
                ModelState.AddModelError("IdImage", Resource1.NoData);
                return RedirectToAction(nameof(SubscribedMembersInCourses));
            }
            #endregion

            #region Check existing images or uploaded files
            bool idExists = FileHelper.IsFileExist(user.IdImagePath) || (model.IdImage != null && model.IdImage.Length > 0);
            bool passportExists = FileHelper.IsFileExist(user.PassportImagePath) || (model.PassportImage != null && model.PassportImage.Length > 0);

            #region Ensure folder exists
            Directory.CreateDirectory(_imageSavePath);

            string? SaveImage(IFormFile file)
            {
                if (file == null || file.Length == 0) return null;

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string filePath = Path.Combine(_imageSavePath, fileName);

                using var stream = new FileStream(filePath, FileMode.Create);
                file.CopyTo(stream);

                return $"/uploads/Members/{fileName}";
            }
            #endregion

            if (!idExists)
            {
                model.isHasIDCard = false;
                ModelState.AddModelError("IdImage", Resource1.UploadIDCard);
                FileHelper.DeleteImageFile(user.IdImagePath);
            }
            else
            {
                #region Save ID Card Image
                if (model.IdImage != null && model.IdImage.Length > 0)
                {
                    model.isHasIDCard = true;
                    FileHelper.DeleteImageFile(user.IdImagePath);
                    user.IdImagePath = SaveImage(model.IdImage);
                    _unitOfWork.Members.Update(user);
                    await _unitOfWork.CompleteAsync();
                }
                #endregion
            }

            if (!passportExists)
            {
                model.isHasPassport = false;
                ModelState.AddModelError("PassportImage", Resource1.UploadPassportImage);
                FileHelper.DeleteImageFile(user.PassportImagePath);
            }
            else
            {
                #region Save Passport Image
                if (model.PassportImage != null && model.PassportImage.Length > 0)
                {
                    model.isHasPassport = true;
                    FileHelper.DeleteImageFile(user.PassportImagePath);
                    user.PassportImagePath = SaveImage(model.PassportImage);
                    _unitOfWork.Members.Update(user);
                    await _unitOfWork.CompleteAsync();
                }
                #endregion
            }

            if (!ModelState.IsValid)
                return RedirectToAction(nameof(SubscribedMembersInCourses));
            #endregion



            #region Commented OCR & Validation Code --> Validation If Extracted Data == Database Data
            //if (model.IdImage == null || model.IdImage?.Length == 0)
            //{
            //    model.isHasIDCard = false;
            //    ModelState.AddModelError("IdImage", Resource1.UploadIDCard);
            //    FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
            //    return View(model);
            //}

            //if ((model.IdImage?.Length == model.PassportImage?.Length) && model.IdImage != null && model.PassportImage != null)
            //{
            //    model.isHasPassport = false;
            //    model.isHasIDCard = false;
            //    ModelState.AddModelError("IdImage", Resource1.UploadIDCardDiffrent);
            //    FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
            //    return View(model);
            //}

            //var result = await IDClassification_Json(model.IdImage) as JsonResult;
            //var iDCardExtractedDataVM = result.Value as IDCardExtractedDataVM;

            //// You can use vm here
            //if (iDCardExtractedDataVM?.doneOCR_bool == true)
            //{
            //    MemberDTO memberDTO = _mapper.Map<MemberDTO>(user);
            //    IDCardExtractedDataDTO iDCardExtractedDataDTO = _mapper.Map<IDCardExtractedDataDTO>(iDCardExtractedDataVM);
            //    var (Compaire_percentage_FullEnName, Compaire_percentage_FullArName, Compaire_percentage_IDNumber, Compaire_percentage_BirthDate, Compaire_percentage_ExpiryDate, VM_ExpiryDate_DT) = await _memberService.ValidationCompareAllInputsToExtractedAsync(memberDTO, iDCardExtractedDataDTO);

            //    if (Compaire_percentage_FullEnName > 65 && Compaire_percentage_FullArName > 55 && Compaire_percentage_IDNumber > 99 && Compaire_percentage_BirthDate > 99)
            //    {
            //        iDCardExtractedDataVM.doneValidation_bool = true;
            //        // update ExpiryDate of Member IDCard to day (01/++Month/year) of ExpiryDate
            //        if (VM_ExpiryDate_DT != null && VM_ExpiryDate_DT.HasValue)
            //        {
            //            var NewExpiryDate = VM_ExpiryDate_DT?.Date.AddMonths(1);
            //            user.IdExpiryDate = NewExpiryDate.HasValue
            //                                ? DateOnly.FromDateTime(NewExpiryDate.Value)
            //                                : (DateOnly?)null;
            //            if (user.IdExpiryDate >= DateOnly.FromDateTime(AppDubaiTime.Now))
            //            {
            //                _unitOfWork.Members.Update(user);
            //                await _unitOfWork.CompleteAsync();
            //            }
            //            else
            //            {
            //                model.isHasIDCard = false;
            //                model.isNotExpired = false;
            //                ModelState.AddModelError("IdImage", Resource1.ThisIDCardIsExpired ?? " ");
            //                FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
            //                return View(model);
            //            }

            //        }
            //        else
            //        {
            //            model.isHasIDCard = false;
            //            ModelState.AddModelError("IdImage", iDCardExtractedDataVM?.DoneTextExtracted_Error_Str ?? " ");
            //            FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
            //            return View(model);
            //        }

            //    }
            //    else
            //    {
            //        //if (Compaire_percentage_FullEnName <= 65) ModelState.AddModelError("FullNameEn", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
            //        //if (Compaire_percentage_FullArName <= 55) ModelState.AddModelError("FullNameAr", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
            //        //if (Compaire_percentage_IDNumber <= 99) ModelState.AddModelError("IdNumber", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
            //        //if (Compaire_percentage_BirthDate <= 99) ModelState.AddModelError("DateOfBirth", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
            //        //if (Compaire_percentage_ExpiryDate <= 99) ModelState.AddModelError("IdExpiryDate", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
            //        model.isHasIDCard = false;
            //        ModelState.AddModelError("IdImage", Resource1.ThisIsnotaPreviouslyRegisteredIDCard ?? " ");
            //        FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
            //        return View(model);
            //    }
            //}
            //else
            //{
            //    model.isHasIDCard = false;
            //    ModelState.AddModelError("IdImage", iDCardExtractedDataVM?.DoneTextExtracted_Error_Str ?? Resource1.UploadIDCardThisIsNot ?? " ");
            //    FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
            //    return View(model);
            //}
            #endregion

            #region Subscribe and Notifications
            await _courseMemberService.SubscribeAsync(model.Id, user.Email);
            //await _hubContext.Clients.Groups("CourseManagers")
            //    .SendAsync("ReceiveNotification", new
            //    {
            //        Title = "New Student Joined",
            //        Message = "Ahmed just joined the platform!"
            //    });

            //await _notificationService.SendNotificationToPermissionAsync(
            //    "تسجيل جديد في دورة",
            //    "قام طالب جديد بالتسجيل دي دورة",
            //    "Course.Index"
            //);
            #endregion
            return RedirectToAction(nameof(SubscribedMembersInCourses));
        }


    }

}
