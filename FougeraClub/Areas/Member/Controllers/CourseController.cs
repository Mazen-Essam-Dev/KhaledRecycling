using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Interfaces.Member;
using AutoMapper;
using DocumentFormat.OpenXml.Presentation;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Member.ViewModels;
using FougeraClub.Helpers;
using FougeraClub.Hub;
using FougeraClub.Middelware;
using Infrastructure.Attributes;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using static System.Net.WebRequestMethods;

namespace FougeraClub.Areas.Member.Controllers
{
    [MemberAuthorize]
    [Area("Member")]
    [Route("Member/[controller]/[action]")]
    public class CourseController : Controller
    {
        #region properties
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly Application.Interfaces.Member.ICourseService _courseService;
        private readonly Application.Interfaces.Admin.ICourseService _courseAdminService;
        private readonly IMapper _mapper;
        private readonly IOCRService _iOCRService;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;
        private readonly string _imageSavePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "members");
        private readonly IServiceProvider _serviceProvider;

        #endregion

        #region constructor
        public CourseController(IUnitOfWork UnitOfWork, IHttpContextAccessor httpContextAccessor, IServiceProvider serviceProvider, Application.Interfaces.Member.ICourseService courseService, IMapper mapper
            , Application.Interfaces.Admin.ICourseService courseAdminService, IOCRService oCRService , IHubContext<NotificationHub> hubContext , INotificationService notificationService
            )
        {
            _unitOfWork = UnitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _courseService = courseService;
            _mapper = mapper;
            _courseAdminService = courseAdminService;
            _iOCRService = oCRService;
            _hubContext = hubContext;
            _notificationService = notificationService;
            _serviceProvider = serviceProvider;
        }
        #endregion

