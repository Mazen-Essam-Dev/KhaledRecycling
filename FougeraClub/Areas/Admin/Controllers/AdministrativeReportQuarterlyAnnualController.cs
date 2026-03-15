using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.Enums;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.AdministrativeReportQuarterlyAnnual;
using FougeraClub.Areas.Admin.ViewModels.SMS;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;


namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class AdministrativeReportQuarterlyAnnualController : Controller
    {
        private readonly IAdministrativeReportQuarterlyAnnualService _service;
        private readonly IMapper _mapper;
        private readonly UserManager<Infrastructure.Identity.ApplicationUser> _userManager;
        private readonly ISMSService _smsService;

        public AdministrativeReportQuarterlyAnnualController(
            IAdministrativeReportQuarterlyAnnualService AdministrativeReportQuarterlyAnnualService,
            IMapper mapper,
            UserManager<Infrastructure.Identity.ApplicationUser> userManager,ISMSService sMSService)
        {
            _service = AdministrativeReportQuarterlyAnnualService;
            _mapper = mapper;
            _userManager = userManager;
            _smsService = sMSService;
        }
        [YesGet]
        public async Task<IActionResult> Index(int? selectedYear)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var model = new AdministrativeReportQuarterlyAnnualIndexVM();
            var quarters = SelectListHelper.GetEnumSelectList<QuartersYear>();

            // Year Dropdown
            var years = await _service.GetAllYearsInDb();
            var allMonthsofYearsAnnualy = await _service.GetAllMonthsofYearsAnnualy(selectedYear);
            var list5Quarters = new List<bool>() { false, false, false, false, false };
            var listAllQuarter = new List<List<int>> {
                    new List<int>{ 1,2,3 },
                    new List<int>{4,5,6},
                    new List<int>{7,8,9},
                    new List<int>{10,11,12},
                }.ToList();

            var CollaborativeIsSiggned = await _service.CheckCollaborativeReportIsSiggned(selectedYear);

            var i = 0;
            foreach (var listQuater in listAllQuarter)
            {
                list5Quarters[i] = allMonthsofYearsAnnualy.Where(c => c.month.HasValue && listQuater.Any(x => x == c.month.Value)).Any();
                if (list5Quarters[i] == true)
                {
                    model.AdministrativeCounts[i] = allMonthsofYearsAnnualy.Where(c => c.month.HasValue && listQuater.Any(x => x == c.month.Value)).Sum(a => a.AdministrativeDetailsCount);
                    model.ActivitiesCounts[i] = allMonthsofYearsAnnualy.Where(c => c.month.HasValue && listQuater.Any(x => x == c.month.Value)).Sum(a => a.ActivitiesDetailsCount);
                }
                else
                {
                    model.AdministrativeCounts[i] = 0;
                    model.ActivitiesCounts[i] = 0;
                }
                i++;
            }

            if (list5Quarters.Any(x => x == true))
            {
                list5Quarters[4] = true;
                model.ActivitiesCounts[4] = model.ActivitiesCounts.Sum();
                model.AdministrativeCounts[4] = model.AdministrativeCounts.Sum();
            }

            ViewBag.Years = years.ToList();
            ViewBag.selectedYear = selectedYear;
            ViewBag.list5Quarters = list5Quarters;

            model.Quarters = quarters.ToList();
            model.CollaborativeIsSiggned = CollaborativeIsSiggned;

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", model);
            }

            return View(model);
        }
        [YesGet]
        public async Task<IActionResult> All2Details(int? selectedYear, int? quarter, int page = 1, int pageSize = 50)
        {
            if (!selectedYear.HasValue || !quarter.HasValue || selectedYear == 0 || quarter == 0)
                return RedirectToAction(nameof(Index), new { selectedYear });

            var lang = SessionHelper.GetCurrentLanguage();

            // Delegate the heavy logic to the service
            var result = await _service.GetAdministrativeAndActivitiesDetailsAsync(selectedYear.Value, quarter.Value, lang);

            var VM = _mapper.Map<MonthsOfYearsAnnualyActivitiesAndAdministrative>(result);

            // Populate dropdowns
            ViewBag.Years = (await _service.GetAllYearsInDb()).ToList();
            ViewBag.selectedYear = selectedYear;
            ViewBag.quarter = quarter;


            // No data case
            if ((VM.model_Administrative == null || !VM.model_Administrative.Any()) &&
                (VM.model_Activities == null || !VM.model_Activities.Any()))
            {
                return RedirectToAction(nameof(Index), new { selectedYear });
            }

            #region Signature Get And Viewed

            var quarterCollaborativeVM_null = new QuartersReportCollaborativeDetailsVM()
            {
                Quarter = quarter != null ? (QuartersYear)quarter : null
                ,
                Type = QuarterlyReportType.Collaborative
                ,
                Year = selectedYear
            };
            quarterCollaborativeVM_null.MonthsOfYearsAnnualyActivitiesAndAdministrativeVM = VM;

            if (selectedYear == null || quarter == null)
                return View(quarterCollaborativeVM_null);

            var QuarterSignutre = await _service.GetQuarterSignature(selectedYear, (QuartersYear)quarter, QuarterlyReportType.Collaborative);

            if (QuarterSignutre == null)
                return View(quarterCollaborativeVM_null);

            var quarterCollaborativeVM = _mapper.Map<QuartersReportCollaborativeDetailsVM>(QuarterSignutre);
            quarterCollaborativeVM.MonthsOfYearsAnnualyActivitiesAndAdministrativeVM = VM;

            // Fetch manager full name if signature exists
            if (quarterCollaborativeVM.ManagerSignature != null && !string.IsNullOrEmpty(quarterCollaborativeVM.ManagerSignature.UserId))
            {
                var managerUser = await _userManager.FindByIdAsync(quarterCollaborativeVM.ManagerSignature.UserId);
                if (managerUser != null)
                {
                    quarterCollaborativeVM.ManagerFullName = !string.IsNullOrEmpty(managerUser.FullNameAr) ? managerUser.FullNameAr : managerUser.FullNameEn;
                }
            }

            // Fetch manager full name if signature exists
            if (quarterCollaborativeVM.ManagerSignature != null && !string.IsNullOrEmpty(quarterCollaborativeVM.ManagerSignature.UserId))
            {
                var managerUser = await _userManager.FindByIdAsync(quarterCollaborativeVM.ManagerSignature.UserId);
                if (managerUser != null)
                {
                    quarterCollaborativeVM.ManagerFullName = !string.IsNullOrEmpty(managerUser.FullNameAr) ? managerUser.FullNameAr : managerUser.FullNameEn;
                }
            }

            return View(quarterCollaborativeVM);
            #endregion Signature Get And Viewed
        }
        [YesGet]
        public async Task<IActionResult> AdminstrativeDetails(int? selectedYear, int? quarter, int page = 1, int pageSize = 50)
        {
            if (selectedYear == null || quarter == null)
            {
                //return View(new List<MonthsOfYearsAnnualyWithDetailsVM>());
                return RedirectToAction(nameof(Index), new { selectedYear = selectedYear });
            }
            if (selectedYear.HasValue && selectedYear.Value != 0 && quarter.HasValue && quarter.Value != 0)
            {
                var lang = SessionHelper.GetCurrentLanguage();
                var allMonthsofYearsAnnualy = await _service.GetAllMonthsofYearsAnnualy_Data(selectedYear);

                // Year Dropdown
                var years = await _service.GetAllYearsInDb();

                ViewBag.Years = years.ToList();
                ViewBag.selectedYear = selectedYear;

                var listQuarter = new List<List<int>> {
                    new List<int>{ 1,2,3 },
                    new List<int>{4,5,6},
                    new List<int>{7,8,9},
                    new List<int>{10,11,12},
                }.ToList();

                if (quarter != null)
                {
                    if (quarter == 5)
                    {
                        // Tow Administrative And Activities  For-All-ThisYear
                        allMonthsofYearsAnnualy = allMonthsofYearsAnnualy.OrderBy(x => x.Date).ToList();
                    }
                    if (quarter > 0 && quarter < 5)
                    {
                        // For Handeling Administrative Only
                        int AdministrativeType = (int)MonthlyAdministrativeReportType.Administrative;
                        allMonthsofYearsAnnualy = allMonthsofYearsAnnualy.Where(x => (x.Type.HasValue) && x.Type == AdministrativeType);

                        var listOfQuarter = listQuarter[quarter.Value - 1];
                        allMonthsofYearsAnnualy = allMonthsofYearsAnnualy.Where(c => c.month.HasValue && listOfQuarter.Any(x => x == c.month.Value)).OrderBy(x => x.Date).ToList();
                    }
                }

                var model = _mapper.Map<List<MonthsOfYearsAnnualyWithDetailsVM>>(allMonthsofYearsAnnualy);

                ViewBag.SelectedYear = selectedYear;
                ViewBag.quarter = quarter;
                if (model == null || model.Count == 0)
                {
                    return RedirectToAction(nameof(Index), new { selectedYear = selectedYear });
                }

                #region Signature Get And Viewed

                var quarterAdminstrativeSignutreVM_null = new QuartersReportAdminstrativeDetailsVM()
                {
                    Quarter = quarter != null ? (QuartersYear)quarter : null
                    ,
                    Type = QuarterlyReportType.Administrative
                    ,
                    Year = selectedYear
                };
                quarterAdminstrativeSignutreVM_null.AdminstrativeDetailsVM_List = model;

                if (selectedYear == null || quarter == null)
                    return View(quarterAdminstrativeSignutreVM_null);

                var AdminstrativeSignutre = await _service.GetQuarterSignature(selectedYear.Value, (QuartersYear)quarter.Value, QuarterlyReportType.Administrative);

                if (AdminstrativeSignutre == null)
                    return View(quarterAdminstrativeSignutreVM_null);


                var quarterAdminstrativeSignutreVM = _mapper.Map<QuartersReportAdminstrativeDetailsVM>(AdminstrativeSignutre);
                quarterAdminstrativeSignutreVM.AdminstrativeDetailsVM_List = model;

                // Fetch manager full name if signature exists
                if (quarterAdminstrativeSignutreVM.ManagerSignature != null && !string.IsNullOrEmpty(quarterAdminstrativeSignutreVM.ManagerSignature.UserId))
                {
                    var managerUser = await _userManager.FindByIdAsync(quarterAdminstrativeSignutreVM.ManagerSignature.UserId);
                    if (managerUser != null)
                    {
                        quarterAdminstrativeSignutreVM.ManagerFullName = !string.IsNullOrEmpty(managerUser.FullNameAr) ? managerUser.FullNameAr : managerUser.FullNameEn;
                    }
                }

                return View(quarterAdminstrativeSignutreVM);
                #endregion Signature Get And Viewed
            }
            else
            {
                //return View(new List<MonthsOfYearsAnnualyWithDetailsVM>());
                return RedirectToAction(nameof(Index), new { selectedYear = selectedYear });
            }
        }
        [YesGet]
        public async Task<IActionResult> ActivitiesDetails(int? selectedYear, int? quarter, int page = 1, int pageSize = 50)
        {
            if (selectedYear == null || quarter == null)
            {
                //return View(new List<MonthsOfYearsAnnualyWithDetailsVM>());
                return RedirectToAction(nameof(Index), new { selectedYear });
            }
            if (selectedYear.HasValue && selectedYear.Value != 0 && quarter.HasValue && quarter.Value != 0)
            {
                var lang = SessionHelper.GetCurrentLanguage();
                var allMonthsofYearsAnnualy = await _service.GetAllMonthsofYearsAnnualy_Data(selectedYear);

                // Year Dropdown
                var years = await _service.GetAllYearsInDb();

                ViewBag.Years = years.ToList();
                ViewBag.selectedYear = selectedYear;

                var listQuarter = new List<List<int>> {
                    new List<int>{ 1,2,3 },
                    new List<int>{4,5,6},
                    new List<int>{7,8,9},
                    new List<int>{10,11,12},
                }.ToList();

                var listOfQuarter = listQuarter[quarter.Value - 1];
                if (quarter != null)
                {
                    if (quarter == 5)
                    {
                        // Tow Administrative And Activities  For-All-ThisYear
                        allMonthsofYearsAnnualy = allMonthsofYearsAnnualy.OrderBy(x => x.Date).ToList();
                    }
                    if (quarter > 0 && quarter < 5)
                    {
                        // For Handeling Administrative Only
                        int ActivitiesType = (int)MonthlyAdministrativeReportType.Activities;
                        allMonthsofYearsAnnualy = allMonthsofYearsAnnualy.Where(x => (x.Type.HasValue) && x.Type == ActivitiesType);

                        allMonthsofYearsAnnualy = allMonthsofYearsAnnualy.Where(c => c.month.HasValue && listOfQuarter.Any(x => x == c.month.Value)).OrderBy(x => x.Date).ToList();
                    }
                }

                var monthsOfYearsAnnualyWithDetailsVM = _mapper.Map<List<MonthsOfYearsAnnualyWithDetailsVM>>(allMonthsofYearsAnnualy);

                ViewBag.SelectedYear = selectedYear;
                ViewBag.quarter = quarter;
                if (monthsOfYearsAnnualyWithDetailsVM == null || monthsOfYearsAnnualyWithDetailsVM.Count == 0)
                {
                    return RedirectToAction(nameof(Index), new { selectedYear = selectedYear });
                }

                var pieChartData = new PieChartVM();


                int count = 0;
                foreach (var month in listOfQuarter)
                {
                    // Get details for this month
                    var details = allMonthsofYearsAnnualy
                        .FirstOrDefault(c => c.month.HasValue && c.month.Value == month)?
                        .Details;

                    // Get month name
                    string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                    pieChartData.MonthsLables.Add(monthName);

                    if (details != null)
                    {
                        foreach (var detail in details)
                        {
                            var duration = 0;
                            if (detail.ActivityStartDate != null && detail?.ActivityEndDate != null)
                            {
                                duration += (int)detail.ActivityEndDate.Value.Day - (int)detail.ActivityStartDate.Value.Day;
                            }
                        }
                    }

                    var noOfParticipations = details?
                        .Select(s => s.NumberOfParticipants ?? 0)
                        .ToList() ?? new List<int>();

                    // Assign to correct month collection by index
                    switch (count)
                    {
                        case 0:
                            pieChartData.Month1NoOfParticipations = noOfParticipations;
                            break;
                        case 1:
                            pieChartData.Month2NoOfParticipations = noOfParticipations;
                            break;
                        case 2:
                            pieChartData.Month3NoOfParticipations = noOfParticipations;
                            break;
                    }

                    count++;
                }




                var model = new ActivitiesReportQuarterlyVM
                {
                    MonthsOfYearsAnnualyWithDetails = monthsOfYearsAnnualyWithDetailsVM,
                    PieChart = pieChartData
                };

                #region Signature Get And Viewed

                var quarterActivitiesSignutreVM_null = new QuartersReportActivitiesDetailsVM()
                {
                    Quarter = quarter != null ? (QuartersYear)quarter : null,
                    Type = QuarterlyReportType.Activities,
                    Year = selectedYear
                };
                quarterActivitiesSignutreVM_null.ActivitiesDetailsVM = model;

                if (selectedYear == null || quarter == null)
                    return View(quarterActivitiesSignutreVM_null);

                var ActivitiesSignutre = await _service.GetQuarterSignature(selectedYear.Value, (QuartersYear)quarter.Value, QuarterlyReportType.Activities);

                if (ActivitiesSignutre == null)
                    return View(quarterActivitiesSignutreVM_null);


                var quarterActivitiesSignutreVM = _mapper.Map<QuartersReportActivitiesDetailsVM>(ActivitiesSignutre);
                quarterActivitiesSignutreVM.ActivitiesDetailsVM = model;

                // Fetch manager full name if signature exists
                if (quarterActivitiesSignutreVM.ManagerSignature != null && !string.IsNullOrEmpty(quarterActivitiesSignutreVM.ManagerSignature.UserId))
                {
                    var managerUser = await _userManager.FindByIdAsync(quarterActivitiesSignutreVM.ManagerSignature.UserId);
                    if (managerUser != null)
                    {
                        quarterActivitiesSignutreVM.ManagerFullName = !string.IsNullOrEmpty(managerUser.FullNameAr) ? managerUser.FullNameAr : managerUser.FullNameEn;
                    }
                }

                return View(quarterActivitiesSignutreVM);
                #endregion Signature Get And Viewed

            }
            else
            {
                return RedirectToAction(nameof(Index), new { selectedYear = selectedYear });
            }
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintActivitiesDetails(int? selectedYear, int? quarter, int page = 1, int pageSize = 50)
        {
            if (selectedYear == null || quarter == null)
            {
                //return View(new List<MonthsOfYearsAnnualyWithDetailsVM>());
                return RedirectToAction(nameof(Index), new { selectedYear = selectedYear });
            }
            if (selectedYear.HasValue && selectedYear.Value != 0 && quarter.HasValue && quarter.Value != 0)
            {
                var lang = SessionHelper.GetCurrentLanguage();
                var allMonthsofYearsAnnualy = await _service.GetAllMonthsofYearsAnnualy_Data(selectedYear);

                // Year Dropdown
                var years = await _service.GetAllYearsInDb();

                ViewBag.Years = years.ToList();
                ViewBag.selectedYear = selectedYear;

                var listQuarter = new List<List<int>> {
                    new List<int>{ 1,2,3 },
                    new List<int>{4,5,6},
                    new List<int>{7,8,9},
                    new List<int>{10,11,12},
                }.ToList();

                var listOfQuarter = listQuarter[quarter.Value - 1];
                if (quarter != null)
                {
                    if (quarter == 5)
                    {
                        // Tow Administrative And Activities  For-All-ThisYear
                        allMonthsofYearsAnnualy = allMonthsofYearsAnnualy.OrderBy(x => x.Date).ToList();
                    }
                    if (quarter > 0 && quarter < 5)
                    {
                        // For Handeling Administrative Only
                        int ActivitiesType = (int)MonthlyAdministrativeReportType.Activities;
                        allMonthsofYearsAnnualy = allMonthsofYearsAnnualy.Where(x => (x.Type.HasValue) && x.Type == ActivitiesType);

                        allMonthsofYearsAnnualy = allMonthsofYearsAnnualy.Where(c => c.month.HasValue && listOfQuarter.Any(x => x == c.month.Value)).OrderBy(x => x.Date).ToList();
                    }
                }

                var monthsOfYearsAnnualyWithDetailsVM = _mapper.Map<List<MonthsOfYearsAnnualyWithDetailsVM>>(allMonthsofYearsAnnualy);

                ViewBag.SelectedYear = selectedYear;
                ViewBag.quarter = quarter;
                if (monthsOfYearsAnnualyWithDetailsVM == null || monthsOfYearsAnnualyWithDetailsVM.Count == 0)
                {
                    return RedirectToAction(nameof(Index), new { selectedYear = selectedYear });
                }

                var pieChartData = new PieChartVM();


                int count = 0;
                foreach (var month in listOfQuarter)
                {
                    // Get details for this month
                    var details = allMonthsofYearsAnnualy
                        .FirstOrDefault(c => c.month.HasValue && c.month.Value == month)?
                        .Details;

                    // Get month name
                    string monthName = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(month);
                    pieChartData.MonthsLables.Add(monthName);

                    if (details != null)
                    {
                        foreach (var detail in details)
                        {
                            var duration = 0;
                            if (detail.ActivityStartDate != null && detail?.ActivityEndDate != null)
                            {
                                duration += (int)detail.ActivityEndDate.Value.Day - (int)detail.ActivityStartDate.Value.Day;
                            }
                        }
                    }

                    var noOfParticipations = details?
                        .Select(s => s.NumberOfParticipants ?? 0)
                        .ToList() ?? new List<int>();

                    // Assign to correct month collection by index
                    switch (count)
                    {
                        case 0:
                            pieChartData.Month1NoOfParticipations = noOfParticipations;
                            break;
                        case 1:
                            pieChartData.Month2NoOfParticipations = noOfParticipations;
                            break;
                        case 2:
                            pieChartData.Month3NoOfParticipations = noOfParticipations;
                            break;
                    }

                    count++;
                }




                var model = new ActivitiesReportQuarterlyVM
                {
                    MonthsOfYearsAnnualyWithDetails = monthsOfYearsAnnualyWithDetailsVM,
                    PieChart = pieChartData
                };
                #region Signature Get And Viewed

                var quarterActivitiesSignutreVM_null = new QuartersReportActivitiesDetailsVM()
                {
                    Quarter = quarter != null ? (QuartersYear)quarter : null,
                    Type = QuarterlyReportType.Activities,
                    Year = selectedYear
                };
                quarterActivitiesSignutreVM_null.ActivitiesDetailsVM = model;

                if (selectedYear == null || quarter == null)
                    return View(quarterActivitiesSignutreVM_null);

                var ActivitiesSignutre = await _service.GetQuarterSignature(selectedYear.Value, (QuartersYear)quarter.Value, QuarterlyReportType.Activities);

                if (ActivitiesSignutre == null)
                    return View(quarterActivitiesSignutreVM_null);


                var quarterActivitiesSignutreVM = _mapper.Map<QuartersReportActivitiesDetailsVM>(ActivitiesSignutre);
                quarterActivitiesSignutreVM.ActivitiesDetailsVM = model;

                return View(quarterActivitiesSignutreVM);
                #endregion Signature Get And Viewed
            }
            else
            {
                return RedirectToAction(nameof(Index), new { selectedYear = selectedYear });
            }
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintAdminstrativeDetails(int? selectedYear, int? quarter, int page = 1, int pageSize = 50)
        {
            if (selectedYear == null || quarter == null)
            {
                //return View(new List<MonthsOfYearsAnnualyWithDetailsVM>());
                return RedirectToAction(nameof(Index), new { selectedYear = selectedYear });
            }
            if (selectedYear.HasValue && selectedYear.Value != 0 && quarter.HasValue && quarter.Value != 0)
            {
                var lang = SessionHelper.GetCurrentLanguage();
                var allMonthsofYearsAnnualy = await _service.GetAllMonthsofYearsAnnualy_Data(selectedYear);

                // Year Dropdown
                var years = await _service.GetAllYearsInDb();

                ViewBag.Years = years.ToList();
                ViewBag.selectedYear = selectedYear;

                var listQuarter = new List<List<int>> {
                    new List<int>{ 1,2,3 },
                    new List<int>{4,5,6},
                    new List<int>{7,8,9},
                    new List<int>{10,11,12},
                }.ToList();

                if (quarter != null)
                {
                    if (quarter == 5)
                    {
                        // Tow Administrative And Activities  For-All-ThisYear
                        allMonthsofYearsAnnualy = allMonthsofYearsAnnualy.OrderBy(x => x.Date).ToList();
                    }
                    if (quarter > 0 && quarter < 5)
                    {
                        // For Handeling Administrative Only
                        int AdministrativeType = (int)MonthlyAdministrativeReportType.Administrative;
                        allMonthsofYearsAnnualy = allMonthsofYearsAnnualy.Where(x => (x.Type.HasValue) && x.Type == AdministrativeType);

                        var listOfQuarter = listQuarter[quarter.Value - 1];
                        allMonthsofYearsAnnualy = allMonthsofYearsAnnualy.Where(c => c.month.HasValue && listOfQuarter.Any(x => x == c.month.Value)).OrderBy(x => x.Date).ToList();
                    }
                }

                var model = _mapper.Map<List<MonthsOfYearsAnnualyWithDetailsVM>>(allMonthsofYearsAnnualy);

                ViewBag.SelectedYear = selectedYear;
                ViewBag.quarter = quarter;
                if (model == null || model.Count == 0)
                {
                    return RedirectToAction(nameof(Index), new { selectedYear = selectedYear });
                }
                #region Signature Get And Viewed

                var quarterAdminstrativeSignutreVM_null = new QuartersReportAdminstrativeDetailsVM()
                {
                    Quarter = quarter != null ? (QuartersYear)quarter : null,
                    Type = QuarterlyReportType.Administrative,
                    Year = selectedYear
                };
                quarterAdminstrativeSignutreVM_null.AdminstrativeDetailsVM_List = model;

                if (selectedYear == null || quarter == null)
                    return View(quarterAdminstrativeSignutreVM_null);

                var AdminstrativeSignutre = await _service.GetQuarterSignature(selectedYear.Value, (QuartersYear)quarter.Value, QuarterlyReportType.Administrative);

                if (AdminstrativeSignutre == null)
                    return View(quarterAdminstrativeSignutreVM_null);


                var quarterAdminstrativeSignutreVM = _mapper.Map<QuartersReportAdminstrativeDetailsVM>(AdminstrativeSignutre);
                quarterAdminstrativeSignutreVM.AdminstrativeDetailsVM_List = model;

                // Fetch manager full name if signature exists
                if (quarterAdminstrativeSignutreVM.ManagerSignature != null && !string.IsNullOrEmpty(quarterAdminstrativeSignutreVM.ManagerSignature.UserId))
                {
                    var managerUser = await _userManager.FindByIdAsync(quarterAdminstrativeSignutreVM.ManagerSignature.UserId);
                    if (managerUser != null)
                    {
                        quarterAdminstrativeSignutreVM.ManagerFullName = !string.IsNullOrEmpty(managerUser.FullNameAr) ? managerUser.FullNameAr : managerUser.FullNameEn;
                    }
                }

                return View(quarterAdminstrativeSignutreVM);
                #endregion Signature Get And Viewed
            }
            else
            {
                //return View(new List<MonthsOfYearsAnnualyWithDetailsVM>());
                return RedirectToAction(nameof(Index), new { selectedYear = selectedYear });
            }
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintAll2Details(int? selectedYear, int? quarter, int page = 1, int pageSize = 50)
        {
            if (!selectedYear.HasValue || !quarter.HasValue || selectedYear == 0 || quarter == 0)
                return RedirectToAction(nameof(Index), new { selectedYear });

            var lang = SessionHelper.GetCurrentLanguage();

            // Delegate the heavy logic to the service
            var result = await _service.GetAdministrativeAndActivitiesDetailsAsync(selectedYear.Value, quarter.Value, lang);

            var VM = _mapper.Map<MonthsOfYearsAnnualyActivitiesAndAdministrative>(result);

            // Populate dropdowns
            ViewBag.Years = (await _service.GetAllYearsInDb()).ToList();
            ViewBag.selectedYear = selectedYear;
            ViewBag.quarter = quarter;

            // No data case
            if ((VM.model_Administrative == null || !VM.model_Administrative.Any()) &&
                (VM.model_Activities == null || !VM.model_Activities.Any()))
            {
                return RedirectToAction(nameof(Index), new { selectedYear });
            }

            #region Signature Get And Viewed

            var quarterCollaborativeVM_null = new QuartersReportCollaborativeDetailsVM()
            {
                Quarter = quarter != null ? (QuartersYear)quarter : null,
                Type = QuarterlyReportType.Collaborative,
                Year = selectedYear
            };
            quarterCollaborativeVM_null.MonthsOfYearsAnnualyActivitiesAndAdministrativeVM = VM;

            if (selectedYear == null || quarter == null)
                return View(quarterCollaborativeVM_null);

            var QuarterSignutre = await _service.GetQuarterSignature(selectedYear, (QuartersYear)quarter, QuarterlyReportType.Collaborative);

            if (QuarterSignutre == null)
                return View(quarterCollaborativeVM_null);

            var quarterCollaborativeVM = _mapper.Map<QuartersReportCollaborativeDetailsVM>(QuarterSignutre);
            quarterCollaborativeVM.MonthsOfYearsAnnualyActivitiesAndAdministrativeVM = VM;

            return View(quarterCollaborativeVM);
            #endregion Signature Get And Viewed
        }

        #region signature by otp
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
        public async Task<IActionResult> ValidateOtp([FromBody] OTPRequest request)
        {
            // request: { id, code, role }
            var result = await _service.ValidateOtpAsync(request.Type ?? 0, request.Quarter ?? 0, request.Year ?? 0, request.Code, request.Role, User);
            return Json(new { success = result.success, message = result.message });
        }
        #endregion
    }
}