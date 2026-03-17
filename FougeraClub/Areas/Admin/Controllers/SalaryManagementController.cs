using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;

using Domain.Entities.SalaryManage;
using Domain.Enums;
using Domain.Resources;
using KhaledTeamRecycling.Areas.Admin.ViewModels.Employees;
using KhaledTeamRecycling.Areas.Admin.ViewModels.SalaryManagement;
using KhaledTeamRecycling.Areas.Admin.ViewModels.SalaryManagement;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Helpers;
using KhaledTeamRecycling.Middelware;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Globalization;
using Domain.DTOs.Admin.SalaryManagement;
using KhaledTeamRecycling.Areas.Admin.ViewModels.SMS;

namespace KhaledTeamRecycling.Areas.Admin.Controllers
{
    //[AdminAuthorize]
    [Area("Admin")]
    public class SalaryManagementController : Controller
    {
        private readonly ISalaryManagementService _salaryManagementService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IHubContext<Hub.NotificationHub> _hubContext;
        private readonly INotificationService _notificationService;

        public SalaryManagementController(ISalaryManagementService salaryManagementService, IUnitOfWork unitOfWork, IMapper mapper, IHubContext<Hub.NotificationHub> hubContext, INotificationService notificationService)
        {
            _salaryManagementService = salaryManagementService;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _hubContext = hubContext;
            _notificationService = notificationService;

        }

  

