using Application.Helpers;
using Application.Services.Admin;
using Domain.DTOs;
using Domain.Entities;
using Domain.Resources;
using Humanizer;
using Infrastructure.Repositories.InterfacesDB;
using KhaledTeamRecycling.Areas.Admin.ViewModels.LogHistory;
using KhaledTeamRecycling.Attributes;
using KhaledTeamRecycling.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class LogHistoryController : Controller
    {
        [IgnoreAction]
        public async Task<IActionResult> Timeline(string? selectedUserId, string? selectedMemberEmail, DateTime? dateFrom, DateTime? dateTo, int page = 1, int pageSize = 40)
        {
            var lang = SessionHelper.GetCurrentLanguage();
            

            var allUsers = await _unitOfWork.Users.Table.Select(u => new
            {
                Id = u.Id,
                FullNameAr = u.FullNameAr,
                FullNameEn = u.FullNameEn
            }).ToListAsync();

            var Logs_query = from log in _unitOfWork.RequestLogs.Table
                             join user in _unitOfWork.Users.Table
                             on log.UserId equals user.Id into userGroup
                             from user in userGroup.DefaultIfEmpty() // Left Join
                            
                             select new LogsVM
                             {
                                 Id = log.Id,
                                 UserId = log.UserId,
                                 Path = log.Path,
                                 Method = log.Method,
                                 Controller = log.Controller,
                                 Action = log.Action,
                                 NameAr = log.NameAr,
                                 NameEn = log.NameEn,
                                 LogTarget = log.LogTarget,
                                 RequestTime = log.RequestTime,
                                 // Prefer application user name; fallback to member name when user is null (no "member -" prefix)
                                 UserFullName = lang == "ar"
                                 ? (user != null ? user.FullNameAr : "")
                                 : (user != null ? user.FullNameEn : ""),
                                 UserFullNameAr = user != null ? user.FullNameAr :  null,
                                 UserFullNameEn = user != null ? user.FullNameEn : null,
                             };

            // Exclude any logs that contain the word "negotiate" (case-insensitive) in key fields
            Logs_query = Logs_query.Where(c => !(
                (!string.IsNullOrEmpty(c.NameAr) && c.NameAr.ToLower().Contains("negotiate")) ||
                (!string.IsNullOrEmpty(c.NameEn) && c.NameEn.ToLower().Contains("negotiate")) ||
                (!string.IsNullOrEmpty(c.LogTarget) && c.LogTarget.ToLower().Contains("negotiate")) ||
                (!string.IsNullOrEmpty(c.Action) && c.Action.ToLower().Contains("negotiate")) ||
                (!string.IsNullOrEmpty(c.Controller) && c.Controller.ToLower().Contains("hubs.notifications")) ||
                (!string.IsNullOrEmpty(c.Path) && c.Path.ToLower().Contains("negotiate"))
            ));

            // Apply selection filters: selected user id and/or member email (same as Index)
            if (!string.IsNullOrWhiteSpace(selectedUserId) || !string.IsNullOrWhiteSpace(selectedMemberEmail))
            {
                Logs_query = Logs_query.Where(c =>
                    (!string.IsNullOrWhiteSpace(selectedUserId) && c.UserId == selectedUserId) ||
                    (!string.IsNullOrWhiteSpace(selectedMemberEmail) && c.UserId == selectedMemberEmail)
                );
            }

            if (dateFrom.HasValue)
            {
                Logs_query = Logs_query.Where(c => c.RequestTime >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                Logs_query = Logs_query.Where(c => c.RequestTime <= dateTo.Value.AddDays(1).AtMidnight());
            }
            ViewBag.CountRecords = Logs_query?.Count();
            if (Logs_query != null && Logs_query.Any())
            {
                Logs_query = Logs_query.OrderByDescending(l => l.Id);
            }

            var paginated = PaginatedList<LogsVM>.Create(Logs_query, page, pageSize, null);
            ViewBag.AllUsersList = SelectListHelper.BindSelectList(allUsers, null, "Id", "FullNameAr", "FullNameEn").Distinct();
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            return View("Timeline", paginated);
        }

        private readonly IUnitOfWork _unitOfWork;
        public LogHistoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index(string? searchTerm, string? selectedUserId, string? selectedMemberEmail, DateTime? dateFrom, DateTime? dateTo, int page = 1, int pageSize = 40)
        {

            var lang = SessionHelper.GetCurrentLanguage();


            var allUsers = await _unitOfWork.Users.Table.Select(u => new
            {
                Id = u.Id,
                FullNameAr = u.FullNameAr,
                FullNameEn = u.FullNameEn
            }).ToListAsync();

            var Logs_query = from log in _unitOfWork.RequestLogs.Table
                             join user in _unitOfWork.Users.Table
                             on log.UserId equals user.Id into userGroup
                             from user in userGroup.DefaultIfEmpty() // Left Join

                             select new LogsVM
                             {
                                 Id = log.Id,
                                 UserId = log.UserId,
                                 Path = log.Path,
                                 Method = log.Method,
                                 Controller = log.Controller,
                                 Action = log.Action,
                                 NameAr = log.NameAr,
                                 NameEn = log.NameEn,
                                 LogTarget = log.LogTarget,
                                 RequestTime = log.RequestTime,
                                 // Prefer application user name; fallback to member name when user is null (no "member -" prefix)
                                 UserFullName = lang == "ar"
                                 ? (user != null ? user.FullNameAr : "")
                                 : (user != null ? user.FullNameEn : ""),
                                 UserFullNameAr = user != null ? user.FullNameAr : null,
                                 UserFullNameEn = user != null ? user.FullNameEn : null,
                             };

            // Exclude any logs that contain the word "negotiate" (case-insensitive) in key fields
            Logs_query = Logs_query.Where(c => !(
                (!string.IsNullOrEmpty(c.NameAr) && c.NameAr.ToLower().Contains("negotiate")) ||
                (!string.IsNullOrEmpty(c.NameEn) && c.NameEn.ToLower().Contains("negotiate")) ||
                (!string.IsNullOrEmpty(c.LogTarget) && c.LogTarget.ToLower().Contains("negotiate")) ||
                (!string.IsNullOrEmpty(c.Action) && c.Action.ToLower().Contains("negotiate")) ||
                (!string.IsNullOrEmpty(c.Controller) && c.Controller.ToLower().Contains("hubs.notifications")) ||
                (!string.IsNullOrEmpty(c.Path) && c.Path.ToLower().Contains("negotiate"))
            ));

            // Apply selection filters: selected user id and/or member email
            if (!string.IsNullOrWhiteSpace(selectedUserId) || !string.IsNullOrWhiteSpace(selectedMemberEmail))
            {
                Logs_query = Logs_query.Where(c =>
                    (!string.IsNullOrWhiteSpace(selectedUserId) && c.UserId == selectedUserId) ||
                    (!string.IsNullOrWhiteSpace(selectedMemberEmail) && c.UserId == selectedMemberEmail)
                );
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                Logs_query = Logs_query.Where(c => c.RequestTime >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                Logs_query = Logs_query.Where(c => c.RequestTime <= dateTo.Value.AddDays(1).AtMidnight());
            }
            ViewBag.CountRecords = Logs_query?.Count();
            if (Logs_query != null && Logs_query.Any())
            {
                Logs_query = Logs_query.OrderByDescending(l => l.Id);
            }

            var paginated = PaginatedList<LogsVM>.Create(Logs_query, page, pageSize, searchTerm);


            ViewBag.AllUsersList = SelectListHelper.BindSelectList(allUsers, null, "Id", "FullNameAr", "FullNameEn").Distinct();

            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_ListPartial", paginated);
            }

            return View(paginated);

        }

        [IgnoreAction]
        public async Task<IActionResult> PrintLogHistory(string? searchTerm, string? selectedUserId, string? selectedMemberEmail, DateTime? dateFrom, DateTime? dateTo, int page = 1, int pageSize = 500)
        {

            var lang = SessionHelper.GetCurrentLanguage();

          
            var allUsers = await _unitOfWork.Users.Table.Select(u => new
            {
                Id = u.Id,
                FullNameAr = u.FullNameAr,
                FullNameEn = u.FullNameEn
            }).ToListAsync();

            ViewBag.AllUsersList = SelectListHelper.BindSelectList(allUsers, null, "Id", "FullNameAr", "FullNameEn").Distinct();


            var Logs_query = from log in _unitOfWork.RequestLogs.Table
                             join user in _unitOfWork.Users.Table
                             on log.UserId equals user.Id into userGroup
                             from user in userGroup.DefaultIfEmpty() // Left Join

                             select new LogsVM
                             {
                                 Id = log.Id,
                                 UserId = log.UserId,
                                 Path = log.Path,
                                 Method = log.Method,
                                 Controller = log.Controller,
                                 Action = log.Action,
                                 NameAr = log.NameAr,
                                 NameEn = log.NameEn,
                                 LogTarget = log.LogTarget,
                                 RequestTime = log.RequestTime,
                                 // Prefer application user name; fallback to member name when user is null (no "member -" prefix)
                                 UserFullName = lang == "ar"
                                 ? (user != null ? user.FullNameAr : "")
                                 : (user != null ? user.FullNameEn : ""),
                                 UserFullNameAr = user != null ? user.FullNameAr : null,
                                 UserFullNameEn = user != null ? user.FullNameEn : null,
                             };

            Logs_query = Logs_query.Where(c => !(
                (!string.IsNullOrEmpty(c.Action) && c.Action.ToLower().Contains("negotiate")) ||
                (!string.IsNullOrEmpty(c.Controller) && c.Controller.ToLower().Contains("hubs.notifications"))
             ));

            // Apply selection filters (user/member) if provided
            if (!string.IsNullOrWhiteSpace(selectedUserId) || !string.IsNullOrWhiteSpace(selectedMemberEmail))
            {
                Logs_query = Logs_query.Where(c =>
                    (!string.IsNullOrWhiteSpace(selectedUserId) && c.UserId == selectedUserId) ||
                    (!string.IsNullOrWhiteSpace(selectedMemberEmail) && c.UserId == selectedMemberEmail)
                );
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                Logs_query = Logs_query.Where(c => c.RequestTime >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                Logs_query = Logs_query.Where(c => c.RequestTime <= dateTo.Value.AddDays(1).AtMidnight());
            }
            ViewBag.CountRecords = Logs_query?.Count();
            if (Logs_query != null && Logs_query.Any())
            {
                Logs_query = Logs_query.OrderByDescending(l => l.Id);
            }


            ViewBag.searchTerm = searchTerm;
            ViewBag.dateFrom = dateFrom?.ToString("yyyy-MM-dd");
            ViewBag.dateTo = dateTo?.ToString("yyyy-MM-dd");

            return View(Logs_query);

        }

        [IgnoreAction]
        public async Task<IActionResult> createExcelReport_Download(string? searchTerm, string? selectedUserId, string? selectedMemberEmail, DateTime? dateFrom, DateTime? dateTo, int page = 1, int pageSize = 500)
        {

            var lang = SessionHelper.GetCurrentLanguage();



            var allUsers = await _unitOfWork.Users.Table.Select(u => new
            {
                Id = u.Id,
                FullNameAr = u.FullNameAr,
                FullNameEn = u.FullNameEn
            }).ToListAsync();
            ViewBag.AllUsersList = SelectListHelper.BindSelectList(allUsers, null, "Id", "FullNameAr", "FullNameEn").Distinct();



            var Logs_query = from log in _unitOfWork.RequestLogs.Table
                             join user in _unitOfWork.Users.Table
                             on log.UserId equals user.Id into userGroup
                             from user in userGroup.DefaultIfEmpty() // Left Join

                             select new LogsVM
                             {
                                 Id = log.Id,
                                 UserId = log.UserId,
                                 Path = log.Path,
                                 Method = log.Method,
                                 Controller = log.Controller,
                                 Action = log.Action,
                                 NameAr = log.NameAr,
                                 NameEn = log.NameEn,
                                 LogTarget = log.LogTarget,
                                 RequestTime = log.RequestTime,
                                 // Prefer application user name; fallback to member name when user is null (no "member -" prefix)
                                 UserFullName = lang == "ar"
                                 ? (user != null ? user.FullNameAr : "")
                                 : (user != null ? user.FullNameEn : ""),
                                 UserFullNameAr = user != null ? user.FullNameAr : null,
                                 UserFullNameEn = user != null ? user.FullNameEn : null,
                             };

            Logs_query = Logs_query.Where(c => !(
                (!string.IsNullOrEmpty(c.Action) && c.Action.ToLower().Contains("negotiate")) ||
                (!string.IsNullOrEmpty(c.Controller) && c.Controller.ToLower().Contains("hubs.notifications"))
             ));

            // Apply selection filters (user/member) if provided
            if (!string.IsNullOrWhiteSpace(selectedUserId) || !string.IsNullOrWhiteSpace(selectedMemberEmail))
            {
                Logs_query = Logs_query.Where(c =>
                    (!string.IsNullOrWhiteSpace(selectedUserId) && c.UserId == selectedUserId) ||
                    (!string.IsNullOrWhiteSpace(selectedMemberEmail) && c.UserId == selectedMemberEmail)
                );
            }

            // ✅ Date filtering — replace `c.YourDateProperty` with your actual property
            if (dateFrom.HasValue)
            {
                Logs_query = Logs_query.Where(c => c.RequestTime >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                Logs_query = Logs_query.Where(c => c.RequestTime <= dateTo.Value.AddDays(1).AtMidnight());
            }
            ViewBag.CountRecords = Logs_query?.Count();
            if (Logs_query != null && Logs_query.Any())
            {
                Logs_query = Logs_query.OrderByDescending(l => l.Id);
            }

            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {
                lang = SessionHelper.GetCurrentLanguage();
                var allData_list = Logs_query.ToList();
                var ListTitles = new List<string>
                {
                    "#", Resource2.Username, Resource2.Type, Resource1.Date, Resource1.Time, Resource2.Description,
                };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    //var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    //{
                    //    t1 = lang == "ar" ? single.SupplierNameAr : single.SupplierNameEn,
                    //    t2 = single.Email,
                    //    t3 = single.Mobile,
                    //    t4 = single.VATNumber,
                    //    t5 = single.LogTarget,
                    //}).ToList();
                    var excelDataDTO = allData_list.Select((log, index) =>
                    {
                        // Specify the user's full name
                        string userFullName = log.UserFullName;

                        if (!(userFullName?.Length >2 ))
                        {
                            userFullName = Resource1.NotFoundNow;
                        }

                        // Specify the name of the translated controller
                        string key = log.Controller + "s";
                        if (log.Controller.Contains("."))
                        {
                            key = log.Controller.Substring(0, log.Controller.IndexOf('.'));
                        }

                        string controllerNameResource =
                            string.IsNullOrEmpty(Resource2.ResourceManager.GetString(key))
                            ? Resource1.ResourceManager.GetString(key)
                            : Resource2.ResourceManager.GetString(key);

                        if (string.IsNullOrEmpty(controllerNameResource))
                        {
                            controllerNameResource =
                                string.IsNullOrEmpty(Resource1.ResourceManager.GetString(log.Controller))
                                ? Resource2.ResourceManager.GetString(log.Controller)
                                : Resource1.ResourceManager.GetString(log.Controller);
                        }

                        // Specify the text for this operation
                        var actionText = Resource2.ResourceManager.GetString(log.Action)
                                         ?? Resource1.ResourceManager.GetString(log.Action);

                        var processDescription = $"{Resource2.PerformedAnOperation} " +
                            $"{(actionText ?? log.Action)}" +
                            $"{(string.IsNullOrEmpty(log.LogTarget) ? "" : $" ({log.LogTarget})")} " +
                            $"{Resource2.InPage} {(controllerNameResource ?? log.Controller)}";

                        // Determine user kind label
                        string userKindLabel = "";
                        if (log.UserKind == "Member")
                        {
                            userKindLabel = Resource1.member ?? "Member";
                        }
                        else
                        {
                            userKindLabel = lang == "ar" ? "مستخدم نظام" : "System User";
                        }

                        return new ExcelDataDTO
                        {
                            t1 = index + 1,
                            t2 = userFullName,
                            t3 = userKindLabel,
                            t4 = log.RequestTime.ToString("yyyy-MMMM-dd"),
                            t5 = log.RequestTime.ToString("hh:mm:ss tt"),
                            t6 = processDescription
                        };
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
                    var fileExcelName = Resource1.LogsUnite;
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

        [HttpPost] // Use POST instead of DELETE if you call it from Form or AJAX without a pure API
        [ValidateAntiForgeryToken]
        //[HttpPost("DeleteLogs")]
        public async Task<IActionResult> DeleteLogs(string actionName, DateTime? date)
        {
            if (string.IsNullOrEmpty(actionName) && !date.HasValue)
            {
                TempData["Error"] = "يجب تحديد اسم العملية أو التاريخ.";
                return RedirectToAction(nameof(Index));
            }

            var logsQuery = _unitOfWork.RequestLogs.Table.Where(log =>
                (string.IsNullOrEmpty(actionName) || log.NameAr.Contains(actionName) || log.NameEn.Contains(actionName)) &&
                (!date.HasValue || log.RequestTime.Date == date.Value.Date));

            var logsToDelete = await logsQuery.ToListAsync();

            if (logsToDelete.Count == 0)
            {
                TempData["Info"] = "لا توجد سجلات للحذف.";
                return RedirectToAction(nameof(Index));
            }

            _unitOfWork.RequestLogs.RemoveRange(logsToDelete);
            await _unitOfWork.CompleteAsync();

            TempData["Success"] = $"تم حذف {logsToDelete.Count} سجل بنجاح.";
            return RedirectToAction(nameof(Index));
        }

        [HttpPost] // Use POST instead of DELETE if you call it from Form or AJAX without a pure API
        [ValidateAntiForgeryToken]
        //[HttpPost("DeleteAllLogs")]
        public async Task<IActionResult> DeleteAllLogs()
        {
            var all_Logs = await _unitOfWork.RequestLogs.GetAllAsync();
            _unitOfWork.RequestLogs.RemoveRange(all_Logs);
            await _unitOfWork.CompleteAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