        #region IDClassification_Json
        [HttpPost]
        public async Task<IActionResult> IDClassification_Json(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    return Json(new IDCardExtractedDataVM
                    {
                        lable = "Invalid File",
                        ProbabilityString = "0"
                    });
                }

                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    fileBytes = memoryStream.ToArray();
                }

                var IDClassificationModel = new IDClassificationMLModel.ModelInput()
                {
                    ImageSource = fileBytes,
                };

                var sortedScoresWithLabel = IDClassificationMLModel.PredictAllLabels(IDClassificationModel);
                var hightestPrediction = sortedScoresWithLabel.FirstOrDefault();

                string probabilityString = $"{hightestPrediction.Value * 100:0.##}%";
                double probability = hightestPrediction.Value * 100;
                int maxPercent = 90;

                if (!(hightestPrediction.Key.ToUpper() == "ID"))
                {
                    return Json(new IDCardExtractedDataVM
                    {
                        doneAI_bool = false,
                        DoneTextExtracted_Error_Str = Resource1.UploadIDCardThisIsNot,
                        lable = hightestPrediction.Key.ToUpper()
                    });
                }

                if (probability < maxPercent)
                {
                    return Json(new IDCardExtractedDataVM
                    {
                        doneAI_bool = false,
                        DoneTextExtracted_Error_Str = Resource1.CaptureThisImageAgainFromFrontFace,
                        lable = hightestPrediction.Key.ToUpper(),
                        ProbabilityString = probabilityString,
                        Probability_double = probability
                    });
                }

                var grayPath = await _iOCRService.ReadGrayTextAsync(file);
                var dto = await _iOCRService.ExtractAllTextDataFrom_IDCardGray_Async(grayPath);
                var removedgrayPath = "temp" + grayPath.Split("temp")[1];
                FileHelper.DeleteImageFile(removedgrayPath);
                var vm = _mapper.Map<IDCardExtractedDataVM>(dto);
                vm.doneAI_bool = true;
                vm.lable = hightestPrediction.Key.ToUpper();
                vm.ProbabilityString = probabilityString;
                vm.Probability_double = probability;

                vm.DoneTextExtracted_Error_Str =
                    (vm.doneOCR_bool == true)
                        ? Resource1.DataExtractedCorrectly
                        : Resource1.CaptureThisImageAgainWithHighQuality;

                return Json(vm);
            }
            catch (Exception ex)
            {
                return Json(new IDCardExtractedDataVM
                {
                    lable = "Error",
                    DoneTextExtracted_Error_Str = ex.Message,
                    ProbabilityString = "0"
                });
            }
        }
        #endregion

        #region actions
        // // GET: CoursesController
        [YesGet]
        public async Task<IActionResult> Index()
        {
            var session = _httpContextAccessor?.HttpContext?.Session;
            var username = session?.GetString("Email");
            var user = await _unitOfWork.Members.GetByColumnAsync(u => u.Email == username);

            #region courses
            var (Courses, coursesSubscriptions) = await _courseService.GetAllAsync(username);

            var coursesVM = _mapper.Map<List<CourseVM>>(Courses);
            foreach (var vm in coursesVM)
            {
                vm.SubscriptionId = coursesSubscriptions.FirstOrDefault(e => e.SubscribedInId == vm.Id && e.MemberId == user.Id)?.Id;
                vm.Subscribed = coursesSubscriptions.Any(e => e.SubscribedInId == vm.Id);
                vm.selectedRate = coursesSubscriptions.FirstOrDefault(e => e.SubscribedInId == vm.Id)?.Rate;
                vm.Attendance = coursesSubscriptions.FirstOrDefault(e => e.SubscribedInId == vm.Id)?.Attendance;
                vm.Accepted = coursesSubscriptions.FirstOrDefault(e => e.SubscribedInId == vm.Id)?.Acceptance;
                vm.RejectionNotes = coursesSubscriptions.FirstOrDefault(e => e.SubscribedInId == vm.Id)?.Notes;
            }
            #endregion

            // Order courses by start date ascending (show earliest dates first)
            coursesVM = coursesVM.OrderBy(x => x.StartDate ?? DateOnly.FromDateTime(DateTime.MaxValue)).ToList();
            return View(coursesVM);
        }


        [HttpGet]
        [NoLogging]
        public async Task<IActionResult> TestSignalR()
        {
            await _hubContext.Clients.Groups("CourseManagers", "EventManagers")
             .SendAsync("ReceiveNotification", new
             {
                 Title = "New Student Joined",
                 Message = "Ahmed just joined the platform!"
             });


            await _notificationService.SendNotificationToPermissionAsync(
        
        "New Student Joined",
        "Ahmed just joined the platform!",
        "Course.Index"
    );

            // You can return JSON or a normal view
            return Json(new { success = true, message = "Notification sent!" });
        }

        [HttpGet]
        [YesGet]
        public async Task<IActionResult> GetCourse(int id)
        {
            var course = await _unitOfWork.Courses.GetByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }
            var session = _httpContextAccessor?.HttpContext?.Session;
            var username = session?.GetString("Email");
            var userData = await _unitOfWork.Members.GetAllAsync(x => x.Email == username);
            var Department = await _unitOfWork.Departments.GetByIdAsync(x => x.Id == course.DepartmentId);

            if (!FileHelper.IsFileExist(course.AttachmentPath)) course.AttachmentPath = null;

            return Json(new
            {
                id = course.Id,
                department = SessionHelper.GetCurrentLanguage() == "ar" ? Department?.NameAr : Department?.NameEn,
                title = SessionHelper.GetCurrentLanguage() == "ar" ? course.TitleAr : course.TitleEn,
                startDate = course.StartDate?.ToString("d")?.Replace("/","-"),
                endDate = course.EndDate?.ToString("d")?.Replace("/","-"),
                location = course.Location,
                time = course.Time,
                description = course.Description,
                attachmentPath = course.AttachmentPath?.Replace("~", "")
            });
        }


        public async Task<IActionResult> Subscribe(int? id)
        {
            if (id == null) return NotFound();

            var course = await _unitOfWork.Courses.GetByIdAsync(id.Value);
            if (course == null) return NotFound();

            CourseVM coursesVM = new CourseVM
            {
                Id = course.Id,
                Title = SessionHelper.GetCurrentLanguage() == "ar" ? course.TitleAr : course.TitleEn,
                StartDate = course.StartDate,
                EndDate = course.EndDate,
            };
            var session = _httpContextAccessor?.HttpContext?.Session;
            var username = session?.GetString("Email");
            var user = await _unitOfWork.Members.GetByColumnAsync(u => u.Email == username);
            if (user == null || coursesVM.Id == null)
            {
                //ModelState.AddModelError("IdImage", Resource1.NoData);
                coursesVM.isHasIDCard = false;
                coursesVM.isHasPassport = false;
                coursesVM.isNotExpired = false;
                return View(coursesVM);
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
            return View(coursesVM);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Subscribe(CourseVM model)
        {
            ModelState.Clear();

            #region Validate Files are Images and Size         
            // Local function to validate the uploaded image size & type
            async Task ValidateImageAsync(IFormFile? file, string? filePath, string key)
            {
                var result = await FileHelper.CheckFileIsImage_3Mg_Async(file);
                if (result != "OK" && result != "null")
                {
                    // Add validation error
                    ModelState.AddModelError(key, result);

                    // Delete old actual image from Final folder if exists
                    FileHelper.DeleteImageFile(filePath);
                }
            }
            IFormFile? idImageTemp = !string.IsNullOrEmpty(model.IdImage_TempFilePath) ? FileHelper.ConvertToIFormFile(model.IdImage_TempFilePath) : model.IdImage;
            string? idImagepath = !string.IsNullOrEmpty(model.IdImage_TempFilePath) ? model.IdImage_TempFilePath : model.IdImagePath;
            await ValidateImageAsync(idImageTemp, idImagepath, "IdImage");
            IFormFile? passportImageTemp = !string.IsNullOrEmpty(model.PassportImage_TempFilePath) ? FileHelper.ConvertToIFormFile(model.PassportImage_TempFilePath) : model.PassportImage;
            string? passportImagepath = !string.IsNullOrEmpty(model.PassportImage_TempFilePath) ? model.PassportImage_TempFilePath : model.PassportImagePath;
            await ValidateImageAsync(passportImageTemp, passportImagepath, "PassportImage");
            #endregion


            #region Get current user
            var username = _httpContextAccessor?.HttpContext?.Session?.GetString("Email");
            var user = await _unitOfWork.Members.GetByColumnAsync(u => u.Email == username);

            if (user == null || model.Id == null)
            {
                model.isHasIDCard = false;
                model.isHasPassport = false;
                model.isNotExpired = false;
                ModelState.Clear();
                ModelState.AddModelError("IdImage", Resource1.NoData);
                return View(model);
            }
            #endregion


            #region TEMP UPLOAD HANDLING (New Logic)
            // Ensure temp directory exists
            Directory.CreateDirectory("wwwroot/uploads/temp");

            // Save file temporarily (returns filename only)
            async Task<string?> SaveTempAsync(IFormFile? file)
            {
                if (file == null || file.Length == 0) return null;

                string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string tempPath = Path.Combine("wwwroot/uploads/temp", fileName);

                using (var stream = new FileStream(tempPath, FileMode.Create))
                    await file.CopyToAsync(stream);

                return tempPath;
            }

            // Save ID Image temporarily
            if (model.IdImage != null && model.IdImage.Length > 0)
            {
                // Store temp file path in model (not IFormFile)
                model.IdImage_TempFilePath = await SaveTempAsync(model.IdImage);
            }

            // Save Passport Image temporarily
            if (model.PassportImage != null && model.PassportImage.Length > 0)
            {
                model.PassportImage_TempFilePath = await SaveTempAsync(model.PassportImage);
            }

            #region Check existing images or uploaded files
            bool idExists = FileHelper.IsFileExist(model.IdImage_TempFilePath?.Replace("wwwroot/","")) || (model.IdImage != null && model.IdImage.Length > 0 );
            bool odlIDExist = FileHelper.IsFileExist(model.IdImagePath);
            bool passportExists = FileHelper.IsFileExist(model.PassportImage_TempFilePath?.Replace("wwwroot/", "")) || (model.PassportImage != null && model.PassportImage.Length > 0) || (FileHelper.IsFileExist(model.PassportImagePath));

            if (!passportExists)
            {
                model.isHasPassport = false;
                ModelState.AddModelError("PassportImage", Resource1.UploadPassportImage);
                FileHelper.DeleteImageFile(user.PassportImagePath);
            }
            if ((!odlIDExist && !idExists )|| (!idExists && ( model.isNotExpired.HasValue && !model.isNotExpired.Value )))
            {
                model.isHasIDCard = false;
                ModelState.AddModelError("IdImage", Resource1.UploadIDCard);
                FileHelper.DeleteImageFile(user.IdImagePath);
            }

            // If ModelState fails → return View with temp paths (files not lost)
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            #endregion

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

            #region FINAL SAVE LOGIC (Executed only if model is valid)

            // Ensure final folder exists
            Directory.CreateDirectory(_imageSavePath);

            // Move temp file to final folder
            string MoveTempToFinal(string tempFileName)
            {
                //string tempPath = Path.Combine("wwwroot/uploads/temp", tempFileName);
                string tempPath = tempFileName;
                string finalName = Guid.NewGuid() + Path.GetExtension(tempFileName);
                string finalPath = Path.Combine(_imageSavePath, finalName);

                if (System.IO.File.Exists(tempPath))
                    System.IO.File.Move(tempPath, finalPath);

                return $"uploads/Members/{finalName}";
            }

            #region Save Final ID Card Image
            if (!string.IsNullOrEmpty(model.IdImage_TempFilePath))
            {
                // Delete old Final image
                FileHelper.DeleteImageFile(user.IdImagePath);

                // Move temp → final
                user.IdImagePath = MoveTempToFinal(model.IdImage_TempFilePath);

                _unitOfWork.Members.Update(user);
                await _unitOfWork.CompleteAsync();
            }
            #endregion

            #region Save Final Passport Image
            if (!string.IsNullOrEmpty(model.PassportImage_TempFilePath))
            {
                FileHelper.DeleteImageFile(user.PassportImagePath);

                user.PassportImagePath = MoveTempToFinal(model.PassportImage_TempFilePath);

                _unitOfWork.Members.Update(user);
                await _unitOfWork.CompleteAsync();
            }
            #endregion

            #endregion


            #region Subscribe and Notifications
            await _courseService.SubscribeAsync(model.Id, username);

            await _hubContext.Clients.Groups("CourseManagers")
                .SendAsync("ReceiveNotification", new
                {
                    Title = "New Student Joined",
                    Message = "Ahmed just joined the platform!"
                });

            await _notificationService.SendNotificationToPermissionAsync(
                "تسجيل جديد في دورة",
                "قام طالب جديد بالتسجيل في دورة",
                "Course.Index"
            );
            #endregion

            return RedirectToAction(nameof(Index));
        }


        // // old 
        //     public async Task<IActionResult> Subscribe(CourseVM model)
        //     {
        //         #region Validate Files are Images and Size
        //         var IdImage_Text = await FileHelper.CheckFileIsImage_3Mg_Async(model.IdImage);
        //         if ((IdImage_Text != "OK" && IdImage_Text != "null")) {
        //             ModelState.AddModelError("IdImage", IdImage_Text);
        //             FileHelper.DeleteImageFile(model.IdImagePath); // Delete old
        //         }
        //         var PassportImage_Text = await FileHelper.CheckFileIsImage_3Mg_Async(model.PassportImage);
        //         if ((PassportImage_Text != "OK" && PassportImage_Text != "null")) {
        //             ModelState.AddModelError("PassportImage", PassportImage_Text);
        //             FileHelper.DeleteImageFile(model.PassportImagePath); // Delete old
        //         }
        //         #endregion Validate Files are Images and Size

        //         #region get this User
        //         var session = _httpContextAccessor?.HttpContext?.Session;
        //         var username = session?.GetString("Email");
        //         var user = await _unitOfWork.Members.GetByColumnAsync(u => u.Email == username);
        //         if (user == null || model.Id == null)
        //         {
        //             model.isHasIDCard = false;
        //             model.isHasPassport = false;
        //             model.isNotExpired = false;
        //             ModelState.AddModelError("IdImage", Resource1.NoData);
        //             return View(model);
        //         }
        //         #endregion get this User

        //         #region check Has images or Not
        //         if (!FileHelper.IsFileExist(user.IdImagePath) && model.IdImage==null || model.IdImage?.Length ==0)
        //         {
        //             model.isHasIDCard = false;
        //             ModelState.AddModelError("IdImage", Resource1.UploadIDCard);
        //             FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
        //         }
        //         if (!FileHelper.IsFileExist(user.PassportImagePath) && model.PassportImage == null || model.PassportImage?.Length == 0)
        //         {
        //             model.isHasPassport = false;
        //             ModelState.AddModelError("PassportImage", Resource1.UploadPassportImage);
        //             FileHelper.DeleteImageFile(user.PassportImagePath); // Delete old
        //         }
        //         #endregion check Has images or Not

        //         if (!ModelState.IsValid)
        //         {
        //             return View(model);
        //         }

        //         if (ModelState.IsValid)
        //         {
        //             #region check images

        //             if ((string.IsNullOrEmpty(user.IdImagePath) && (model.IdImage == null || model.IdImage?.Length == 0)) && (string.IsNullOrEmpty(user.PassportImagePath) && (model.PassportImage == null || model.PassportImage?.Length == 0)))
        //             {
        //                 ModelState.AddModelError("PassportImage", Resource1.UploadPassportImage);
        //                 ModelState.AddModelError("IdImage", Resource1.UploadIDCard);
        //                 FileHelper.DeleteImageFile(user.PassportImagePath); // Delete old
        //                 FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
        //                 return View(model);
        //             }
        //             if ((string.IsNullOrEmpty(user.IdImagePath) && (model.IdImage == null || model.IdImage?.Length == 0)))
        //             {
        //                 model.isHasIDCard = false;
        //                 ModelState.AddModelError("IdImage", Resource1.UploadIDCard);
        //                 FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
        //                 return View(model);
        //             }
        //             //else model.isHasIDCard = true;
        //             if ((string.IsNullOrEmpty(user.PassportImagePath) && (model.PassportImage == null || model.PassportImage?.Length == 0)))
        //             {
        //                 model.isHasPassport = false;
        //                 ModelState.AddModelError("PassportImage", Resource1.UploadPassportImage);
        //                 FileHelper.DeleteImageFile(user.PassportImagePath); // Delete old
        //                 return View(model);
        //             }
        //             //else model.isHasPassport = true;


        //             // Ensure folder exists
        //             Directory.CreateDirectory(_imageSavePath);

        //             // Save images if uploaded
        //             string? SaveImage(IFormFile file)
        //             {
        //                 if (file == null || file.Length == 0) return null;

        //                 string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
        //                 string filePath = Path.Combine(_imageSavePath, fileName);

        //                 using (var stream = new FileStream(filePath, FileMode.Create))
        //                 {
        //                     file.CopyTo(stream);
        //                 }

        //                 return $"/uploads/Members/{fileName}"; // Save relative path for web access
        //             }

        //             if (model.PassportImage != null && model.PassportImage?.Length > 0)
        //             {
        //                 model.isHasPassport = true;
        //                 FileHelper.DeleteImageFile(user.PassportImagePath);
        //                 user.PassportImagePath = SaveImage(model.PassportImage);
        //                 _unitOfWork.Members.Update(user);
        //                 await _unitOfWork.CompleteAsync();
        //             }
        //             #endregion check images

        //             if ((model.IdImage != null && model.IdImage?.Length > 0) || (!user.IdExpiryDate.HasValue || (user.IdExpiryDate.HasValue && user.IdExpiryDate < DateOnly.FromDateTime(AppDubaiTime.Now))))
        //             {
        //                 //#region  Validation If Extracted Data == Database Data

        //                 //if (model.IdImage == null || model.IdImage?.Length == 0)
        //                 //{
        //                 //    model.isHasIDCard = false;
        //                 //    ModelState.AddModelError("IdImage", Resource1.UploadIDCard);
        //                 //    FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
        //                 //    return View(model);
        //                 //}

        //                 //if ((model.IdImage?.Length == model.PassportImage?.Length) && model.IdImage != null && model.PassportImage != null)
        //                 //{
        //                 //    model.isHasPassport = false;
        //                 //    model.isHasIDCard = false;
        //                 //    ModelState.AddModelError("IdImage", Resource1.UploadIDCardDiffrent);
        //                 //    FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
        //                 //    return View(model);
        //                 //}

        //                 //var result = await IDClassification_Json(model.IdImage) as JsonResult;
        //                 //var iDCardExtractedDataVM = result.Value as IDCardExtractedDataVM;

        //                 //// You can use vm here
        //                 //if (iDCardExtractedDataVM?.doneOCR_bool == true)
        //                 //{
        //                 //    MemberDTO memberDTO = _mapper.Map<MemberDTO>(user);
        //                 //    IDCardExtractedDataDTO iDCardExtractedDataDTO = _mapper.Map<IDCardExtractedDataDTO>(iDCardExtractedDataVM);
        //                 //    var (Compaire_percentage_FullEnName, Compaire_percentage_FullArName, Compaire_percentage_IDNumber, Compaire_percentage_BirthDate, Compaire_percentage_ExpiryDate, VM_ExpiryDate_DT) = await _memberService.ValidationCompareAllInputsToExtractedAsync(memberDTO, iDCardExtractedDataDTO);

        //                 //    if (Compaire_percentage_FullEnName > 65 && Compaire_percentage_FullArName > 55 && Compaire_percentage_IDNumber > 99 && Compaire_percentage_BirthDate > 99)
        //                 //    {
        //                 //        iDCardExtractedDataVM.doneValidation_bool = true;
        //                 //        // update ExpiryDate of Member IDCard to day (01/++Month/year) of ExpiryDate
        //                 //        if (VM_ExpiryDate_DT != null && VM_ExpiryDate_DT.HasValue)
        //                 //        {
        //                 //            var NewExpiryDate = VM_ExpiryDate_DT?.Date.AddMonths(1);
        //                 //            user.IdExpiryDate = NewExpiryDate.HasValue
        //                 //                                ? DateOnly.FromDateTime(NewExpiryDate.Value)
        //                 //                                : (DateOnly?)null;
        //                 //            if (user.IdExpiryDate >= DateOnly.FromDateTime(AppDubaiTime.Now))
        //                 //            {
        //                 //                _unitOfWork.Members.Update(user);
        //                 //                await _unitOfWork.CompleteAsync();
        //                 //            }
        //                 //            else
        //                 //            {
        //                 //                model.isHasIDCard = false;
        //                 //                model.isNotExpired = false;
        //                 //                ModelState.AddModelError("IdImage", Resource1.ThisIDCardIsExpired ?? " ");
        //                 //                FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
        //                 //                return View(model);
        //                 //            }

        //                 //        }
        //                 //        else
        //                 //        {
        //                 //            model.isHasIDCard = false;
        //                 //            ModelState.AddModelError("IdImage", iDCardExtractedDataVM?.DoneTextExtracted_Error_Str ?? " ");
        //                 //            FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
        //                 //            return View(model);
        //                 //        }

        //                 //    }
        //                 //    else
        //                 //    {
        //                 //        //if (Compaire_percentage_FullEnName <= 65) ModelState.AddModelError("FullNameEn", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
        //                 //        //if (Compaire_percentage_FullArName <= 55) ModelState.AddModelError("FullNameAr", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
        //                 //        //if (Compaire_percentage_IDNumber <= 99) ModelState.AddModelError("IdNumber", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
        //                 //        //if (Compaire_percentage_BirthDate <= 99) ModelState.AddModelError("DateOfBirth", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
        //                 //        //if (Compaire_percentage_ExpiryDate <= 99) ModelState.AddModelError("IdExpiryDate", Resource1.ThisFieldDoesnotMatchIDCardImage ?? " ");
        //                 //        model.isHasIDCard = false;
        //                 //        ModelState.AddModelError("IdImage", Resource1.ThisIsnotaPreviouslyRegisteredIDCard ?? " ");
        //                 //        FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
        //                 //        return View(model);
        //                 //    }
        //                 //}
        //                 //else
        //                 //{
        //                 //    model.isHasIDCard = false;
        //                 //    ModelState.AddModelError("IdImage", iDCardExtractedDataVM?.DoneTextExtracted_Error_Str ?? Resource1.UploadIDCardThisIsNot ?? " ");
        //                 //    FileHelper.DeleteImageFile(user.IdImagePath); // Delete old
        //                 //    return View(model);
        //                 //}
        //                 //#endregion
        //             }
        //             if (model.IdImage != null && model.IdImage?.Length > 0)
        //             {
        //                 FileHelper.DeleteImageFile(user.IdImagePath);
        //                 user.IdImagePath = SaveImage(model.IdImage);
        //             }
        //             await _courseService.SubscribeAsync(model.Id, username);
        //             await _hubContext.Clients.Groups("CourseManagers")
        // .SendAsync("ReceiveNotification", new
        // {
        //     Title = "New Student Joined",
        //     Message = "Ahmed just joined the platform!"
        // });

        //             await _notificationService.SendNotificationToPermissionAsync(

        //    "تسجيل جديد في دورة",
        //    "قام طالب جديد بالتسجيل دي دورة",
        //    "Course.Index"
        //);
        //             return RedirectToAction(nameof(Index));
        //         }
        //         return RedirectToAction(nameof(Subscribe), new { model.Id });
        //     }


        [HttpPost]
        public async Task<IActionResult> DoRate(int ratingValue, int? id)
        {
            if (id == null) return NotFound();

            var course = await _unitOfWork.Courses.GetByIdAsync(id.Value);
            if (course == null) return NotFound();

            var username = _httpContextAccessor?.HttpContext?.Session?.GetString("Email");
            if (string.IsNullOrEmpty(username)) return Unauthorized();

            await _courseService.AddRateAsync(id.Value, username, ratingValue);
            return Ok(new { success = true });
        }
        [YesGet]
        public async Task<IActionResult> PrintCertificate(int subscriptionId)
        {
            var config = _serviceProvider.GetRequiredService<IConfiguration>();
            var CertificateURLWebsite = config["CertificateURL:BaseUrl"]; // must be set in appsettings.json or secrets

            var data = await _courseAdminService.GetCertificateData(subscriptionId);
            var model = _mapper.Map<Admin.ViewModels.Course.CertificateVM>(data);
            var certificateSerialHashed = HashHelper.Encrypt(model.CertificateSerial??"0");
            string encodedCertificateSerialHashed = Uri.EscapeDataString(certificateSerialHashed); // save + , % وهكذا 
            model.CertificateSerialHashed = encodedCertificateSerialHashed;
            var url = CertificateURLWebsite + "/Certificate/CertificateVerified?serialHashed=" + encodedCertificateSerialHashed;
            // Change to Remote URL
            var qrCode = QrCodeHelper.GenerateQrBase64(url);
            model.QrCodeBase64 = qrCode;
            return View(model);
        }


        #endregion
    }
}
