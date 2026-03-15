using Application.Helpers;
using Application.Interfaces.Admin;
using DocumentFormat.OpenXml.Spreadsheet;
using Domain.DTOs.Admin.AdministrativeReportQuarterlyAnnual;
using Domain.DTOs.Admin.QuartersReport;
using Domain.Entities.MonthlyAdministrativeReport;
using Domain.Entities.QuartersReport;
using Domain.Enums;
using Domain.Resources;
using Infrastructure.Repositories.InterfacesDB;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Application.Services.Admin
{
    public class AdministrativeReportQuarterlyAnnualService : IAdministrativeReportQuarterlyAnnualService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _env;
        private readonly string FileName = "AdministrativeReportQuarterlyAnnual";
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ISMSForSendingOTPService _SMSForSendingOTPService;



        public AdministrativeReportQuarterlyAnnualService(IUnitOfWork unitOfWork, IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor, ISMSForSendingOTPService sMSForSendingOTPService)
        {
            _unitOfWork = unitOfWork;
            _env = env;
            _httpContextAccessor = httpContextAccessor;
            _SMSForSendingOTPService = sMSForSendingOTPService;
        }

        public async Task<IEnumerable<MonthlyAdministrativeReport>> GetAllAsync()
        {
            return await _unitOfWork.MonthlyAdministrativeReports.GetAllAsync();
        }

        public async Task<MonthlyAdministrativeReport?> GetByIdAsync(int id)
        {
            return await _unitOfWork.MonthlyAdministrativeReports
                .GetByIdAsync(e => e.Id == id, e => e.Details);
        }
        public async Task<IEnumerable<MonthsOfYearsAnnualyDTO>> GetAllMonthsofYearsAnnualy(int? year)
        {
            if (year == null)
            {
                return (new List<MonthsOfYearsAnnualyDTO>());

            }

            return await _unitOfWork.MonthlyAdministrativeReports.Table.Where(x => x.Date.HasValue && x.Date.Value.Year == year).Select(x => new MonthsOfYearsAnnualyDTO
            {
                Id = x.Id,
                Date = x.Date,
                year = (x.Date.HasValue) ? x.Date.Value.Year : null,
                month = (x.Date.HasValue) ? x.Date.Value.Month : null,
                ActivitiesDetailsCount = (x.Type == MonthlyAdministrativeReportType.Activities) ? x.Details.Count() : 0,
                AdministrativeDetailsCount = (x.Type == MonthlyAdministrativeReportType.Administrative) ? x.Details.Count() : 0,
            }).ToListAsync();


        }

        public async Task<IEnumerable<MonthsOfYearsAnnualyWithDetailsDTO>> GetAllMonthsofYearsAnnualy_Data(int? year)
        {
            if (year == null)
            {
                return (new List<MonthsOfYearsAnnualyWithDetailsDTO>());
            }
            else
            {
                return await _unitOfWork.MonthlyAdministrativeReports.Table.Where(x => x.Date.HasValue && x.Date.Value.Year == year).Select(x => new MonthsOfYearsAnnualyWithDetailsDTO
                {
                    Id = x.Id,
                    Date = x.Date,
                    year = (x.Date.HasValue) ? x.Date.Value.Year : null,
                    month = (x.Date.HasValue) ? x.Date.Value.Month : null,
                    Type = (x.Type.HasValue) ? (int)x.Type : null,
                    Details = x.Details,
                }).ToListAsync();
            }

        }

        public async Task<IEnumerable<int>> GetAllYearsInDb()
        {
            var allRecords = await _unitOfWork.MonthlyAdministrativeReports.GetAllAsync();
            var allYears = allRecords
                .Where(e => e.Date.HasValue)
                .Select(e => e.Date.Value.Year)
                .Distinct()
                .OrderBy(y => y)
                .ToList();

            return allYears;
        }
        public async Task<bool> CheckCollaborativeReportIsSiggned(int? year)
        {
            if(year == null)
                return false;


            var Collaborative = await _unitOfWork.QuartersReports.GetAllAsync(x=>x.Year == year && x.Quarter == QuartersYear.yearFull && x.Type == QuarterlyReportType.Collaborative, x=>x.ManagerSignature);
            if (Collaborative != null && Collaborative.Any() && Collaborative.FirstOrDefault()?.ManagerSignature != null && !string.IsNullOrEmpty(Collaborative.FirstOrDefault()?.ManagerSignature?.ImagePath))
            {
                return true;
            }
            return false;
        }

        public async Task<MonthsOfYearsAnnualyActivitiesAndAdministrativeDTO> GetAdministrativeAndActivitiesDetailsAsync(int selectedYear, int quarter, string lang)
        {
            int ActivitiesType = (int)MonthlyAdministrativeReportType.Activities;
            int AdministrativeType = (int)MonthlyAdministrativeReportType.Administrative;

            var allMonthsofYearsAnnualy = await GetAllMonthsofYearsAnnualy_Data(selectedYear);

            var listQuarter = new List<List<int>> {
        new List<int>{ 1,2,3 },
        new List<int>{4,5,6},
        new List<int>{7,8,9},
        new List<int>{10,11,12},
    };

            List<int> monthsInQuarter;
            if (quarter == 5) // all year
            {
                monthsInQuarter = Enumerable.Range(1, 12).ToList();
                allMonthsofYearsAnnualy = allMonthsofYearsAnnualy.OrderBy(x => x.Date).ToList();
            }
            else
            {
                monthsInQuarter = listQuarter[quarter - 1];
                allMonthsofYearsAnnualy = allMonthsofYearsAnnualy
                    .Where(c => c.month.HasValue && monthsInQuarter.Contains(c.month.Value));
            }

            var allAdministrative = allMonthsofYearsAnnualy
                .Where(x => x.Type == AdministrativeType)
                .OrderBy(x => x.Date)
                .ToList();

            var allActivities = allMonthsofYearsAnnualy
                .Where(x => x.Type == ActivitiesType)
                .OrderBy(x => x.Date)
                .ToList();

            //var modelAdministrative = _mapper.Map<List<MonthsOfYearsAnnualyWithDetailsVM>>(allAdministrative);
            //var modelActivities = _mapper.Map<List<MonthsOfYearsAnnualyWithDetailsVM>>(allActivities);

            // Chart building
            List<string> labelsMonths = new();
            List<int> administrativeCounts = new();
            List<int> activitiesCounts = new();

            // Build chart for any valid quarter (1..4). Previously quarter 1 was skipped, causing empty charts.
            if (quarter > 0 && quarter < 5)
            {
                foreach (var month in monthsInQuarter)
                {
                    var date = new DateTime(selectedYear, month, 1);
                    labelsMonths.Add(date.ToString("MMMM yyyy") + " " + Resource1.millad);

                    var recordAdmin = allAdministrative.FirstOrDefault(c => c.month == month);
                    var recordActivities = allActivities.FirstOrDefault(c => c.month == month);

                    administrativeCounts.Add(recordAdmin?.Details?.Count() ?? 0);
                    activitiesCounts.Add(recordActivities?.Details?.Count() ?? 0);
                }
            }

            return new MonthsOfYearsAnnualyActivitiesAndAdministrativeDTO
            {
                model_Activities = allActivities,
                model_Administrative = allAdministrative,
                listOfQuarter = monthsInQuarter,
                labelsMonths = labelsMonths,
                adminstrative = administrativeCounts,
                activities = activitiesCounts
            };
        }

        // Send SMS OTP  --> Signature/OTP for Trainer and Manager
        public async Task<bool> SendOtpAsync(int id, string role)
        {
            var (status, code) = await OTPHelper.SaveOtpAsync(_httpContextAccessor, _unitOfWork);

            if(status == false) return false;

            var resultStatus = await _SMSForSendingOTPService.SendOtpSMSAsync(code);

            return resultStatus.Item1;
            //if (resultStatus.Item1)
            //{
            //    return (true, "Done");
            //}
            //else
            //{
            //    return (false, resultStatus.Item2);
            //}
        }

       

        public async Task<(bool success, string? message)> ValidateOtpAsync(int type, int quarter, int year, string code, string role, ClaimsPrincipal user)
        {
            var success = await OTPHelper.ValidateOtpAsync(_httpContextAccessor, _unitOfWork, code);
            if (!success)
                return (false, "Invalid OTP");

            var userId = user.GetUserId();
            var allSignatures = await _unitOfWork.Signatures.GetAllAsync();
            var latestSignature = allSignatures
                .Where(s => s.UserId == userId)
                .OrderByDescending(s => s.CreatedAt)
                .FirstOrDefault();
            if (latestSignature == null)
                return (false, "Signature not found");

            var entity = new QuartersReport
            {
                ManagerSignitureId = latestSignature.Id,
                Type = (QuarterlyReportType)type,
                Quarter = (QuartersYear)quarter,
                Year = year,
            };
            await _unitOfWork.QuartersReports.AddAsync(entity);
            await _unitOfWork.CompleteAsync();
            return (true, null);
        }

        public async Task<QuartersReportDTO?> GetQuarterSignature(int? selectedYear, QuartersYear? quarter, QuarterlyReportType? ActionType)
        {
            var quarterSignature = await _unitOfWork.QuartersReports.Table
                .Where(x => x.Year == selectedYear && x.Quarter == quarter && x.Type == ActionType).Include(x => x.ManagerSignature)
                .Select(x => new QuartersReportDTO
                {
                    Id = x.Id,
                    Type = x.Type,
                    Quarter = x.Quarter,
                    Year = x.Year,
                    ManagerSignitureId = x.ManagerSignitureId,
                    ManagerSignature = x.ManagerSignature
                })
                .FirstOrDefaultAsync();


            return quarterSignature;
        }


    }
}