        [YesGet]
        public async Task<IActionResult> Index(int? selectedEmployee, int? selectedYear, int? selectedMonth, int page = 1, int pageSize = 50)
        {
            var allSalaryManagement = await _salaryManagementService.GetAllAsync();
            var allYears = await _unitOfWork.SalaryManagements.Table.Select(x => x.Year).Distinct().Where(x => x != null).OrderByDescending(x => x).ToListAsync();
            ViewBag.allYears = allYears;
            ViewBag.selectedYear = selectedYear;
            var allEmployeesNames = await _salaryManagementService.GetAllEmplyeeNames();
            ViewBag.EmployeesNames = allEmployeesNames;
            ViewBag.selectedEmployee = selectedEmployee;
            var salaryManagementSingleVMs = _mapper.Map<List<SalaryManagementVM>>(allSalaryManagement).OrderByDescending(x=>x.EmployeeId).ThenByDescending(x => x.Year).ThenByDescending(x => x.Month).AsQueryable();

            if (selectedEmployee != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.EmployeeId== selectedEmployee)
                );
            }
            if (selectedYear != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.Year == selectedYear)
                );
            }
            if (selectedMonth != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.Month == selectedMonth)
                );
            }


            ViewBag.TotalCount = salaryManagementSingleVMs.Count();
            var paginated = PaginatedList<SalaryManagementVM>.Create(salaryManagementSingleVMs, page, pageSize, null);

            paginated.IsAbleToOpen = paginated.Items != null && paginated.Items.Any() ? true : false;

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);
        }

        public async Task<IActionResult> AddEdit(int? id)
        {
            var vm = new SalaryManagementVM();
            var allEmployeesNames = await _salaryManagementService.GetAllEmplyeeNames();
            ViewBag.EmployeesNames = allEmployeesNames;
            var lang = SessionHelper.GetCurrentLanguage();
            ViewBag.EmployeesNames = allEmployeesNames;

            if (id == null || id == 0)
                return View(vm);

            var salaryManagementSingle = await _salaryManagementService.GetByIdAsync(id.Value);
            if (salaryManagementSingle == null) return NotFound();

            vm = _mapper.Map<SalaryManagementVM>(salaryManagementSingle);
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> AddEdit(SalaryManagementVM model)
        {
            var allEmployeesNames = await _salaryManagementService.GetAllEmplyeeNames();
            ViewBag.EmployeesNames = allEmployeesNames;
            if (!ModelState.IsValid)
                return View(model);

            // Server-side calculation of totals/deductions to ensure data integrity
            decimal basic = model.BasicSalary ?? 0m;
            decimal allowances = model.Allowances ?? 0m;
            int workDays = model.WorkDays ?? 0;
            int absentDays = model.AbsentDays ?? 0;
            int sickLeaveDays = model.SickLeaveDays ?? 0;
            int annualLeaveDays = model.AnnualLeaveDays ?? 0;
            decimal deductionsAddition = model.DeductionsAddition ?? 0m;
            decimal bonuses = model.BonusesAndMissions ?? 0m;

            // Total salary = basic + allowances
            decimal totalSalary = basic + allowances;

            // Absence deduction: total salary / 30 * absentDays
            decimal absenceDeduction = 0m;
            if (absentDays > 0)
            {
                absenceDeduction = (totalSalary / 30m) * absentDays;
            }

            // Annual vacation deduction: use constant divisor 30 (per requirement)
            decimal annualVacationDeduction = 0m;
            if (annualLeaveDays > 0)
            {
                decimal perDayAllowance = allowances / 30m;
                annualVacationDeduction = perDayAllowance * 0.5m * annualLeaveDays;
            }

            // If client submitted ReducedWorkDays use it; otherwise compute fallback.
            if (model.ReducedWorkDays.HasValue)
            {
                model.WorkDays = model.ReducedWorkDays.Value;
            }
            else
            {
                int reducedWorkDaysComputed = workDays - absentDays - sickLeaveDays - annualLeaveDays;
                if (reducedWorkDaysComputed < 0) reducedWorkDaysComputed = 0;
                model.WorkDays = reducedWorkDaysComputed;
            }

            decimal totalDeductions = absenceDeduction + annualVacationDeduction + deductionsAddition;
            decimal netSalary = totalSalary - totalDeductions + bonuses;

            // Assign computed values back to the model to be persisted
            model.TotalSalary = Math.Round(totalSalary, 2);
            model.TotalDeductions = Math.Round(totalDeductions, 2);
            model.NetSalary = Math.Round(netSalary, 2);

            var entity = _mapper.Map<SalaryManagement>(model);

            if (model.Id == 0)
            {
                var CheckthisYearMonth = await _unitOfWork.SalaryManagements.GetAllAsync(x => x.Month == model.Month && x.Year == model.Year);
                if (CheckthisYearMonth.Count()==0)
                {
                    await _hubContext.Clients.Groups("Accountant")
                        .SendAsync("ReceiveNotification", new
                        {
                            Title = "",
                            Message = ""
                        });
                    await _notificationService.SendNotificationToRoleAsync(
                          "تقرير للرواتب والاجور جديد",
                          $"يوجد تقرير للرواتب والاجور تاريخ {model?.Month + " - " + model?.Year} جديد جاهز للإعتماد",
                          (int)RoleNumber.Accountant
                      );

                    await _hubContext.Clients.Groups("Accountant")
                      .SendAsync("ReceiveNotification", new
                      {
                          Title = "",
                          Message = ""
                      });
                    await _notificationService.SendNotificationToRoleAsync(
                          "تقرير للخصومات والعلاوات جديد",
                          $"يوجد تقرير للخصومات والعلاوات تاريخ {model?.Month + " - " + model?.Year} جديد جاهز للإعتماد",
                          (int)RoleNumber.Accountant
                      );
                }
                model.Id = await _salaryManagementService.AddAsync(entity);
                return RedirectToAction(nameof(Index)); // After Add New
            }
            else
            {
                await _salaryManagementService.UpdateAsync(entity);
            }

            return RedirectToAction(nameof(AddEdit), new { model.Id }); // After Edit
        }


        [HttpGet]
        [IgnoreAction]
        public IActionResult GetAvailableMonths(int employeeId, int year)
        {
            // Example: retrieve months not already assigned for that employee/year
            var existingMonths = _unitOfWork.SalaryManagements.Table
                .Where(s => s.EmployeeId == employeeId && s.Year == year)
                .Select(s => s.Month)
                .ToList();

            // Build available months list
            var months = Enumerable.Range(1, 12)
                .Where(m => !existingMonths.Contains(m))
                .Select(m => new
                {
                    value = m,
                    text = System.Globalization.CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(m)
                })
                .ToList();

            return Json(months);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _salaryManagementService.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        [YesGet]
        public async Task<IActionResult> OpenDetails_payrollReport(int? selectedEmployee, int? selectedYear, int? selectedMonth, int page = 1, int pageSize = 50)
        {
            var allSalaryManagement = await _salaryManagementService.GetAllAsync();
            var allYears = await _unitOfWork.SalaryManagements.Table.Select(x => x.Year).Distinct().Where(x => x != null).OrderByDescending(x => x).ToListAsync();
            ViewBag.allYears = allYears;
            ViewBag.selectedYear = selectedYear;
            ViewBag.selectedMonth = selectedMonth;
            var allEmployeesNames = await _salaryManagementService.GetAllEmplyeeNames();
            ViewBag.EmployeesNames = allEmployeesNames;

            var salaryManagementSingleVMs = _mapper.Map<List<SalaryManagementVM>>(allSalaryManagement).OrderByDescending(x => x.EmployeeId).ThenByDescending(x => x.Year).ThenByDescending(x => x.Month).AsQueryable();

            if (selectedEmployee != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.EmployeeId == selectedEmployee)
                );
            }
            if (selectedYear != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.Year == selectedYear)
                );
            }
            if (selectedMonth != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.Month == selectedMonth)
                );
            }
            var lang = SessionHelper.GetCurrentLanguage();

            ViewBag.PrintInnerTitle = Resource1.AlFujairScientificClubSalariesRevealedfor + " " + (selectedMonth != null ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(selectedMonth.Value) : "") + " " + (selectedYear != null ? selectedYear.ToString() : "") + " " + (selectedEmployee != null ? (lang == "ar" ? allEmployeesNames?.Where(y => y.Id == selectedEmployee)?.FirstOrDefault()?.FullNameAr : allEmployeesNames?.Where(y => y.Id == selectedEmployee)?.FirstOrDefault()?.FullNameEn) : "");
            ViewBag.TotalCount = salaryManagementSingleVMs.Count();

            if (salaryManagementSingleVMs.Count() == 0)
            {
                return RedirectToAction("Index",new { selectedYear = selectedYear , selectedMonth = selectedMonth});
            }

            var ExistSigns = await _unitOfWork.SalaryReportSigns.GetAllAsync(x=> x.ReportSalaryTypeId == (int)ReportSalaryTypeEnum.SalaryReport &&(x.Year!=null &&  x.Year == selectedYear && x.Month != null && x.Month == selectedMonth), S => S.AcountantSignature, S => S.ManagerSignature);
            var singleSign = ExistSigns.FirstOrDefault();
            var SalaryReportVM = new SalaryReportVM
            {
                salaryManagementVMs = salaryManagementSingleVMs,
                Id = singleSign?.Id,
                ManagerSignature = singleSign?.ManagerSignature,
                ManagerSignitureId = singleSign?.ManagerSignitureId,
                AcountantSignature = singleSign?.AcountantSignature,
                AcountantSignatureId = singleSign?.AcountantSignatureId,
                month = selectedMonth,
                year = selectedYear,
            };

            return View(SalaryReportVM);
        }

   
        [YesGet]
        public async Task<IActionResult> OpenDetails_DiscountsAndBonusesReport(int? selectedEmployee, int? selectedYear, int? selectedMonth, int page = 1, int pageSize = 50)
        {
            var allSalaryManagement = await _salaryManagementService.GetAllAsync();
            var allYears = await _unitOfWork.SalaryManagements.Table.Select(x => x.Year).Distinct().Where(x => x != null).OrderByDescending(x => x).ToListAsync();
            ViewBag.allYears = allYears;
            ViewBag.selectedYear = selectedYear;
            var allEmployeesNames = await _salaryManagementService.GetAllEmplyeeNames();
            ViewBag.EmployeesNames = allEmployeesNames;

            var salaryManagementSingleVMs = _mapper.Map<List<SalaryManagementVM>>(allSalaryManagement).OrderByDescending(x => x.EmployeeId).ThenByDescending(x => x.Year).ThenByDescending(x => x.Month).AsQueryable();

            if (selectedEmployee != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.EmployeeId == selectedEmployee)
                );
            }
            if (selectedYear != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.Year == selectedYear)
                );
            }
            if (selectedMonth != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.Month == selectedMonth)
                );
            }
            var lang = SessionHelper.GetCurrentLanguage();

            ViewBag.PrintInnerTitle = Resource1.AlFujairScientificClubSalariesRevealedfor + " " + (selectedMonth != null ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(selectedMonth.Value) : "") + " " + (selectedYear != null ? selectedYear.ToString() : "") + " " + (selectedEmployee != null ? (lang == "ar" ? allEmployeesNames?.Where(y => y.Id == selectedEmployee)?.FirstOrDefault()?.FullNameAr : allEmployeesNames?.Where(y => y.Id == selectedEmployee)?.FirstOrDefault()?.FullNameEn) : "");
            ViewBag.TotalCount = salaryManagementSingleVMs.Count();

            if (salaryManagementSingleVMs.Count() == 0)
            {
                return RedirectToAction("Index", new { selectedYear = selectedYear, selectedMonth = selectedMonth });
            }

            var ExistSigns = await _unitOfWork.SalaryReportSigns.GetAllAsync(x => x.ReportSalaryTypeId == (int)ReportSalaryTypeEnum.DiscountsAndBonusesReport && (x.Year != null && x.Year == selectedYear && x.Month != null && x.Month == selectedMonth), S => S.AcountantSignature, S => S.ManagerSignature);
            var singleSign = ExistSigns.FirstOrDefault();
            var SalaryReportVM = new SalaryReportVM
            {
                salaryManagementVMs = salaryManagementSingleVMs,
                Id = singleSign?.Id,
                ManagerSignature = singleSign?.ManagerSignature,
                ManagerSignitureId = singleSign?.ManagerSignitureId,
                AcountantSignature = singleSign?.AcountantSignature,
                AcountantSignatureId = singleSign?.AcountantSignatureId,
                month = selectedMonth,
                year = selectedYear,
            };

            return View(SalaryReportVM);
        }

        //[IgnoreAction]
        //[HttpPost]
        //public async Task<IActionResult> ValidateOtp_OpenDetails_DiscountsAndBonusesReport([FromBody] OtpValidationRequest request)
        //{
        //    // request: { year, month, code, role }
        //    var result = await _salaryManagementService.ValidateOtp_OpenDetails_DiscountsAndBonusesReportAsync(request.Year.Value, request.Month.Value, request.Code, request.Role, User);
        //    var report = await _unitOfWork.SalaryReportSigns.GetByColumnAsync(
        //        e => e.Year == request.Year && e.Month == request.Month && e.ReportSalaryTypeId == (int)ReportSalaryTypeEnum.DiscountsAndBonusesReport);
        //    //var report = await _expenseService.get
        //    if (result.success == true)
        //    {
        //        if (request.Role == "Acountant")
        //        {
        //            await _hubContext.Clients.Groups("Manager")
        //                .SendAsync("ReceiveNotification", new
        //                {
        //                    Title = "",
        //                    Message = ""
        //                });
        //            await _notificationService.SendNotificationToRoleAsync(
        //                  "تقرير للخصومات والعلاوات جديد",
        //                  $"يوجد تقرير للخصومات والعلاوات تاريخ {report?.Month + " - " + report?.Year} جديد جاهز للإعتماد",
        //                  (int)RoleNumber.Manager
        //              );
        //        }
        //    }
        //    return Json(new { success = result.success, message = result.message });
        //}

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> createExcelReport_Download_payrollReport(int? selectedEmployee, int? selectedYear, int? selectedMonth, int page = 1, int pageSize = 50)
        {
            var allSalaryManagement = await _salaryManagementService.GetAllAsync();
            var allYears = await _unitOfWork.SalaryManagements.Table.Select(x => x.Year).Distinct().Where(x => x != null).OrderByDescending(x => x).ToListAsync();
            ViewBag.allYears = allYears;
            ViewBag.selectedYear = selectedYear;
            var allEmployeesNames = await _salaryManagementService.GetAllEmplyeeNames();
            ViewBag.EmployeesNames = allEmployeesNames;
            var salaryManagementSingleVMs = _mapper.Map<List<SalaryManagementVM>>(allSalaryManagement).OrderByDescending(x => x.EmployeeId).ThenByDescending(x => x.Year).ThenByDescending(x => x.Month).AsQueryable();


            if (selectedEmployee != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.EmployeeId == selectedEmployee)
                );
            }
            if (selectedYear != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.Year == selectedYear)
                );
            }
            if (selectedMonth != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.Month == selectedMonth)
                );
            }
            var lang = SessionHelper.GetCurrentLanguage();

            ViewBag.PrintInnerTitle = Resource1.AlFujairScientificClubSalariesRevealedfor + " " + (selectedMonth != null ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(selectedMonth.Value) : "") + " " + (selectedYear != null ? selectedYear.ToString() : "") + " " + (selectedEmployee != null ? (lang == "ar" ? allEmployeesNames?.Where(y => y.Id == selectedEmployee)?.FirstOrDefault()?.FullNameAr : allEmployeesNames?.Where(y => y.Id == selectedEmployee)?.FirstOrDefault()?.FullNameEn) : "");
            ViewBag.TotalCount = salaryManagementSingleVMs.Count();
            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                // Only include records where TotalDeductions or BonusesAndMissions are non-zero
                var allData_list = salaryManagementSingleVMs.Where(s => (s.TotalDeductions.HasValue && s.TotalDeductions.Value != 0m) 
                                                                      || (s.BonusesAndMissions.HasValue && s.BonusesAndMissions.Value != 0m));
                var ListTitles = new List<string>
        {
            Resource1.Name,Resource1.JobTitle1,Resource1.BasicSalary,
            Resource1.Allowances,Resource1.TotalSalary,Resource1.WorkDays,Resource1.AbsentDays,Resource1.SickLeaveDays,
            Resource1.AnnualLeaveDays,Resource1.TotalDeductions,Resource1.BonusesAndMissions,Resource1.NetSalary
        };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = (allEmployeesNames != null && allEmployeesNames.Where(y => y.Id == single.EmployeeId).Count() > 0) ?((lang == "ar" ? allEmployeesNames.Where(y => y.Id == single.EmployeeId).FirstOrDefault().FullNameAr : allEmployeesNames.Where(y => y.Id == single.EmployeeId).FirstOrDefault().FullNameEn)):"",
                        t2 = single.JobTitle,
                        t3 = (single.BasicSalary == 0 || single.BasicSalary == null) ? 0 : single.BasicSalary,
                        t4 = (single.Allowances == 0 || single.Allowances == null) ? 0 : single.Allowances,
                        t5 = (single.TotalSalary == 0 || single.TotalSalary == null) ? 0 : single.TotalSalary,
                        t6 = (single.WorkDays == 0 || single.WorkDays == null) ? 0 : single.WorkDays,
                        t7 = (single.AbsentDays == 0 || single.AbsentDays == null) ? 0 : single.AbsentDays,
                        t8 = (single.SickLeaveDays == 0 || single.SickLeaveDays == null) ? 0 : single.SickLeaveDays,
                        t9 = (single.AnnualLeaveDays == 0 || single.AnnualLeaveDays == null) ? 0 : single.AnnualLeaveDays,
                        t10 = (single.TotalDeductions == 0 || single.TotalDeductions == null) ? 0 : single.TotalDeductions,
                        t11 = (single.BonusesAndMissions == 0 || single.BonusesAndMissions == null) ? 0 : single.BonusesAndMissions,
                        t12 = (single.NetSalary == 0 || single.NetSalary == null) ? 0 : single.NetSalary,
                    }).ToList();

                    var strTitle = $"{ViewBag.PrintInnerTitle} ";

                    if (excelDataDTO.Count>0)
                    {
                        var BasicSalary = allData_list.Sum(y => y.BasicSalary ?? 0m);
                        var Allowances = allData_list.Sum(y => y.Allowances ?? 0m);
                        var TotalSalary = allData_list.Sum(y => y.TotalSalary ?? 0m);
                        var WorkDays = allData_list.Sum(y => y.WorkDays);
                        var AbsentDays = allData_list.Sum(y => y.AbsentDays ?? 0);
                        var SickLeaveDays = allData_list.Sum(y => y.SickLeaveDays ?? 0);
                        var AnnualLeaveDays = allData_list.Sum(y => y.AnnualLeaveDays ?? 0);
                        var TotalDeductions = allData_list.Sum(y => y.TotalDeductions ?? 0m);
                        var BonusesAndMissions = allData_list.Sum(y => y.BonusesAndMissions ?? 0m);
                        var NetSalary = allData_list.Sum(y => y.NetSalary ?? 0m);

                        var lastRow = new ExcelDataDTO
                        {
                            t1 = Resource2.Total,
                            t2 = "-",
                            t3 = (BasicSalary == 0 || BasicSalary == null) ? 0 : BasicSalary,
                            t4 = (Allowances == 0 || Allowances == null) ? 0 : Allowances,
                            t5 = (TotalSalary == 0 || TotalSalary == null) ? 0 : TotalSalary,
                            t6 = (WorkDays == 0 || WorkDays == null) ? 0 : WorkDays,
                            t7 = (AbsentDays == 0 || AbsentDays == null) ? 0 : AbsentDays,
                            t8 = (SickLeaveDays == 0 || SickLeaveDays == null) ? 0 : SickLeaveDays,
                            t9 = (AnnualLeaveDays == 0 || AnnualLeaveDays == null) ? 0 : AnnualLeaveDays,
                            t10 = (TotalDeductions == 0 || TotalDeductions == null) ? 0 : TotalDeductions,
                            t11 = (BonusesAndMissions == 0 || BonusesAndMissions == null) ? 0 : BonusesAndMissions,
                            t12 = (NetSalary == 0 || NetSalary == null) ? 0 : NetSalary,
                        };
                        excelDataDTO.Add(new ExcelDataDTO());
                        excelDataDTO.Add(lastRow);
                    }
                    

                    if (lang == "ar")
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "ar", strTitle);
                    }
                    else
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "en", strTitle);
                    }
                }

                FileContentResult? Excelfile = null;
                if (fileBytes != null && fileBytes.Length > 0 && boolStatus == true)
                {
                    var fileExcelName = ViewBag.PrintInnerTitle ?? "Excel";
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
        public async Task<IActionResult> createExcelReport_Download_DiscountsAndBonusesReport(int? selectedEmployee, int? selectedYear, int? selectedMonth, int page = 1, int pageSize = 50)
        {
            var allSalaryManagement = await _salaryManagementService.GetAllAsync();
            var allYears = await _unitOfWork.SalaryManagements.Table.Select(x => x.Year).Distinct().Where(x => x != null).OrderByDescending(x => x).ToListAsync();
            ViewBag.allYears = allYears;
            ViewBag.selectedYear = selectedYear;
            var allEmployeesNames = await _salaryManagementService.GetAllEmplyeeNames();
            ViewBag.EmployeesNames = allEmployeesNames;
            var salaryManagementSingleVMs = _mapper.Map<List<SalaryManagementVM>>(allSalaryManagement).OrderByDescending(x => x.EmployeeId).ThenByDescending(x => x.Year).ThenByDescending(x => x.Month).AsQueryable();

            if (selectedEmployee != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.EmployeeId == selectedEmployee)
                );
            }
            if (selectedYear != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.Year == selectedYear)
                );
            }
            if (selectedMonth != null)
            {
                salaryManagementSingleVMs = salaryManagementSingleVMs.Where(s =>
                    (s.Month == selectedMonth)
                );
            }
            var lang = SessionHelper.GetCurrentLanguage();
            ViewBag.PrintInnerTitle = Resource1.ReportDiscountsAndBonusesFor + " " + (selectedMonth != null ? CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(selectedMonth.Value) : "") + " " + (selectedYear != null ? selectedYear.ToString() : "") + " " + (selectedEmployee != null ? (lang == "ar" ? allEmployeesNames?.Where(y => y.Id == selectedEmployee)?.FirstOrDefault()?.FullNameAr : allEmployeesNames?.Where(y => y.Id == selectedEmployee)?.FirstOrDefault()?.FullNameEn) : "");

            ViewBag.TotalCount = salaryManagementSingleVMs.Count();
            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                var allData_list = salaryManagementSingleVMs;
                var ListTitles = new List<string>
        {
            Resource1.Name,Resource1.JobTitle1,Resource1.BasicSalary,
            Resource1.Allowances,Resource1.TotalDeductions,Resource1.BonusesAndMissions,Resource1.NetSalary
        };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = (allEmployeesNames != null && allEmployeesNames.Where(y => y.Id == single.EmployeeId).Count() > 0) ? ((lang == "ar" ? allEmployeesNames.Where(y => y.Id == single.EmployeeId).FirstOrDefault().FullNameAr : allEmployeesNames.Where(y => y.Id == single.EmployeeId).FirstOrDefault().FullNameEn)) : "",
                        t2 = single.JobTitle,
                        t3 = (single.BasicSalary==0|| single.BasicSalary == null) ?0: single.BasicSalary,
                        t4 = (single.Allowances == 0 || single.Allowances == null) ? 0 : single.Allowances,
                        t5 = (single.TotalDeductions == 0 || single.TotalDeductions == null) ? 0 : single.TotalDeductions,
                        t6 = (single.BonusesAndMissions == 0 || single.BonusesAndMissions == null) ? 0 : single.BonusesAndMissions,
                        t7 = (single.NetSalary == 0 || single.NetSalary == null) ? 0 : single.NetSalary,
                    }).ToList();

                    var strTitle = $"{ViewBag.PrintInnerTitle} ";

                    if (excelDataDTO.Count > 0)
                    {
                        var BasicSalary = allData_list.Sum(y => y.BasicSalary ?? 0m);
                        var Allowances = allData_list.Sum(y => y.Allowances ?? 0m);
                        var TotalDeductions = allData_list.Sum(y => y.TotalDeductions ?? 0m);
                        var BonusesAndMissions = allData_list.Sum(y => y.BonusesAndMissions ?? 0m);
                        var NetSalary = allData_list.Sum(y => y.NetSalary ?? 0m);

                        var lastRow = new ExcelDataDTO
                        {
                            t1 = Resource2.Total,
                            t2 = "-",
                            t3 = (BasicSalary == 0 || BasicSalary == null) ? 0 : BasicSalary,
                            t4 = (Allowances == 0 || Allowances == null) ? 0 : Allowances,
                            t5 = (TotalDeductions == 0 || TotalDeductions == null) ? 0 : TotalDeductions,
                            t6 = (BonusesAndMissions == 0 || BonusesAndMissions == null) ? 0 : BonusesAndMissions,
                            t7 = (NetSalary == 0 || NetSalary == null) ? 0 : NetSalary,
                        };
                        excelDataDTO.Add(new ExcelDataDTO());
                        excelDataDTO.Add(lastRow);
                    }

                    if (lang == "ar")
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "ar", strTitle);
                    }
                    else
                    {
                        (boolStatus, fileBytes) = ExcelStaticReport.ExcelReportArEn_(excelDataDTO, ListTitles, 0, "en", strTitle);
                    }
                }

                FileContentResult? Excelfile = null;
                if (fileBytes != null && fileBytes.Length > 0 && boolStatus == true)
                {
                    var fileExcelName = ViewBag.PrintInnerTitle ?? "Excel";
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
        #region Absences
        [YesGet]
        public async Task<IActionResult> Absences(int? empId, int? fromYear, int? toYear, int? fromMonth, int? toMonth, int page = 1, int pageSize = 50)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            IEnumerable<AbsenceDTO> allAbsences = [];


            if (empId.HasValue)
                allAbsences = await _salaryManagementService.GetAllAbsencesAsync();

            var absencesVMs = _mapper.Map<List<AbsenceVM>>(allAbsences).OrderByDescending(x => x.EmpId).ThenByDescending(x => x.Year).ThenByDescending(x => x.Month).AsQueryable();

            if (empId.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.EmpId == empId);
            }
            if (fromYear.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.Year >= fromYear);
            }

            if (toYear.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.Year <= toYear);
            }

            if (fromMonth.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.Month >= fromMonth);
            }

            if (toMonth.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.Month <= toMonth);
            }

            var paginated = PaginatedList<AbsenceVM>.Create(absencesVMs, page, pageSize, "");


            var allEmployees = await _unitOfWork.Employees.GetAllAsync();
            var years = await _salaryManagementService.GetAllYearsInDb();
            var months = Enumerable.Range(1, 12).Select(m => new SelectListItem
            {
                Value = m.ToString(),
                Text = new DateTime(1, m, 1).ToString("MMMM")
            }).ToList();

            ViewBag.TotalAbsencesDays = absencesVMs.Sum(a => a.NoOfDays) ?? 0;
            ViewBag.Employees = SelectListHelper.BindSelectList(allEmployees.ToList(), empId, "Id", "FullNameAr", "FullNameEn");
            ViewBag.Years = years.ToList();
            ViewBag.Months = months;

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_AbsencesListPartial", paginated);
            }

            return View(paginated);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintAbsences(int? empId, int? fromYear, int? toYear, int? fromMonth, int? toMonth)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var allAbsences = await _salaryManagementService.GetAllAbsencesAsync();

            var absencesVMs = _mapper.Map<List<AbsenceVM>>(allAbsences).OrderByDescending(x => x.EmpId).ThenByDescending(x => x.Year).ThenByDescending(x => x.Month).AsQueryable();

            if (empId.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.EmpId == empId);
            }
            if (fromYear.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.Year >= fromYear);
            }

            if (toYear.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.Year <= toYear);
            }

            if (fromMonth.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.Month >= fromMonth);
            }

            if (toMonth.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.Month <= toMonth);
            }
            ViewBag.TotalAbsencesDays = absencesVMs.Sum(a => a.NoOfDays) ?? 0;

            return View(absencesVMs);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> DownloadAbsencesExcel(int? empId, int? fromYear, int? toYear, int? fromMonth, int? toMonth)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            var allAbsences = await _salaryManagementService.GetAllAbsencesAsync();

            var absencesVMs = _mapper.Map<List<AbsenceVM>>(allAbsences).OrderByDescending(x => x.EmpId).ThenByDescending(x => x.Year).ThenByDescending(x => x.Month).AsQueryable();

            if (empId.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.EmpId == empId);
            }
            if (fromYear.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.Year >= fromYear);
            }

            if (toYear.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.Year <= toYear);
            }

            if (fromMonth.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.Month >= fromMonth);
            }

            if (toMonth.HasValue)
            {
                absencesVMs = absencesVMs.Where(c => c.Month <= toMonth);
            }

            ViewBag.TotalAbsencesDays = absencesVMs.Sum(a => a.NoOfDays??0);
            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                var allData_list = absencesVMs;
                var ListTitles = new List<string>
                {
                    Resource1.Name,Resource1.Job,@Resource2.Month , @Resource2.Year,Resource2.NoOfAbsences,
                };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = lang == "ar" ? single.FullNameAr : single.FullNameEn,
                        t2 = single.JobTitle,
                        t3 = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(single.Month ?? 0),
                        t4 = single.Year,
                        t5 = single.NoOfDays??0,
                    }).ToList();

                    if (excelDataDTO.Count>0)
                    {
                        excelDataDTO.Add(new ExcelDataDTO
                        {
                            t1 = Resource2.TotalNumberOfAbsences,
                            t2 = "-",
                            t3 = "-",
                            t4 = "-",
                            t5 = absencesVMs.Sum(a => a.NoOfDays ?? 0),
                        });
                    }

                    
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
                    var fileExcelName = Resource2.AbsenceList;
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
        [HttpGet]
        public async Task<IActionResult> GetEmployeeJobAndSalary(int employeeId)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            if (employeeId <= 0)
                return BadRequest("Invalid employee ID");

            // ابحث عن الموظف من قاعدة البيانات
            var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);

            if (employee == null)
                return NotFound();


            // أرسل البيانات المطلوبة فقط
            return Json(new
            {
                JobId = "",
                JobTitle = employee.JobTitle,
                BasicSalary = employee.Salary,
            });
        }
        #endregion

        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> GetSalaryManagementAttachments(int id)
        {
            var attachmentsDTO = await _salaryManagementService.GetAttachmentsAsync(id);
            var model = _mapper.Map<SalaryManagementAttachmentsVM>(attachmentsDTO);
            return PartialView("_SalaryManagementAttachmentsModal", model);
        }

        [IgnoreAction]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UploadSalaryManagementFiles(SalaryManagementAttachmentsVM model)
        {
            if (model == null)
                return Json(new { success = false, message = "Files uploaded not done" });
            var dto = _mapper.Map<SalaryManagementAttachmentsDTO>(model);
            await _salaryManagementService.UploadAttachmentsAsync(dto);

            return RedirectToAction("Index");
        }
    }
}