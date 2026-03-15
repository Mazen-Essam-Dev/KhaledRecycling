using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.Entities;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.ScientificProject;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Hub;
using FougeraClub.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    [Route("Admin/[controller]/[action]")]
    public class ScientificProjectsController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly IScientificProjectsService _service;
        private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMapper _mapper;

        [IgnoreAction]
        [NoLogging]
        public RoleNumber ValidateRoleNumber()
        {
            var roleNumber = _httpContextAccessor.HttpContext?.Session.GetInt32("RoleNumber");
            if (roleNumber != null)
            {
                return (RoleNumber)roleNumber;
            }
            return RoleNumber.NormalUser;
        }

        public ScientificProjectsController(IUnitOfWork unitOfWork,IMapper mapper, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, IScientificProjectsService service, UserManager<Infrastructure.Identity.ApplicationUser> userManager, IHubContext<NotificationHub> hubContext, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _service = service;
            _userManager = userManager;
            _hubContext = hubContext;
            _notificationService = notificationService;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
        }

        [YesGet]
        public async Task<IActionResult> Index(string searchTerm, DateOnly? dateFrom, DateOnly? dateTo, int page = 1, int pageSize = 50)
        {
            var query = _unitOfWork.ScientificProjects.Table;
            var query_Departments = _unitOfWork.Departments.Table;

            // Filter for trainers - only show projects assigned to them
            var userId = User.GetUserId();
            var trainer = await _unitOfWork.Trainers.Table.FirstOrDefaultAsync(t => t.UserId == userId);
            if (trainer != null)
            {
                query = query.Where(p => p.TrainerId == trainer.Id);
            }

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query_Departments = query_Departments.Where(x => x.NameEn.Contains(searchTerm) || x.NameAr.Contains(searchTerm));
                var Departments = await query_Departments.ToListAsync();
                // Extract matching IDs
                var departmentIds = Departments.Select(d => d.Id).ToList();
                query = query.Where(e => e.ProjectNameAr.Contains(searchTerm) || e.ProjectNameEn.Contains(searchTerm) || (departmentIds.Contains(e.DepartmentId ?? 0)));
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(e => e.DateByCalander >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(e => e.DateByCalander <= dateTo.Value);
            }

            var totalRecords = await query.CountAsync();
            var scientificProjects = await query.Include(x => x.ManagerSignature).Include(y => y.ActivityMonitorSigniture).Include(y => y.Trainer1Signiture)
                .OrderByDescending(e => e.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new ScientificProjectsVM
            {
                all_ScientificProjectListVM = scientificProjects,
                all_Departments = await _unitOfWork.Departments.GetAllAsync(),
                CurrentPage = page,
                TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize),
                SearchString = searchTerm,
                PageSize = pageSize,
                StartDate = dateFrom,
                EndDate = dateTo,
                TotalCount = totalRecords
            };

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", model);
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var maxProject = await _unitOfWork.ScientificProjects.GetMaxRecordAsync(x => x.Id, null);

            ScientificProjectsVM projectVM = new ScientificProjectsVM();

            var all_Departments = await _unitOfWork.Departments.GetAllAsync();
            if (all_Departments == null) return NotFound();

            if (maxProject != null)
            {
                projectVM.SerialCode = (Convert.ToInt64(maxProject?.SerialCode) + 1).ToString();
            }
            else
            {
                projectVM.SerialCode = "1000";
            }

            projectVM.GoalsListAr = new List<ScientificProjectGoals>
            {
                new ScientificProjectGoals { LangType = (int)LangEnum.Ar },
                new ScientificProjectGoals { LangType = (int)LangEnum.Ar },
                new ScientificProjectGoals { LangType = (int)LangEnum.Ar },
                new ScientificProjectGoals { LangType = (int)LangEnum.Ar },
            };
            projectVM.GoalsListEn = new List<ScientificProjectGoals>
            {
                new ScientificProjectGoals { LangType = (int)LangEnum.En },
                new ScientificProjectGoals { LangType =  (int)LangEnum.En },
                new ScientificProjectGoals { LangType =  (int)LangEnum.En },
                new ScientificProjectGoals { LangType =  (int)LangEnum.En },
            };

            projectVM.ToolsListAr = new List<ScientificProjectTools>
            {
                new ScientificProjectTools { LangType = (int)LangEnum.Ar },
                new ScientificProjectTools { LangType = (int)LangEnum.Ar },
                new ScientificProjectTools { LangType = (int)LangEnum.Ar },
                new ScientificProjectTools { LangType = (int)LangEnum.Ar },
                new ScientificProjectTools { LangType = (int)LangEnum.Ar },
            };
            projectVM.ToolsListEn = new List<ScientificProjectTools>
            {
                new ScientificProjectTools { LangType = (int)LangEnum.En },
                new ScientificProjectTools { LangType =  (int)LangEnum.En },
                new ScientificProjectTools { LangType =  (int)LangEnum.En },
                new ScientificProjectTools { LangType =  (int)LangEnum.En },
                new ScientificProjectTools { LangType =  (int)LangEnum.En },
            };

            projectVM.IndividualsListAr = new List<ScientificProjectIndividuals>
            {
                new ScientificProjectIndividuals { LangType = (int)LangEnum.Ar },
                new ScientificProjectIndividuals { LangType = (int)LangEnum.Ar },
                new ScientificProjectIndividuals { LangType = (int)LangEnum.Ar },
                new ScientificProjectIndividuals { LangType = (int)LangEnum.Ar },
            };
            projectVM.IndividualsListEn = new List<ScientificProjectIndividuals>
            {
                new ScientificProjectIndividuals { LangType = (int)LangEnum.En },
                new ScientificProjectIndividuals { LangType =  (int)LangEnum.En },
                new ScientificProjectIndividuals { LangType =  (int)LangEnum.En },
                new ScientificProjectIndividuals { LangType =  (int)LangEnum.En },
            };
            projectVM.all_Departments = all_Departments;
            projectVM.DepartmentsList = SelectListHelper.BindSelectList(all_Departments.ToList(), projectVM.DepartmentId).ToList();
            projectVM.CurrentDate = AppDubaiTime.Now.ToString("yyyy-MM-dd");

            return View(projectVM);
        }

        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ScientificProjectsVM model)
        {
            var FolderEntityWillSaveIn = "Scientific Projects";
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
            var filePreliminary1_Text = await ValidateImageAsync(model.TempFilePreliminary1_Path, model.FilePreliminary1, model.FilePreliminaryPath1, "FilePreliminary1");
            var filePreliminary2_Text = await ValidateImageAsync(model.TempFilePreliminary2_Path, model.FilePreliminary2, model.FilePreliminaryPath2, "FilePreliminary2");
            var filePreliminary3_Text = await ValidateImageAsync(model.TempFilePreliminary3_Path, model.FilePreliminary3, model.FilePreliminaryPath3, "FilePreliminary3");

            var file1_Text = await ValidateImageAsync(model.TempFile1_Path, model.File1, model.FilePath1, "File1");
            var file2_Text = await ValidateImageAsync(model.TempFile2_Path, model.File2, model.FilePath2, "File2");
            var file3_Text = await ValidateImageAsync(model.TempFile3_Path, model.File3, model.FilePath3, "File3");
            var file4_Text = await ValidateImageAsync(model.TempFile4_Path, model.File4, model.FilePath4, "File4");
            var file5_Text = await ValidateImageAsync(model.TempFile5_Path, model.File5, model.FilePath5, "File5");
            var file6_Text = await ValidateImageAsync(model.TempFile6_Path, model.File6, model.FilePath6, "File6");

            if (filePreliminary1_Text != "OK" && filePreliminary1_Text != "null") ModelState.AddModelError("FilePreliminary1", filePreliminary1_Text ?? " ");
            if (filePreliminary2_Text != "OK" && filePreliminary2_Text != "null") ModelState.AddModelError("FilePreliminary2", filePreliminary2_Text ?? " ");
            if (filePreliminary3_Text != "OK" && filePreliminary3_Text != "null") ModelState.AddModelError("FilePreliminary3", filePreliminary3_Text ?? " ");
            if (file1_Text != "OK" && file1_Text != "null") ModelState.AddModelError("File1", file1_Text ?? " ");
            if (file2_Text != "OK" && file2_Text != "null") ModelState.AddModelError("File2", file2_Text ?? " ");
            if (file3_Text != "OK" && file3_Text != "null") ModelState.AddModelError("File3", file3_Text ?? " ");
            if (file4_Text != "OK" && file4_Text != "null") ModelState.AddModelError("File4", file4_Text ?? " ");
            if (file5_Text != "OK" && file5_Text != "null") ModelState.AddModelError("File5", file5_Text ?? " ");
            if (file6_Text != "OK" && file6_Text != "null") ModelState.AddModelError("File6", file6_Text ?? " ");

            #endregion Validate Is File is PDF And MG // Validate PDF

            #region validation fails // !ModelState.IsValid
            // If validation fails
            if (!ModelState.IsValid)
            {
                #region Remove ModelState Image Errors To Rebind New Data Temp
                ModelState.Remove("TempFilePreliminary1_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("OldFilePreliminary1_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("FilePreliminary1"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFilePreliminary2_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("OldFilePreliminary2_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("FilePreliminary2"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFilePreliminary3_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("OldFilePreliminary3_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("FilePreliminary3"); // its Important To Bind New Data Temp

                ModelState.Remove("TempFile1_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old1_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File1"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile2_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old2_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File2"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile3_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old3_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File3"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile4_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old4_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File4"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile5_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old5_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File5"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile6_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old6_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File6"); // its Important To Bind New Data Temp
                #endregion Remove ModelState Image Errors To Rebind New Data Temp
                if (filePreliminary1_Text != "OK" && filePreliminary1_Text != "null") ModelState.AddModelError("FilePreliminary1", filePreliminary1_Text ?? " ");
                if (filePreliminary2_Text != "OK" && filePreliminary2_Text != "null") ModelState.AddModelError("FilePreliminary2", filePreliminary2_Text ?? " ");
                if (filePreliminary3_Text != "OK" && filePreliminary3_Text != "null") ModelState.AddModelError("FilePreliminary3", filePreliminary3_Text ?? " ");
                if (file1_Text != "OK" && file1_Text != "null") ModelState.AddModelError("File1", file1_Text ?? " ");
                if (file2_Text != "OK" && file2_Text != "null") ModelState.AddModelError("File2", file2_Text ?? " ");
                if (file3_Text != "OK" && file3_Text != "null") ModelState.AddModelError("File3", file3_Text ?? " ");
                if (file4_Text != "OK" && file4_Text != "null") ModelState.AddModelError("File4", file4_Text ?? " ");
                if (file5_Text != "OK" && file5_Text != "null") ModelState.AddModelError("File5", file5_Text ?? " ");
                if (file6_Text != "OK" && file6_Text != "null") ModelState.AddModelError("File6", file6_Text ?? " ");

                // If the user uploads a new file → cache it before returning
                if (model.FilePreliminary1 != null)
                    model.TempFilePreliminary1_Path = await FileHelper.SaveTempAsync(model.FilePreliminary1);
                if (model.FilePreliminary2 != null)
                    model.TempFilePreliminary2_Path = await FileHelper.SaveTempAsync(model.FilePreliminary2);
                if (model.FilePreliminary3 != null)
                    model.TempFilePreliminary3_Path = await FileHelper.SaveTempAsync(model.FilePreliminary3);

                if (model.File1 != null)
                    model.TempFile1_Path = await FileHelper.SaveTempAsync(model.File1);
                if (model.File2 != null)
                    model.TempFile2_Path = await FileHelper.SaveTempAsync(model.File2);
                if (model.File3 != null)
                    model.TempFile3_Path = await FileHelper.SaveTempAsync(model.File3);
                if (model.File4 != null)
                    model.TempFile4_Path = await FileHelper.SaveTempAsync(model.File4);
                if (model.File5 != null)
                    model.TempFile5_Path = await FileHelper.SaveTempAsync(model.File5);
                if (model.File6 != null)
                    model.TempFile6_Path = await FileHelper.SaveTempAsync(model.File6);

                // Re-fall and Re-populate enum select list
                model.all_Departments = await _unitOfWork.Departments.GetAllAsync();
                model.DepartmentsList = SelectListHelper.BindSelectList(model.all_Departments.ToList(), model.DepartmentId).ToList();

                TempData.Keep(); // for safety if re-rendered
                // Ensure lists are not null and contain required count
                model.GoalsListAr ??= new List<ScientificProjectGoals>();
                model.GoalsListEn ??= new List<ScientificProjectGoals>();
                model.ToolsListAr ??= new List<ScientificProjectTools>();
                model.ToolsListEn ??= new List<ScientificProjectTools>();
                model.IndividualsListAr ??= new List<ScientificProjectIndividuals>();
                model.IndividualsListEn ??= new List<ScientificProjectIndividuals>();

                while (model.GoalsListAr.Count < 4) model.GoalsListAr.Add(new ScientificProjectGoals());
                while (model.GoalsListEn.Count < 4) model.GoalsListEn.Add(new ScientificProjectGoals());

                while (model.ToolsListAr.Count < 5) model.ToolsListAr.Add(new ScientificProjectTools());
                while (model.ToolsListEn.Count < 5) model.ToolsListEn.Add(new ScientificProjectTools());

                while (model.IndividualsListAr.Count < 4) model.IndividualsListAr.Add(new ScientificProjectIndividuals());
                while (model.IndividualsListEn.Count < 4) model.IndividualsListEn.Add(new ScientificProjectIndividuals());

                return View(model);
            }
            #endregion validation fails // !ModelState.IsValid

            // ---------------------------
            //  PROCESS FINAL FILE
            // ---------------------------
            #region PROCESS FINAL FILE Handeling Save in newpath From Temp
            string finalFileName = null;

            // 1) If there is a new filePreliminary uploaded by the user
            if (model.FilePreliminary1 != null)
            {
                FileHelper.DeleteImageFile(model.OldFilePreliminary1_Path);
                model.FilePreliminaryPath1 = await FileHelper.SaveImageAsync(model.FilePreliminary1, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved filePreliminary
            else if (!string.IsNullOrEmpty(model.TempFilePreliminary1_Path))
            {
                FileHelper.DeleteImageFile(model.OldFilePreliminary1_Path);
                model.FilePreliminaryPath1 = FileHelper.MoveTempToFinal(
                    model.TempFilePreliminary1_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            //// 3) If neither this nor that → use the old filePreliminary (Edit only)
            //else
            //{
            //    model.FilePreliminaryPath1 = model.Old1_Path;
            //}


            // 1) If there is a new filePreliminary uploaded by the user
            if (model.FilePreliminary2 != null)
            {
                FileHelper.DeleteImageFile(model.OldFilePreliminary2_Path);
                model.FilePreliminaryPath2 = await FileHelper.SaveImageAsync(model.FilePreliminary2, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved filePreliminary
            else if (!string.IsNullOrEmpty(model.TempFilePreliminary2_Path))
            {
                FileHelper.DeleteImageFile(model.OldFilePreliminary2_Path);
                model.FilePreliminaryPath2 = FileHelper.MoveTempToFinal(
                    model.TempFilePreliminary2_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            //// 3) If neither this nor that → use the old filePreliminary (Edit only)
            //else
            //{
            //    model.FilePreliminaryPath2 = model.Old2_Path;
            //}

            // 1) If there is a new filePreliminary uploaded by the user
            if (model.FilePreliminary3 != null)
            {
                FileHelper.DeleteImageFile(model.OldFilePreliminary3_Path);
                model.FilePreliminaryPath3 = await FileHelper.SaveImageAsync(model.FilePreliminary3, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved filePreliminary
            else if (!string.IsNullOrEmpty(model.TempFilePreliminary3_Path))
            {
                FileHelper.DeleteImageFile(model.OldFilePreliminary3_Path);
                model.FilePreliminaryPath3 = FileHelper.MoveTempToFinal(
                    model.TempFilePreliminary3_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            //// 3) If neither this nor that → use the old filePreliminary (Edit only)
            //else
            //{
            //    model.FilePreliminaryPath3 = model.Old3_Path;
            //}

            // 1) If there is a new file uploaded by the user
            if (model.File1 != null)
            {
                FileHelper.DeleteImageFile(model.Old1_Path);
                model.FilePath1 = await FileHelper.SaveImageAsync(model.File1, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile1_Path))
            {
                FileHelper.DeleteImageFile(model.Old1_Path);
                model.FilePath1 = FileHelper.MoveTempToFinal(
                    model.TempFile1_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            //// 3) If neither this nor that → use the old file (Edit only)
            //else
            //{
            //    model.FilePath1 = model.Old1_Path;
            //}


            // 1) If there is a new file uploaded by the user
            if (model.File2 != null)
            {
                FileHelper.DeleteImageFile(model.Old2_Path);
                model.FilePath2 = await FileHelper.SaveImageAsync(model.File2, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile2_Path))
            {
                FileHelper.DeleteImageFile(model.Old2_Path);
                model.FilePath2 = FileHelper.MoveTempToFinal(
                    model.TempFile2_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            //// 3) If neither this nor that → use the old file (Edit only)
            //else
            //{
            //    model.FilePath2 = model.Old2_Path;
            //}

            // 1) If there is a new file uploaded by the user
            if (model.File3 != null)
            {
                FileHelper.DeleteImageFile(model.Old3_Path);
                model.FilePath3 = await FileHelper.SaveImageAsync(model.File3, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile3_Path))
            {
                FileHelper.DeleteImageFile(model.Old3_Path);
                model.FilePath3 = FileHelper.MoveTempToFinal(
                    model.TempFile3_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            //// 3) If neither this nor that → use the old file (Edit only)
            //else
            //{
            //    model.FilePath3 = model.Old3_Path;
            //}

            // 1) If there is a new file uploaded by the user
            if (model.File4 != null)
            {
                FileHelper.DeleteImageFile(model.Old4_Path);
                model.FilePath4 = await FileHelper.SaveImageAsync(model.File4, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile4_Path))
            {
                FileHelper.DeleteImageFile(model.Old4_Path);
                model.FilePath4 = FileHelper.MoveTempToFinal(
                    model.TempFile4_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            //// 3) If neither this nor that → use the old file (Edit only)
            //else
            //{
            //    model.FilePath4 = model.Old4_Path;
            //}

            // File5
            if (model.File5 != null)
            {
                model.FilePath5 = await FileHelper.SaveImageAsync(model.File5, FolderEntityWillSaveIn);
            }
            else if (!string.IsNullOrEmpty(model.TempFile5_Path))
            {
                model.FilePath5 = FileHelper.MoveTempToFinal(
                    model.TempFile5_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }

            // File6
            if (model.File6 != null)
            {
                model.FilePath6 = await FileHelper.SaveImageAsync(model.File6, FolderEntityWillSaveIn);
            }
            else if (!string.IsNullOrEmpty(model.TempFile6_Path))
            {
                model.FilePath6 = FileHelper.MoveTempToFinal(
                    model.TempFile6_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            #endregion PROCESS FINAL FILE Handeling Save in newpath From Temp
            // ---------------------------------------

            var project = _mapper.Map<ScientificProjects>(model);

            //var project = new ScientificProjects
            //{
            //    SerialCode = model.SerialCode,
            //    DateByCalander = model.DateByCalander,
            //    DepartmentId = model.DepartmentId,
            //    TrainerId = model.TrainerId,
            //    ProjectNameAr = model.ProjectNameAr,
            //    ProjectNameEn = model.ProjectNameEn,
            //    IdeaOwnerAr = model.IdeaOwnerAr,
            //    IdeaOwnerEn = model.IdeaOwnerEn,
            //    ProjectIdeaAr = model.ProjectIdeaAr,
            //    ProjectIdeaEN = model.ProjectIdeaEN,
            //    ProjectElement1 = model.ProjectElement1,
            //    ProjectElement2 = model.ProjectElement2,
            //    ProjectElement3 = model.ProjectElement3,
            //    ProjectElement4 = model.ProjectElement4,
            //    ProjectElement5 = model.ProjectElement5,
            //    ProjectElement6 = model.ProjectElement6,
            //    ProjectElement7 = model.ProjectElement7,
            //    ProjectElement8 = model.ProjectElement8,
            //    InstallationRecommendations = model.InstallationRecommendations,
            //    DeliveryData = model.DeliveryData,
            //    Participant1 = model.Participant1,
            //    Participant2 = model.Participant2,
            //    Participant3 = model.Participant3,
            //    Participant4 = model.Participant4,
            //    Supervisor1 = model.Supervisor1,
            //    Supervisor2 = model.Supervisor2,
            //    Supervisor3 = model.Supervisor3,
            //    Supervisor4 = model.Supervisor4,
            //    Supervisor5 = model.Supervisor5,
            //    Supervisor6 = model.Supervisor6,
            //    ExpectedCost = model.ExpectedCost,
            //    FilePath1 = model.FilePath1,
            //    FilePath2 = model.FilePath2,
            //    FilePath3 = model.FilePath3,
            //    FilePath4 = model.FilePath4,
            //    FilePath5 = model.FilePath5,
            //    FilePath6 = model.FilePath6
            //};

            var serialCodeExists = _unitOfWork.ScientificProjects.Table.Any(x => x.SerialCode == project.SerialCode);
            if (serialCodeExists)
            {
                project.SerialCode = (_unitOfWork.ScientificProjects.Table.Max(s => Convert.ToInt32(s.SerialCode)) + 1).ToString();
            }

            var newProject = await _unitOfWork.ScientificProjects.AddAsync(project);
            await _unitOfWork.CompleteAsync();

            if (model.ToolsListAr != null && model.ToolsListAr.Count > 0)
            {
                foreach (var item in model.ToolsListAr)
                {
                    if (string.IsNullOrWhiteSpace(item.ToolDescription)) continue;
                    
                    item.ScientificProjectId = newProject.Id;
                    item.LangType = (int)LangEnum.Ar;
                    await _unitOfWork.ScientificProjectTools.AddAsync(item);                    
                }
            }
            if (model.ToolsListEn != null && model.ToolsListEn.Count > 0)
            {
                foreach (var item in model.ToolsListEn)
                {
                    if (string.IsNullOrWhiteSpace(item.ToolDescription)) continue;

                    item.ScientificProjectId = newProject.Id;
                    item.LangType = (int)LangEnum.En;
                    await _unitOfWork.ScientificProjectTools.AddAsync(item);
                }
            }
            if (model.GoalsListAr != null && model.GoalsListAr.Count > 0)
            {
                foreach (var item in model.GoalsListAr)
                {
                    if (string.IsNullOrWhiteSpace(item.GoalDescription)) continue;

                    item.ScientificProjectId = newProject.Id;
                    item.LangType = (int)LangEnum.Ar;
                    await _unitOfWork.ScientificProjectGoals.AddAsync(item);
                }
            }
            if (model.GoalsListEn != null && model.GoalsListEn.Count > 0)
            {
                foreach (var item in model.GoalsListEn)
                {
                    if (string.IsNullOrWhiteSpace(item.GoalDescription)) continue;

                    item.ScientificProjectId = newProject.Id;
                    item.LangType = (int)LangEnum.En;
                    await _unitOfWork.ScientificProjectGoals.AddAsync(item);
                }
            }
            if (model.IndividualsListAr != null && model.IndividualsListAr.Count > 0)
            {
                foreach (var item in model.IndividualsListAr)
                {
                    if (string.IsNullOrWhiteSpace(item.IndividualName)) continue;

                    item.ScientificProjectId = newProject.Id;
                    item.LangType = (int)LangEnum.Ar;
                    await _unitOfWork.ScientificProjectIndividuals.AddAsync(item);
                }
            }
            if (model.IndividualsListEn != null && model.IndividualsListEn.Count > 0)
            {
                foreach (var item in model.IndividualsListEn)
                {
                    if (string.IsNullOrWhiteSpace(item.IndividualName)) continue;

                    item.ScientificProjectId = newProject.Id;
                    item.LangType = (int)LangEnum.En;
                    await _unitOfWork.ScientificProjectIndividuals.AddAsync(item);
                }
            }
            await _unitOfWork.CompleteAsync();

            var thisTrainer = await _unitOfWork.Trainers.GetByIdAsync(newProject.TrainerId??0);
            var thisTrainerUserId = thisTrainer?.UserId;
            //await _hubContext.Clients.Groups("Manager")
            //       .SendAsync("ReceiveNotification", new
            //       {
            //           Title = "",
            //           Message = ""
            //       });
            if(thisTrainerUserId != null && thisTrainerUserId.Length > 5)
            {
                await _notificationService.SendNotificationToUsersAsync(  // To Send Notification To Trainer
                  "مشروع علمي جديد",
                  $"يوجد مشروع علمي جديد رقم {newProject.SerialCode} جاهز للإعتماد",
                  new List<string> { thisTrainerUserId }
              );
            }

            return RedirectToAction(nameof(Index)); // After Add New
        }


        [HttpGet]
        [YesGet]
        public async Task<IActionResult> Details(int id)
        {
            if (id == 0)
                return NotFound();

            var project = await _unitOfWork.ScientificProjects.GetByIdAsync(x => x.Id == id, x => x.ActivityMonitorSigniture, y => y.ManagerSignature, z => z.Trainer, a => a.Trainer1Signiture);
            if (project == null)
                return NotFound();

            var model = _mapper.Map<ScientificProjectsVM>(project);
            model.all_Departments = await _unitOfWork.Departments.GetAllAsync();
            //var model = new ScientificProjectsVM
            //{
            //    Id = project.Id,
            //    SerialCode = project.SerialCode,
            //    DateByCalander = project.DateByCalander,
            //    DepartmentId = project.DepartmentId,
            //    TrainerId = project.TrainerId,
            //    ProjectNameAr = project.ProjectNameAr,
            //    ProjectNameEn = project.ProjectNameEn,
            //    IdeaOwnerAr = project.IdeaOwnerAr,
            //    IdeaOwnerEn = project.IdeaOwnerEn,
            //    ProjectIdeaAr = project.ProjectIdeaAr,
            //    ProjectIdeaEN = project.ProjectIdeaEN,
            //    ProjectElement1 = project.ProjectElement1,
            //    ProjectElement2 = project.ProjectElement2,
            //    ProjectElement3 = project.ProjectElement3,
            //    ProjectElement4 = project.ProjectElement4,
            //    ProjectElement5 = project.ProjectElement5,
            //    ProjectElement6 = project.ProjectElement6,
            //    ProjectElement7 = project.ProjectElement7,
            //    ProjectElement8 = project.ProjectElement8,
            //    InstallationRecommendations = project.InstallationRecommendations,
            //    DeliveryData = project.DeliveryData,
            //    Participant1 = project.Participant1,
            //    Participant2 = project.Participant2,
            //    Participant3 = project.Participant3,
            //    Participant4 = project.Participant4,
            //    Supervisor1 = project.Supervisor1,
            //    Supervisor2 = project.Supervisor2,
            //    Supervisor3 = project.Supervisor3,
            //    Supervisor4 = project.Supervisor4,
            //    Supervisor5 = project.Supervisor5,
            //    Supervisor6 = project.Supervisor6,
            //    ExpectedCost = project.ExpectedCost,
            //    FilePath1 = project.FilePath1,
            //    FilePath2 = project.FilePath2,
            //    FilePath3 = project.FilePath3,
            //    FilePath4 = project.FilePath4,
            //    FilePath5 = project.FilePath5,
            //    FilePath6 = project.FilePath6,


            //    ManagerSignature = project.ManagerSignature,
            //    ActivityMonitorSigniture = project.ActivityMonitorSigniture,
            //    ActivityMonitorSignitureId = project.ActivityMonitorSignitureId,
            //    ManagerSignitureId = project.ManagerSignitureId,

            //    all_Departments = await _unitOfWork.Departments.GetAllAsync()
            //};

            // Populate Departments select list (preselect current department)
            model.DepartmentsList = SelectListHelper.BindSelectList(model.all_Departments.ToList(), model.DepartmentId).ToList();

            // Build trainers list filtered by selected department and preselect current trainer
            var trainers = await (from t in _unitOfWork.Trainers.Table
                                  join u in _unitOfWork.Users.Table on t.UserId equals u.Id
                                  select new { Id = t.Id, DepartmentId = t.DepartmentId, FullNameAr = u.FullNameAr, FullNameEn = u.FullNameEn })
                                 .ToListAsync();

            var filteredTrainers = trainers.Where(t => t.DepartmentId == model.DepartmentId).ToList();
            model.TrainersList = SelectListHelper.BindSelectList(filteredTrainers, selected: model.TrainerId, valueProperty: "Id", nameAr: "FullNameAr", nameEn: "FullNameEn").ToList();

            model.DepartmentsList = SelectListHelper.BindSelectList(model.all_Departments.ToList(), model.DepartmentId).ToList();

            model.DepartmentsList = SelectListHelper.BindSelectList(model.all_Departments.ToList(), model.DepartmentId).ToList();

            // Set SupervisorFullName
            if (project.ActivityMonitorSigniture != null && !string.IsNullOrEmpty(project.ActivityMonitorSigniture.UserId))
            {
                var supervisor = await _userManager.FindByIdAsync(project.ActivityMonitorSigniture.UserId);
                model.SupervisorFullName = supervisor?.FullNameAr ?? supervisor?.FullNameEn ?? supervisor?.UserName ?? supervisor?.Email ?? "";
            }
            // Set TrainerFullName and Trainer signature
            if (project.Trainer != null && !string.IsNullOrEmpty(project.Trainer.UserId))
            {
                var trainerUser = await _userManager.FindByIdAsync(project.Trainer.UserId);
                model.TrainerFullName = trainerUser?.FullNameAr ?? trainerUser?.FullNameEn ?? trainerUser?.UserName ?? trainerUser?.Email ?? "";
            }
            // ensure Trainer1Signiture is assigned to model for view use
            model.Trainer1Signiture = project.Trainer1Signiture;
            // Set ManagerFullName
            if (project.ManagerSignature != null && !string.IsNullOrEmpty(project.ManagerSignature.UserId))
            {
                var manager = await _userManager.FindByIdAsync(project.ManagerSignature.UserId);
                model.ManagerFullName = manager?.FullNameAr ?? manager?.FullNameEn ?? manager?.UserName ?? manager?.Email ?? "";
            }

            return View(model);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintDetails(int id)
        {
            if (id == 0)
                return NotFound();

            var project = await _unitOfWork.ScientificProjects.GetByIdAsync(x => x.Id == id, x => x.ActivityMonitorSigniture, y => y.ManagerSignature, z => z.Trainer, a => a.Trainer1Signiture);
            if (project == null)
                return NotFound();

            var model = _mapper.Map<ScientificProjectsVM>(project);
            model.all_Departments = await _unitOfWork.Departments.GetAllAsync();
            //var model = new ScientificProjectsVM
            //{
            //    Id = project.Id,
            //    SerialCode = project.SerialCode,
            //    DateByCalander = project.DateByCalander,
            //    DepartmentId = project.DepartmentId,
            //    ProjectNameAr = project.ProjectNameAr,
            //    ProjectNameEn = project.ProjectNameEn,
            //    IdeaOwnerAr = project.IdeaOwnerAr,
            //    IdeaOwnerEn = project.IdeaOwnerEn,
            //    ProjectIdeaAr = project.ProjectIdeaAr,
            //    ProjectIdeaEN = project.ProjectIdeaEN,
            //    ProjectElement1 = project.ProjectElement1,
            //    ProjectElement2 = project.ProjectElement2,
            //    ProjectElement3 = project.ProjectElement3,
            //    ProjectElement4 = project.ProjectElement4,
            //    ProjectElement5 = project.ProjectElement5,
            //    ProjectElement6 = project.ProjectElement6,
            //    ProjectElement7 = project.ProjectElement7,
            //    ProjectElement8 = project.ProjectElement8,
            //    InstallationRecommendations = project.InstallationRecommendations,
            //    DeliveryData = project.DeliveryData,
            //    Participant1 = project.Participant1,
            //    Participant2 = project.Participant2,
            //    Participant3 = project.Participant3,
            //    Participant4 = project.Participant4,
            //    Supervisor1 = project.Supervisor1,
            //    Supervisor2 = project.Supervisor2,
            //    Supervisor3 = project.Supervisor3,
            //    Supervisor4 = project.Supervisor4,
            //    Supervisor5 = project.Supervisor5,
            //    Supervisor6 = project.Supervisor6,
            //    ExpectedCost = project.ExpectedCost,
            //    FilePath1 = project.FilePath1,
            //    FilePath2 = project.FilePath2,
            //    FilePath3 = project.FilePath3,
            //    FilePath4 = project.FilePath4,

            //    ManagerSignature = project.ManagerSignature,
            //    ActivityMonitorSigniture = project.ActivityMonitorSigniture,
            //    ActivityMonitorSignitureId = project.ActivityMonitorSignitureId,
            //    ManagerSignitureId = project.ManagerSignitureId,

            //    all_Departments = await _unitOfWork.Departments.GetAllAsync()
            //};

            model.DepartmentsList = SelectListHelper.BindSelectList(model.all_Departments.ToList(), model.DepartmentId).ToList();

            // set trainer name and signature
            if (project.Trainer != null && !string.IsNullOrEmpty(project.Trainer.UserId))
            {
                var trainerUser = await _userManager.FindByIdAsync(project.Trainer.UserId);
                model.TrainerFullName = trainerUser?.FullNameAr ?? trainerUser?.FullNameEn ?? trainerUser?.UserName ?? trainerUser?.Email ?? "";
            }
            model.Trainer1Signiture = project.Trainer1Signiture;

            return View(model);
        }
        [YesGet]
        public async Task<IActionResult> ProjectCard(int id)
        {
            if (id == 0)
                return NotFound();

            var project = await _unitOfWork.ScientificProjects.GetByIdAsync(x => x.Id == id, x => x.ActivityMonitorSigniture, y => y.ManagerSignature, z => z.Trainer, a => a.Trainer1Signiture, a1 => a1.ScientificProjectGoalsNavigation, a2 => a2.ScientificProjectToolsNavigation, a3 => a3.ScientificProjectIndividualsNavigation);
            if (project == null)
                return NotFound();

            var model = _mapper.Map<ScientificProjectsVM>(project);
            model.all_Departments = await _unitOfWork.Departments.GetAllAsync();

            model.GoalsListAr = project.ScientificProjectGoalsNavigation?.Where(x => x.LangType == (int)LangEnum.Ar).Take(4).ToList();
            model.GoalsListEn = project.ScientificProjectGoalsNavigation?.Where(x => x.LangType == (int)LangEnum.En).Take(4).ToList();

            model.ToolsListAr = project.ScientificProjectToolsNavigation?.Where(x => x.LangType == (int)LangEnum.Ar).Take(5).ToList();
            model.ToolsListEn = project.ScientificProjectToolsNavigation?.Where(x => x.LangType == (int)LangEnum.En).Take(5).ToList();

            model.IndividualsListAr = project.ScientificProjectIndividualsNavigation?.Where(x => x.LangType == (int)LangEnum.Ar).Take(4).ToList();
            model.IndividualsListEn = project.ScientificProjectIndividualsNavigation?.Where(x => x.LangType == (int)LangEnum.En).Take(4).ToList();

            model.MaxRowsGoals = Math.Max(
                                    model.GoalsListAr?.Count ?? 0,
                                    model.GoalsListEn?.Count ?? 0
                                 );

            model.MaxRowsTools = Math.Max(
                                    model.ToolsListAr?.Count ?? 0,
                                    model.ToolsListEn?.Count ?? 0
                                 );

            model.MaxRowsIndividuals = Math.Max(
                        model.IndividualsListAr?.Count ?? 0,
                        model.IndividualsListEn?.Count ?? 0
                     );

            //var model = new ScientificProjectsVM
            //{
            //    Id = project.Id,
            //    SerialCode = project.SerialCode,
            //    DateByCalander = project.DateByCalander,
            //    DepartmentId = project.DepartmentId,
            //    TrainerId = project.TrainerId,
            //    ProjectNameAr = project.ProjectNameAr,
            //    ProjectNameEn = project.ProjectNameEn,
            //    IdeaOwnerAr = project.IdeaOwnerAr,
            //    IdeaOwnerEn = project.IdeaOwnerEn,
            //    ProjectIdeaAr = project.ProjectIdeaAr,
            //    ProjectIdeaEN = project.ProjectIdeaEN,
            //    ProjectElement1 = project.ProjectElement1,
            //    ProjectElement2 = project.ProjectElement2,
            //    ProjectElement3 = project.ProjectElement3,
            //    ProjectElement4 = project.ProjectElement4,
            //    ProjectElement5 = project.ProjectElement5,
            //    ProjectElement6 = project.ProjectElement6,
            //    ProjectElement7 = project.ProjectElement7,
            //    ProjectElement8 = project.ProjectElement8,
            //    InstallationRecommendations = project.InstallationRecommendations,
            //    DeliveryData = project.DeliveryData,
            //    Participant1 = project.Participant1,
            //    Participant2 = project.Participant2,
            //    Participant3 = project.Participant3,
            //    Participant4 = project.Participant4,
            //    Supervisor1 = project.Supervisor1,
            //    Supervisor2 = project.Supervisor2,
            //    Supervisor3 = project.Supervisor3,
            //    Supervisor4 = project.Supervisor4,
            //    Supervisor5 = project.Supervisor5,
            //    Supervisor6 = project.Supervisor6,
            //    ExpectedCost = project.ExpectedCost,
            //    FilePath1 = project.FilePath1,
            //    FilePath2 = project.FilePath2,
            //    FilePath3 = project.FilePath3,
            //    FilePath4 = project.FilePath4,
            //    FilePath5 = project.FilePath5,
            //    FilePath6 = project.FilePath6,

            //    ManagerSignature = project.ManagerSignature,
            //    ActivityMonitorSigniture = project.ActivityMonitorSigniture,
            //    ActivityMonitorSignitureId = project.ActivityMonitorSignitureId,
            //    ManagerSignitureId = project.ManagerSignitureId,

            //    all_Departments = await _unitOfWork.Departments.GetAllAsync()
            //};

            model.DepartmentsList = SelectListHelper.BindSelectList(model.all_Departments.ToList(), model.DepartmentId).ToList();

            if (project.Trainer != null && !string.IsNullOrEmpty(project.Trainer.UserId))
            {
                var trainerUser = await _userManager.FindByIdAsync(project.Trainer.UserId);
                model.TrainerFullName = trainerUser?.FullNameAr ?? trainerUser?.FullNameEn ?? trainerUser?.UserName ?? trainerUser?.Email ?? "";
            }
            model.Trainer1Signiture = project.Trainer1Signiture;

            return View(model);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintProjectCard(int id)
        {
            if (id == 0)
                return NotFound();

            var project = await _unitOfWork.ScientificProjects.GetByIdAsync(x => x.Id == id, x => x.ActivityMonitorSigniture, y => y.ManagerSignature);
            if (project == null)
                return NotFound();

            var model = _mapper.Map<ScientificProjectsVM>(project);
            model.all_Departments = await _unitOfWork.Departments.GetAllAsync();
            
            //var model = new ScientificProjectsVM
            //{
            //    Id = project.Id,
            //    SerialCode = project.SerialCode,
            //    DateByCalander = project.DateByCalander,
            //    DepartmentId = project.DepartmentId,
            //    TrainerId = project.TrainerId,
            //    ProjectNameAr = project.ProjectNameAr,
            //    ProjectNameEn = project.ProjectNameEn,
            //    IdeaOwnerAr = project.IdeaOwnerAr,
            //    IdeaOwnerEn = project.IdeaOwnerEn,
            //    ProjectIdeaAr = project.ProjectIdeaAr,
            //    ProjectIdeaEN = project.ProjectIdeaEN,
            //    ProjectElement1 = project.ProjectElement1,
            //    ProjectElement2 = project.ProjectElement2,
            //    ProjectElement3 = project.ProjectElement3,
            //    ProjectElement4 = project.ProjectElement4,
            //    ProjectElement5 = project.ProjectElement5,
            //    ProjectElement6 = project.ProjectElement6,
            //    ProjectElement7 = project.ProjectElement7,
            //    ProjectElement8 = project.ProjectElement8,
            //    InstallationRecommendations = project.InstallationRecommendations,
            //    DeliveryData = project.DeliveryData,
            //    Participant1 = project.Participant1,
            //    Participant2 = project.Participant2,
            //    Participant3 = project.Participant3,
            //    Participant4 = project.Participant4,
            //    Supervisor1 = project.Supervisor1,
            //    Supervisor2 = project.Supervisor2,
            //    Supervisor3 = project.Supervisor3,
            //    Supervisor4 = project.Supervisor4,
            //    Supervisor5 = project.Supervisor5,
            //    Supervisor6 = project.Supervisor6,
            //    ExpectedCost = project.ExpectedCost,
            //    FilePath1 = project.FilePath1,
            //    FilePath2 = project.FilePath2,
            //    FilePath3 = project.FilePath3,
            //    FilePath4 = project.FilePath4,
            //    FilePath5 = project.FilePath5,
            //    FilePath6 = project.FilePath6,

            //    ManagerSignature = project.ManagerSignature,
            //    ActivityMonitorSigniture = project.ActivityMonitorSigniture,
            //    ActivityMonitorSignitureId = project.ActivityMonitorSignitureId,
            //    ManagerSignitureId = project.ManagerSignitureId,

            //    all_Departments = await _unitOfWork.Departments.GetAllAsync()
            //};

            return View(model);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var project = await _unitOfWork.ScientificProjects.GetByIdAsync(id);
            if (project == null)
                return NotFound();

            var model = _mapper.Map<ScientificProjectsVM>(project);
            model.all_Departments = await _unitOfWork.Departments.GetAllAsync();
            //var model = new ScientificProjectsVM
            //{
            //    Id = project.Id,
            //    SerialCode = project.SerialCode,
            //    DateByCalander = project.DateByCalander,
            //    DepartmentId = project.DepartmentId,
            //    TrainerId = project.TrainerId,
            //    ProjectNameAr = project.ProjectNameAr,
            //    ProjectNameEn = project.ProjectNameEn,
            //    IdeaOwnerAr = project.IdeaOwnerAr,
            //    IdeaOwnerEn = project.IdeaOwnerEn,
            //    ProjectIdeaAr = project.ProjectIdeaAr,
            //    ProjectIdeaEN = project.ProjectIdeaEN,
            //    ProjectElement1 = project.ProjectElement1,
            //    ProjectElement2 = project.ProjectElement2,
            //    ProjectElement3 = project.ProjectElement3,
            //    ProjectElement4 = project.ProjectElement4,
            //    ProjectElement5 = project.ProjectElement5,
            //    ProjectElement6 = project.ProjectElement6,
            //    ProjectElement7 = project.ProjectElement7,
            //    ProjectElement8 = project.ProjectElement8,
            //    InstallationRecommendations = project.InstallationRecommendations,
            //    DeliveryData = project.DeliveryData,
            //    Participant1 = project.Participant1,
            //    Participant2 = project.Participant2,
            //    Participant3 = project.Participant3,
            //    Participant4 = project.Participant4,
            //    Supervisor1 = project.Supervisor1,
            //    Supervisor2 = project.Supervisor2,
            //    Supervisor3 = project.Supervisor3,
            //    Supervisor4 = project.Supervisor4,
            //    Supervisor5 = project.Supervisor5,
            //    Supervisor6 = project.Supervisor6,
            //    ExpectedCost = project.ExpectedCost,
            //    FilePath1 = project.FilePath1,
            //    FilePath2 = project.FilePath2,
            //    FilePath3 = project.FilePath3,
            //    FilePath4 = project.FilePath4,
            //    FilePath5 = project.FilePath5,
            //    FilePath6 = project.FilePath6,
            //    Old1_Path = project.FilePath1,
            //    Old2_Path = project.FilePath2,
            //    Old3_Path = project.FilePath3,
            //    Old4_Path = project.FilePath4,
            //    Old5_Path = project.FilePath5,
            //    Old6_Path = project.FilePath6,
            //    all_Departments = await _unitOfWork.Departments.GetAllAsync()
            //};
            model.DepartmentsList = SelectListHelper.BindSelectList(model.all_Departments.ToList(), model.DepartmentId).ToList();

            // Build trainers list filtered by selected department and preselect current trainer
            var trainers = await (from t in _unitOfWork.Trainers.Table
                                  join u in _unitOfWork.Users.Table on t.UserId equals u.Id
                                  select new { Id = t.Id, DepartmentId = t.DepartmentId, FullNameAr = u.FullNameAr, FullNameEn = u.FullNameEn })
                                 .ToListAsync();

            var filteredTrainers = trainers.Where(t => t.DepartmentId == model.DepartmentId).ToList();
            model.TrainersList = SelectListHelper.BindSelectList(filteredTrainers, selected: model.TrainerId, valueProperty: "Id", nameAr: "FullNameAr", nameEn: "FullNameEn").ToList();

            return View(model);
        }

        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ScientificProjectsVM model, IFormFile File1, IFormFile File2, IFormFile File3, IFormFile File4, IFormFile File5, IFormFile File6)
        {
            //var thisProject = await _unitOfWork.ScientificProjects.GetByIdAsync((model.Id) ?? 0);
            //if (thisProject != null)
            //{
            //    model.FilePath1 = thisProject.FilePath1;
            //    model.FilePath2 = thisProject.FilePath2;
            //    model.FilePath3 = thisProject.FilePath3;
            //    model.FilePath4 = thisProject.FilePath4;

            //}
            model.File1 = File1;
            model.File2 = File2;
            model.File3 = File3;
            model.File4 = File4;
            model.File5 = File5;
            model.File6 = File6;
            var FolderEntityWillSaveIn = "Scientific Projects";
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

            var file1_Text = await ValidateImageAsync(model.TempFile1_Path, model.File1, model.FilePath1, "File1");
            var file2_Text = await ValidateImageAsync(model.TempFile2_Path, model.File2, model.FilePath2, "File2");
            var file3_Text = await ValidateImageAsync(model.TempFile3_Path, model.File3, model.FilePath3, "File3");
            var file4_Text = await ValidateImageAsync(model.TempFile4_Path, model.File4, model.FilePath4, "File4");
            var file5_Text = await ValidateImageAsync(model.TempFile5_Path, model.File5, model.FilePath5, "File5");
            var file6_Text = await ValidateImageAsync(model.TempFile6_Path, model.File6, model.FilePath6, "File6");
            ModelState.Remove("File1"); // its Important To Bind New Data Temp
            ModelState.Remove("File2"); // its Important To Bind New Data Temp
            ModelState.Remove("File3"); // its Important To Bind New Data Temp
            ModelState.Remove("File4"); // its Important To Bind New Data Temp
            ModelState.Remove("File5"); // its Important To Bind New Data Temp
            ModelState.Remove("File6"); // its Important To Bind New Data Temp

            if (file1_Text != "OK" && file1_Text != "null") { ModelState.AddModelError("File1", file1_Text ?? " "); }
            if (file2_Text != "OK" && file2_Text != "null") { ModelState.AddModelError("File2", file2_Text ?? " "); }
            if (file3_Text != "OK" && file3_Text != "null") { ModelState.AddModelError("File3", file3_Text ?? " "); }
            if (file4_Text != "OK" && file4_Text != "null") { ModelState.AddModelError("File4", file4_Text ?? " "); }
            if (file5_Text != "OK" && file5_Text != "null") { ModelState.AddModelError("File5", file5_Text ?? " "); }
            if (file6_Text != "OK" && file6_Text != "null") { ModelState.AddModelError("File6", file6_Text ?? " "); }

            #endregion Validate Is File is PDF And MG // Validate PDF

            #region validation fails // !ModelState.IsValid
            // If validation fails
            if (!ModelState.IsValid)
            {
                #region Remove ModelState Image Errors To Rebind New Data Temp
                ModelState.Remove("TempFile1_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old1_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File1"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile2_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old2_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File2"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile3_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old3_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File3"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile4_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old4_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File4"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile5_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old5_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File5"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile6_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old6_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File6"); // its Important To Bind New Data Temp
                #endregion Remove ModelState Image Errors To Rebind New Data Temp
                if (file1_Text != "OK" && file1_Text != "null") { ModelState.AddModelError("File1", file1_Text ?? " "); }
                if (file2_Text != "OK" && file2_Text != "null") { ModelState.AddModelError("File2", file2_Text ?? " "); }
                if (file3_Text != "OK" && file3_Text != "null") { ModelState.AddModelError("File3", file3_Text ?? " "); }
                if (file4_Text != "OK" && file4_Text != "null") { ModelState.AddModelError("File4", file4_Text ?? " "); }
                if (file5_Text != "OK" && file5_Text != "null") { ModelState.AddModelError("File5", file5_Text ?? " "); }
                if (file6_Text != "OK" && file6_Text != "null") { ModelState.AddModelError("File6", file6_Text ?? " "); }

                // If the user uploads a new file → cache it before returning
                if (model.File1 != null)
                    model.TempFile1_Path = await FileHelper.SaveTempAsync(model.File1);
                if (model.File2 != null)
                    model.TempFile2_Path = await FileHelper.SaveTempAsync(model.File2);
                if (model.File3 != null)
                    model.TempFile3_Path = await FileHelper.SaveTempAsync(model.File3);
                if (model.File4 != null)
                    model.TempFile4_Path = await FileHelper.SaveTempAsync(model.File4);
                if (model.File5 != null)
                    model.TempFile5_Path = await FileHelper.SaveTempAsync(model.File5);
                if (model.File6 != null)
                    model.TempFile6_Path = await FileHelper.SaveTempAsync(model.File6);

                // Re-fall and Re-populate enum select list
                model.all_Departments = await _unitOfWork.Departments.GetAllAsync();


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
            if (model.File1 != null)
            {
                FileHelper.DeleteImageFile(model.Old1_Path);
                model.FilePath1 = await FileHelper.SaveImageAsync(model.File1, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile1_Path))
            {
                FileHelper.DeleteImageFile(model.Old1_Path);
                model.FilePath1 = FileHelper.MoveTempToFinal(
                    model.TempFile1_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.FilePath1 = model.Old1_Path;
            }


            // 1) If there is a new file uploaded by the user
            if (model.File2 != null)
            {
                FileHelper.DeleteImageFile(model.Old2_Path);
                model.FilePath2 = await FileHelper.SaveImageAsync(model.File2, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile2_Path))
            {
                FileHelper.DeleteImageFile(model.Old2_Path);
                model.FilePath2 = FileHelper.MoveTempToFinal(
                    model.TempFile2_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.FilePath2 = model.Old2_Path;
            }

            // 1) If there is a new file uploaded by the user
            if (model.File3 != null)
            {
                FileHelper.DeleteImageFile(model.Old3_Path);
                model.FilePath3 = await FileHelper.SaveImageAsync(model.File3, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile3_Path))
            {
                FileHelper.DeleteImageFile(model.Old3_Path);
                model.FilePath3 = FileHelper.MoveTempToFinal(
                    model.TempFile3_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.FilePath3 = model.Old3_Path;
            }

            // 1) If there is a new file uploaded by the user
            if (model.File4 != null)
            {
                FileHelper.DeleteImageFile(model.Old4_Path);
                model.FilePath4 = await FileHelper.SaveImageAsync(model.File4, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile4_Path))
            {
                FileHelper.DeleteImageFile(model.Old4_Path);
                model.FilePath4 = FileHelper.MoveTempToFinal(
                    model.TempFile4_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            // 3) If neither this nor that → use the old file (Edit only)
            else
            {
                model.FilePath4 = model.Old4_Path;
            }

            // File5
            if (model.File5 != null)
            {
                FileHelper.DeleteImageFile(model.Old5_Path);
                model.FilePath5 = await FileHelper.SaveImageAsync(model.File5, FolderEntityWillSaveIn);
            }
            else if (!string.IsNullOrEmpty(model.TempFile5_Path))
            {
                FileHelper.DeleteImageFile(model.Old5_Path);
                model.FilePath5 = FileHelper.MoveTempToFinal(
                    model.TempFile5_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            else
            {
                model.FilePath5 = model.Old5_Path;
            }

            // File6
            if (model.File6 != null)
            {
                FileHelper.DeleteImageFile(model.Old6_Path);
                model.FilePath6 = await FileHelper.SaveImageAsync(model.File6, FolderEntityWillSaveIn);
            }
            else if (!string.IsNullOrEmpty(model.TempFile6_Path))
            {
                FileHelper.DeleteImageFile(model.Old6_Path);
                model.FilePath6 = FileHelper.MoveTempToFinal(
                    model.TempFile6_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            else
            {
                model.FilePath6 = model.Old6_Path;
            }
            #endregion PROCESS FINAL FILE Handeling Save in newpath From Temp
            // ---------------------------------------


            var project = await _unitOfWork.ScientificProjects.GetByIdAsync(model.Id ?? 0);
            if (project == null)
                return NotFound();

            // اعمل mapping على نفس الـ object
            _mapper.Map(model, project);

            //// Update fields
            //project.DateByCalander = model.DateByCalander;
            //project.DepartmentId = model.DepartmentId;
            //project.TrainerId = model.TrainerId;
            //project.ProjectNameAr = model.ProjectNameAr;
            //project.ProjectNameEn = model.ProjectNameEn;
            //project.IdeaOwnerAr = model.IdeaOwnerAr;
            //project.IdeaOwnerEn = model.IdeaOwnerEn;
            //project.ProjectIdeaAr = model.ProjectIdeaAr;
            //project.ProjectIdeaEN = model.ProjectIdeaEN;
            //project.ProjectElement1 = model.ProjectElement1;
            //project.ProjectElement2 = model.ProjectElement2;
            //project.ProjectElement3 = model.ProjectElement3;
            //project.ProjectElement4 = model.ProjectElement4;
            //project.ProjectElement5 = model.ProjectElement5;
            //project.ProjectElement6 = model.ProjectElement6;
            //project.ProjectElement7 = model.ProjectElement7;
            //project.ProjectElement8 = model.ProjectElement8;
            //project.InstallationRecommendations = model.InstallationRecommendations;
            //project.DeliveryData = model.DeliveryData;
            //project.Participant1 = model.Participant1;
            //project.Participant2 = model.Participant2;
            //project.Participant3 = model.Participant3;
            //project.Participant4 = model.Participant4;
            //project.Supervisor1 = model.Supervisor1;
            //project.Supervisor2 = model.Supervisor2;
            //project.Supervisor3 = model.Supervisor3;
            //project.Supervisor4 = model.Supervisor4;
            //project.Supervisor5 = model.Supervisor5;
            //project.Supervisor6 = model.Supervisor6;
            //project.ExpectedCost = model.ExpectedCost;
            //project.FilePath1 = model.FilePath1;
            //project.FilePath2 = model.FilePath2;
            //project.FilePath3 = model.FilePath3;
            //project.FilePath4 = model.FilePath4;
            //project.FilePath5 = model.FilePath5;
            //project.FilePath6 = model.FilePath6;

            if (ValidateRoleNumber() == RoleNumber.ActivitiesSupervisor) // Activities Supervisor who Do Edit
            {
                project.Trainer1Signiture = null;
                project.Trainer1SignitureId = null;
                project.ActivityMonitorSigniture = null;
                project.ActivityMonitorSignitureId = null;

                var thisTrainer = await _unitOfWork.Trainers.GetByIdAsync(project.TrainerId ?? 0);
                var thisTrainerUserId = thisTrainer?.UserId;

                //await _hubContext.Clients.Groups("Manager")
                //       .SendAsync("ReceiveNotification", new
                //       {
                //           Title = "",
                //           Message = ""
                //       });
                if (thisTrainerUserId != null && thisTrainerUserId.Length > 5)
                {
                    await _notificationService.SendNotificationToUsersAsync(  // To Send Notification To Trainer
                      "مشروع علمي تم تحديثه",
                      $"يوجد مشروع علمي تم تحديثه رقم {project.SerialCode} جاهز للإعتماد",
                      new List<string> { thisTrainerUserId }
                  );
                }
            }
            else if (ValidateRoleNumber() == RoleNumber.Manager || ValidateRoleNumber() == RoleNumber.NormalUser) // Manager Or AnyOne who Do Edit
            {
                project.Trainer1Signiture = null;
                project.Trainer1SignitureId = null;
                project.ActivityMonitorSigniture = null;
                project.ActivityMonitorSignitureId = null;
                project.ManagerSignature = null;
                project.ManagerSignitureId = null;

                var thisTrainer = await _unitOfWork.Trainers.GetByIdAsync(project.TrainerId ?? 0);
                var thisTrainerUserId = thisTrainer?.UserId;

                //await _hubContext.Clients.Groups("Manager")
                //       .SendAsync("ReceiveNotification", new
                //       {
                //           Title = "",
                //           Message = ""
                //       });
                if (thisTrainerUserId != null && thisTrainerUserId.Length > 5)
                {
                    await _notificationService.SendNotificationToUsersAsync(  // To Send Notification To Trainer
                      "مشروع علمي تم تحديثه",
                      $"يوجد مشروع علمي تم تحديثه رقم {project.SerialCode} جاهز للإعتماد",
                      new List<string> { thisTrainerUserId }
                  );
                }
            }

            _unitOfWork.ScientificProjects.Update(project);
            await _unitOfWork.CompleteAsync();



            return RedirectToAction(nameof(Edit), new { id = model.Id }); // After Edit

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id == null)
                return NotFound();

            var scientificProject = await _unitOfWork.ScientificProjects.GetByIdAsync(id);
            if (scientificProject == null)
                return NotFound();

            _unitOfWork.ScientificProjects.Delete(scientificProject);
            await _unitOfWork.CompleteAsync();

            return RedirectToAction(nameof(Index));
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> Print(string searchTerm, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var query = _unitOfWork.ScientificProjects.Table;
            var query_Departments = _unitOfWork.Departments.Table;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query_Departments = query_Departments.Where(x => x.NameEn.Contains(searchTerm) || x.NameAr.Contains(searchTerm));
                var Departments = await query_Departments.ToListAsync();
                // Extract matching IDs
                var departmentIds = Departments.Select(d => d.Id).ToList();
                query = query.Where(e => e.ProjectNameAr.Contains(searchTerm) || e.ProjectNameEn.Contains(searchTerm) || (departmentIds.Contains(e.DepartmentId ?? 0)));
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(e => e.DateByCalander >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(e => e.DateByCalander <= dateTo.Value);
            }
            query = query.OrderByDescending(x => x.Id);


            var model = new ScientificProjectsVM
            {
                all_ScientificProjectListVM = query,
                all_Departments = await _unitOfWork.Departments.GetAllAsync(),
            };
            return View(model);
        }

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download(string searchTerm, DateOnly? dateFrom, DateOnly? dateTo)
        {
            var query = _unitOfWork.ScientificProjects.Table;
            var query_Departments = _unitOfWork.Departments.Table;

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query_Departments = query_Departments.Where(x => x.NameEn != null && x.NameEn.Contains(searchTerm) || x.NameAr != null && x.NameAr.Contains(searchTerm));
                var Departments = await query_Departments.ToListAsync();
                // Extract matching IDs
                var departmentIds = Departments.Select(d => d.Id).ToList();
                query = query.Where(e => e.ProjectNameAr != null && e.ProjectNameAr.Contains(searchTerm) || e.ProjectNameEn != null && e.ProjectNameEn.Contains(searchTerm) || (departmentIds.Contains(e.DepartmentId ?? 0)));
            }

            if (dateFrom.HasValue)
            {
                query = query.Where(e => e.DateByCalander >= dateFrom.Value);
            }

            if (dateTo.HasValue)
            {
                query = query.Where(e => e.DateByCalander <= dateTo.Value);
            }
            query = query.OrderByDescending(x => x.Id);

            var model = new ScientificProjectsVM
            {
                all_ScientificProjectListVM = query,
                all_Departments = await _unitOfWork.Departments.GetAllAsync(),
            };

            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {
                var lang = SessionHelper.GetCurrentLanguage();
                var allData_list = model.all_ScientificProjectListVM;
                var ListTitles = new List<string>
                {
                    Resource2.ProjectNo,Resource1.ProjectName,Resource1.Student_Employee,Resource1.DateByCalander,Resource2.ScientificSpecialization
                };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = single.SerialCode,
                        t2 = lang == "ar" ? single.ProjectNameEn : single.ProjectNameEn,
                        t3 = lang == "ar" ? single.Student_EmployeeAr : single.Student_EmployeeEn,
                        t4 = single.DateByCalander.HasValue ? single.DateByCalander.Value.ToString("d")?.Replace("/","-") : "",
                        t5 = lang == "ar" ? single.Department?.NameAr : single.Department?.NameEn
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
                    var fileExcelName = Resource1.ScientificProjectsList;
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
            var report = await _unitOfWork.ScientificProjects.GetByColumnAsync(
                e => e.SerialCode != null && e.Id == (int)request.Id);

            if (result.success == true)
            {
                if (request.Role == "Trainer")
                {
                    await _hubContext.Clients.Groups("ActivitiesSupervisor")
                        .SendAsync("ReceiveNotification", new
                        {
                            Title = "",
                            Message = ""
                        });
                    await _notificationService.SendNotificationToRoleAsync(
                          "مشروع علمي",
                          $"يوجد مشروع علمي رقم {report?.SerialCode} جاهز للإعتماد",
                          (int)RoleNumber.ActivitiesSupervisor
                      );
                }
                else if (request.Role == "activityMonitor")
                {
                    await _hubContext.Clients.Groups("Manager")
                        .SendAsync("ReceiveNotification", new
                        {
                            Title = "",
                            Message = ""
                        });
                    await _notificationService.SendNotificationToRoleAsync(
                          "مشروع علمي",
                          $"يوجد مشروع علمي رقم {report?.SerialCode} جاهز للإعتماد",
                          (int)RoleNumber.Manager
                      );
                }
            }
            return Json(new { success = result.success, message = result.message });
        }


        [HttpGet]
        public async Task<IActionResult> AddEdit(int? id)
        {
            ScientificProjectsVM projectVM;

            var all_Departments = await _unitOfWork.Departments.GetAllAsync();
            if (all_Departments == null) return NotFound();

            if (id == null || id == 0)
            {
                // =========================
                // ADD MODE
                // =========================
                var maxProject = await _unitOfWork.ScientificProjects
                    .GetMaxRecordAsync(x => x.Id, null);

                projectVM = new ScientificProjectsVM();

                if (maxProject != null)
                    projectVM.SerialCode =
                        (Convert.ToInt64(maxProject?.SerialCode) + 1).ToString();
                else
                    projectVM.SerialCode = "1000";

                projectVM.CurrentDate = AppDubaiTime.Now.ToString("yyyy-MM-dd");
            }
            else
            {
                // =========================
                // EDIT MODE
                // =========================
                var project = await _unitOfWork.ScientificProjects
                    .GetByIdAsync(id.Value);

                if (project == null) return NotFound();

                projectVM = _mapper.Map<ScientificProjectsVM>(project);

                // تحميل القوائم المرتبطة
                projectVM.GoalsListAr = _unitOfWork.ScientificProjectGoals
                    .Table.Where(x => x.ScientificProjectId == id && x.LangType == (int)LangEnum.Ar)
                    .ToList();

                projectVM.GoalsListEn = _unitOfWork.ScientificProjectGoals
                    .Table.Where(x => x.ScientificProjectId == id && x.LangType == (int)LangEnum.En)
                    .ToList();

                projectVM.ToolsListAr = _unitOfWork.ScientificProjectTools
                    .Table.Where(x => x.ScientificProjectId == id && x.LangType == (int)LangEnum.Ar)
                    .ToList();

                projectVM.ToolsListEn = _unitOfWork.ScientificProjectTools
                    .Table.Where(x => x.ScientificProjectId == id && x.LangType == (int)LangEnum.En)
                    .ToList();

                projectVM.IndividualsListAr = _unitOfWork.ScientificProjectIndividuals
                    .Table.Where(x => x.ScientificProjectId == id && x.LangType == (int)LangEnum.Ar)
                    .ToList();

                projectVM.IndividualsListEn = _unitOfWork.ScientificProjectIndividuals
                    .Table.Where(x => x.ScientificProjectId == id && x.LangType == (int)LangEnum.En)
                    .ToList();
            }

            // تأكد من عدد العناصر
            projectVM.GoalsListAr ??= new List<ScientificProjectGoals>();
            projectVM.GoalsListEn ??= new List<ScientificProjectGoals>();
            projectVM.ToolsListAr ??= new List<ScientificProjectTools>();
            projectVM.ToolsListEn ??= new List<ScientificProjectTools>();
            projectVM.IndividualsListAr ??= new List<ScientificProjectIndividuals>();
            projectVM.IndividualsListEn ??= new List<ScientificProjectIndividuals>();

            while (projectVM.GoalsListAr.Count < 4)
                projectVM.GoalsListAr.Add(new ScientificProjectGoals());

            while (projectVM.GoalsListEn.Count < 4)
                projectVM.GoalsListEn.Add(new ScientificProjectGoals());

            while (projectVM.ToolsListAr.Count < 5)
                projectVM.ToolsListAr.Add(new ScientificProjectTools());

            while (projectVM.ToolsListEn.Count < 5)
                projectVM.ToolsListEn.Add(new ScientificProjectTools());

            while (projectVM.IndividualsListAr.Count < 4)
                projectVM.IndividualsListAr.Add(new ScientificProjectIndividuals());

            while (projectVM.IndividualsListEn.Count < 4)
                projectVM.IndividualsListEn.Add(new ScientificProjectIndividuals());

            projectVM.all_Departments = all_Departments;
            projectVM.DepartmentsList =
                SelectListHelper.BindSelectList(all_Departments.ToList(), projectVM.DepartmentId).ToList();

            projectVM.Old1_Path = projectVM.FilePath1;
            projectVM.Old2_Path = projectVM.FilePath2;
            projectVM.Old3_Path = projectVM.FilePath3;
            projectVM.Old4_Path = projectVM.FilePath4;
            projectVM.Old5_Path = projectVM.FilePath5;
            projectVM.Old6_Path = projectVM.FilePath6;

            projectVM.OldFilePreliminary1_Path = projectVM.FilePreliminaryPath1;
            projectVM.OldFilePreliminary2_Path = projectVM.FilePreliminaryPath2;
            projectVM.OldFilePreliminary3_Path = projectVM.FilePreliminaryPath3;

            return View(projectVM);
        }

        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddEdit(ScientificProjectsVM model)
        {
            var FolderEntityWillSaveIn = "Scientific Projects";
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
            var filePreliminary1_Text = await ValidateImageAsync(model.TempFilePreliminary1_Path, model.FilePreliminary1, model.FilePreliminaryPath1, "FilePreliminary1");
            var filePreliminary2_Text = await ValidateImageAsync(model.TempFilePreliminary2_Path, model.FilePreliminary2, model.FilePreliminaryPath2, "FilePreliminary2");
            var filePreliminary3_Text = await ValidateImageAsync(model.TempFilePreliminary3_Path, model.FilePreliminary3, model.FilePreliminaryPath3, "FilePreliminary3");

            var file1_Text = await ValidateImageAsync(model.TempFile1_Path, model.File1, model.FilePath1, "File1");
            var file2_Text = await ValidateImageAsync(model.TempFile2_Path, model.File2, model.FilePath2, "File2");
            var file3_Text = await ValidateImageAsync(model.TempFile3_Path, model.File3, model.FilePath3, "File3");
            var file4_Text = await ValidateImageAsync(model.TempFile4_Path, model.File4, model.FilePath4, "File4");
            var file5_Text = await ValidateImageAsync(model.TempFile5_Path, model.File5, model.FilePath5, "File5");
            var file6_Text = await ValidateImageAsync(model.TempFile6_Path, model.File6, model.FilePath6, "File6");

            if (filePreliminary1_Text != "OK" && filePreliminary1_Text != "null") ModelState.AddModelError("FilePreliminary1", filePreliminary1_Text ?? " ");
            if (filePreliminary2_Text != "OK" && filePreliminary2_Text != "null") ModelState.AddModelError("FilePreliminary2", filePreliminary2_Text ?? " ");
            if (filePreliminary3_Text != "OK" && filePreliminary3_Text != "null") ModelState.AddModelError("FilePreliminary3", filePreliminary3_Text ?? " ");
            if (file1_Text != "OK" && file1_Text != "null") ModelState.AddModelError("File1", file1_Text ?? " ");
            if (file2_Text != "OK" && file2_Text != "null") ModelState.AddModelError("File2", file2_Text ?? " ");
            if (file3_Text != "OK" && file3_Text != "null") ModelState.AddModelError("File3", file3_Text ?? " ");
            if (file4_Text != "OK" && file4_Text != "null") ModelState.AddModelError("File4", file4_Text ?? " ");
            if (file5_Text != "OK" && file5_Text != "null") ModelState.AddModelError("File5", file5_Text ?? " ");
            if (file6_Text != "OK" && file6_Text != "null") ModelState.AddModelError("File6", file6_Text ?? " ");

            #endregion Validate Is File is PDF And MG // Validate PDF

            #region validation fails // !ModelState.IsValid
            // If validation fails
            if (!ModelState.IsValid)
            {
                #region Remove ModelState Image Errors To Rebind New Data Temp
                ModelState.Remove("TempFilePreliminary1_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("OldFilePreliminary1_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("FilePreliminary1"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFilePreliminary2_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("OldFilePreliminary2_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("FilePreliminary2"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFilePreliminary3_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("OldFilePreliminary3_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("FilePreliminary3"); // its Important To Bind New Data Temp

                ModelState.Remove("TempFile1_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old1_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File1"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile2_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old2_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File2"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile3_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old3_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File3"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile4_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old4_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File4"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile5_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old5_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File5"); // its Important To Bind New Data Temp
                ModelState.Remove("TempFile6_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("Old6_Path"); // its Important To Bind New Data Temp
                ModelState.Remove("File6"); // its Important To Bind New Data Temp
                #endregion Remove ModelState Image Errors To Rebind New Data Temp
                if (filePreliminary1_Text != "OK" && filePreliminary1_Text != "null") ModelState.AddModelError("FilePreliminary1", filePreliminary1_Text ?? " ");
                if (filePreliminary2_Text != "OK" && filePreliminary2_Text != "null") ModelState.AddModelError("FilePreliminary2", filePreliminary2_Text ?? " ");
                if (filePreliminary3_Text != "OK" && filePreliminary3_Text != "null") ModelState.AddModelError("FilePreliminary3", filePreliminary3_Text ?? " ");
                if (file1_Text != "OK" && file1_Text != "null") ModelState.AddModelError("File1", file1_Text ?? " ");
                if (file2_Text != "OK" && file2_Text != "null") ModelState.AddModelError("File2", file2_Text ?? " ");
                if (file3_Text != "OK" && file3_Text != "null") ModelState.AddModelError("File3", file3_Text ?? " ");
                if (file4_Text != "OK" && file4_Text != "null") ModelState.AddModelError("File4", file4_Text ?? " ");
                if (file5_Text != "OK" && file5_Text != "null") ModelState.AddModelError("File5", file5_Text ?? " ");
                if (file6_Text != "OK" && file6_Text != "null") ModelState.AddModelError("File6", file6_Text ?? " ");

                // If the user uploads a new file → cache it before returning
                if (model.FilePreliminary1 != null)
                    model.TempFilePreliminary1_Path = await FileHelper.SaveTempAsync(model.FilePreliminary1);
                if (model.FilePreliminary2 != null)
                    model.TempFilePreliminary2_Path = await FileHelper.SaveTempAsync(model.FilePreliminary2);
                if (model.FilePreliminary3 != null)
                    model.TempFilePreliminary3_Path = await FileHelper.SaveTempAsync(model.FilePreliminary3);

                if (model.File1 != null)
                    model.TempFile1_Path = await FileHelper.SaveTempAsync(model.File1);
                if (model.File2 != null)
                    model.TempFile2_Path = await FileHelper.SaveTempAsync(model.File2);
                if (model.File3 != null)
                    model.TempFile3_Path = await FileHelper.SaveTempAsync(model.File3);
                if (model.File4 != null)
                    model.TempFile4_Path = await FileHelper.SaveTempAsync(model.File4);
                if (model.File5 != null)
                    model.TempFile5_Path = await FileHelper.SaveTempAsync(model.File5);
                if (model.File6 != null)
                    model.TempFile6_Path = await FileHelper.SaveTempAsync(model.File6);

                // Re-fall and Re-populate enum select list
                model.all_Departments = await _unitOfWork.Departments.GetAllAsync();
                model.DepartmentsList = SelectListHelper.BindSelectList(model.all_Departments.ToList(), model.DepartmentId).ToList();

                TempData.Keep(); // for safety if re-rendered
                // Ensure lists are not null and contain required count
                model.GoalsListAr ??= new List<ScientificProjectGoals>();
                model.GoalsListEn ??= new List<ScientificProjectGoals>();
                model.ToolsListAr ??= new List<ScientificProjectTools>();
                model.ToolsListEn ??= new List<ScientificProjectTools>();
                model.IndividualsListAr ??= new List<ScientificProjectIndividuals>();
                model.IndividualsListEn ??= new List<ScientificProjectIndividuals>();

                while (model.GoalsListAr.Count < 4) model.GoalsListAr.Add(new ScientificProjectGoals());
                while (model.GoalsListEn.Count < 4) model.GoalsListEn.Add(new ScientificProjectGoals());

                while (model.ToolsListAr.Count < 5) model.ToolsListAr.Add(new ScientificProjectTools());
                while (model.ToolsListEn.Count < 5) model.ToolsListEn.Add(new ScientificProjectTools());

                while (model.IndividualsListAr.Count < 4) model.IndividualsListAr.Add(new ScientificProjectIndividuals());
                while (model.IndividualsListEn.Count < 4) model.IndividualsListEn.Add(new ScientificProjectIndividuals());

                return View(model);
            }
            #endregion validation fails // !ModelState.IsValid

            // ---------------------------
            //  PROCESS FINAL FILE
            // ---------------------------
            #region PROCESS FINAL FILE Handeling Save in newpath From Temp
            string finalFileName = null;

            // 1) If there is a new filePreliminary uploaded by the user
            if (model.FilePreliminary1 != null)
            {
                FileHelper.DeleteImageFile(model.OldFilePreliminary1_Path);
                model.FilePreliminaryPath1 = await FileHelper.SaveImageAsync(model.FilePreliminary1, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved filePreliminary
            else if (!string.IsNullOrEmpty(model.TempFilePreliminary1_Path))
            {
                FileHelper.DeleteImageFile(model.OldFilePreliminary1_Path);
                model.FilePreliminaryPath1 = FileHelper.MoveTempToFinal(
                    model.TempFilePreliminary1_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            //// 3) If neither this nor that → use the old filePreliminary (Edit only)
            //else
            //{
            //    model.FilePreliminaryPath1 = model.Old1_Path;
            //}


            // 1) If there is a new filePreliminary uploaded by the user
            if (model.FilePreliminary2 != null)
            {
                FileHelper.DeleteImageFile(model.OldFilePreliminary2_Path);
                model.FilePreliminaryPath2 = await FileHelper.SaveImageAsync(model.FilePreliminary2, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved filePreliminary
            else if (!string.IsNullOrEmpty(model.TempFilePreliminary2_Path))
            {
                FileHelper.DeleteImageFile(model.OldFilePreliminary2_Path);
                model.FilePreliminaryPath2 = FileHelper.MoveTempToFinal(
                    model.TempFilePreliminary2_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            //// 3) If neither this nor that → use the old filePreliminary (Edit only)
            //else
            //{
            //    model.FilePreliminaryPath2 = model.Old2_Path;
            //}

            // 1) If there is a new filePreliminary uploaded by the user
            if (model.FilePreliminary3 != null)
            {
                FileHelper.DeleteImageFile(model.OldFilePreliminary3_Path);
                model.FilePreliminaryPath3 = await FileHelper.SaveImageAsync(model.FilePreliminary3, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved filePreliminary
            else if (!string.IsNullOrEmpty(model.TempFilePreliminary3_Path))
            {
                FileHelper.DeleteImageFile(model.OldFilePreliminary3_Path);
                model.FilePreliminaryPath3 = FileHelper.MoveTempToFinal(
                    model.TempFilePreliminary3_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            //// 3) If neither this nor that → use the old filePreliminary (Edit only)
            //else
            //{
            //    model.FilePreliminaryPath3 = model.Old3_Path;
            //}

            // 1) If there is a new file uploaded by the user
            if (model.File1 != null)
            {
                FileHelper.DeleteImageFile(model.Old1_Path);
                model.FilePath1 = await FileHelper.SaveImageAsync(model.File1, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile1_Path))
            {
                FileHelper.DeleteImageFile(model.Old1_Path);
                model.FilePath1 = FileHelper.MoveTempToFinal(
                    model.TempFile1_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            //// 3) If neither this nor that → use the old file (Edit only)
            //else
            //{
            //    model.FilePath1 = model.Old1_Path;
            //}


            // 1) If there is a new file uploaded by the user
            if (model.File2 != null)
            {
                FileHelper.DeleteImageFile(model.Old2_Path);
                model.FilePath2 = await FileHelper.SaveImageAsync(model.File2, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile2_Path))
            {
                FileHelper.DeleteImageFile(model.Old2_Path);
                model.FilePath2 = FileHelper.MoveTempToFinal(
                    model.TempFile2_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            //// 3) If neither this nor that → use the old file (Edit only)
            //else
            //{
            //    model.FilePath2 = model.Old2_Path;
            //}

            // 1) If there is a new file uploaded by the user
            if (model.File3 != null)
            {
                FileHelper.DeleteImageFile(model.Old3_Path);
                model.FilePath3 = await FileHelper.SaveImageAsync(model.File3, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile3_Path))
            {
                FileHelper.DeleteImageFile(model.Old3_Path);
                model.FilePath3 = FileHelper.MoveTempToFinal(
                    model.TempFile3_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            //// 3) If neither this nor that → use the old file (Edit only)
            //else
            //{
            //    model.FilePath3 = model.Old3_Path;
            //}

            // 1) If there is a new file uploaded by the user
            if (model.File4 != null)
            {
                FileHelper.DeleteImageFile(model.Old4_Path);
                model.FilePath4 = await FileHelper.SaveImageAsync(model.File4, FolderEntityWillSaveIn);
            }
            //2) If there is no new upload but there is a temporarily saved file
            else if (!string.IsNullOrEmpty(model.TempFile4_Path))
            {
                FileHelper.DeleteImageFile(model.Old4_Path);
                model.FilePath4 = FileHelper.MoveTempToFinal(
                    model.TempFile4_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            //// 3) If neither this nor that → use the old file (Edit only)
            //else
            //{
            //    model.FilePath4 = model.Old4_Path;
            //}

            // File5
            if (model.File5 != null)
            {
                model.FilePath5 = await FileHelper.SaveImageAsync(model.File5, FolderEntityWillSaveIn);
            }
            else if (!string.IsNullOrEmpty(model.TempFile5_Path))
            {
                model.FilePath5 = FileHelper.MoveTempToFinal(
                    model.TempFile5_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }

            // File6
            if (model.File6 != null)
            {
                model.FilePath6 = await FileHelper.SaveImageAsync(model.File6, FolderEntityWillSaveIn);
            }
            else if (!string.IsNullOrEmpty(model.TempFile6_Path))
            {
                model.FilePath6 = FileHelper.MoveTempToFinal(
                    model.TempFile6_Path,
                    out finalFileName,
                    FolderEntityWillSaveIn
                );
            }
            #endregion PROCESS FINAL FILE Handeling Save in newpath From Temp
            // ---------------------------------------

            var project = _mapper.Map<ScientificProjects>(model);

            //var project = new ScientificProjects
            //{
            //    SerialCode = model.SerialCode,
            //    DateByCalander = model.DateByCalander,
            //    DepartmentId = model.DepartmentId,
            //    TrainerId = model.TrainerId,
            //    ProjectNameAr = model.ProjectNameAr,
            //    ProjectNameEn = model.ProjectNameEn,
            //    IdeaOwnerAr = model.IdeaOwnerAr,
            //    IdeaOwnerEn = model.IdeaOwnerEn,
            //    ProjectIdeaAr = model.ProjectIdeaAr,
            //    ProjectIdeaEN = model.ProjectIdeaEN,
            //    ProjectElement1 = model.ProjectElement1,
            //    ProjectElement2 = model.ProjectElement2,
            //    ProjectElement3 = model.ProjectElement3,
            //    ProjectElement4 = model.ProjectElement4,
            //    ProjectElement5 = model.ProjectElement5,
            //    ProjectElement6 = model.ProjectElement6,
            //    ProjectElement7 = model.ProjectElement7,
            //    ProjectElement8 = model.ProjectElement8,
            //    InstallationRecommendations = model.InstallationRecommendations,
            //    DeliveryData = model.DeliveryData,
            //    Participant1 = model.Participant1,
            //    Participant2 = model.Participant2,
            //    Participant3 = model.Participant3,
            //    Participant4 = model.Participant4,
            //    Supervisor1 = model.Supervisor1,
            //    Supervisor2 = model.Supervisor2,
            //    Supervisor3 = model.Supervisor3,
            //    Supervisor4 = model.Supervisor4,
            //    Supervisor5 = model.Supervisor5,
            //    Supervisor6 = model.Supervisor6,
            //    ExpectedCost = model.ExpectedCost,
            //    FilePath1 = model.FilePath1,
            //    FilePath2 = model.FilePath2,
            //    FilePath3 = model.FilePath3,
            //    FilePath4 = model.FilePath4,
            //    FilePath5 = model.FilePath5,
            //    FilePath6 = model.FilePath6
            //};

            var serialCodeExists = _unitOfWork.ScientificProjects.Table.Any(x => x.SerialCode == project.SerialCode);
            if (serialCodeExists)
            {
                project.SerialCode = (_unitOfWork.ScientificProjects.Table.Max(s => Convert.ToInt32(s.SerialCode)) + 1).ToString();
            }

            ScientificProjects dbProject;

            if (model.Id == 0)
            {
                // ================= ADD =================
                dbProject = await _unitOfWork.ScientificProjects.AddAsync(project);
            }
            else
            {
                // ================= EDIT =================
                dbProject = await _unitOfWork.ScientificProjects.GetByIdAsync(model.Id);
                if (dbProject == null) return NotFound();

                _mapper.Map(model, dbProject);

                _unitOfWork.ScientificProjects.Update(dbProject);

                // احذف القديم من Goals/Tools/Individuals
                var oldGoals = _unitOfWork.ScientificProjectGoals
                    .Table.Where(x => x.ScientificProjectId == dbProject.Id);
                foreach (var item in oldGoals)
                    _unitOfWork.ScientificProjectGoals.Delete(item);

                var oldTools = _unitOfWork.ScientificProjectTools
                    .Table.Where(x => x.ScientificProjectId == dbProject.Id);
                foreach (var item in oldTools)
                    _unitOfWork.ScientificProjectTools.Delete(item);

                var oldIndividuals = _unitOfWork.ScientificProjectIndividuals
                    .Table.Where(x => x.ScientificProjectId == dbProject.Id);
                foreach (var item in oldIndividuals)
                    _unitOfWork.ScientificProjectIndividuals.Delete(item);
            }

            await _unitOfWork.CompleteAsync();

            if (model.ToolsListAr != null && model.ToolsListAr.Count > 0)
            {
                foreach (var item in model.ToolsListAr)
                {
                    if (string.IsNullOrWhiteSpace(item.ToolDescription)) continue;

                    item.ScientificProjectId = dbProject.Id;
                    item.LangType = (int)LangEnum.Ar;
                    await _unitOfWork.ScientificProjectTools.AddAsync(item);
                }
            }
            if (model.ToolsListEn != null && model.ToolsListEn.Count > 0)
            {
                foreach (var item in model.ToolsListEn)
                {
                    if (string.IsNullOrWhiteSpace(item.ToolDescription)) continue;

                    item.ScientificProjectId = dbProject.Id;
                    item.LangType = (int)LangEnum.En;
                    await _unitOfWork.ScientificProjectTools.AddAsync(item);
                }
            }
            if (model.GoalsListAr != null && model.GoalsListAr.Count > 0)
            {
                foreach (var item in model.GoalsListAr)
                {
                    if (string.IsNullOrWhiteSpace(item.GoalDescription)) continue;

                    item.ScientificProjectId = dbProject.Id;
                    item.LangType = (int)LangEnum.Ar;
                    await _unitOfWork.ScientificProjectGoals.AddAsync(item);
                }
            }
            if (model.GoalsListEn != null && model.GoalsListEn.Count > 0)
            {
                foreach (var item in model.GoalsListEn)
                {
                    if (string.IsNullOrWhiteSpace(item.GoalDescription)) continue;

                    item.ScientificProjectId = dbProject.Id;
                    item.LangType = (int)LangEnum.En;
                    await _unitOfWork.ScientificProjectGoals.AddAsync(item);
                }
            }
            if (model.IndividualsListAr != null && model.IndividualsListAr.Count > 0)
            {
                foreach (var item in model.IndividualsListAr)
                {
                    if (string.IsNullOrWhiteSpace(item.IndividualName)) continue;

                    item.ScientificProjectId = dbProject.Id;
                    item.LangType = (int)LangEnum.Ar;
                    await _unitOfWork.ScientificProjectIndividuals.AddAsync(item);
                }
            }
            if (model.IndividualsListEn != null && model.IndividualsListEn.Count > 0)
            {
                foreach (var item in model.IndividualsListEn)
                {
                    if (string.IsNullOrWhiteSpace(item.IndividualName)) continue;

                    item.ScientificProjectId = dbProject.Id;
                    item.LangType = (int)LangEnum.En;
                    await _unitOfWork.ScientificProjectIndividuals.AddAsync(item);
                }
            }
            await _unitOfWork.CompleteAsync();

            var thisTrainer = await _unitOfWork.Trainers.GetByIdAsync(dbProject.TrainerId ?? 0);
            var thisTrainerUserId = thisTrainer?.UserId;
            //await _hubContext.Clients.Groups("Manager")
            //       .SendAsync("ReceiveNotification", new
            //       {
            //           Title = "",
            //           Message = ""
            //       });
            if (thisTrainerUserId != null && thisTrainerUserId.Length > 5)
            {
                if (model.Id == 0)
                {
                    await _notificationService.SendNotificationToUsersAsync(  // To Send Notification To Trainer
                        "مشروع علمي جديد",
                        $"يوجد مشروع علمي جديد رقم {dbProject.SerialCode} جاهز للإعتماد",
                        new List<string> { thisTrainerUserId }
                    );
                }
            }

            return RedirectToAction(nameof(Index)); // After Add New
        }

    }

}
