using Application.Helpers;
using Application.Interfaces.Admin;
using Application.Services.Admin;
using AutoMapper;
using Domain.DTOs;
using Domain.DTOs.Admin.SMSDTO;
using Domain.Resources;
using FougeraClub.Areas.Admin.ViewModels.Member;
using FougeraClub.Areas.Admin.ViewModels.SMS;
using FougeraClub.Attributes;
using FougeraClub.Helpers;
using FougeraClub.Middelware;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace FougeraClub.Areas.Admin.Controllers
{
    [AdminAuthorize]
    [Area("Admin")]
    public class SMSController : Controller
    {
        private readonly ISMSService _SMSService;
        private readonly IMemberService _memberService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config; // 👈 new


        public SMSController(ISMSService SMSService, IMemberService memberService, IMapper mapper, IConfiguration config)
        {
            _SMSService = SMSService;
            _memberService = memberService;
            _mapper = mapper;
            _config = config;

        }


        [HttpGet]

        public async Task<IActionResult> SendSMS()
        {
            string to = "971559153004"; // Replace with dynamic value later
            string body = "Hello, this is a test message";

            //string userId = _config["SMS:UserId"];
            //string password = _config["SMS:Password"];
            //string sender = _config["SMS:Sender"];
            //string apiUrl = _config["SMS:url"];
            //string msgType = _config["SMS:msgType"];

            var result = await _SMSService.SendSMSAsync(to, body);

            if (result.success)
                return Ok(new { success = true, message = result.message, response = result.response });
            else
                return BadRequest(new { success = false, message = result.message, response = result.response });
        }


        [YesGet]
        public async Task<IActionResult> Members(string? searchTerm, int page = 1, int pageSize = 50)
        {
            var allMembers = await _memberService.GetAllAsync();
            var memberVMs = _mapper.Map<List<MemberVM>>(allMembers);


            #region search by word
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.Trim();

                memberVMs = memberVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.FullNameAr) && c.FullNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.FullNameEn) && c.FullNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.IdNumber) && c.IdNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }
            #endregion

            var paginated = PaginatedList<MemberVM>.Create(memberVMs, page, pageSize, searchTerm);

            //// Has Error Caling Twice And 2 Without Searching Data
            //// Calling from Ajax Return PartialView
            //if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            //{
            //    return PartialView("_MembersListPartial", paginated);
            //}

            return View(paginated);

        }
        [HttpPost]
        [IgnoreAction]
        public async Task<IActionResult> Send(SendingVM vm)
        {
            var dto = _mapper.Map<SendingDTO>(vm);
            var success = await _SMSService.SendMessageAsync(dto);
            return Json(new { success });
        }

        [YesGet]
        public async Task<IActionResult> MessagesLogs(string? searchTerm, int page = 1, int pageSize = 50)
        {
            var allMessages = await _SMSService.GetAllMessagesLogsAsync();
            var messagesLogsVMs = _mapper.Map<List<MessageLogVM>>(allMessages).AsQueryable();

            #region search by word
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                messagesLogsVMs = messagesLogsVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.MemberNameAr) && c.MemberNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.MemberNameEn) && c.MemberNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.IdNumber) && c.IdNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Message) && c.Message.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            #endregion

            var paginated = PaginatedList<MessageLogVM>.Create(messagesLogsVMs, page, pageSize, searchTerm);

            // Calling from Ajax Return PartialView
            if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
            {
                return PartialView("_MessagesLogsListPartial", paginated);
            }

            return View(paginated);
        }
        [IgnoreAction]
        [YesGet]
        public async Task<IActionResult> PrintMessagesLogs(string? searchTerm)
        {
            var allMessages = await _SMSService.GetAllMessagesLogsAsync();
            var messagesLogsVMs = _mapper.Map<List<MessageLogVM>>(allMessages).AsQueryable();

            #region search by word
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                messagesLogsVMs = messagesLogsVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.MemberNameAr) && c.MemberNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.MemberNameEn) && c.MemberNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.IdNumber) && c.IdNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Message) && c.Message.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            #endregion

            return View(messagesLogsVMs);
        }
        [IgnoreAction]

        public async Task<IActionResult> createExcelReport_Download(string? searchTerm)
        {
            var allMessages = await _SMSService.GetAllMessagesLogsAsync();
            var messagesLogsVMs = _mapper.Map<List<MessageLogVM>>(allMessages).AsQueryable();

            #region search by word
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                messagesLogsVMs = messagesLogsVMs.Where(c =>
                    (!string.IsNullOrEmpty(c.MemberNameAr) && c.MemberNameAr.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.MemberNameEn) && c.MemberNameEn.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.IdNumber) && c.IdNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.Message) && c.Message.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (!string.IsNullOrEmpty(c.PhoneNumber) && c.PhoneNumber.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                );
            }
            #endregion

            // ---- End Get Data As Print

            var boolStatus = false;
            byte[]? fileBytes = null;
            var pathNewFile = "";
            try
            {

                var lang = SessionHelper.GetCurrentLanguage();
                //var allActivitys = await _ActivityService.GetAllAsync();
                var allData_list = messagesLogsVMs;
                var ListTitles = new List<string>
                {
                    Resource1.subscriberName,Resource2.MessageText,Resource2.ReceiverPhone,Resource2.Date + "/" + Resource1.Time,Resource2.Status
                };
                if (allData_list != null || allData_list?.Count() > 0)
                {
                    var excelDataDTO = allData_list.Select(single => new ExcelDataDTO
                    {
                        t1 = lang == "ar" ? single.MemberNameAr : single.MemberNameEn,
                        t2 = single.Message,
                        t3 = single.PhoneNumber,
                        t4 = single.DateAndTime.HasValue ? single.DateAndTime.Value.ToString("HH:mm:ss") + (" / ") + single.DateAndTime.Value.ToString("d").Replace("/", "-") : "",
                        t5 = single.IsDelivered
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
                    var fileExcelName = Resource2.MessageLog;
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
